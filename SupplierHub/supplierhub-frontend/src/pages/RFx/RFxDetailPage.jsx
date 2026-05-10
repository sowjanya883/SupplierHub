import { useState } from 'react'
import { useParams, useNavigate } from 'react-router-dom'
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query'
import { useForm } from 'react-hook-form'
import toast from 'react-hot-toast'
import { rfxApi } from '../../api/procurement.api'
import { StatusPill } from '../../components/ui/StatusPill'
import { Spinner, EmptyState } from '../../components/ui/index'
import Modal from '../../components/ui/Modal'

const TABS = ['Lines','Invites','Bids','Awards']

export default function RFxDetailPage() {
  const { id } = useParams()
  const navigate = useNavigate()
  const [tab, setTab] = useState('Lines')

  const { data, isLoading } = useQuery({ queryKey: ['rfx', id], queryFn: () => rfxApi.getRfxById(id) })

  if (isLoading) return <div className="flex justify-center py-16"><Spinner /></div>
  const rfx = data?.data ?? data
  if (!rfx) return <p>RFx not found.</p>

  const rfxId = Number(id)

  return (
    <div>
      <div className="page-header">
        <div>
          <button onClick={() => navigate(-1)} className="text-sm text-blue-600 hover:underline mb-1 block">← Back</button>
          <h1 className="page-title">{rfx.title}</h1>
        </div>
        <div className="flex items-center gap-2">
          <span className={`pill ${rfx.type === 'RFQ' ? 'pill-blue' : 'pill-purple'}`}>{rfx.type}</span>
          <StatusPill status={rfx.status} />
        </div>
      </div>

      <div className="sh-card mb-4">
        <div className="grid grid-cols-3 gap-4 text-sm">
          <div><span className="text-gray-500">RFx ID</span><p className="font-medium mt-0.5">#{rfx.rfxID ?? rfx.rfXID}</p></div>
          <div><span className="text-gray-500">Category ID</span><p className="font-medium mt-0.5">{rfx.categoryID ?? '—'}</p></div>
          <div><span className="text-gray-500">Created By</span><p className="font-medium mt-0.5">{rfx.createdBy ?? '—'}</p></div>
          <div><span className="text-gray-500">Open Date</span><p className="font-medium mt-0.5">{rfx.openDate ? new Date(rfx.openDate).toLocaleDateString() : '—'}</p></div>
          <div><span className="text-gray-500">Close Date</span><p className="font-medium mt-0.5">{rfx.closeDate ? new Date(rfx.closeDate).toLocaleDateString() : '—'}</p></div>
        </div>
      </div>

      <div className="flex gap-1 mb-4 border-b border-gray-200">
        {TABS.map(t => (
          <button key={t} onClick={() => setTab(t)}
            className={`px-4 py-2 text-sm font-medium border-b-2 transition-colors ${
              tab === t ? 'border-blue-600 text-blue-700' : 'border-transparent text-gray-500 hover:text-gray-700'}`}>
            {t}
          </button>
        ))}
      </div>

      {tab === 'Lines'   && <LinesTab   rfxId={rfxId} />}
      {tab === 'Invites' && <InvitesTab rfxId={rfxId} />}
      {tab === 'Bids'    && <BidsTab    rfxId={rfxId} />}
      {tab === 'Awards'  && <AwardsTab  rfxId={rfxId} />}
    </div>
  )
}

/* ─── Lines ─── */
function LinesTab({ rfxId }) {
  const qc = useQueryClient()
  const [open, setOpen] = useState(false)
  const { register, handleSubmit, reset } = useForm()
  const { data, isLoading } = useQuery({ queryKey: ['rfx-lines', rfxId], queryFn: () => rfxApi.getRfxLines(rfxId) })
  const rows = (data?.data ?? data ?? [])

  const addMut = useMutation({
    mutationFn: rfxApi.addRfxLine,
    onSuccess: () => { qc.invalidateQueries(['rfx-lines', rfxId]); setOpen(false); reset(); toast.success('Line added') },
    onError: e => toast.error(extract(e) ?? 'Failed to add line'),
  })

  const onSubmit = (form) => addMut.mutate({
    rfxID:       rfxId,
    itemID:      Number(form.itemID),
    qty:         form.qty === '' ? null : Number(form.qty),
    uoM:         form.uoM,
    targetPrice: form.targetPrice === '' ? null : Number(form.targetPrice),
    notes:       form.notes ?? '',
  })

  return (
    <div>
      <div className="flex items-center justify-between mb-3">
        <div className="text-sm text-gray-600">Line items requested</div>
        <button className="btn btn-primary btn-sm" onClick={() => { reset({ itemID: '', qty: 1, uoM: 'EA', targetPrice: '', notes: '' }); setOpen(true) }}>+ Add Line</button>
      </div>
      <div className="sh-card p-0 overflow-hidden">
        {isLoading ? <div className="py-8 flex justify-center"><Spinner /></div>
          : rows.length === 0 ? <EmptyState message="No lines yet." />
          : (
            <table className="sh-table">
              <thead><tr><th>Line ID</th><th>Item ID</th><th>Qty</th><th>UoM</th><th>Target Price</th><th>Notes</th></tr></thead>
              <tbody>{rows.map(l => (
                <tr key={l.rfxLineID}><td className="text-gray-400 text-xs">{l.rfxLineID}</td>
                  <td>{l.itemID}</td><td>{l.qty}</td><td>{l.uoM}</td><td>{l.targetPrice}</td><td className="max-w-xs truncate text-sm">{l.notes}</td></tr>
              ))}</tbody>
            </table>
          )}
      </div>

      {open && (
        <Modal title="Add RFx Line" onClose={() => setOpen(false)}
          footer={<><button className="btn btn-secondary btn-sm" onClick={() => setOpen(false)}>Cancel</button>
            <button className="btn btn-primary btn-sm" onClick={handleSubmit(onSubmit)} disabled={addMut.isPending}>{addMut.isPending ? 'Adding…' : 'Add'}</button></>}>
          <form className="space-y-3" noValidate>
            <div><label className="sh-label">Item ID *</label><input type="number" className="sh-input" {...register('itemID', { required: true })} /></div>
            <div className="grid grid-cols-2 gap-3">
              <div><label className="sh-label">Qty</label><input type="number" step="0.01" className="sh-input" {...register('qty')} /></div>
              <div><label className="sh-label">UoM</label><input className="sh-input" {...register('uoM')} /></div>
            </div>
            <div><label className="sh-label">Target Price</label><input type="number" step="0.01" className="sh-input" {...register('targetPrice')} /></div>
            <div><label className="sh-label">Notes</label><textarea rows={2} className="sh-input" {...register('notes')} /></div>
          </form>
        </Modal>
      )}
    </div>
  )
}

/* ─── Invites ─── */
function InvitesTab({ rfxId }) {
  const qc = useQueryClient()
  const [open, setOpen] = useState(false)
  const { register, handleSubmit, reset } = useForm()
  const { data, isLoading } = useQuery({ queryKey: ['rfx-invites', rfxId], queryFn: () => rfxApi.getInvites(rfxId) })
  const rows = (data?.data ?? data ?? [])

  const addMut = useMutation({
    mutationFn: rfxApi.addInvite,
    onSuccess: () => { qc.invalidateQueries(['rfx-invites', rfxId]); setOpen(false); reset(); toast.success('Supplier invited') },
    onError: e => toast.error(extract(e) ?? 'Failed to invite'),
  })

  const onSubmit = (form) => addMut.mutate({
    rfxID:       rfxId,
    supplierID:  Number(form.supplierID),
    invitedDate: new Date().toISOString(),
    status:      form.status ?? 'Invited',
  })

  return (
    <div>
      <div className="flex items-center justify-between mb-3">
        <div className="text-sm text-gray-600">Suppliers invited to this RFx</div>
        <button className="btn btn-primary btn-sm" onClick={() => { reset({ supplierID: '', status: 'Invited' }); setOpen(true) }}>+ Invite Supplier</button>
      </div>
      <div className="sh-card p-0 overflow-hidden">
        {isLoading ? <div className="py-8 flex justify-center"><Spinner /></div>
          : rows.length === 0 ? <EmptyState message="No suppliers invited." />
          : (
            <table className="sh-table">
              <thead><tr><th>Invite ID</th><th>Supplier ID</th><th>Invited Date</th><th>Status</th></tr></thead>
              <tbody>{rows.map(i => (
                <tr key={i.inviteID}><td className="text-gray-400 text-xs">{i.inviteID}</td><td>{i.supplierID}</td>
                  <td className="text-xs text-gray-500">{i.invitedDate ? new Date(i.invitedDate).toLocaleDateString() : '—'}</td>
                  <td><StatusPill status={i.status} /></td>
                </tr>
              ))}</tbody>
            </table>
          )}
      </div>

      {open && (
        <Modal title="Invite Supplier" onClose={() => setOpen(false)}
          footer={<><button className="btn btn-secondary btn-sm" onClick={() => setOpen(false)}>Cancel</button>
            <button className="btn btn-primary btn-sm" onClick={handleSubmit(onSubmit)} disabled={addMut.isPending}>{addMut.isPending ? 'Adding…' : 'Invite'}</button></>}>
          <form className="space-y-3" noValidate>
            <div><label className="sh-label">Supplier ID *</label><input type="number" className="sh-input" {...register('supplierID', { required: true })} /></div>
            <div><label className="sh-label">Status</label>
              <select className="sh-select" {...register('status')}>
                <option>Invited</option><option>Accepted</option><option>Declined</option>
              </select>
            </div>
          </form>
        </Modal>
      )}
    </div>
  )
}

/* ─── Bids ─── */
function BidsTab({ rfxId }) {
  const qc = useQueryClient()
  const [open, setOpen] = useState(false)
  const [linesFor, setLinesFor] = useState(null)
  const { register, handleSubmit, reset } = useForm()
  const { data, isLoading } = useQuery({ queryKey: ['rfx-bids', rfxId], queryFn: () => rfxApi.getBids(rfxId) })
  const rows = (data?.data ?? data ?? [])

  const addMut = useMutation({
    mutationFn: rfxApi.addBid,
    onSuccess: () => { qc.invalidateQueries(['rfx-bids', rfxId]); setOpen(false); reset(); toast.success('Bid created') },
    onError: e => toast.error(extract(e) ?? 'Failed to create bid'),
  })

  const onSubmit = (form) => addMut.mutate({
    rfxID:      rfxId,
    supplierID: Number(form.supplierID),
    bidDate:    new Date().toISOString(),
    totalValue: Number(form.totalValue),
    currency:   form.currency || 'USD',
    status:     form.status ?? 'Submitted',
  })

  return (
    <div>
      <div className="flex items-center justify-between mb-3">
        <div className="text-sm text-gray-600">Supplier bids submitted</div>
        <button className="btn btn-primary btn-sm" onClick={() => { reset({ supplierID: '', totalValue: '', currency: 'USD', status: 'Submitted' }); setOpen(true) }}>+ Add Bid</button>
      </div>
      <div className="sh-card p-0 overflow-hidden">
        {isLoading ? <div className="py-8 flex justify-center"><Spinner /></div>
          : rows.length === 0 ? <EmptyState message="No bids submitted." />
          : (
            <table className="sh-table">
              <thead><tr><th>Bid ID</th><th>Supplier ID</th><th>Bid Date</th><th>Total</th><th>Currency</th><th>Status</th><th>Lines</th></tr></thead>
              <tbody>{rows.map(b => (
                <tr key={b.bidID}>
                  <td className="text-gray-400 text-xs">{b.bidID}</td><td>{b.supplierID}</td>
                  <td className="text-xs text-gray-500">{b.bidDate ? new Date(b.bidDate).toLocaleDateString() : '—'}</td>
                  <td>{b.totalValue}</td><td>{b.currency}</td><td><StatusPill status={b.status} /></td>
                  <td><button className="btn btn-ghost btn-sm" onClick={() => setLinesFor(b.bidID)}>View / + Line</button></td>
                </tr>
              ))}</tbody>
            </table>
          )}
      </div>

      {open && (
        <Modal title="Add Bid" onClose={() => setOpen(false)}
          footer={<><button className="btn btn-secondary btn-sm" onClick={() => setOpen(false)}>Cancel</button>
            <button className="btn btn-primary btn-sm" onClick={handleSubmit(onSubmit)} disabled={addMut.isPending}>{addMut.isPending ? 'Adding…' : 'Add'}</button></>}>
          <form className="space-y-3" noValidate>
            <div><label className="sh-label">Supplier ID *</label><input type="number" className="sh-input" {...register('supplierID', { required: true })} /></div>
            <div><label className="sh-label">Total Value *</label><input type="number" step="0.01" className="sh-input" {...register('totalValue', { required: true })} /></div>
            <div><label className="sh-label">Currency</label><input className="sh-input" {...register('currency')} /></div>
            <div><label className="sh-label">Status</label>
              <select className="sh-select" {...register('status')}>
                <option>Submitted</option><option>Withdrawn</option>
              </select>
            </div>
          </form>
        </Modal>
      )}

      {linesFor && <BidLinesModal bidId={linesFor} onClose={() => setLinesFor(null)} />}
    </div>
  )
}

function BidLinesModal({ bidId, onClose }) {
  const qc = useQueryClient()
  const { register, handleSubmit, reset } = useForm()
  const { data, isLoading } = useQuery({ queryKey: ['bid-lines', bidId], queryFn: () => rfxApi.getBidLines(bidId) })
  const rows = (data?.data ?? data ?? [])

  const addMut = useMutation({
    mutationFn: rfxApi.addBidLine,
    onSuccess: () => { qc.invalidateQueries(['bid-lines', bidId]); reset(); toast.success('Bid line added') },
    onError: e => toast.error(extract(e) ?? 'Failed to add bid line'),
  })

  const onSubmit = (form) => addMut.mutate({
    bidID:        bidId,
    rfxLineID:    Number(form.rfxLineID),
    unitPrice:    Number(form.unitPrice),
    leadTimeDays: form.leadTimeDays === '' ? null : Number(form.leadTimeDays),
    currency:     form.currency || 'USD',
    notes:        form.notes ?? '',
  })

  return (
    <Modal title={`Bid #${bidId} — Lines`} onClose={onClose} footer={<button className="btn btn-secondary btn-sm" onClick={onClose}>Close</button>}>
      <div className="space-y-4">
        {isLoading ? <Spinner />
          : rows.length === 0 ? <p className="text-sm text-gray-500">No bid lines yet.</p>
          : (
            <table className="sh-table">
              <thead><tr><th>ID</th><th>RFx Line</th><th>Unit Price</th><th>Lead Time</th><th>Currency</th></tr></thead>
              <tbody>{rows.map(l => (
                <tr key={l.bidLineID}><td className="text-gray-400 text-xs">{l.bidLineID}</td>
                  <td>{l.rfxLineID}</td><td>{l.unitPrice}</td><td>{l.leadTimeDays}</td><td>{l.currency}</td></tr>
              ))}</tbody>
            </table>
          )}

        <div className="border-t pt-4">
          <h4 className="text-sm font-semibold text-gray-700 mb-2">Add Bid Line</h4>
          <form className="grid grid-cols-2 gap-2" onSubmit={handleSubmit(onSubmit)} noValidate>
            <input type="number" placeholder="RFx Line ID *" className="sh-input" {...register('rfxLineID', { required: true })} />
            <input type="number" step="0.01" placeholder="Unit Price *" className="sh-input" {...register('unitPrice', { required: true })} />
            <input type="number" placeholder="Lead Time (days)" className="sh-input" {...register('leadTimeDays')} />
            <input placeholder="Currency" defaultValue="USD" className="sh-input" {...register('currency')} />
            <input placeholder="Notes" className="sh-input col-span-2" {...register('notes')} />
            <button type="submit" className="btn btn-primary btn-sm col-span-2" disabled={addMut.isPending}>
              {addMut.isPending ? 'Adding…' : 'Add Bid Line'}
            </button>
          </form>
        </div>
      </div>
    </Modal>
  )
}

/* ─── Awards ─── */
function AwardsTab({ rfxId }) {
  const qc = useQueryClient()
  const [open, setOpen] = useState(false)
  const { register, handleSubmit, reset } = useForm()
  const { data, isLoading } = useQuery({ queryKey: ['rfx-awards', rfxId], queryFn: () => rfxApi.getAwards(rfxId) })
  const rows = (data?.data ?? data ?? [])

  const addMut = useMutation({
    mutationFn: rfxApi.addAward,
    onSuccess: () => { qc.invalidateQueries(['rfx-awards', rfxId]); setOpen(false); reset(); toast.success('Award created') },
    onError: e => toast.error(extract(e) ?? 'Failed to create award'),
  })

  const onSubmit = (form) => addMut.mutate({
    rfxID:      rfxId,
    supplierID: Number(form.supplierID),
    awardDate:  new Date().toISOString(),
    awardValue: Number(form.awardValue),
    currency:   form.currency || 'USD',
    notes:      form.notes ?? '',
    status:     form.status ?? 'Awarded',
  })

  return (
    <div>
      <div className="flex items-center justify-between mb-3">
        <div className="text-sm text-gray-600">Awards issued from this RFx</div>
        <button className="btn btn-primary btn-sm" onClick={() => { reset({ supplierID: '', awardValue: '', currency: 'USD', notes: '', status: 'Awarded' }); setOpen(true) }}>+ Add Award</button>
      </div>
      <div className="sh-card p-0 overflow-hidden">
        {isLoading ? <div className="py-8 flex justify-center"><Spinner /></div>
          : rows.length === 0 ? <EmptyState message="No awards yet." />
          : (
            <table className="sh-table">
              <thead><tr><th>Award ID</th><th>Supplier ID</th><th>Award Date</th><th>Value</th><th>Currency</th><th>Status</th><th>Notes</th></tr></thead>
              <tbody>{rows.map(a => (
                <tr key={a.awardID}>
                  <td className="text-gray-400 text-xs">{a.awardID}</td><td>{a.supplierID}</td>
                  <td className="text-xs text-gray-500">{a.awardDate ? new Date(a.awardDate).toLocaleDateString() : '—'}</td>
                  <td>{a.awardValue}</td><td>{a.currency}</td>
                  <td><StatusPill status={a.status} /></td>
                  <td className="max-w-xs truncate text-sm">{a.notes ?? '—'}</td>
                </tr>
              ))}</tbody>
            </table>
          )}
      </div>

      {open && (
        <Modal title="Add Award" onClose={() => setOpen(false)}
          footer={<><button className="btn btn-secondary btn-sm" onClick={() => setOpen(false)}>Cancel</button>
            <button className="btn btn-primary btn-sm" onClick={handleSubmit(onSubmit)} disabled={addMut.isPending}>{addMut.isPending ? 'Adding…' : 'Add'}</button></>}>
          <form className="space-y-3" noValidate>
            <div><label className="sh-label">Supplier ID *</label><input type="number" className="sh-input" {...register('supplierID', { required: true })} /></div>
            <div><label className="sh-label">Award Value *</label><input type="number" step="0.01" className="sh-input" {...register('awardValue', { required: true })} /></div>
            <div><label className="sh-label">Currency</label><input className="sh-input" {...register('currency')} /></div>
            <div><label className="sh-label">Status</label>
              <select className="sh-select" {...register('status')}>
                <option>Awarded</option><option>Pending</option><option>Cancelled</option>
              </select>
            </div>
            <div><label className="sh-label">Notes</label><textarea rows={2} className="sh-input" {...register('notes')} /></div>
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
