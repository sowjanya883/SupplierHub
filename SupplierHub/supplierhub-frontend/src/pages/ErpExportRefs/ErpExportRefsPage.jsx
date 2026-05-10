import { useState } from 'react'
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query'
import { useForm } from 'react-hook-form'
import toast from 'react-hot-toast'
import { erpExportRefsApi } from '../../api/procurement.api'
import { PageHeader, Spinner, EmptyState } from '../../components/ui/index'
import Modal from '../../components/ui/Modal'
import { StatusPill } from '../../components/ui/StatusPill'

export default function ErpExportRefsPage() {
  const qc = useQueryClient()
  const [modalOpen, setModalOpen] = useState(false)
  const [editing, setEditing] = useState(null)
  const [confirmDelete, setConfirmDelete] = useState(null)

  const { data, isLoading } = useQuery({ queryKey: ['erp-exports'], queryFn: erpExportRefsApi.getAll })
  const rows = (data?.data ?? data ?? []).filter(r => !r.isDeleted)

  const { register, handleSubmit, reset, setValue } = useForm()

  const createMut = useMutation({
    mutationFn: erpExportRefsApi.create,
    onSuccess: () => { qc.invalidateQueries(['erp-exports']); close(); toast.success('Export ref created') },
    onError: e => toast.error(extract(e) ?? 'Failed to create'),
  })
  const updateMut = useMutation({
    mutationFn: ({ id, dto }) => erpExportRefsApi.update(id, dto),
    onSuccess: () => { qc.invalidateQueries(['erp-exports']); close(); toast.success('Export ref updated') },
    onError: e => toast.error(extract(e) ?? 'Failed to update'),
  })
  const deleteMut = useMutation({
    mutationFn: erpExportRefsApi.delete,
    onSuccess: () => { qc.invalidateQueries(['erp-exports']); setConfirmDelete(null); toast.success('Export ref deleted') },
    onError: e => toast.error(extract(e) ?? 'Failed to delete'),
  })

  const open = (r) => {
    setEditing(r ?? null)
    if (r) {
      setValue('entityType',    r.entityType)
      setValue('payloadUri',    r.payloadUri ?? '')
      setValue('correlationID', r.correlationID ?? '')
      setValue('exportDate',    r.exportDate ? r.exportDate.slice(0, 10) : '')
      setValue('status',        r.status ?? 'Queued')
    } else {
      reset({ entityType: 'PO', payloadUri: '', correlationID: '', exportDate: new Date().toISOString().slice(0, 10), status: 'Queued' })
    }
    setModalOpen(true)
  }
  const close = () => { setModalOpen(false); setEditing(null); reset() }

  const onSubmit = (form) => {
    const dto = {
      entityType:    form.entityType,
      payloadUri:    form.payloadUri?.trim() || '',
      correlationID: form.correlationID?.trim() || '',
      exportDate:    form.exportDate ? new Date(form.exportDate).toISOString() : null,
      status:        form.status,
    }
    if (editing) {
      updateMut.mutate({
        id: editing.erprefID,
        dto: { ...dto, erprefID: editing.erprefID },
      })
    } else {
      createMut.mutate(dto)
    }
  }

  const isPending = createMut.isPending || updateMut.isPending

  return (
    <div>
      <PageHeader
        title="ERP Export References"
        subtitle="Outbound payloads to ERP/WMS/Finance systems"
        action={<button className="btn btn-primary btn-sm" onClick={() => open(null)}>+ New Export</button>}
      />

      <div className="sh-card p-0 overflow-hidden">
        {isLoading ? <div className="flex justify-center py-12"><Spinner /></div>
          : rows.length === 0 ? <EmptyState message="No ERP exports recorded." />
          : (
            <div className="overflow-x-auto">
              <table className="sh-table">
                <thead>
                  <tr>
                    <th>ID</th>
                    <th>Entity Type</th>
                    <th>Correlation ID</th>
                    <th>Payload URI</th>
                    <th>Export Date</th>
                    <th>Status</th>
                    <th>Actions</th>
                  </tr>
                </thead>
                <tbody>
                  {rows.map(r => (
                    <tr key={r.erprefID}>
                      <td className="text-gray-400 text-xs">{r.erprefID}</td>
                      <td><span className="pill pill-blue">{r.entityType}</span></td>
                      <td className="font-mono text-xs">{r.correlationID || '—'}</td>
                      <td className="max-w-xs truncate text-xs text-gray-600">{r.payloadUri || '—'}</td>
                      <td className="text-xs text-gray-500">{r.exportDate ? new Date(r.exportDate).toLocaleString() : '—'}</td>
                      <td>
                        <span className={`pill ${
                          r.status === 'Posted' ? 'pill-green'
                          : r.status === 'Failed' ? 'pill-red'
                          : 'pill-amber'
                        }`}>{r.status}</span>
                      </td>
                      <td>
                        <button className="btn btn-ghost btn-sm" onClick={() => open(r)}>Edit</button>
                        <button className="btn btn-ghost btn-sm" onClick={() => setConfirmDelete(r)}>Delete</button>
                      </td>
                    </tr>
                  ))}
                </tbody>
              </table>
            </div>
          )}
      </div>

      {modalOpen && (
        <Modal
          title={editing ? `Edit Export #${editing.erprefID}` : 'New ERP Export'}
          onClose={close}
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
              <label className="sh-label">Entity Type *</label>
              <select className="sh-select" {...register('entityType', { required: true })}>
                <option>PO</option><option>ASN</option><option>Invoice</option>
              </select>
            </div>
            <div>
              <label className="sh-label">Correlation ID *</label>
              <input className="sh-input font-mono" placeholder="e.g. PO-2026-001" {...register('correlationID', { required: true })} />
            </div>
            <div>
              <label className="sh-label">Payload URI</label>
              <input className="sh-input" placeholder="https://storage/erp/po-001.json" {...register('payloadUri')} />
            </div>
            <div>
              <label className="sh-label">Export Date</label>
              <input type="date" className="sh-input" {...register('exportDate')} />
            </div>
            <div>
              <label className="sh-label">Status *</label>
              <select className="sh-select" {...register('status', { required: true })}>
                <option>Queued</option><option>Posted</option><option>Failed</option>
              </select>
            </div>
          </form>
        </Modal>
      )}

      {confirmDelete && (
        <Modal
          title="Delete Export Reference"
          onClose={() => setConfirmDelete(null)}
          footer={
            <>
              <button className="btn btn-secondary btn-sm" onClick={() => setConfirmDelete(null)}>Cancel</button>
              <button className="btn btn-danger btn-sm" onClick={() => deleteMut.mutate(confirmDelete.erprefID)} disabled={deleteMut.isPending}>
                {deleteMut.isPending ? 'Deleting…' : 'Delete'}
              </button>
            </>
          }>
          <p className="text-sm text-gray-600">Soft-delete export <strong>#{confirmDelete.erprefID}</strong> ({confirmDelete.entityType} / {confirmDelete.correlationID})?</p>
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
