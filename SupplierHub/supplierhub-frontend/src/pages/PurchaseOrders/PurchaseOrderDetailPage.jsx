import { useState } from 'react'
import { useParams, useNavigate } from 'react-router-dom'
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query'
import { useForm } from 'react-hook-form'
import toast from 'react-hot-toast'
import { purchaseOrdersApi, poLinesApi, poAcksApi, poRevisionsApi } from '../../api/procurement.api'
import { StatusPill } from '../../components/ui/StatusPill'
import { Spinner, EmptyState } from '../../components/ui/index'
import Modal from '../../components/ui/Modal'

const TABS = ['Lines', 'Acknowledgements', 'Revisions']

export default function PurchaseOrderDetailPage() {
  const { id } = useParams()
  const navigate = useNavigate()
  const [tab, setTab] = useState('Lines')

  const { data, isLoading } = useQuery({ queryKey: ['po', id], queryFn: () => purchaseOrdersApi.getById(id) })

  if (isLoading) return <div className="flex justify-center py-16"><Spinner /></div>
  const po = data?.data ?? data
  if (!po) return <p>PO not found.</p>

  const poId = Number(id)

  return (
    <div>
      <div className="page-header">
        <div>
          <button onClick={() => navigate(-1)} className="text-sm text-blue-600 hover:underline mb-1 block">← Back</button>
          <h1 className="page-title">Purchase Order #{po.poID ?? po.poid}</h1>
        </div>
        <StatusPill status={po.status} />
      </div>

      <div className="sh-card mb-4">
        <div className="grid grid-cols-3 gap-4 text-sm">
          <div><span className="text-gray-500">Supplier ID</span><p className="font-medium mt-0.5">{po.supplierID}</p></div>
          <div><span className="text-gray-500">Org ID</span><p className="font-medium mt-0.5">{po.orgID}</p></div>
          <div><span className="text-gray-500">PO Date</span><p className="font-medium mt-0.5">{po.poDate ? new Date(po.poDate).toLocaleDateString() : '—'}</p></div>
          <div><span className="text-gray-500">Currency</span><p className="font-medium mt-0.5">{po.currency}</p></div>
          <div><span className="text-gray-500">Incoterms</span><p className="font-medium mt-0.5">{po.incoterms || '—'}</p></div>
          <div><span className="text-gray-500">Payment Terms</span><p className="font-medium mt-0.5">{po.paymentTerms || '—'}</p></div>
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

      {tab === 'Lines'           && <LinesTab     poId={poId} supplierId={po.supplierID} />}
      {tab === 'Acknowledgements'&& <AcksTab      poId={poId} supplierId={po.supplierID} />}
      {tab === 'Revisions'       && <RevisionsTab poId={poId} />}
    </div>
  )
}

/* ─── Lines ─── */
function LinesTab({ poId }) {
  const qc = useQueryClient()
  const [open, setOpen] = useState(false)
  const { register, handleSubmit, reset } = useForm()
  const { data, isLoading } = useQuery({ queryKey: ['po-lines', poId], queryFn: () => poLinesApi.getByPoId(poId) })
  const rows = (data?.data ?? data ?? [])

  const addMut = useMutation({
    mutationFn: poLinesApi.create,
    onSuccess: () => { qc.invalidateQueries(['po-lines', poId]); setOpen(false); reset(); toast.success('Line added') },
    onError: e => toast.error(extract(e) ?? 'Failed to add line'),
  })

  const onSubmit = (form) => {
    const qty = Number(form.qty)
    const unitPrice = Number(form.unitPrice)
    addMut.mutate({
      poID:         poId,
      itemID:       Number(form.itemID),
      description:  form.description ?? '',
      qty,
      uoM:          form.uoM,
      unitPrice,
      lineTotal:    qty * unitPrice,
      deliveryDate: form.deliveryDate ? new Date(form.deliveryDate).toISOString() : null,
      status:       form.status ?? 'Open',
    })
  }

  return (
    <div>
      <div className="flex items-center justify-between mb-3">
        <div className="text-sm text-gray-600">PO line items</div>
        <button className="btn btn-primary btn-sm" onClick={() => { reset({ itemID: '', description: '', qty: 1, uoM: 'EA', unitPrice: 0, deliveryDate: '', status: 'Open' }); setOpen(true) }}>+ Add Line</button>
      </div>
      <div className="sh-card p-0 overflow-hidden">
        {isLoading ? <div className="py-8 flex justify-center"><Spinner /></div>
          : rows.length === 0 ? <EmptyState message="No lines yet." />
          : (
            <table className="sh-table">
              <thead><tr><th>Line ID</th><th>Item</th><th>Description</th><th>Qty</th><th>UoM</th><th>Unit Price</th><th>Line Total</th><th>Status</th></tr></thead>
              <tbody>{rows.map(l => (
                <tr key={l.poLineID ?? l.poLineId}>
                  <td className="text-gray-400 text-xs">{l.poLineID ?? l.poLineId}</td>
                  <td>{l.itemID}</td><td className="max-w-xs truncate text-sm">{l.description}</td>
                  <td>{l.qty}</td><td>{l.uoM}</td><td>{l.unitPrice}</td><td>{l.lineTotal}</td>
                  <td><StatusPill status={l.status} /></td>
                </tr>
              ))}</tbody>
            </table>
          )}
      </div>

      {open && (
        <Modal title="Add PO Line" onClose={() => setOpen(false)}
          footer={<><button className="btn btn-secondary btn-sm" onClick={() => setOpen(false)}>Cancel</button>
            <button className="btn btn-primary btn-sm" onClick={handleSubmit(onSubmit)} disabled={addMut.isPending}>{addMut.isPending ? 'Adding…' : 'Add'}</button></>}>
          <form className="space-y-3" noValidate>
            <div className="grid grid-cols-2 gap-3">
              <div><label className="sh-label">Item ID *</label><input type="number" className="sh-input" {...register('itemID', { required: true })} /></div>
              <div><label className="sh-label">UoM</label><input className="sh-input" {...register('uoM')} /></div>
            </div>
            <div><label className="sh-label">Description</label><input className="sh-input" {...register('description')} /></div>
            <div className="grid grid-cols-2 gap-3">
              <div><label className="sh-label">Qty *</label><input type="number" step="0.01" className="sh-input" {...register('qty', { required: true })} /></div>
              <div><label className="sh-label">Unit Price *</label><input type="number" step="0.01" className="sh-input" {...register('unitPrice', { required: true })} /></div>
            </div>
            <div><label className="sh-label">Delivery Date</label><input type="date" className="sh-input" {...register('deliveryDate')} /></div>
            <div><label className="sh-label">Status</label>
              <select className="sh-select" {...register('status')}>
                <option>Open</option><option>Shipped</option><option>Closed</option><option>Cancelled</option>
              </select>
            </div>
          </form>
        </Modal>
      )}
    </div>
  )
}

/* ─── Acknowledgements ─── */
function AcksTab({ poId, supplierId }) {
  const qc = useQueryClient()
  const [open, setOpen] = useState(false)
  const { register, handleSubmit, reset } = useForm()
  const { data, isLoading } = useQuery({ queryKey: ['po-acks'], queryFn: poAcksApi.getAll })
  const rows = (data?.data ?? data ?? []).filter(a => a.poID == poId)

  const addMut = useMutation({
    mutationFn: poAcksApi.create,
    onSuccess: () => { qc.invalidateQueries(['po-acks']); setOpen(false); reset(); toast.success('Acknowledgement recorded') },
    onError: e => toast.error(extract(e) ?? 'Failed to record acknowledgement'),
  })

  const onSubmit = (form) => addMut.mutate({
    poID:            poId,
    supplierID:      Number(form.supplierID),
    acknowledgeDate: new Date().toISOString(),
    decision:        form.decision,
    counterNotes:    form.counterNotes ?? '',
    status:          form.status ?? 'Active',
  })

  return (
    <div>
      <div className="flex items-center justify-between mb-3">
        <div className="text-sm text-gray-600">Supplier acknowledgements</div>
        <button className="btn btn-primary btn-sm" onClick={() => { reset({ supplierID: supplierId ?? '', decision: 'Accept', counterNotes: '', status: 'Active' }); setOpen(true) }}>+ Add Ack</button>
      </div>
      <div className="sh-card p-0 overflow-hidden">
        {isLoading ? <div className="py-8 flex justify-center"><Spinner /></div>
          : rows.length === 0 ? <EmptyState message="No acknowledgements." />
          : (
            <table className="sh-table">
              <thead><tr><th>Ack ID</th><th>Supplier ID</th><th>Decision</th><th>Date</th><th>Notes</th></tr></thead>
              <tbody>{rows.map(a => (
                <tr key={a.pocfmID}>
                  <td className="text-gray-400 text-xs">{a.pocfmID}</td><td>{a.supplierID}</td>
                  <td><StatusPill status={a.decision} /></td>
                  <td className="text-xs text-gray-500">{a.acknowledgeDate ? new Date(a.acknowledgeDate).toLocaleDateString() : '—'}</td>
                  <td className="max-w-xs truncate text-sm">{a.counterNotes || '—'}</td>
                </tr>
              ))}</tbody>
            </table>
          )}
      </div>

      {open && (
        <Modal title="Add PO Acknowledgement" onClose={() => setOpen(false)}
          footer={<><button className="btn btn-secondary btn-sm" onClick={() => setOpen(false)}>Cancel</button>
            <button className="btn btn-primary btn-sm" onClick={handleSubmit(onSubmit)} disabled={addMut.isPending}>{addMut.isPending ? 'Saving…' : 'Save'}</button></>}>
          <form className="space-y-3" noValidate>
            <div><label className="sh-label">Supplier ID *</label><input type="number" className="sh-input" {...register('supplierID', { required: true })} /></div>
            <div><label className="sh-label">Decision *</label>
              <select className="sh-select" {...register('decision', { required: true })}>
                <option>Accept</option><option>Counter</option><option>Decline</option>
              </select>
            </div>
            <div><label className="sh-label">Counter Notes</label><textarea rows={3} className="sh-input" {...register('counterNotes')} /></div>
            <div><label className="sh-label">Status</label>
              <select className="sh-select" {...register('status')}>
                <option>Active</option><option>Inactive</option>
              </select>
            </div>
          </form>
        </Modal>
      )}
    </div>
  )
}

/* ─── Revisions ─── */
function RevisionsTab({ poId }) {
  const qc = useQueryClient()
  const [open, setOpen] = useState(false)
  const { register, handleSubmit, reset } = useForm()
  const { data, isLoading } = useQuery({ queryKey: ['po-revs', poId], queryFn: () => poRevisionsApi.getByPoId(poId) })
  const rows = (data?.data ?? data ?? [])

  const addMut = useMutation({
    mutationFn: poRevisionsApi.create,
    onSuccess: () => { qc.invalidateQueries(['po-revs', poId]); setOpen(false); reset(); toast.success('Revision recorded') },
    onError: e => toast.error(extract(e) ?? 'Failed to record revision'),
  })

  const onSubmit = (form) => addMut.mutate({
    poID:           poId,
    revisionNo:     Number(form.revisionNo),
    changedBy:      form.changedBy?.trim() || 'system',
    changeLogJSON:  form.changeLogJSON?.trim() || '{}',
    changeDate:     new Date().toISOString(),
  })

  return (
    <div>
      <div className="flex items-center justify-between mb-3">
        <div className="text-sm text-gray-600">PO change history</div>
        <button className="btn btn-primary btn-sm" onClick={() => { reset({ revisionNo: (rows.length || 0) + 1, changedBy: '', changeLogJSON: '{}' }); setOpen(true) }}>+ Add Revision</button>
      </div>
      <div className="sh-card p-0 overflow-hidden">
        {isLoading ? <div className="py-8 flex justify-center"><Spinner /></div>
          : rows.length === 0 ? <EmptyState message="No revisions." />
          : (
            <table className="sh-table">
              <thead><tr><th>Rev ID</th><th>Rev No</th><th>Changed By</th><th>Change Date</th><th>Change Log</th></tr></thead>
              <tbody>{rows.map(r => (
                <tr key={r.porevID}>
                  <td className="text-gray-400 text-xs">{r.porevID}</td><td>{r.revisionNo}</td>
                  <td>{r.changedBy}</td>
                  <td className="text-xs text-gray-500">{r.changeDate ? new Date(r.changeDate).toLocaleDateString() : '—'}</td>
                  <td className="font-mono text-xs max-w-xs truncate">{r.changeLogJSON}</td>
                </tr>
              ))}</tbody>
            </table>
          )}
      </div>

      {open && (
        <Modal title="Add PO Revision" onClose={() => setOpen(false)}
          footer={<><button className="btn btn-secondary btn-sm" onClick={() => setOpen(false)}>Cancel</button>
            <button className="btn btn-primary btn-sm" onClick={handleSubmit(onSubmit)} disabled={addMut.isPending}>{addMut.isPending ? 'Saving…' : 'Save'}</button></>}>
          <form className="space-y-3" noValidate>
            <div><label className="sh-label">Revision No *</label><input type="number" className="sh-input" {...register('revisionNo', { required: true })} /></div>
            <div><label className="sh-label">Changed By</label><input className="sh-input" placeholder="username or system" {...register('changedBy')} /></div>
            <div><label className="sh-label">Change Log (JSON)</label><textarea rows={4} className="sh-input font-mono text-xs" {...register('changeLogJSON')} /></div>
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
