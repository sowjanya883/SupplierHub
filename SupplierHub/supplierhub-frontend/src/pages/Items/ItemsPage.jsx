import { useState } from 'react'
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query'
import { useForm } from 'react-hook-form'
import toast from 'react-hot-toast'
import { itemsApi } from '../../api/catalog.api'
import { categoriesApi } from '../../api/catalog.api'
import { StatusPill } from '../../components/ui/StatusPill'
import { PageHeader, Spinner, EmptyState, ConfirmDialog } from '../../components/ui/index'
import Modal from '../../components/ui/Modal'
import useAuthStore from '../../store/auth.store'

export default function ItemsPage() {
  const qc = useQueryClient()
  const { user } = useAuthStore()

  const isAdmin           = user?.roles?.includes('Admin')
  const isCategoryManager = user?.roles?.includes('CategoryManager')
  const canCreateEdit     = isAdmin || isCategoryManager
  const canDelete         = isAdmin

  const [modal, setModal]       = useState(null)   // 'form' | 'delete'
  const [selected, setSelected] = useState(null)
  const [search, setSearch]     = useState('')
  const [loadingEdit, setLoadingEdit] = useState(false)

  // ── Fetch all items ────────────────────────────────────
  const { data, isLoading } = useQuery({
    queryKey: ['items'],
    queryFn: itemsApi.getAll,
  })

  // ── Fetch all categories for dropdown ─────────────────
  const { data: catData } = useQuery({
    queryKey: ['categories'],
    queryFn: categoriesApi.getAll,
  })
  const categories = catData?.data ?? catData ?? []

  const rows = (data?.data ?? data ?? []).filter(i =>
    (i.sku ?? '').toLowerCase().includes(search.toLowerCase())
  )

  // ── Form ───────────────────────────────────────────────
  const { register, handleSubmit, reset, formState: { errors } } = useForm()

  // ── Mutations ──────────────────────────────────────────
  const createMut = useMutation({
    mutationFn: itemsApi.create,
    onSuccess: () => {
      qc.invalidateQueries(['items'])
      setModal(null)
      toast.success('Item created successfully')
    },
    onError: e => {
      console.error('Item create failed:', e?.response?.status, e?.response?.data)
      toast.error(extractApiError(e) ?? `Failed to create item (HTTP ${e?.response?.status ?? '?'})`)
    },
  })

  const updateMut = useMutation({
    mutationFn: ({ id, dto }) => itemsApi.update(id, dto),
    onSuccess: () => {
      qc.invalidateQueries(['items'])
      setModal(null)
      toast.success('Item updated successfully')
    },
    onError: e => toast.error(extractApiError(e) ?? 'Failed to update item'),
  })

  const deleteMut = useMutation({
    mutationFn: (dto) => itemsApi.delete(dto),
    onSuccess: () => {
      qc.invalidateQueries(['items'])
      setModal(null)
      toast.success('Item deleted successfully')
    },
    onError: e => toast.error(extractApiError(e) ?? 'Failed to delete item'),
  })

  // ── Handlers ───────────────────────────────────────────
  const openCreate = () => {
    reset({
      CategoryID:   '',
      Sku:          '',
      Description:  '',
      Uom:          '',
      LeadTimeDays: '',
      SpecsJson:    '',
      Status:       'Active',
    })
    setSelected(null)
    setModal('form')
  }

  // GetAll only returns ItemID, Sku, Status
  // So we fetch GetById to get all fields when editing
  const openEdit = async (row) => {
    setLoadingEdit(true)
    try {
      const res = await itemsApi.getById(row.itemID)
      const full = res?.data ?? res
      setSelected(full)
      reset({
        CategoryID:   full.categoryID   ?? '',
        Sku:          full.sku          ?? '',
        Description:  full.description  ?? '',
        Uom:          full.uom          ?? '',
        LeadTimeDays: full.leadTimeDays ?? '',
        SpecsJson:    full.specsJson    ?? '',
        Status:       full.status       ?? 'Active',
      })
      setModal('form')
    } catch {
      toast.error('Failed to load item details')
    } finally {
      setLoadingEdit(false)
    }
  }

  const onSubmit = (d) => {
    const payload = {
      CategoryID:   Number(d.CategoryID),
      Sku:          d.Sku,
      Description:  d.Description   || null,
      Uom:          d.Uom           || null,
      LeadTimeDays: d.LeadTimeDays  ? Number(d.LeadTimeDays) : null,
      SpecsJson:    d.SpecsJson     || null,
      Status:       d.Status,
    }

    if (selected) {
      updateMut.mutate({
        id:  selected.itemID,
        dto: { ...payload, ItemID: selected.itemID },
      })
    } else {
      createMut.mutate(payload)
    }
  }

  const isPending = createMut.isPending || updateMut.isPending

  // ── Render ─────────────────────────────────────────────
  return (
    <div>
      <PageHeader
        title="Items"
        subtitle="Item master catalogue — SKUs, specs and lead times"
        action={
          canCreateEdit && (
            <button className="btn btn-primary" onClick={openCreate}>
              + Add Item
            </button>
          )
        }
      />

      {/* Search */}
      <div className="mb-4">
        <input
          className="sh-input max-w-xs"
          placeholder="Search by SKU…"
          value={search}
          onChange={e => setSearch(e.target.value)}
        />
      </div>

      {/* Table */}
      <div className="sh-card p-0 overflow-hidden">
        {isLoading ? (
          <div className="flex justify-center py-12"><Spinner /></div>
        ) : rows.length === 0 ? (
          <EmptyState message="No items found." />
        ) : (
          <table className="sh-table">
            <thead>
              <tr>
                <th>Sr. No.</th>
                <th>ID</th>
                <th>SKU</th>
                <th>Status</th>
                {(canCreateEdit || canDelete) && <th>Actions</th>}
              </tr>
            </thead>
            <tbody>
              {rows.map((i, index) => (
                <tr key={i.itemID}>
                  <td className="text-gray-400 text-xs">{index + 1}</td>
                  <td className="text-gray-400 text-xs">{i.itemID}</td>
                  <td className="font-medium font-mono text-sm">{i.sku}</td>
                  <td><StatusPill status={i.status} /></td>
                  {(canCreateEdit || canDelete) && (
                    <td>
                      <div className="flex gap-1">
                        {/* Edit — Admin + CategoryManager */}
                        {canCreateEdit && (
                          <button
                            className="btn btn-secondary btn-sm"
                            onClick={() => openEdit(i)}
                            disabled={loadingEdit}
                          >
                            {loadingEdit ? '…' : 'Edit'}
                          </button>
                        )}
                        {/* Delete — Admin only */}
                        {canDelete && (
                          <button
                            className="btn btn-danger btn-sm"
                            onClick={() => { setSelected(i); setModal('delete') }}
                          >
                            Delete
                          </button>
                        )}
                      </div>
                    </td>
                  )}
                </tr>
              ))}
            </tbody>
          </table>
        )}
      </div>

      {/* notice about limited table data */}
      {rows.length > 0 && (
        <p className="text-xs text-gray-400 mt-2">
          * Table shows SKU and Status only. Click Edit to see full item details.
          To show Description, UoM and Category in this table, add those fields
          to <code className="bg-gray-100 px-1 rounded">ItemGetAllDto.cs</code> in the backend.
        </p>
      )}

      {/* Create / Edit Modal */}
      {modal === 'form' && (
        <Modal
          title={selected ? `Edit — ${selected.sku}` : 'New Item'}
          onClose={() => setModal(null)}
          footer={
            <>
              <button
                className="btn btn-secondary btn-sm"
                onClick={() => setModal(null)}
              >
                Cancel
              </button>
              <button
                className="btn btn-primary btn-sm"
                onClick={handleSubmit(onSubmit)}
                disabled={isPending}
              >
                {isPending ? 'Saving…' : selected ? 'Update' : 'Create'}
              </button>
            </>
          }
        >
          <form className="space-y-3" noValidate>

            {/* Category dropdown */}
            <div>
              <label className="sh-label">Category *</label>
              <select
                className="sh-select"
                {...register('CategoryID', { required: 'Category is required' })}
              >
                <option value="">— Select a category —</option>
                {categories.map(c => (
                  <option key={c.categoryID} value={c.categoryID}>
                    {c.categoryName}
                  </option>
                ))}
              </select>
              {errors.CategoryID && (
                <p className="text-red-500 text-xs mt-1">{errors.CategoryID.message}</p>
              )}
            </div>

            {/* SKU */}
            <div>
              <label className="sh-label">SKU *</label>
              <input
                className="sh-input font-mono"
                placeholder="e.g. STEEL-HR-5MM"
                {...register('Sku', { required: 'SKU is required' })}
              />
              {errors.Sku && (
                <p className="text-red-500 text-xs mt-1">{errors.Sku.message}</p>
              )}
            </div>

            {/* Description */}
            <div>
              <label className="sh-label">Description</label>
              <input
                className="sh-input"
                placeholder="e.g. Hot rolled steel coil 5mm thickness"
                {...register('Description')}
              />
            </div>

            {/* UoM + Lead Time in one row */}
            <div className="grid grid-cols-2 gap-3">
              <div>
                <label className="sh-label">Unit of Measure</label>
                <input
                  className="sh-input"
                  placeholder="e.g. KG, PCS, LTR"
                  {...register('Uom')}
                />
              </div>
              <div>
                <label className="sh-label">Lead Time (Days)</label>
                <input
                  type="number"
                  min="0"
                  className="sh-input"
                  placeholder="e.g. 7"
                  {...register('LeadTimeDays')}
                />
              </div>
            </div>

            {/* Specs JSON */}
            <div>
              <label className="sh-label">
                Specs JSON
                <span className="text-gray-400 font-normal ml-1">(optional)</span>
              </label>
              <input
                className="sh-input font-mono text-xs"
                placeholder='{"weight":"5kg","grade":"A"}'
                {...register('SpecsJson')}
              />
              <p className="text-gray-400 text-xs mt-1">
                Enter technical specifications as a JSON object
              </p>
            </div>

            {/* Status */}
            <div>
              <label className="sh-label">Status *</label>
              <select
                className="sh-select"
                {...register('Status', { required: true })}
              >
                <option>Active</option>
                <option>Inactive</option>
              </select>
            </div>

          </form>
        </Modal>
      )}

      {/* Delete Confirmation */}
      {modal === 'delete' && (
        <ConfirmDialog
          message={`Delete item "${selected?.sku}"? This cannot be undone. Any catalogs, contracts or PO lines linked to this item may be affected.`}
          onConfirm={() => deleteMut.mutate({ ItemID: selected.itemID })}
          onCancel={() => setModal(null)}
          loading={deleteMut.isPending}
        />
      )}
    </div>
  )
}

function extractApiError(err) {
  if (!err) return null
  if (err.response?.status === 403) return 'You do not have permission to do this. Try logging out and back in to refresh your role.'
  const data = err.response?.data
  if (!data) return null
  const firstModelError = data.errors ? Object.values(data.errors).flat().find(Boolean) : null
  return firstModelError ?? data.message ?? data.detail ?? data.error ?? (typeof data === 'string' ? data : null)
}