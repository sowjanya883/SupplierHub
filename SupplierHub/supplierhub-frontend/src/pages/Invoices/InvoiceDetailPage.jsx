import { useState } from 'react'
import { useParams, useNavigate } from 'react-router-dom'
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query'
import { useForm } from 'react-hook-form'
import toast from 'react-hot-toast'
import { invoicesApi, invoiceLinesApi, matchRefsApi } from '../../api/operations.api'
import { StatusPill } from '../../components/ui/StatusPill'
import { Spinner, EmptyState } from '../../components/ui/index'
import Modal from '../../components/ui/Modal'

export default function InvoiceDetailPage() {
  const { id } = useParams()
  const navigate = useNavigate()
  const [tab, setTab] = useState('Lines')

  const { data, isLoading } = useQuery({ queryKey: ['invoice', id], queryFn: () => invoicesApi.getById(id) })

  if (isLoading) return <div className="flex justify-center py-16"><Spinner /></div>
  const inv = data?.data ?? data
  if (!inv) return <p>Invoice not found.</p>

  return (
    <div>
      <div className="page-header">
        <div>
          <button onClick={() => navigate(-1)} className="text-sm text-blue-600 hover:underline mb-1 block">← Back</button>
          <h1 className="page-title">{inv.invoiceNo}</h1>
        </div>
        <StatusPill status={inv.status} />
      </div>

      <div className="sh-card mb-4">
        <div className="grid grid-cols-3 gap-4 text-sm">
          <div><span className="text-gray-500">Invoice ID</span><p className="font-medium mt-0.5">#{inv.invoiceID}</p></div>
          <div><span className="text-gray-500">Supplier ID</span><p className="font-medium mt-0.5">{inv.supplierID}</p></div>
          <div><span className="text-gray-500">PO ID</span><p className="font-medium mt-0.5">{inv.poID || '—'}</p></div>
          <div><span className="text-gray-500">Invoice Date</span><p className="font-medium mt-0.5">{inv.invoiceDate ? new Date(inv.invoiceDate).toLocaleDateString() : '—'}</p></div>
          <div><span className="text-gray-500">Total Amount</span><p className="font-medium mt-0.5">{inv.totalAmount} {inv.currency}</p></div>
        </div>
      </div>

      <div className="flex gap-1 mb-4 border-b border-gray-200">
        {['Lines','Match Refs'].map(t => (
          <button key={t} onClick={() => setTab(t)}
            className={`px-4 py-2 text-sm font-medium border-b-2 transition-colors ${
              tab === t ? 'border-blue-600 text-blue-700' : 'border-transparent text-gray-500 hover:text-gray-700'}`}>
            {t}
          </button>
        ))}
      </div>

      {tab === 'Lines'      && <LinesTab invoiceId={Number(id)} />}
      {tab === 'Match Refs' && <MatchRefsSection invoiceId={Number(id)} poId={inv.poID} />}
    </div>
  )
}

/* ─── Invoice Lines ─── */
function LinesTab({ invoiceId }) {
  const qc = useQueryClient()
  const [open, setOpen] = useState(false)
  const { register, handleSubmit, reset } = useForm()
  const { data, isLoading } = useQuery({ queryKey: ['inv-lines', invoiceId], queryFn: () => invoiceLinesApi.getByInvoiceId(invoiceId) })
  const rows = (data?.data ?? data ?? [])

  const addMut = useMutation({
    mutationFn: invoiceLinesApi.create,
    onSuccess: () => { qc.invalidateQueries(['inv-lines', invoiceId]); setOpen(false); reset(); toast.success('Line added') },
    onError: e => toast.error(extract(e) ?? 'Failed to add line'),
  })

  const onSubmit = (form) => {
    const qty = Number(form.qty)
    const unitPrice = Number(form.unitPrice)
    addMut.mutate({
      invoiceID:   invoiceId,
      poLineID:    Number(form.poLineID),
      qty,
      unitPrice,
      lineTotal:   qty * unitPrice,
      taxJSON:     form.taxJSON?.trim() || '{}',
      matchStatus: form.matchStatus,
    })
  }

  return (
    <div>
      <div className="flex items-center justify-between mb-3">
        <div className="text-sm text-gray-600">Invoice line items</div>
        <button className="btn btn-primary btn-sm" onClick={() => { reset({ poLineID: '', qty: 1, unitPrice: 0, taxJSON: '{}', matchStatus: 'Pending' }); setOpen(true) }}>+ Add Line</button>
      </div>

      <div className="sh-card p-0 overflow-hidden">
        {isLoading ? <div className="py-8 flex justify-center"><Spinner /></div>
          : rows.length === 0 ? <EmptyState message="No lines yet." />
          : (
            <table className="sh-table">
              <thead><tr><th>Line ID</th><th>PO Line ID</th><th>Qty</th><th>Unit Price</th><th>Line Total</th><th>Match Status</th></tr></thead>
              <tbody>
                {rows.map(l => (
                  <tr key={l.invLineID}>
                    <td className="text-gray-400 text-xs">{l.invLineID}</td>
                    <td>{l.poLineID}</td>
                    <td>{l.qty}</td><td>{l.unitPrice}</td><td>{l.lineTotal}</td>
                    <td><StatusPill status={l.matchStatus} /></td>
                  </tr>
                ))}
              </tbody>
            </table>
          )}
      </div>

      {open && (
        <Modal title="Add Invoice Line" onClose={() => setOpen(false)}
          footer={<><button className="btn btn-secondary btn-sm" onClick={() => setOpen(false)}>Cancel</button>
            <button className="btn btn-primary btn-sm" onClick={handleSubmit(onSubmit)} disabled={addMut.isPending}>{addMut.isPending ? 'Adding…' : 'Add'}</button></>}>
          <form className="space-y-3" noValidate>
            <div><label className="sh-label">PO Line ID *</label><input type="number" className="sh-input" {...register('poLineID', { required: true })} /></div>
            <div className="grid grid-cols-2 gap-3">
              <div><label className="sh-label">Qty *</label><input type="number" step="0.01" className="sh-input" {...register('qty', { required: true })} /></div>
              <div><label className="sh-label">Unit Price *</label><input type="number" step="0.01" className="sh-input" {...register('unitPrice', { required: true })} /></div>
            </div>
            <div><label className="sh-label">Tax (JSON)</label><input className="sh-input font-mono" {...register('taxJSON')} /></div>
            <div><label className="sh-label">Match Status</label>
              <select className="sh-select" {...register('matchStatus')}>
                <option>Pending</option><option>Matched</option><option>Mismatch</option>
              </select>
            </div>
          </form>
        </Modal>
      )}
    </div>
  )
}

/* ─── MatchRef create + edit (uses backend MatchResult / MatchRefStatus enum names) ─── */
function MatchRefsSection({ invoiceId, poId }) {
  const qc = useQueryClient()
  const [modalOpen, setModalOpen] = useState(false)
  const [editing, setEditing] = useState(null)

  const { data, isLoading } = useQuery({
    queryKey: ['match-refs', invoiceId],
    queryFn: () => matchRefsApi.getByInvoiceId(invoiceId),
  })
  const rows = (data?.data ?? data ?? []).filter(m => !m.isDeleted)

  const { register, handleSubmit, reset, setValue } = useForm()

  const createMut = useMutation({
    mutationFn: matchRefsApi.create,
    onSuccess: () => { qc.invalidateQueries(['match-refs', invoiceId]); close(); toast.success('Match recorded') },
    onError: e => toast.error(extract(e) ?? 'Failed to create match'),
  })
  const updateMut = useMutation({
    mutationFn: ({ id, dto }) => matchRefsApi.update(id, dto),
    onSuccess: () => { qc.invalidateQueries(['match-refs', invoiceId]); close(); toast.success('Match updated') },
    onError: e => toast.error(extract(e) ?? 'Failed to update match'),
  })

  const open = (m) => {
    setEditing(m ?? null)
    if (m) {
      setValue('poID',   m.poID ?? poId ?? '')
      setValue('grnID',  m.grnID ?? '')
      setValue('result', m.result ?? 'Matched')
      setValue('notes',  m.notes ?? '')
      setValue('status', m.status ?? 'Active')
    } else {
      reset({ poID: poId ?? '', grnID: '', result: 'Matched', notes: '', status: 'Active' })
    }
    setModalOpen(true)
  }
  const close = () => { setModalOpen(false); setEditing(null); reset() }

  const onSubmit = (form) => {
    const dto = {
      invoiceID: invoiceId,
      poID:      form.poID === '' ? null : Number(form.poID),
      grnID:     form.grnID === '' ? null : Number(form.grnID),
      result:    form.result,
      notes:     form.notes ?? '',
      status:    form.status,
    }
    if (editing) {
      updateMut.mutate({ id: editing.matchID, dto: { ...dto, matchID: editing.matchID } })
    } else {
      createMut.mutate(dto)
    }
  }

  return (
    <div>
      <div className="flex items-center justify-between mb-3">
        <div className="text-sm text-gray-600">PO ↔ GRN ↔ Invoice match records</div>
        <button className="btn btn-primary btn-sm" onClick={() => open(null)}>+ New Match</button>
      </div>

      <div className="sh-card p-0 overflow-hidden">
        {isLoading ? <div className="py-12 flex justify-center"><Spinner /></div>
          : rows.length === 0 ? <EmptyState message="No match records yet." />
          : (
            <table className="sh-table">
              <thead><tr><th>Match ID</th><th>PO ID</th><th>GRN ID</th><th>Result</th><th>Status</th><th>Notes</th><th>Actions</th></tr></thead>
              <tbody>
                {rows.map(m => (
                  <tr key={m.matchID}>
                    <td className="text-gray-400 text-xs">{m.matchID}</td>
                    <td>{m.poID ?? '—'}</td>
                    <td>{m.grnID ?? '—'}</td>
                    <td><span className={`pill ${m.result === 'Matched' ? 'pill-green' : m.result === 'Mismatch' ? 'pill-red' : 'pill-amber'}`}>{m.result}</span></td>
                    <td><StatusPill status={m.status} /></td>
                    <td className="max-w-xs truncate text-sm">{m.notes ?? '—'}</td>
                    <td>
                      <button className="btn btn-ghost btn-sm" onClick={() => open(m)}>Edit</button>
                    </td>
                  </tr>
                ))}
              </tbody>
            </table>
          )}
      </div>

      {modalOpen && (
        <Modal title={editing ? `Edit Match #${editing.matchID}` : 'New Match Record'} onClose={close}
          footer={
            <>
              <button className="btn btn-secondary btn-sm" onClick={close}>Cancel</button>
              <button className="btn btn-primary btn-sm" onClick={handleSubmit(onSubmit)} disabled={createMut.isPending || updateMut.isPending}>
                {(createMut.isPending || updateMut.isPending) ? 'Saving…' : editing ? 'Update' : 'Create'}
              </button>
            </>
          }>
          <form className="space-y-3" noValidate>
            <div>
              <label className="sh-label">PO ID</label>
              <input type="number" className="sh-input" {...register('poID')} />
            </div>
            <div>
              <label className="sh-label">GRN ID</label>
              <input type="number" className="sh-input" {...register('grnID')} />
            </div>
            <div>
              <label className="sh-label">Result *</label>
              <select className="sh-select" {...register('result', { required: true })}>
                <option>Matched</option>
                <option>Mismatch</option>
                <option>Partial</option>
                <option>Pending</option>
              </select>
            </div>
            <div>
              <label className="sh-label">Status</label>
              <select className="sh-select" {...register('status')}>
                <option>Active</option><option>Inactive</option><option>Closed</option>
              </select>
            </div>
            <div>
              <label className="sh-label">Notes</label>
              <textarea className="sh-input" rows={3} {...register('notes')} />
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
