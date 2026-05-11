import { useState } from 'react'
import { useParams, useNavigate } from 'react-router-dom'
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query'
import { useForm } from 'react-hook-form'
import toast from 'react-hot-toast'
import {
  purchaseOrdersApi,
  poLinesApi,
  poAcksApi,
  poRevisionsApi,
} from '../../api/procurement.api'
import { suppliersApi } from '../../api/suppliers.api'
import { itemsApi } from '../../api/catalog.api'
import { StatusPill } from '../../components/ui/StatusPill'
import { Spinner, EmptyState, ConfirmDialog } from '../../components/ui/index'
import Modal from '../../components/ui/Modal'
import useAuthStore from '../../store/auth.store'
 
// ── Enums from backend ─────────────────────────────────
const PO_STATUSES    = ['Open','Acknowledged','Partially_Shipped','Shipped','Closed','Cancelled']
const LINE_STATUSES  = ['Active','Cancelled','Recieved','Returned','Closed']
const ACK_DECISIONS  = ['Accept','Reject','Counter']
const ACK_STATUSES   = ['Active','Accepted','Rejected','Countered']
const REV_STATUSES   = ['Active','Cancelled','Completed']
 
const statusColor = (s) => {
  const m = {
    Open: 'pill-blue', Acknowledged: 'pill-teal',
    Partially_Shipped: 'pill-amber', Shipped: 'pill-purple',
    Closed: 'pill-green', Cancelled: 'pill-red',
    Active: 'pill-blue', Accepted: 'pill-green',
    Rejected: 'pill-red', Countered: 'pill-amber',
    Completed: 'pill-green', Recieved: 'pill-teal',
    Returned: 'pill-orange',
  }
  return `pill ${m[s] ?? 'pill-gray'}`
}
 
const TABS = ['Lines', 'Acknowledgements', 'Revisions']
 
// ─────────────────────────────────────────────────────────
export default function PurchaseOrderDetailPage() {
  const { id }     = useParams()
  const navigate   = useNavigate()
  const { user }   = useAuthStore()
  const [tab, setTab] = useState('Lines')
 
  const isAdmin    = user?.roles?.includes('Admin')
  const isBuyer    = user?.roles?.includes('Buyer')
  const isSupplier = user?.roles?.includes('SupplierUser')
 
  // ── Fetch PO header ────────────────────────────────────
  const { data: poData, isLoading } = useQuery({
    queryKey: ['po', id],
    queryFn:  () => purchaseOrdersApi.getById(id),
  })
  const po = poData?.data ?? poData
 
  // ── Fetch supplier name ────────────────────────────────
  const { data: suppData } = useQuery({
    queryKey: ['suppliers'],
    queryFn: suppliersApi.getAll,
  })
  const suppliers = suppData?.data ?? suppData ?? []
  const getSupplierName = (sid) =>
    suppliers.find(s => s.supplierID === sid)?.legalName ?? `Supplier #${sid}`
 
  if (isLoading) return <div className="flex justify-center py-16"><Spinner /></div>
  if (!po) return <p className="text-gray-500 p-4">Purchase order not found.</p>
 
  return (
    <div>
      {/* ── Header ────────────────────────────────────────── */}
      <div className="mb-5">
        <button
          className="text-sm text-blue-600 hover:underline mb-2 block"
          onClick={() => navigate(-1)}
        >
          ← Back to Purchase Orders
        </button>
 
        <div className="sh-card">
          <div className="flex items-start justify-between">
            <div>
              <h1 className="text-xl font-semibold text-gray-900">PO-{po.poID}</h1>
              <p className="text-sm text-gray-500 mt-1">
                {getSupplierName(po.supplierID)}
              </p>
            </div>
            <span className={statusColor(po.status)}>{po.status}</span>
          </div>
 
          <div className="grid grid-cols-2 gap-4 mt-4 text-sm md:grid-cols-4">
            <Detail label="PO ID"         value={`PO-${po.poID}`} />
            <Detail label="Org ID"         value={po.orgID} />
            <Detail label="Supplier"       value={getSupplierName(po.supplierID)} />
            <Detail label="PO Date"        value={po.poDate ? new Date(po.poDate).toLocaleDateString() : '—'} />
            <Detail label="Currency"       value={po.currency ?? '—'} />
            <Detail label="Incoterms"      value={po.incoterms ?? '—'} />
            <Detail label="Payment Terms"  value={po.paymentTerms ?? '—'} />
            <Detail label="Created"        value={po.createdOn ? new Date(po.createdOn).toLocaleDateString() : '—'} />
          </div>
        </div>
      </div>
 
      {/* ── Tabs ──────────────────────────────────────────── */}
      <div className="flex gap-1 mb-4 border-b border-gray-200">
        {TABS.map(t => (
          <button
            key={t}
            onClick={() => setTab(t)}
            className={`px-4 py-2 text-sm font-medium border-b-2 transition-colors ${
              tab === t
                ? 'border-blue-600 text-blue-700'
                : 'border-transparent text-gray-500 hover:text-gray-700'
            }`}
          >
            {t}
          </button>
        ))}
      </div>
 
      {tab === 'Lines'            && <LinesTab           po={po} />}
      {tab === 'Acknowledgements' && <AcknowledgementsTab po={po} suppliers={suppliers} isSupplier={isSupplier} />}
      {tab === 'Revisions'        && <RevisionsTab        po={po} user={user} />}
    </div>
  )
}
 
// ── Small detail display ───────────────────────────────
function Detail({ label, value }) {
  return (
    <div>
      <p className="text-gray-500 text-xs font-medium">{label}</p>
      <p className="text-gray-800 font-medium mt-0.5 text-sm">{value}</p>
    </div>
  )
}
 
// ─────────────────────────────────────────────────────────
// LINES TAB
// ─────────────────────────────────────────────────────────
function LinesTab({ po }) {
  const qc = useQueryClient()
  const [modal, setModal]       = useState(null)   // 'form' | 'delete'
  const [selected, setSelected] = useState(null)
 
  // PoLine API uses PoId (lowercase d) — confirmed from DTO
  const { data, isLoading } = useQuery({
    queryKey: ['po-lines', po.poID],
    queryFn:  () => poLinesApi.getByPoId(po.poID),
  })
  const lines = data?.data ?? data ?? []
 
  const { data: itemsData } = useQuery({
    queryKey: ['items'],
    queryFn:  itemsApi.getAll,
  })
  const items = itemsData?.data ?? itemsData ?? []
 
  const { register, handleSubmit, reset, formState: { errors } } = useForm()
 
  const createMut = useMutation({
    mutationFn: poLinesApi.create,
    onSuccess: () => {
      qc.invalidateQueries(['po-lines', po.poID])
      setModal(null)
      toast.success('PO line added')
    },
    onError: e => toast.error(e.response?.data?.message ?? 'Failed to add line'),
  })
 
  const updateMut = useMutation({
    mutationFn: ({ id, dto }) => poLinesApi.update(id, dto),
    onSuccess: () => {
      qc.invalidateQueries(['po-lines', po.poID])
      setModal(null)
      toast.success('PO line updated')
    },
    onError: e => toast.error(e.response?.data?.message ?? 'Failed to update line'),
  })
 
  const deleteMut = useMutation({
    mutationFn: (id) => poLinesApi.delete(id),
    onSuccess: () => {
      qc.invalidateQueries(['po-lines', po.poID])
      setModal(null)
      toast.success('PO line deleted')
    },
    onError: e => toast.error(e.response?.data?.message ?? 'Failed to delete line'),
  })
 
  const openCreate = () => {
    reset({
      ItemId: '', Description: '', Qty: '', UoM: '',
      UnitPrice: '', LineTotal: '', DeliveryDate: '', Status: 'Active',
    })
    setSelected(null)
    setModal('form')
  }
 
  const openEdit = (row) => {
    setSelected(row)
    reset({
      // PoLineResponseDto uses PoLineId, PoId, ItemId, UoM
      ItemId:       row.itemId       ?? '',
      Description:  row.description  ?? '',
      Qty:          row.qty          ?? '',
      UoM:          row.uoM          ?? '',
      UnitPrice:    row.unitPrice    ?? '',
      LineTotal:    row.lineTotal    ?? '',
      DeliveryDate: row.deliveryDate?.split('T')[0] ?? '',
      Status:       row.status       ?? 'Active',
    })
    setModal('form')
  }
 
  const onSubmit = (d) => {
    const payload = {
      // Must use PoId (lowercase d) as per PoLineCreateDto
      PoId:         po.poID,
      ItemId:       d.ItemId ? Number(d.ItemId) : null,
      Description:  d.Description  || null,
      Qty:          d.Qty          ? Number(d.Qty)       : null,
      UoM:          d.UoM          || null,
      UnitPrice:    d.UnitPrice    ? Number(d.UnitPrice) : null,
      LineTotal:    d.LineTotal    ? Number(d.LineTotal) : null,
      DeliveryDate: d.DeliveryDate || null,
      Status:       d.Status,
    }
    if (selected) {
      updateMut.mutate({
        id:  selected.poLineId,
        dto: { ...payload, PoLineId: selected.poLineId },
      })
    } else {
      createMut.mutate(payload)
    }
  }
 
  const isPending = createMut.isPending || updateMut.isPending
  const getItemSku = (id) => items.find(i => i.itemID === id)?.sku ?? (id ? `#${id}` : '—')
 
  return (
    <div>
      <div className="flex justify-between items-center mb-3">
        <h4 className="text-sm font-medium text-gray-700">
          PO Lines — {lines.length} item{lines.length !== 1 ? 's' : ''}
        </h4>
        <button className="btn btn-primary btn-sm" onClick={openCreate}>
          + Add Line
        </button>
      </div>
 
      <div className="sh-card p-0 overflow-hidden">
        {isLoading ? (
          <div className="flex justify-center py-8"><Spinner /></div>
        ) : lines.length === 0 ? (
          <div className="text-center py-8">
            <p className="text-sm text-gray-400 mb-3">No lines added yet.</p>
            <button className="btn btn-primary btn-sm" onClick={openCreate}>
              + Add First Line
            </button>
          </div>
        ) : (
          <table className="sh-table">
            <thead>
              <tr>
                <th>Sr. No.</th>
                <th>Line ID</th>
                <th>Item</th>
                <th>Description</th>
                <th>Qty</th>
                <th>UoM</th>
                <th>Unit Price</th>
                <th>Line Total</th>
                <th>Delivery Date</th>
                <th>Status</th>
                <th>Actions</th>
              </tr>
            </thead>
            <tbody>
              {lines.map((l, i) => (
                <tr key={l.poLineId}>
                  <td className="text-gray-400 text-xs">{i + 1}</td>
                  <td className="text-gray-400 text-xs">{l.poLineId}</td>
                  <td className="font-mono text-xs font-medium">{getItemSku(l.itemId)}</td>
                  <td className="text-gray-500 text-xs max-w-xs truncate">{l.description ?? '—'}</td>
                  <td>{l.qty ?? '—'}</td>
                  <td>{l.uoM ?? '—'}</td>
                  <td className="font-medium text-green-700">{l.unitPrice?.toLocaleString() ?? '—'}</td>
                  <td className="font-medium">{l.lineTotal?.toLocaleString() ?? '—'}</td>
                  <td className="text-xs text-gray-500">
                    {l.deliveryDate ? new Date(l.deliveryDate).toLocaleDateString() : '—'}
                  </td>
                  <td><span className={statusColor(l.status)}>{l.status}</span></td>
                  <td>
                    <div className="flex gap-1">
                      <button className="btn btn-ghost btn-sm" onClick={() => openEdit(l)}>Edit</button>
                      <button className="btn btn-danger btn-sm"
                        onClick={() => { setSelected(l); setModal('delete') }}>Del</button>
                    </div>
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
        )}
      </div>
 
      {/* Form Modal */}
      {modal === 'form' && (
        <Modal
          title={selected ? `Edit Line #${selected.poLineId}` : 'Add PO Line'}
          onClose={() => setModal(null)}
          footer={
            <>
              <button className="btn btn-secondary btn-sm" onClick={() => setModal(null)}>Cancel</button>
              <button className="btn btn-primary btn-sm"
                onClick={handleSubmit(onSubmit)} disabled={isPending}>
                {isPending ? 'Saving…' : selected ? 'Update' : 'Add Line'}
              </button>
            </>
          }
        >
          <form className="space-y-3" noValidate>
            <div>
              <label className="sh-label">Item <span className="text-gray-400 font-normal">(optional)</span></label>
              <select className="sh-select" {...register('ItemId')}>
                <option value="">— Free-text description —</option>
                {items.map(i => (
                  <option key={i.itemID} value={i.itemID}>{i.sku}</option>
                ))}
              </select>
            </div>
            <div>
              <label className="sh-label">Description</label>
              <input className="sh-input" {...register('Description')} />
            </div>
            <div className="grid grid-cols-2 gap-3">
              <div>
                <label className="sh-label">Quantity</label>
                <input type="number" step="0.01" className="sh-input" {...register('Qty')} />
              </div>
              <div>
                <label className="sh-label">UoM</label>
                <input className="sh-input" placeholder="KG, PCS…" {...register('UoM')} />
              </div>
            </div>
            <div className="grid grid-cols-2 gap-3">
              <div>
                <label className="sh-label">Unit Price</label>
                <input type="number" step="0.01" className="sh-input" {...register('UnitPrice')} />
              </div>
              <div>
                <label className="sh-label">Line Total</label>
                <input type="number" step="0.01" className="sh-input" {...register('LineTotal')} />
              </div>
            </div>
            <div>
              <label className="sh-label">Delivery Date</label>
              <input type="date" className="sh-input" {...register('DeliveryDate')} />
            </div>
            <div>
              <label className="sh-label">Status *</label>
              <select className="sh-select" {...register('Status', { required: true })}>
                {LINE_STATUSES.map(s => <option key={s}>{s}</option>)}
              </select>
            </div>
          </form>
        </Modal>
      )}
 
      {modal === 'delete' && (
        <ConfirmDialog
          message={`Delete line #${selected?.poLineId}? This cannot be undone.`}
          onConfirm={() => deleteMut.mutate(selected.poLineId)}
          onCancel={() => setModal(null)}
          loading={deleteMut.isPending}
        />
      )}
    </div>
  )
}
 
// ─────────────────────────────────────────────────────────
// ACKNOWLEDGEMENTS TAB
// ─────────────────────────────────────────────────────────
function AcknowledgementsTab({ po, suppliers, isSupplier }) {
  const qc = useQueryClient()
  const [modal, setModal]       = useState(null)
  const [selected, setSelected] = useState(null)
 
  const { data, isLoading } = useQuery({
    queryKey: ['po-acks'],
    queryFn:  poAcksApi.getAll,
    // filter client-side by PoId (lowercase d from DTO)
    select: (d) => (d?.data ?? d ?? []).filter(a => a.poId === po.poID),
  })
  const acks = data ?? []
 
  const { register, handleSubmit, reset } = useForm()
 
  const createMut = useMutation({
    mutationFn: poAcksApi.create,
    onSuccess: () => {
      qc.invalidateQueries(['po-acks'])
      setModal(null)
      toast.success('Acknowledgement submitted')
    },
    onError: e => toast.error(e.response?.data?.message ?? 'Failed'),
  })
 
  const updateMut = useMutation({
    mutationFn: ({ id, dto }) => poAcksApi.update(id, dto),
    onSuccess: () => {
      qc.invalidateQueries(['po-acks'])
      setModal(null)
      toast.success('Acknowledgement updated')
    },
    onError: e => toast.error(e.response?.data?.message ?? 'Failed'),
  })
 
  const openCreate = () => {
    reset({
      SupplierId:       po.supplierID,
      AcknowledgeDate:  new Date().toISOString().split('T')[0],
      Decision:         'Accept',
      CounterNotes:     '',
      Status:           'Active',
    })
    setSelected(null)
    setModal('form')
  }
 
  const openEdit = (row) => {
    setSelected(row)
    reset({
      SupplierId:      row.supplierId ?? po.supplierID,
      AcknowledgeDate: row.acknowledgeDate?.split('T')[0] ?? '',
      // Decision may come as integer (0,1,2) or string — handle both
      Decision:        typeof row.decision === 'number'
        ? ACK_DECISIONS[row.decision - 1] ?? 'Accept'
        : row.decision ?? 'Accept',
      CounterNotes:    row.counterNotes ?? '',
      Status:          typeof row.status === 'number'
        ? ACK_STATUSES[row.status] ?? 'Active'
        : row.status ?? 'Active',
    })
    setModal('form')
  }
 
  const onSubmit = (d) => {
    // PoAckCreateDto uses PoId and SupplierId (lowercase d)
    const payload = {
      PoId:            po.poID,
      SupplierId:      Number(d.SupplierId),
      AcknowledgeDate: d.AcknowledgeDate || null,
      Decision:        d.Decision,
      CounterNotes:    d.CounterNotes || null,
      Status:          d.Status,
    }
    if (selected) {
      updateMut.mutate({
        id:  selected.pocfmID,
        dto: { ...payload, PocfmID: selected.pocfmID },
      })
    } else {
      createMut.mutate(payload)
    }
  }
 
  const isPending = createMut.isPending || updateMut.isPending
  const getSupplierName = (id) =>
    suppliers.find(s => s.supplierID === id)?.legalName ?? `Supplier #${id}`
 
  // Decode enum value (may come as int or string from backend)
  const decodeDecision = (v) =>
    typeof v === 'number' ? (ACK_DECISIONS[v - 1] ?? v) : (v ?? '—')
  const decodeStatus = (v) =>
    typeof v === 'number' ? (ACK_STATUSES[v] ?? v) : (v ?? '—')
 
  return (
    <div>
      <div className="flex justify-between items-center mb-3">
        <div>
          <h4 className="text-sm font-medium text-gray-700">Supplier Acknowledgements</h4>
          <p className="text-xs text-gray-400 mt-0.5">
            Supplier must Accept, Counter, or Reject this PO
          </p>
        </div>
        <button className="btn btn-primary btn-sm" onClick={openCreate}>
          + Add Acknowledgement
        </button>
      </div>
 
      <div className="sh-card p-0 overflow-hidden">
        {isLoading ? (
          <div className="flex justify-center py-8"><Spinner /></div>
        ) : acks.length === 0 ? (
          <div className="text-center py-8">
            <p className="text-sm text-gray-400 mb-1">No acknowledgements yet.</p>
            <p className="text-xs text-gray-400 mb-3">
              Supplier needs to Accept, Counter or Reject this PO.
            </p>
            <button className="btn btn-primary btn-sm" onClick={openCreate}>
              + Submit Acknowledgement
            </button>
          </div>
        ) : (
          <table className="sh-table">
            <thead>
              <tr>
                <th>Sr. No.</th>
                <th>Ack ID</th>
                <th>Supplier</th>
                <th>Ack Date</th>
                <th>Decision</th>
                <th>Counter Notes</th>
                <th>Status</th>
                <th>Actions</th>
              </tr>
            </thead>
            <tbody>
              {acks.map((a, i) => {
                const decision = decodeDecision(a.decision)
                const status   = decodeStatus(a.status)
                return (
                  <tr key={a.pocfmID}>
                    <td className="text-gray-400 text-xs">{i + 1}</td>
                    <td className="text-gray-400 text-xs">{a.pocfmID}</td>
                    <td className="font-medium">{getSupplierName(a.supplierId)}</td>
                    <td className="text-xs text-gray-500">
                      {a.acknowledgeDate ? new Date(a.acknowledgeDate).toLocaleDateString() : '—'}
                    </td>
                    <td>
                      <span className={`pill ${
                        decision === 'Accept' ? 'pill-green'
                        : decision === 'Reject' ? 'pill-red'
                        : 'pill-amber'
                      }`}>
                        {decision}
                      </span>
                    </td>
                    <td className="text-xs text-gray-500 max-w-xs truncate">
                      {a.counterNotes || '—'}
                    </td>
                    <td><span className={statusColor(status)}>{status}</span></td>
                    <td>
                      <button className="btn btn-ghost btn-sm" onClick={() => openEdit(a)}>
                        Edit
                      </button>
                    </td>
                  </tr>
                )
              })}
            </tbody>
          </table>
        )}
      </div>
 
      {modal === 'form' && (
        <Modal
          title={selected ? `Edit Ack #${selected.pocfmID}` : 'Submit Acknowledgement'}
          onClose={() => setModal(null)}
          footer={
            <>
              <button className="btn btn-secondary btn-sm" onClick={() => setModal(null)}>Cancel</button>
              <button className="btn btn-primary btn-sm"
                onClick={handleSubmit(onSubmit)} disabled={isPending}>
                {isPending ? 'Saving…' : selected ? 'Update' : 'Submit'}
              </button>
            </>
          }
        >
          <form className="space-y-3" noValidate>
            {/* Supplier ID (pre-filled from PO) */}
            <div>
              <label className="sh-label">Supplier ID</label>
              <input
                type="number"
                className="sh-input bg-gray-50"
                readOnly
                {...register('SupplierId')}
              />
              <p className="text-xs text-gray-400 mt-1">Auto-filled from PO supplier</p>
            </div>
 
            <div>
              <label className="sh-label">Acknowledge Date</label>
              <input type="date" className="sh-input" {...register('AcknowledgeDate')} />
            </div>
 
            {/* Decision — from PoAckDecision enum: Accept=1, Reject=2, Counter=3 */}
            <div>
              <label className="sh-label">Decision *</label>
              <select className="sh-select" {...register('Decision', { required: true })}>
                {ACK_DECISIONS.map(d => <option key={d}>{d}</option>)}
              </select>
            </div>
 
            <div>
              <label className="sh-label">
                Counter Notes
                <span className="text-gray-400 font-normal ml-1">
                  (required if Decision = Counter)
                </span>
              </label>
              <input
                className="sh-input"
                placeholder="Proposed changes or counter terms…"
                {...register('CounterNotes')}
              />
            </div>
 
            {/* Status — from PoAckStatus enum */}
            <div>
              <label className="sh-label">Status *</label>
              <select className="sh-select" {...register('Status', { required: true })}>
                {ACK_STATUSES.map(s => <option key={s}>{s}</option>)}
              </select>
            </div>
          </form>
        </Modal>
      )}
    </div>
  )
}
 
// ─────────────────────────────────────────────────────────
// REVISIONS TAB
// ─────────────────────────────────────────────────────────
function RevisionsTab({ po, user }) {
  const qc = useQueryClient()
  const [showCreate, setShowCreate] = useState(false)
 
  const { data, isLoading } = useQuery({
    queryKey: ['po-revisions', po.poID],
    queryFn:  () => poRevisionsApi.getByPoId(po.poID),
  })
  const revisions = data?.data ?? data ?? []
 
  const { register, handleSubmit, reset, formState: { errors } } = useForm()
 
  const createMut = useMutation({
    mutationFn: poRevisionsApi.create,
    onSuccess: () => {
      qc.invalidateQueries(['po-revisions', po.poID])
      setShowCreate(false)
      toast.success('Revision recorded')
    },
    onError: e => toast.error(e.response?.data?.message ?? 'Failed to create revision'),
  })
 
  const nextRevNo = revisions.length > 0
    ? Math.max(...revisions.map(r => r.revisionNo ?? 0)) + 1
    : 1
 
  const openCreate = () => {
    reset({
      RevisionNo:   nextRevNo,
      ChangelogJson:'',
      ChangeDate:   new Date().toISOString().split('T')[0],
      Status:       'Active',
    })
    setShowCreate(true)
  }
 
  const onSubmit = (d) => {
    createMut.mutate({
      // PoRevisionCreateDto uses PoID (uppercase ID)
      PoID:          po.poID,
      RevisionNo:    Number(d.RevisionNo),
      ChangedBy:     user?.userId ?? null,
      ChangelogJson: d.ChangelogJson || null,
      ChangeDate:    d.ChangeDate   || null,
      Status:        d.Status,
    })
  }
 
  return (
    <div>
      <div className="flex justify-between items-center mb-3">
        <div>
          <h4 className="text-sm font-medium text-gray-700">Change Orders / Revisions</h4>
          <p className="text-xs text-gray-400 mt-0.5">
            Each change to this PO creates a new revision — immutable audit trail
          </p>
        </div>
        <button className="btn btn-primary btn-sm" onClick={openCreate}>
          + New Revision
        </button>
      </div>
 
      <div className="sh-card p-0 overflow-hidden">
        {isLoading ? (
          <div className="flex justify-center py-8"><Spinner /></div>
        ) : revisions.length === 0 ? (
          <div className="text-center py-8">
            <p className="text-sm text-gray-400 mb-3">No revisions yet.</p>
            <button className="btn btn-primary btn-sm" onClick={openCreate}>
              + Record First Change
            </button>
          </div>
        ) : (
          <table className="sh-table">
            <thead>
              <tr>
                <th>Sr. No.</th>
                <th>Rev ID</th>
                <th>Rev No.</th>
                <th>Changed By</th>
                <th>Change Date</th>
                <th>Status</th>
                <th>Change Log</th>
              </tr>
            </thead>
            <tbody>
              {[...revisions]
                .sort((a, b) => (b.revisionNo ?? 0) - (a.revisionNo ?? 0))
                .map((r, i) => (
                <tr key={r.porevID}>
                  <td className="text-gray-400 text-xs">{i + 1}</td>
                  <td className="text-gray-400 text-xs">{r.porevID}</td>
                  <td>
                    <span className="pill pill-blue">Rev {r.revisionNo}</span>
                  </td>
                  <td className="text-gray-500 text-sm">{r.changedBy ?? '—'}</td>
                  <td className="text-xs text-gray-500">
                    {r.changeDate ? new Date(r.changeDate).toLocaleDateString() : '—'}
                  </td>
                  <td>
                    <span className={statusColor(
                      typeof r.status === 'number'
                        ? REV_STATUSES[r.status] ?? r.status
                        : r.status
                    )}>
                      {typeof r.status === 'number'
                        ? REV_STATUSES[r.status] ?? r.status
                        : r.status}
                    </span>
                  </td>
                  <td className="max-w-xs">
                    {r.changelogJson ? (
                      <details>
                        <summary className="text-xs text-blue-600 cursor-pointer">
                          View changelog
                        </summary>
                        <pre className="text-xs bg-gray-50 p-2 rounded mt-1 overflow-auto max-h-24">
                          {(() => {
                            try { return JSON.stringify(JSON.parse(r.changelogJson), null, 2) }
                            catch { return r.changelogJson }
                          })()}
                        </pre>
                      </details>
                    ) : '—'}
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
        )}
      </div>
 
      {/* Create Revision Modal */}
      {showCreate && (
        <Modal
          title={`New Revision — Rev ${nextRevNo}`}
          onClose={() => setShowCreate(false)}
          footer={
            <>
              <button className="btn btn-secondary btn-sm" onClick={() => setShowCreate(false)}>
                Cancel
              </button>
              <button
                className="btn btn-primary btn-sm"
                onClick={handleSubmit(onSubmit)}
                disabled={createMut.isPending}
              >
                {createMut.isPending ? 'Saving…' : 'Record Revision'}
              </button>
            </>
          }
        >
          <div className="bg-amber-50 border border-amber-200 rounded-lg px-3 py-2 mb-4 text-xs text-amber-700">
            ⚠ Revisions are append-only — they cannot be edited after creation.
          </div>
          <form className="space-y-3" noValidate>
            <div>
              <label className="sh-label">Revision Number</label>
              <input
                type="number"
                className="sh-input bg-gray-50"
                readOnly
                {...register('RevisionNo')}
              />
            </div>
            <div>
              <label className="sh-label">Change Date</label>
              <input type="date" className="sh-input" {...register('ChangeDate')} />
            </div>
            <div>
              <label className="sh-label">
                Changelog (JSON)
                <span className="text-gray-400 font-normal ml-1">(optional)</span>
              </label>
              <input
                className="sh-input font-mono text-xs"
                placeholder='{"changed":"quantity","from":100,"to":150}'
                {...register('ChangelogJson')}
              />
              <p className="text-xs text-gray-400 mt-1">
                Describe what changed in this revision as a JSON object
              </p>
            </div>
            <div>
              <label className="sh-label">Status</label>
              <select className="sh-select" {...register('Status')}>
                {REV_STATUSES.map(s => <option key={s}>{s}</option>)}
              </select>
            </div>
          </form>
        </Modal>
      )}
    </div>
  )
}