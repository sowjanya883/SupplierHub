import { useState } from 'react'
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query'
import { useForm } from 'react-hook-form'
import toast from 'react-hot-toast'
import { organizationsApi } from '../../api/organizations.api'
import { StatusPill } from '../../components/ui/StatusPill'
import { PageHeader, Spinner, EmptyState, ConfirmDialog } from '../../components/ui/index'
import Modal from '../../components/ui/Modal'

export default function OrganizationsPage() {
  const qc = useQueryClient()
  const [modal, setModal] = useState(null)   // 'create' | 'edit' | 'delete'
  const [selected, setSelected] = useState(null)
  const [search, setSearch] = useState('')

  const { data, isLoading } = useQuery({
    queryKey: ['organizations'],
    queryFn: organizationsApi.getAll,
  })
  const rows = (data?.data ?? data ?? []).filter(o =>
    (o.organizationName ?? '').toLowerCase().includes(search.toLowerCase())
  )

  const { register, handleSubmit, reset, formState: { errors } } = useForm()

  const createMut = useMutation({
    mutationFn: organizationsApi.create,
    onSuccess: () => { qc.invalidateQueries(['organizations']); setModal(null); toast.success('Organization created') },
    onError: e => toast.error(e.response?.data?.message ?? 'Failed to create'),
  })
  const updateMut = useMutation({
    mutationFn: ({ id, dto }) => organizationsApi.update(id, dto),
    onSuccess: () => { qc.invalidateQueries(['organizations']); setModal(null); toast.success('Organization updated') },
    onError: e => toast.error(e.response?.data?.message ?? 'Failed to update'),
  })
  const deleteMut = useMutation({
    mutationFn: (dto) => organizationsApi.delete(dto),
    onSuccess: () => { qc.invalidateQueries(['organizations']); setModal(null); toast.success('Organization deleted') },
    onError: e => toast.error(e.response?.data?.message ?? 'Failed to delete'),
  })

const openCreate = () => {
  reset({
    OrganizationName: '',
    TaxID:            '',
    AddressJson:      '',
    Status:           'Active',
  })
  setSelected(null)
  setModal('create')
}

const openEdit = (row) => {
  setSelected(row)
  reset({
    OrganizationName: row.organizationName ?? '',
    TaxID:            row.taxID            ?? '',
    AddressJson:      row.addressJson       ?? '',
    Status:           row.status            ?? 'Active',
  })
  setModal('edit')
}

const onSubmit = (d) => {
  const payload = {
    OrganizationName: d.OrganizationName,
    TaxID:            d.TaxID,
    AddressJson:      d.AddressJson?.trim() || null,
    Status:           d.Status,
  }

  if (selected) {
    updateMut.mutate({
      id:  selected.orgID,
      dto: { ...payload, OrgID: selected.orgID }
    })
  } else {
    createMut.mutate(payload)
  }
}

  const isPending = createMut.isPending || updateMut.isPending

  return (
    <div>
      <PageHeader
        title="Organizations"
        subtitle="Manage buyer organisations"
        action={
          <button className="btn btn-primary" onClick={openCreate}>
            + Add Organization
          </button>
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
          <EmptyState message="No organizations found." />
        ) : (
          <table className="sh-table">
            <thead>
              <tr>
                <th>Sr.No.</th>
                <th>ID</th>
                <th>Name</th>
                <th>Tax ID</th>
                <th>Status</th>
                <th>Actions</th>
              </tr>
            </thead>
            <tbody>
              {rows.map((o, index) => (
                <tr key={o.orgID}>
                  <td className="text-gray-400 text-xs">{index + 1}</td>
                  <td className="text-gray-400 text-xs">{o.orgID}</td>
                  <td className="font-medium">{o.organizationName ?? o.name}</td>
                  <td className="text-gray-500">{o.taxID || '—'}</td>
                  <td><StatusPill status={o.status} /></td>
                  <td>
                    <div className="flex gap-1">
                      <button className="btn btn-ghost btn-sm" onClick={() => openEdit(o)}>
                        Edit
                      </button>
                      <button className="btn btn-danger btn-sm" onClick={() => { setSelected(o); setModal('delete') }}>
                        Delete
                      </button>
                    </div>
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
        )}
      </div>

      {/* Create / Edit Modal */}
      {(modal === 'create' || modal === 'edit') && (
        <Modal
          title={modal === 'edit' ? `Edit — ${selected?.name}` : 'New Organization'}
          onClose={() => setModal(null)}
          footer={
            <>
              <button className="btn btn-secondary btn-sm" onClick={() => setModal(null)}>Cancel</button>
              <button className="btn btn-primary btn-sm" onClick={handleSubmit(onSubmit)} disabled={isPending}>
                {isPending ? 'Saving…' : modal === 'edit' ? 'Update' : 'Create'}
              </button>
            </>
          }
        >
          <form className="space-y-3" noValidate>
            <div>
              <label className="sh-label">Name *</label>
              <input className="sh-input" {...register('OrganizationName', { required: 'Name is required' })} />
              {errors.OrganizationName && <p className="text-red-500 text-xs mt-1">{errors.OrganizationName.message}</p>}
            </div>
            <div>
              <label className="sh-label">Tax ID</label>
              <input className="sh-input" placeholder="e.g. GSTIN / VAT number" {...register('TaxID')} />
            </div>
            <div>
              <label className="sh-label">Address</label>
              <input className="sh-input" placeholder='e.g. {"city":"Mumbai","country":"IN"}' {...register('AddressJson')} />
            </div>
            <div>
              <label className="sh-label">Status</label>
              <select className="sh-select" {...register('Status',{required: true})}>
                <option>Active</option>
                <option>Inactive</option>
              </select>
            </div>
          </form>
        </Modal>
      )}

      {/* Delete Confirm */}
      {modal === 'delete' && (
        <ConfirmDialog
          message={`Delete "${selected?.name}"? This cannot be undone.`}
          onConfirm={() => deleteMut.mutate({ orgID: selected.orgID })}
          onCancel={() => setModal(null)}
          loading={deleteMut.isPending}
        />
      )}
    </div>
  )
}