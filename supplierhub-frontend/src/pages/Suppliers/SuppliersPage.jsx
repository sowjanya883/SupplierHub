import { useState } from 'react'
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query'
import { useNavigate } from 'react-router-dom'
import { useForm } from 'react-hook-form'
import toast from 'react-hot-toast'
import { suppliersApi } from '../../api/suppliers.api'
import { StatusPill } from '../../components/ui/StatusPill'
import { PageHeader, Spinner, EmptyState, ConfirmDialog } from '../../components/ui/index'
import Modal from '../../components/ui/Modal'
import useAuthStore from '../../store/auth.store'

export default function SuppliersPage() {
  const navigate = useNavigate()
  const qc = useQueryClient()
  const { user } = useAuthStore()
  const isAdmin = user?.roles?.includes('Admin')

  const [modal, setModal] = useState(null)   // 'form' | 'delete'
  const [selected, setSelected] = useState(null)
  const [search, setSearch] = useState('')

  const { data, isLoading } = useQuery({
    queryKey: ['suppliers'],
    queryFn: suppliersApi.getAll,
  })

  const rows = (data?.data ?? data ?? []).filter(s =>
    (s.legalName ?? '').toLowerCase().includes(search.toLowerCase())
  )

  const { register, handleSubmit, reset, formState: { errors } } = useForm()

  const createMut = useMutation({
    mutationFn: suppliersApi.create,
    onSuccess: () => {
      qc.invalidateQueries(['suppliers'])
      setModal(null)
      toast.success('Supplier created successfully')
    },
    onError: e => toast.error(e.response?.data?.message ?? 'Failed to create supplier'),
  })

  const updateMut = useMutation({
    mutationFn: ({ id, dto }) => suppliersApi.update(id, dto),
    onSuccess: () => {
      qc.invalidateQueries(['suppliers'])
      setModal(null)
      toast.success('Supplier updated successfully')
    },
    onError: e => toast.error(e.response?.data?.message ?? 'Failed to update supplier'),
  })

  const deleteMut = useMutation({
    mutationFn: (dto) => suppliersApi.delete(dto),
    onSuccess: () => {
      qc.invalidateQueries(['suppliers'])
      setModal(null)
      toast.success('Supplier deleted successfully')
    },
    onError: e => toast.error(e.response?.data?.message ?? 'Failed to delete supplier'),
  })

  const openCreate = () => {
    reset({
      LegalName:    '',
      DunsOrRegNo:  '',
      TaxID:        '',
      BankInfoJson: '',
      Status:       'Active',
    })
    setSelected(null)
    setModal('form')
  }

  const openEdit = (row) => {
    setSelected(row)
    reset({
      LegalName:    row.legalName    ?? '',
      DunsOrRegNo:  row.dunsOrRegNo  ?? '',
      TaxID:        row.taxID        ?? '',
      BankInfoJson: row.bankInfoJson ?? '',
      Status:       row.status       ?? 'Active',
    })
    setModal('form')
  }

  const onSubmit = (d) => {
    const payload = {
      LegalName:    d.LegalName,
      DunsOrRegNo:  d.DunsOrRegNo  || null,
      TaxID:        d.TaxID        || null,
      BankInfoJson: d.BankInfoJson || null,
      Status:       d.Status,
    }
    if (selected) {
      updateMut.mutate({
        id:  selected.supplierID,
        dto: { ...payload, SupplierID: selected.supplierID },
      })
    } else {
      createMut.mutate(payload)
    }
  }

  const isPending = createMut.isPending || updateMut.isPending

  return (
    <div>
      <PageHeader
        title="Suppliers"
        subtitle="Manage registered supplier organisations"
        action={
          isAdmin && (
            <button className="btn btn-primary" onClick={openCreate}>
              + Add Supplier
            </button>
          )
        }
      />

      <div className="mb-4">
        <input
          className="sh-input max-w-xs"
          placeholder="Search by name…"
          value={search}
          onChange={e => setSearch(e.target.value)}
        />
      </div>

      <div className="sh-card p-0 overflow-hidden">
        {isLoading ? (
          <div className="flex justify-center py-12"><Spinner /></div>
        ) : rows.length === 0 ? (
          <EmptyState message="No suppliers found." />
        ) : (
          <table className="sh-table">
            <thead>
              <tr>
                <th>Sr.No.</th> 
                <th>ID</th>
                <th>Legal Name</th>
                <th>Status</th>
                <th>Actions</th>
              </tr>
            </thead>
            <tbody>
              {rows.map((s, index) => (
                <tr key={s.supplierID}>
                  <td className="text-gray-400 text-xs">{index + 1}</td>
                  <td className="text-gray-400 text-xs">{s.supplierID}</td>
                  <td className="font-medium">{s.legalName}</td>
                  <td><StatusPill status={s.status} /></td>
                  <td>
                    <div className="flex gap-1">
                      {/* View — available to all roles */}
                      <button
                        className="btn btn-ghost btn-sm"
                        onClick={() => navigate(`/suppliers/${s.supplierID}`)}
                      >
                        View
                      </button>

                      {/* Edit — Admin only */}
                      {isAdmin && (
                        <button
                          className="btn btn-secondary btn-sm"
                          onClick={() => openEdit(s)}
                        >
                          Edit
                        </button>
                      )}

                      {/* Delete — Admin only */}
                      {isAdmin && (
                        <button
                          className="btn btn-danger btn-sm"
                          onClick={() => { setSelected(s); setModal('delete') }}
                        >
                          Delete
                        </button>
                      )}
                    </div>
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
        )}
      </div>

      {/* Create / Edit Modal */}
      {modal === 'form' && (
        <Modal
          title={selected ? `Edit — ${selected.legalName}` : 'New Supplier'}
          onClose={() => setModal(null)}
          footer={
            <>
              <button className="btn btn-secondary btn-sm" onClick={() => setModal(null)}>
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
              <label className="sh-label">Legal Name *</label>
              <input
                className="sh-input"
                {...register('LegalName', { required: 'Legal name is required' })}
              />
              {errors.LegalName && (
                <p className="text-red-500 text-xs mt-1">{errors.LegalName.message}</p>
              )}
            </div>
            <div>
              <label className="sh-label">DUNS / Reg No</label>
              <input className="sh-input" {...register('DunsOrRegNo')} />
            </div>
            <div>
              <label className="sh-label">Tax ID</label>
              <input className="sh-input" placeholder="e.g. GSTIN number" {...register('TaxID')} />
            </div>
            <div>
              <label className="sh-label">Bank Info (JSON)</label>
              <input
                className="sh-input"
                placeholder='{"bank":"SBI","account":"123456"}'
                {...register('BankInfoJson')}
              />
            </div>
            <div>
              <label className="sh-label">Status *</label>
              <select className="sh-select" {...register('Status', { required: true })}>
                <option>Active</option>
                <option>Suspended</option>
              </select>
            </div>
          </form>
        </Modal>
      )}

      {/* Delete Confirm */}
      {modal === 'delete' && (
        <ConfirmDialog
          message={`Delete "${selected?.legalName}"? This cannot be undone.`}
          onConfirm={() => deleteMut.mutate({ SupplierID: selected.supplierID })}
          onCancel={() => setModal(null)}
          loading={deleteMut.isPending}
        />
      )}
    </div>
  )
}