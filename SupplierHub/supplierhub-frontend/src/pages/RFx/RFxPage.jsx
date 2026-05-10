import { useState } from 'react'
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query'
import { useNavigate } from 'react-router-dom'
import { useForm } from 'react-hook-form'
import toast from 'react-hot-toast'
import { rfxApi } from '../../api/procurement.api'
import { StatusPill } from '../../components/ui/StatusPill'
import { PageHeader, Spinner, EmptyState } from '../../components/ui/index'
import Modal from '../../components/ui/Modal'

export default function RFxPage() {
  const navigate = useNavigate()
  const qc = useQueryClient()
  const [showCreate, setShowCreate] = useState(false)

  const { data, isLoading } = useQuery({ queryKey: ['rfx'], queryFn: rfxApi.getAllRfx })
  const rows = data?.data ?? data ?? []

  const { register, handleSubmit, reset } = useForm()
  const createMut = useMutation({
    mutationFn: rfxApi.createRfx,
    onSuccess: () => { qc.invalidateQueries(['rfx']); setShowCreate(false); toast.success('RFx event created') },
    onError: e => toast.error(e.response?.data?.message ?? 'Error'),
  })

  return (
    <div>
      <PageHeader
        title="RFx Events"
        subtitle="RFI / RFP / RFQ events and bid management"
        action={<button className="btn btn-primary" onClick={() => { reset(); setShowCreate(true) }}>+ New RFx</button>}
      />
      <div className="sh-card p-0 overflow-hidden">
        {isLoading ? <div className="flex justify-center py-12"><Spinner /></div>
          : rows.length === 0 ? <EmptyState message="No RFx events yet." />
          : (
          <table className="sh-table">
            <thead><tr><th>ID</th><th>Type</th><th>Title</th><th>Category ID</th><th>Open Date</th><th>Close Date</th><th>Status</th><th></th></tr></thead>
            <tbody>
              {rows.map(r => (
                <tr key={r.rfxID ?? r.rfXID}>
                  <td>#{r.rfxID ?? r.rfXID}</td>
                  <td><span className={`pill ${r.type === 'RFQ' ? 'pill-blue' : r.type === 'RFP' ? 'pill-purple' : 'pill-teal'}`}>{r.type}</span></td>
                  <td className="font-medium">{r.title}</td>
                  <td>{r.categoryID}</td>
                  <td className="text-xs text-gray-500">{r.openDate ? new Date(r.openDate).toLocaleDateString() : '—'}</td>
                  <td className="text-xs text-gray-500">{r.closeDate ? new Date(r.closeDate).toLocaleDateString() : '—'}</td>
                  <td><StatusPill status={r.status} /></td>
                  <td><button className="btn btn-ghost btn-sm" onClick={() => navigate(`/rfx/${r.rfxID ?? r.rfXID}`)}>View →</button></td>
                </tr>
              ))}
            </tbody>
          </table>
        )}
      </div>

      {showCreate && (
        <Modal title="Create RFx Event" onClose={() => setShowCreate(false)}
          footer={
            <>
              <button className="btn btn-secondary btn-sm" onClick={() => setShowCreate(false)}>Cancel</button>
              <button className="btn btn-primary btn-sm" onClick={handleSubmit(d => createMut.mutate(d))}
                disabled={createMut.isPending}>{createMut.isPending ? 'Saving…' : 'Create'}</button>
            </>
          }
        >
          <form className="space-y-3" noValidate>
            <div><label className="sh-label">Type *</label>
              <select className="sh-select" {...register('type', { required: true })}>
                <option>RFI</option><option>RFP</option><option>RFQ</option>
              </select></div>
            <div><label className="sh-label">Title *</label>
              <input className="sh-input" {...register('title', { required: true })} /></div>
            <div><label className="sh-label">Category ID</label>
              <input type="number" className="sh-input" {...register('categoryID', { valueAsNumber: true })} /></div>
            <div className="grid grid-cols-2 gap-3">
              <div><label className="sh-label">Open Date</label>
                <input type="date" className="sh-input" {...register('openDate')} /></div>
              <div><label className="sh-label">Close Date</label>
                <input type="date" className="sh-input" {...register('closeDate')} /></div>
            </div>
          </form>
        </Modal>
      )}
    </div>
  )
}
