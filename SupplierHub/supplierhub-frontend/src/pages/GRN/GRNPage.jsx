import { useState } from 'react'
import { useNavigate } from 'react-router-dom'
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query'
import { useForm } from 'react-hook-form'
import toast from 'react-hot-toast'
import { grnApi } from '../../api/operations.api'
import { StatusPill } from '../../components/ui/StatusPill'
import { PageHeader, Spinner, EmptyState } from '../../components/ui/index'
import Modal from '../../components/ui/Modal'
import useAuthStore from '../../store/auth.store'

export default function GRNPage() {
  const navigate = useNavigate()
  const qc = useQueryClient()
  const user = useAuthStore(s => s.user)
  const [open, setOpen] = useState(false)

  const { data, isLoading } = useQuery({ queryKey: ['grn'], queryFn: grnApi.getAll })
  const rows = (data?.data ?? data ?? []).filter(g => !g.isDeleted)

  const { register, handleSubmit, reset } = useForm()

  const createMut = useMutation({
    mutationFn: grnApi.create,
    onSuccess: (res) => {
      qc.invalidateQueries(['grn'])
      setOpen(false); reset()
      toast.success('GRN created')
      const created = res?.data ?? res
      if (created?.grnID) navigate(`/grn/${created.grnID}`)
    },
    onError: e => toast.error(extract(e) ?? 'Failed to create GRN'),
  })

  const onSubmit = (form) => createMut.mutate({
    poID:         Number(form.poID),
    asnID:        form.asnID === '' ? null : Number(form.asnID),
    receivedDate: form.receivedDate ? new Date(form.receivedDate).toISOString() : new Date().toISOString(),
    receivedBy:   form.receivedBy === '' ? null : Number(form.receivedBy),
    status:       form.status ?? 'Open',
  })

  return (
    <div>
      <PageHeader
        title="GRN — Receiving"
        subtitle="Goods receipt notes against POs / ASNs"
        action={<button className="btn btn-primary btn-sm" onClick={() => {
          reset({ poID: '', asnID: '', receivedDate: today(), receivedBy: user?.userId ?? '', status: 'Open' })
          setOpen(true)
        }}>+ Add GRN</button>}
      />

      <div className="sh-card p-0 overflow-hidden">
        {isLoading ? <div className="flex justify-center py-12"><Spinner /></div>
          : rows.length === 0 ? <EmptyState message="No GRNs yet." />
          : (
            <div className="overflow-x-auto">
              <table className="sh-table">
                <thead>
                  <tr>
                    <th>GRN ID</th><th>PO ID</th><th>ASN ID</th><th>Received Date</th><th>Received By</th><th>Status</th><th></th>
                  </tr>
                </thead>
                <tbody>
                  {rows.map(g => (
                    <tr key={g.grnID} className="cursor-pointer hover:bg-gray-50"
                        onClick={() => navigate(`/grn/${g.grnID}`)}>
                      <td className="font-medium">#{g.grnID}</td>
                      <td>{g.poID}</td>
                      <td>{g.asnID ?? '—'}</td>
                      <td className="text-xs text-gray-500">{g.receivedDate ? new Date(g.receivedDate).toLocaleDateString() : '—'}</td>
                      <td>{g.receivedBy ?? '—'}</td>
                      <td><StatusPill status={g.status} /></td>
                      <td><button className="btn btn-ghost btn-sm" onClick={(e) => { e.stopPropagation(); navigate(`/grn/${g.grnID}`) }}>View →</button></td>
                    </tr>
                  ))}
                </tbody>
              </table>
            </div>
          )}
      </div>

      {open && (
        <Modal title="New GRN" onClose={() => setOpen(false)}
          footer={
            <>
              <button className="btn btn-secondary btn-sm" onClick={() => setOpen(false)}>Cancel</button>
              <button className="btn btn-primary btn-sm" onClick={handleSubmit(onSubmit)} disabled={createMut.isPending}>
                {createMut.isPending ? 'Creating…' : 'Create'}
              </button>
            </>
          }>
          <form className="space-y-3" noValidate>
            <div><label className="sh-label">PO ID *</label><input type="number" className="sh-input" {...register('poID', { required: true })} /></div>
            <div><label className="sh-label">ASN ID</label><input type="number" className="sh-input" {...register('asnID')} /></div>
            <div><label className="sh-label">Received Date *</label><input type="date" className="sh-input" {...register('receivedDate', { required: true })} /></div>
            <div><label className="sh-label">Received By <span className="text-gray-400 font-normal">(User ID)</span></label><input type="number" className="sh-input" placeholder="e.g. your user id" {...register('receivedBy')} /></div>
            <div><label className="sh-label">Status</label>
              <select className="sh-select" defaultValue="Pending" {...register('status')}>
                <option>Pending</option><option>Open</option><option>Posted</option>
              </select>
            </div>
          </form>
        </Modal>
      )}
    </div>
  )
}

const today = () => new Date().toISOString().slice(0, 10)

function extract(err) {
  const data = err?.response?.data
  if (!data) return null
  const firstModelError = data.errors ? Object.values(data.errors).flat().find(Boolean) : null
  return firstModelError ?? data.message ?? (typeof data === 'string' ? data : null)
}
