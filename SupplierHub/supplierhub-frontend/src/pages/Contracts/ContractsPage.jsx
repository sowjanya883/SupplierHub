import { useState } from 'react'
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query'
import { useForm } from 'react-hook-form'
import toast from 'react-hot-toast'
import { contractsApi } from '../../api/catalog.api'
import { suppliersApi } from '../../api/suppliers.api'
import { itemsApi } from '../../api/catalog.api'
import { StatusPill } from '../../components/ui/StatusPill'
import { PageHeader, Spinner, EmptyState, ConfirmDialog } from '../../components/ui/index'
import Modal from '../../components/ui/Modal'
import useAuthStore from '../../store/auth.store'

export default function ContractsPage() {
  const qc = useQueryClient()
  const { user } = useAuthStore()

  const isAdmin           = user?.roles?.includes('Admin')
  const isCategoryManager = user?.roles?.includes('CategoryManager')
  const canCreateEdit     = isAdmin || isCategoryManager
  const canDelete         = isAdmin

  const [modal, setModal]         = useState(null)  // 'form' | 'delete'
  const [selected, setSelected]   = useState(null)
  const [search, setSearch]       = useState('')
  const [loadingEdit, setLoadingEdit] = useState(false)
  const [expandedID, setExpandedID]   = useState(null) // for inline details

  // ── Fetch contracts ────────────────────────────────────
  const { data, isLoading } = useQuery({
    queryKey: ['contracts'],
    queryFn: contractsApi.getAll,
  })
  const rows = (data?.data ?? data ?? []).filter(c =>
    String(c.contractID ?? '').includes(search) ||
    String(c.supplierID ?? '').includes(search) ||
    (c.status ?? '').toLowerCase().includes(search.toLowerCase())
  )

  // ── Fetch suppliers & items for dropdowns ──────────────
  const { data: suppData } = useQuery({
    queryKey: ['suppliers'],
    queryFn: suppliersApi.getAll,
  })
  const suppliers = suppData?.data ?? suppData ?? []

  const { data: itemsData } = useQuery({
    queryKey: ['items'],
    queryFn: itemsApi.getAll,
  })
  const items = itemsData?.data ?? itemsData ?? []

  // ── Form ───────────────────────────────────────────────
  const { register, handleSubmit, reset, formState: { errors } } = useForm()

  // ── Mutations ──────────────────────────────────────────
  const createMut = useMutation({
    mutationFn: contractsApi.create,
    onSuccess: () => {
      qc.invalidateQueries(['contracts'])
      setModal(null)
      toast.success('Contract created successfully')
    },
    onError: e => toast.error(e.response?.data?.message ?? 'Failed to create contract'),
  })

  const updateMut = useMutation({
    mutationFn: ({ id, dto }) => contractsApi.update(id, dto),
    onSuccess: () => {
      qc.invalidateQueries(['contracts'])
      setModal(null)
      toast.success('Contract updated successfully')
    },
    onError: e => toast.error(e.response?.data?.message ?? 'Failed to update contract'),
  })

  const deleteMut = useMutation({
    mutationFn: (dto) => contractsApi.delete(dto),
    onSuccess: () => {
      qc.invalidateQueries(['contracts'])
      setModal(null)
      toast.success('Contract deleted successfully')
    },
    onError: e => toast.error(e.response?.data?.message ?? 'Failed to delete contract'),
  })

  // ── Handlers ───────────────────────────────────────────
  const openCreate = () => {
    reset({
      SupplierID: '',
      ItemID:     '',
      Rate:       '',
      Currency:   'INR',
      ValidFrom:  '',
      ValidTo:    '',
      TermsJson:  '',
      Status:     'Active',
    })
    setSelected(null)
    setModal('form')
  }

  // GetAll only has 4 fields so fetch full via GetById before editing
  const openEdit = async (row) => {
    setLoadingEdit(true)
    try {
      const res  = await contractsApi.getById(row.contractID)
      const full = res?.data ?? res
      setSelected(full)
      reset({
        SupplierID: full.supplierID ?? '',
        ItemID:     full.itemID     ?? '',
        Rate:       full.rate       ?? '',
        Currency:   full.currency   ?? 'INR',
        ValidFrom:  full.validFrom?.split('T')[0] ?? '',
        ValidTo:    full.validTo?.split('T')[0]   ?? '',
        TermsJson:  full.termsJson  ?? '',
        Status:     full.status     ?? 'Active',
      })
      setModal('form')
    } catch {
      toast.error('Failed to load contract details')
    } finally {
      setLoadingEdit(false)
    }
  }

  // Toggle inline expanded details row
  const toggleExpand = async (row) => {
    if (expandedID === row.contractID) {
      setExpandedID(null)
      return
    }
    setExpandedID(row.contractID)
  }

  const onSubmit = (d) => {
    const payload = {
      SupplierID: Number(d.SupplierID),
      ItemID:     d.ItemID    ? Number(d.ItemID)   : null,
      Rate:       d.Rate      ? Number(d.Rate)      : null,
      Currency:   d.Currency  || null,
      ValidFrom:  d.ValidFrom || null,
      ValidTo:    d.ValidTo   || null,
      TermsJson:  d.TermsJson || null,
      Status:     d.Status,
    }

    if (selected) {
      updateMut.mutate({
        id:  selected.contractID,
        dto: { ...payload, ContractID: selected.contractID },
      })
    } else {
      createMut.mutate(payload)
    }
  }

  const isPending = createMut.isPending || updateMut.isPending

  // Helper — get supplier name from ID
  const getSupplierName = (id) =>
    suppliers.find(s => s.supplierID === id)?.legalName ?? `Supplier #${id}`

  // Helper — get item SKU from ID
  const getItemSku = (id) =>
    id ? (items.find(i => i.itemID === id)?.sku ?? `Item #${id}`) : null

  // ── Render ─────────────────────────────────────────────
  return (
    <div>
      <PageHeader
        title="Contracts"
        subtitle="Supplier rate contracts and terms agreements"
        action={
          canCreateEdit && (
            <button className="btn btn-primary" onClick={openCreate}>
              + New Contract
            </button>
          )
        }
      />

      {/* Search */}
      <div className="mb-4">
        <input
          className="sh-input max-w-xs"
          placeholder="Search by ID, supplier or status…"
          value={search}
          onChange={e => setSearch(e.target.value)}
        />
      </div>

      {/* Table */}
      <div className="sh-card p-0 overflow-hidden">
        {isLoading ? (
          <div className="flex justify-center py-12"><Spinner /></div>
        ) : rows.length === 0 ? (
          <EmptyState message="No contracts found." />
        ) : (
          <table className="sh-table">
            <thead>
              <tr>
                <th>Sr. No.</th>
                <th>ID</th>
                <th>Supplier</th>
                <th>Rate</th>
                <th>Status</th>
                <th>Actions</th>
              </tr>
            </thead>
            <tbody>
              {rows.map((c, index) => (
                <>
                  <tr
                    key={c.contractID}
                    className={expandedID === c.contractID ? 'bg-blue-50' : ''}
                  >
                    <td className="text-gray-400 text-xs">{index + 1}</td>
                    <td className="text-gray-400 text-xs">{c.contractID}</td>
                    <td className="font-medium">{getSupplierName(c.supplierID)}</td>
                    <td className="font-semibold text-green-700">
                      {c.rate != null ? c.rate.toLocaleString() : '—'}
                    </td>
                    <td><StatusPill status={c.status} /></td>
                    <td>
                      <div className="flex gap-1">

                        {/* View Details — inline expand */}
                        <button
                          className={`btn btn-sm ${
                            expandedID === c.contractID
                              ? 'btn-primary'
                              : 'btn-secondary'
                          }`}
                          onClick={() => toggleExpand(c)}
                        >
                          {expandedID === c.contractID ? 'Hide' : 'Details'}
                        </button>

                        {/* Edit — Admin + CategoryManager */}
                        {canCreateEdit && (
                          <button
                            className="btn btn-ghost btn-sm"
                            onClick={() => openEdit(c)}
                            disabled={loadingEdit}
                          >
                            {loadingEdit ? '…' : 'Edit'}
                          </button>
                        )}

                        {/* Delete — Admin only */}
                        {canDelete && (
                          <button
                            className="btn btn-danger btn-sm"
                            onClick={() => {
                              setSelected(c)
                              setModal('delete')
                            }}
                          >
                            Delete
                          </button>
                        )}
                      </div>
                    </td>
                  </tr>

                  {/* ── Inline expanded details row ──────── */}
                  {expandedID === c.contractID && (
                    <tr key={`${c.contractID}-detail`}>
                      <td colSpan={6} className="bg-blue-50 px-6 py-4">
                        <ExpandedContractDetail
                          contractID={c.contractID}
                          getItemSku={getItemSku}
                          getSupplierName={getSupplierName}
                        />
                      </td>
                    </tr>
                  )}
                </>
              ))}
            </tbody>
          </table>
        )}
      </div>

      {/* GetAll limitation note */}
      {rows.length > 0 && (
        <p className="text-xs text-gray-400 mt-2">
          * Table shows limited fields. Click <strong>Details</strong> to see full contract info.
          To show all fields in the table, add them to{' '}
          <code className="bg-gray-100 px-1 rounded">ContractGetAllDto.cs</code>.
        </p>
      )}

      {/* ── Create / Edit Modal ──────────────────────────── */}
      {modal === 'form' && (
        <Modal
          title={
            selected
              ? `Edit Contract #${selected.contractID}`
              : 'New Contract'
          }
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

            {/* Supplier dropdown */}
            <div>
              <label className="sh-label">Supplier *</label>
              <select
                className="sh-select"
                {...register('SupplierID', { required: 'Supplier is required' })}
              >
                <option value="">— Select a supplier —</option>
                {suppliers.map(s => (
                  <option key={s.supplierID} value={s.supplierID}>
                    {s.legalName}
                  </option>
                ))}
              </select>
              {errors.SupplierID && (
                <p className="text-red-500 text-xs mt-1">{errors.SupplierID.message}</p>
              )}
            </div>

            {/* Item dropdown — optional */}
            <div>
              <label className="sh-label">
                Item
                <span className="text-gray-400 font-normal ml-1">
                  (leave blank for header-level contract)
                </span>
              </label>
              <select className="sh-select" {...register('ItemID')}>
                <option value="">— No specific item (header contract) —</option>
                {items.map(i => (
                  <option key={i.itemID} value={i.itemID}>
                    {i.sku}
                  </option>
                ))}
              </select>
              <p className="text-gray-400 text-xs mt-1">
                Select an item to create an item-specific rate contract,
                or leave blank for a general supplier agreement.
              </p>
            </div>

            {/* Rate + Currency */}
            <div className="grid grid-cols-2 gap-3">
              <div>
                <label className="sh-label">Rate</label>
                <input
                  type="number"
                  step="0.01"
                  min="0"
                  className="sh-input"
                  placeholder="e.g. 1500.00"
                  {...register('Rate')}
                />
              </div>
              <div>
                <label className="sh-label">Currency</label>
                <select className="sh-select" {...register('Currency')}>
                  <option>INR</option>
                  <option>USD</option>
                  <option>EUR</option>
                  <option>GBP</option>
                </select>
              </div>
            </div>

            {/* Valid From + Valid To */}
            <div className="grid grid-cols-2 gap-3">
              <div>
                <label className="sh-label">Valid From</label>
                <input
                  type="date"
                  className="sh-input"
                  {...register('ValidFrom')}
                />
              </div>
              <div>
                <label className="sh-label">Valid To</label>
                <input
                  type="date"
                  className="sh-input"
                  {...register('ValidTo')}
                />
              </div>
            </div>

            {/* Terms JSON */}
            <div>
              <label className="sh-label">
                Terms (JSON)
                <span className="text-gray-400 font-normal ml-1">(optional)</span>
              </label>
              <input
                className="sh-input font-mono text-xs"
                placeholder='{"payment":"Net30","penalty":"2%/week"}'
                {...register('TermsJson')}
              />
              <p className="text-gray-400 text-xs mt-1">
                Payment terms, SLA penalties, delivery conditions as JSON
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
                <option>Expired</option>
                <option>Cancelled</option>
              </select>
            </div>

          </form>
        </Modal>
      )}

      {/* ── Delete Confirmation ──────────────────────────── */}
      {modal === 'delete' && (
        <ConfirmDialog
          message={`Delete Contract #${selected?.contractID}? This cannot be undone.`}
          onConfirm={() =>
            deleteMut.mutate({ ContractID: selected.contractID })
          }
          onCancel={() => setModal(null)}
          loading={deleteMut.isPending}
        />
      )}
    </div>
  )
}

// ── Inline expanded row component ──────────────────────────
function ExpandedContractDetail({ contractID, getItemSku, getSupplierName }) {
  const { data, isLoading } = useQuery({
    queryKey: ['contract', contractID],
    queryFn:  () => contractsApi.getById(contractID),
  })

  if (isLoading) {
    return (
      <div className="flex items-center gap-2 text-sm text-gray-500">
        <Spinner size={14} /> Loading contract details…
      </div>
    )
  }

  const c = data?.data ?? data
  if (!c) return <p className="text-sm text-gray-400">Details not available.</p>

  return (
    <div className="grid grid-cols-2 gap-x-8 gap-y-2 text-sm md:grid-cols-4">
      <Detail label="Contract ID"   value={`#${c.contractID}`} />
      <Detail label="Supplier"      value={getSupplierName(c.supplierID)} />
      <Detail label="Item"
        value={
          c.itemID
            ? getItemSku(c.itemID)
            : <span className="text-gray-400 italic">Header contract (no item)</span>
        }
      />
      <Detail label="Rate"
        value={
          c.rate != null
            ? `${c.rate?.toLocaleString()} ${c.currency ?? ''}`
            : '—'
        }
      />
      <Detail label="Valid From"
        value={c.validFrom ? new Date(c.validFrom).toLocaleDateString() : '—'}
      />
      <Detail label="Valid To"
        value={c.validTo ? new Date(c.validTo).toLocaleDateString() : '—'}
      />
      <Detail label="Status"        value={<StatusPill status={c.status} />} />
      <Detail label="Created"
        value={c.createdOn ? new Date(c.createdOn).toLocaleDateString() : '—'}
      />
      {c.termsJson && (
        <div className="col-span-2 md:col-span-4">
          <span className="text-gray-500 font-medium">Terms JSON</span>
          <pre className="mt-1 text-xs bg-white border border-gray-200 rounded-lg p-3
                          text-gray-700 overflow-auto max-h-32">
            {(() => {
              try { return JSON.stringify(JSON.parse(c.termsJson), null, 2) }
              catch { return c.termsJson }
            })()}
          </pre>
        </div>
      )}
    </div>
  )
}

function Detail({ label, value }) {
  return (
    <div>
      <p className="text-gray-500 text-xs font-medium">{label}</p>
      <p className="text-gray-800 font-medium mt-0.5">{value}</p>
    </div>
  )
}