import { useState } from 'react'
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query'
import { useForm } from 'react-hook-form'
import toast from 'react-hot-toast'
import { categoriesApi } from '../../api/catalog.api'
import { StatusPill } from '../../components/ui/StatusPill'
import { PageHeader, Spinner, EmptyState, ConfirmDialog } from '../../components/ui/index'
import Modal from '../../components/ui/Modal'
import useAuthStore from '../../store/auth.store'

export default function CategoriesPage() {
  const qc = useQueryClient()
  const { user } = useAuthStore()

  const isAdmin            = user?.roles?.includes('Admin')
  const isCategoryManager  = user?.roles?.includes('CategoryManager')
  const canCreateEdit      = isAdmin || isCategoryManager  // Admin + CategoryManager
  const canDelete          = isAdmin                       // Admin only

  const [modal, setModal]     = useState(null)  // 'form' | 'delete'
  const [selected, setSelected] = useState(null)
  const [search, setSearch]   = useState('')

  // ── Fetch ──────────────────────────────────────────────
  const { data, isLoading } = useQuery({
    queryKey: ['categories'],
    queryFn: categoriesApi.getAll,
  })

  const rows = (data?.data ?? data ?? []).filter(c =>
    (c.categoryName ?? '').toLowerCase().includes(search.toLowerCase())
  )

  // ── Form ───────────────────────────────────────────────
  const { register, handleSubmit, reset, formState: { errors } } = useForm()

  // ── Mutations ──────────────────────────────────────────
  const createMut = useMutation({
    mutationFn: categoriesApi.create,
    onSuccess: () => {
      qc.invalidateQueries(['categories'])
      setModal(null)
      toast.success('Category created successfully')
    },
    onError: e => toast.error(e.response?.data?.message ?? 'Failed to create category'),
  })

  const updateMut = useMutation({
    mutationFn: ({ id, dto }) => categoriesApi.update(id, dto),
    onSuccess: () => {
      qc.invalidateQueries(['categories'])
      setModal(null)
      toast.success('Category updated successfully')
    },
    onError: e => toast.error(e.response?.data?.message ?? 'Failed to update category'),
  })

  const deleteMut = useMutation({
    mutationFn: (dto) => categoriesApi.delete(dto),
    onSuccess: () => {
      qc.invalidateQueries(['categories'])
      setModal(null)
      toast.success('Category deleted successfully')
    },
    onError: e => toast.error(e.response?.data?.message ?? 'Failed to delete category'),
  })

  // ── Handlers ───────────────────────────────────────────
  const openCreate = () => {
    reset({
      CategoryName:     '',
      ParentCategoryID: '',
      Status:           'Active',
    })
    setSelected(null)
    setModal('form')
  }

  const openEdit = (row) => {
    setSelected(row)
    reset({
      CategoryName:     row.categoryName     ?? '',
      ParentCategoryID: row.parentCategoryID ?? '',
      Status:           row.status           ?? 'Active',
    })
    setModal('form')
  }

  const onSubmit = (d) => {
    const payload = {
      CategoryName:     d.CategoryName,
      ParentCategoryID: d.ParentCategoryID ? Number(d.ParentCategoryID) : null,
      Status:           d.Status,
    }

    if (selected) {
      updateMut.mutate({
        id:  selected.categoryID,
        dto: { ...payload, CategoryID: selected.categoryID },
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
        title="Categories"
        subtitle="Manage product category tree"
        action={
          canCreateEdit && (
            <button className="btn btn-primary" onClick={openCreate}>
              + Add Category
            </button>
          )
        }
      />

      {/* Search */}
      <div className="mb-4">
        <input
          className="sh-input max-w-xs"
          placeholder="Search by category name…"
          value={search}
          onChange={e => setSearch(e.target.value)}
        />
      </div>

      {/* Table */}
      <div className="sh-card p-0 overflow-hidden">
        {isLoading ? (
          <div className="flex justify-center py-12"><Spinner /></div>
        ) : rows.length === 0 ? (
          <EmptyState message="No categories found." />
        ) : (
          <table className="sh-table">
            <thead>
              <tr>
                <th>Sr. No.</th>
                <th>ID</th>
                <th>Category Name</th>
                <th>Status</th>
                {(canCreateEdit || canDelete) && <th>Actions</th>}
              </tr>
            </thead>
            <tbody>
              {rows.map((c, index) => (
                <tr key={c.categoryID}>
                  <td className="text-gray-400 text-xs">{index + 1}</td>
                  <td className="text-gray-400 text-xs">{c.categoryID}</td>
                  <td className="font-medium">{c.categoryName}</td>
                  <td><StatusPill status={c.status} /></td>
                  {(canCreateEdit || canDelete) && (
                    <td>
                      <div className="flex gap-1">
                        {/* Edit — Admin + CategoryManager */}
                        {canCreateEdit && (
                          <button
                            className="btn btn-secondary btn-sm"
                            onClick={() => openEdit(c)}
                          >
                            Edit
                          </button>
                        )}
                        {/* Delete — Admin only */}
                        {canDelete && (
                          <button
                            className="btn btn-danger btn-sm"
                            onClick={() => { setSelected(c); setModal('delete') }}
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

      {/* Create / Edit Modal */}
      {modal === 'form' && (
        <Modal
          title={selected ? `Edit — ${selected.categoryName}` : 'New Category'}
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

            <div>
              <label className="sh-label">Category Name *</label>
              <input
                className="sh-input"
                placeholder="e.g. Electronics, Raw Materials"
                {...register('CategoryName', { required: 'Category name is required' })}
              />
              {errors.CategoryName && (
                <p className="text-red-500 text-xs mt-1">{errors.CategoryName.message}</p>
              )}
            </div>

            <div>
              <label className="sh-label">
                Parent Category ID
                <span className="text-gray-400 font-normal ml-1">(leave blank for top-level)</span>
              </label>
              <input
                type="number"
                className="sh-input"
                placeholder="e.g. 1"
                {...register('ParentCategoryID')}
              />
              <p className="text-gray-400 text-xs mt-1">
                Leave blank to create a root category. Enter an existing Category ID to create a sub-category.
              </p>
            </div>

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
          message={`Delete category "${selected?.categoryName}"? This cannot be undone. Any items linked to this category may be affected.`}
          onConfirm={() => deleteMut.mutate({ CategoryID: selected.categoryID })}
          onCancel={() => setModal(null)}
          loading={deleteMut.isPending}
        />
      )}
    </div>
  )
}