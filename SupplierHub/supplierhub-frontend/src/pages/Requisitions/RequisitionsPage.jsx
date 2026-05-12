import { useState } from 'react'
import { useNavigate } from 'react-router-dom'
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query'
import { useForm } from 'react-hook-form'
import toast from 'react-hot-toast'
import { requisitionsApi } from '../../api/procurement.api'
import { PageHeader, Spinner, EmptyState } from '../../components/ui/index'
import Modal from '../../components/ui/Modal'
import { StatusPill } from '../../components/ui/StatusPill'
import useAuthStore from '../../store/auth.store'

export default function RequisitionsPage() {
  const navigate = useNavigate()
  const qc = useQueryClient()
  const user = useAuthStore(s => s.user)
  const roles = user?.roles ?? []
  const canCreate = roles.some(r => ['Admin','Buyer'].includes(r))
  const [modalOpen, setModalOpen] = useState(false)

  const { data, isLoading } = useQuery({
    queryKey: ['requisitions'],
    queryFn: requisitionsApi.getAll,
  })
  const rows = (data?.data ?? data ?? []).filter(r => !r.isDeleted)

  const { register, handleSubmit, reset } = useForm()

  const createMut = useMutation({
    mutationFn: requisitionsApi.create,
    onSuccess: (res) => {
      qc.invalidateQueries(['requisitions'])
      setModalOpen(false)
      reset()
      toast.success('Requisition created')
      const created = res?.data ?? res
      if (created?.prID) navigate(`/requisitions/${created.prID}`)
    },
    onError: e => toast.error(extract(e) ?? 'Failed to create'),
  })

  const onSubmit = (form) => {
    createMut.mutate({
      requesterID:     Number(form.requesterID || user?.userId || 0),
      requesterUserID: Number(user?.userId ?? 0),
      orgID:           Number(form.orgID),
      costCenter:      form.costCenter?.trim() || '',
      justification:   form.justification?.trim() || '',
      requestedDate:   form.requestedDate ? new Date(form.requestedDate).toISOString() : null,
      neededByDate:    form.neededByDate ? new Date(form.neededByDate).toISOString() : null,
    })
  }

  return (
    <div>
      <PageHeader
        title="Requisitions (PR)"
        subtitle="Purchase requisitions and approvals"
        action={canCreate ? (
          <button className="btn btn-primary btn-sm" onClick={() => {
            reset({
              requesterID: user?.userId ?? '',
              orgID: '', costCenter: '', justification: '',
              requestedDate: new Date().toISOString().slice(0, 10),
              neededByDate: '',
            })
            setModalOpen(true)
          }}>+ New PR</button>
        ) : null}
      />

      <div className="sh-card p-0 overflow-hidden">
        {isLoading ? (
          <div className="flex justify-center py-12"><Spinner /></div>
        ) : rows.length === 0 ? (
          <EmptyState message="No requisitions yet." />
        ) : (
          <div className="overflow-x-auto">
            <table className="sh-table">
              <thead>
                <tr>
                  <th>PR ID</th>
                  <th>Requester</th>
                  <th>Org</th>
                  <th>Cost Center</th>
                  <th>Needed By</th>
                  <th>Status</th>
                </tr>
              </thead>
              <tbody>
                {rows.map(r => (
                  <tr key={r.prID} className="cursor-pointer hover:bg-gray-50"
                      onClick={() => navigate(`/requisitions/${r.prID}`)}>
                    <td className="font-medium">#{r.prID}</td>
                    <td>{r.requesterID}</td>
                    <td>{r.orgID}</td>
                    <td>{r.costCenter || '—'}</td>
                    <td className="text-xs text-gray-500">{r.neededByDate ? new Date(r.neededByDate).toLocaleDateString() : '—'}</td>
                    <td><StatusPill status={r.status} /></td>
                  </tr>
                ))}
              </tbody>
            </table>
          </div>
        )}
      </div>

      {modalOpen && (
        <Modal title="New Requisition" onClose={() => setModalOpen(false)}
          footer={
            <>
              <button className="btn btn-secondary btn-sm" onClick={() => setModalOpen(false)}>Cancel</button>
              <button className="btn btn-primary btn-sm" onClick={handleSubmit(onSubmit)} disabled={createMut.isPending}>
                {createMut.isPending ? 'Creating…' : 'Create'}
              </button>
            </>
          }>
          <form className="space-y-3" noValidate>
            <div>
              <label className="sh-label">Requester User ID *</label>
              <input type="number" className="sh-input" {...register('requesterID', { required: true })} />
            </div>
            <div>
              <label className="sh-label">Organization ID *</label>
              <input type="number" className="sh-input" {...register('orgID', { required: true })} />
            </div>
            <div>
              <label className="sh-label">Cost Center</label>
              <input className="sh-input" placeholder="e.g. CC-IT-001" {...register('costCenter')} />
            </div>
            <div>
              <label className="sh-label">Justification</label>
              <textarea rows={3} className="sh-input" {...register('justification')} />
            </div>
            <div className="grid grid-cols-2 gap-3">
              <div>
                <label className="sh-label">Requested Date</label>
                <input type="date" className="sh-input" {...register('requestedDate')} />
              </div>
              <div>
                <label className="sh-label">Needed By</label>
                <input type="date" className="sh-input" {...register('neededByDate')} />
              </div>
            </div>
          </form>
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
