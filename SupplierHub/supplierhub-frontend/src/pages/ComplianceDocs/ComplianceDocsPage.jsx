import { useState } from 'react'
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query'
import { useForm } from 'react-hook-form'
import toast from 'react-hot-toast'
import { complianceDocsApi } from '../../api/procurement.api'
import { StatusPill } from '../../components/ui/StatusPill'
import { PageHeader, Spinner, EmptyState } from '../../components/ui/index'
import Modal from '../../components/ui/Modal'

export default function ComplianceDocsPage() {
  const qc = useQueryClient()
  const [modalOpen, setModalOpen] = useState(false)
  const [editing, setEditing] = useState(null)
  const [confirmDelete, setConfirmDelete] = useState(null)

  const { data, isLoading } = useQuery({ queryKey: ['compliance-docs'], queryFn: complianceDocsApi.getAll })
  const rows = (data?.data ?? data ?? []).filter(d => !d.isDeleted)

  const { register, handleSubmit, reset, setValue } = useForm()

  const createMut = useMutation({
    mutationFn: complianceDocsApi.create,
    onSuccess: () => { qc.invalidateQueries(['compliance-docs']); close(); toast.success('Document added') },
    onError: e => toast.error(extract(e) ?? 'Failed to add doc'),
  })
  const updateMut = useMutation({
    mutationFn: ({ id, dto }) => complianceDocsApi.update(id, dto),
    onSuccess: () => { qc.invalidateQueries(['compliance-docs']); close(); toast.success('Document updated') },
    onError: e => toast.error(extract(e) ?? 'Failed to update doc'),
  })
  const deleteMut = useMutation({
    mutationFn: complianceDocsApi.delete,
    onSuccess: () => { qc.invalidateQueries(['compliance-docs']); setConfirmDelete(null); toast.success('Document deleted') },
    onError: e => toast.error(extract(e) ?? 'Failed to delete doc'),
  })

  const open = (d) => {
    setEditing(d ?? null)
    if (d) {
      setValue('supplierID', d.supplierID)
      setValue('docType',    d.docType)
      setValue('fileUri',    d.fileUri ?? '')
      setValue('issueDate',  d.issueDate ? d.issueDate.slice(0, 10) : '')
      setValue('expiryDate', d.expiryDate ? d.expiryDate.slice(0, 10) : '')
      setValue('status',     d.status ?? 'Valid')
    } else {
      reset({ supplierID: '', docType: 'ISO', fileUri: '', issueDate: '', expiryDate: '', status: 'Valid' })
    }
    setModalOpen(true)
  }
  const close = () => { setModalOpen(false); setEditing(null); reset() }

  const onSubmit = (form) => {
    const dto = {
      supplierID: Number(form.supplierID),
      docType:    form.docType,
      fileUri:    form.fileUri?.trim() || '',
      issueDate:  form.issueDate ? new Date(form.issueDate).toISOString() : null,
      expiryDate: form.expiryDate ? new Date(form.expiryDate).toISOString() : null,
      status:     form.status,
    }
    if (editing) updateMut.mutate({ id: editing.docID, dto: { ...dto, docID: editing.docID } })
    else createMut.mutate(dto)
  }

  const isPending = createMut.isPending || updateMut.isPending

  return (
    <div>
      <PageHeader
        title="Compliance Documents"
        subtitle="Certifications and compliance records"
        action={<button className="btn btn-primary btn-sm" onClick={() => open(null)}>+ Add Doc</button>}
      />

      <div className="sh-card p-0 overflow-hidden">
        {isLoading ? <div className="flex justify-center py-12"><Spinner /></div>
          : rows.length === 0 ? <EmptyState message="No compliance documents." />
          : (
            <div className="overflow-x-auto">
              <table className="sh-table">
                <thead>
                  <tr>
                    <th>ID</th><th>Supplier</th><th>Type</th><th>Issue Date</th><th>Expiry</th><th>Status</th><th>Actions</th>
                  </tr>
                </thead>
                <tbody>
                  {rows.map(d => (
                    <tr key={d.docID}>
                      <td className="text-gray-400 text-xs">{d.docID}</td>
                      <td>{d.supplierID}</td>
                      <td><span className="pill pill-blue">{d.docType}</span></td>
                      <td className="text-xs text-gray-500">{d.issueDate ? new Date(d.issueDate).toLocaleDateString() : '—'}</td>
                      <td className="text-xs text-gray-500">{d.expiryDate ? new Date(d.expiryDate).toLocaleDateString() : '—'}</td>
                      <td><StatusPill status={d.status} /></td>
                      <td>
                        <button className="btn btn-ghost btn-sm" onClick={() => open(d)}>Edit</button>
                        <button className="btn btn-ghost btn-sm" onClick={() => setConfirmDelete(d)}>Delete</button>
                      </td>
                    </tr>
                  ))}
                </tbody>
              </table>
            </div>
          )}
      </div>

      {modalOpen && (
        <Modal title={editing ? `Edit Doc #${editing.docID}` : 'New Compliance Doc'} onClose={close}
          footer={
            <>
              <button className="btn btn-secondary btn-sm" onClick={close}>Cancel</button>
              <button className="btn btn-primary btn-sm" onClick={handleSubmit(onSubmit)} disabled={isPending}>
                {isPending ? 'Saving…' : editing ? 'Update' : 'Create'}
              </button>
            </>
          }>
          <form className="space-y-3" noValidate>
            <div>
              <label className="sh-label">Supplier ID *</label>
              <input type="number" className="sh-input" {...register('supplierID', { required: true })} />
            </div>
            <div>
              <label className="sh-label">Doc Type *</label>
              <select className="sh-select" {...register('docType', { required: true })}>
                <option>ISO</option><option>Insurance</option><option>KYC</option><option>ESG</option><option>Other</option>
              </select>
            </div>
            <div>
              <label className="sh-label">File URI</label>
              <input className="sh-input" placeholder="https://files/iso9001.pdf" {...register('fileUri')} />
            </div>
            <div className="grid grid-cols-2 gap-3">
              <div>
                <label className="sh-label">Issue Date</label>
                <input type="date" className="sh-input" {...register('issueDate')} />
              </div>
              <div>
                <label className="sh-label">Expiry Date</label>
                <input type="date" className="sh-input" {...register('expiryDate')} />
              </div>
            </div>
            <div>
              <label className="sh-label">Status *</label>
              <select className="sh-select" {...register('status', { required: true })}>
                <option>Valid</option><option>Expired</option><option>Pending</option>
              </select>
            </div>
          </form>
        </Modal>
      )}

      {confirmDelete && (
        <Modal title="Delete Document" onClose={() => setConfirmDelete(null)}
          footer={
            <>
              <button className="btn btn-secondary btn-sm" onClick={() => setConfirmDelete(null)}>Cancel</button>
              <button className="btn btn-danger btn-sm" onClick={() => deleteMut.mutate({ docID: confirmDelete.docID })} disabled={deleteMut.isPending}>
                {deleteMut.isPending ? 'Deleting…' : 'Delete'}
              </button>
            </>
          }>
          <p className="text-sm text-gray-600">Soft-delete document <strong>#{confirmDelete.docID}</strong>?</p>
        </Modal>
      )}
    </div>
  )
}

function extract(err) {
  const data = err?.response?.data
  if (!data) return null
  const firstModelError = data.errors ? Object.values(data.errors).flat().find(Boolean) : null
  return firstModelError ?? data.message ?? (typeof data === 'string' ? data : null)
}
