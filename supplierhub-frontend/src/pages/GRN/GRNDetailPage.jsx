import { useState } from 'react'
import { useParams, useNavigate } from 'react-router-dom'
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query'
import { useForm } from 'react-hook-form'
import toast from 'react-hot-toast'
import {
  grnApi,
  grnItemApi,
  inspectionApi,
  ncrApi,
} from '../../api/operations.api'
import { StatusPill } from '../../components/ui/StatusPill'
import { Spinner, EmptyState, ConfirmDialog } from '../../components/ui/index'
import Modal from '../../components/ui/Modal'
import useAuthStore from '../../store/auth.store'
 
// ── Enums ──────────────────────────────────────────────
const GRN_STATUSES       = ['Pending', 'Open', 'Posted']
const ITEM_STATUSES      = ['Pending', 'Accepted', 'Rejected', 'Partial']
const INSPECTION_RESULTS = ['Pass', 'Fail']
const INSPECTION_STATUS  = ['Pending', 'Completed', 'Active']
const NCR_SEVERITIES     = ['Minor', 'Major', 'Critical']
const NCR_STATUSES       = ['Open', 'Closed']
const NCR_DISPOSITIONS   = ['UseAsIs', 'Rework', 'Reject', 'Return']
 
const statusColor = (s) => {
  const m = {
    Pending: 'pill-amber', Open: 'pill-blue', Posted: 'pill-green',
    Accepted: 'pill-green', Rejected: 'pill-red', Partial: 'pill-amber',
    Pass: 'pill-green', Fail: 'pill-red', Completed: 'pill-green',
    Active: 'pill-blue', Minor: 'pill-amber', Major: 'pill-orange',
    Critical: 'pill-red', Closed: 'pill-gray',
  }
  return `pill ${m[s] ?? 'pill-gray'}`
}
 
const TABS = ['GRN Items', 'Inspections', 'NCRs']
 
// ─────────────────────────────────────────────────────────
export default function GRNDetailPage() {
  const { id }   = useParams()
  const navigate = useNavigate()
  const { user } = useAuthStore()
  const [tab, setTab] = useState('GRN Items')
 
  const isAdmin    = user?.roles?.includes('Admin')
  const isSupplier = user?.roles?.includes('SupplierUser')
  const canWrite   = isAdmin || isSupplier
 
  // ── Fetch GRN header ───────────────────────────────────
  const { data, isLoading } = useQuery({
    queryKey: ['grn', id],
    queryFn:  () => grnApi.getById(id),
  })
  const grn = data?.data ?? data
 
  // ── Fetch GRN Items (needed by all tabs) ───────────────
  const { data: itemsData } = useQuery({
    queryKey: ['grn-items', id],
    queryFn:  () => grnItemApi.getByGrnId(id),
  })
  const grnItems = itemsData?.data ?? itemsData ?? []
 
  if (isLoading) return <div className="flex justify-center py-16"><Spinner /></div>
  if (!grn) return <p className="text-gray-500 p-4">GRN not found.</p>
 
  return (
    <div>
      {/* ── Header ────────────────────────────────────────── */}
      <div className="mb-5">
        <button
          className="text-sm text-blue-600 hover:underline mb-2 block"
          onClick={() => navigate(-1)}
        >
          ← Back to GRN List
        </button>
 
        <div className="sh-card">
          <div className="flex items-start justify-between">
            <div>
              <h1 className="text-xl font-semibold text-gray-900">GRN-{grn.grnID}</h1>
              <p className="text-sm text-gray-500 mt-1">
                PO-{grn.poID}
                {grn.asnID && <> &nbsp;·&nbsp; ASN-{grn.asnID}</>}
              </p>
            </div>
            <span className={statusColor(grn.status)}>{grn.status}</span>
          </div>
 
          <div className="grid grid-cols-2 gap-4 mt-4 text-sm md:grid-cols-4">
            <Detail label="GRN ID"        value={`GRN-${grn.grnID}`} />
            <Detail label="PO ID"         value={`PO-${grn.poID}`} />
            <Detail label="ASN ID"        value={grn.asnID ? `ASN-${grn.asnID}` : '—'} />
            <Detail label="Received Date" value={grn.receivedDate ? new Date(grn.receivedDate).toLocaleDateString() : '—'} />
            <Detail label="Received By"   value={grn.receivedBy ?? '—'} />
            <Detail label="Created"       value={grn.createdOn ? new Date(grn.createdOn).toLocaleDateString() : '—'} />
            <Detail label="Total Items"   value={grnItems.length} />
            <Detail label="Status"        value={<span className={statusColor(grn.status)}>{grn.status}</span>} />
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
 
      {tab === 'GRN Items'   && <GRNItemsTab   grn={grn} grnItems={grnItems} canWrite={canWrite} isAdmin={isAdmin} />}
      {tab === 'Inspections' && <InspectionsTab grnItems={grnItems} canWrite={canWrite} />}
      {tab === 'NCRs'        && <NCRsTab        grnItems={grnItems} canWrite={canWrite} />}
    </div>
  )
}
 
function Detail({ label, value }) {
  return (
    <div>
      <p className="text-gray-500 text-xs font-medium">{label}</p>
      <p className="text-gray-800 font-medium mt-0.5 text-sm">{value}</p>
    </div>
  )
}
 
// ─────────────────────────────────────────────────────────
// GRN ITEMS TAB
// ─────────────────────────────────────────────────────────
function GRNItemsTab({ grn, grnItems, canWrite, isAdmin }) {
  const qc = useQueryClient()
  const [modal, setModal]       = useState(null)
  const [selected, setSelected] = useState(null)
 
  const { register, handleSubmit, reset, formState: { errors } } = useForm()
 
  const createMut = useMutation({
    mutationFn: grnItemApi.create,
    onSuccess: () => {
      qc.invalidateQueries(['grn-items', String(grn.grnID)])
      setModal(null)
      toast.success('GRN item added')
    },
    onError: e => toast.error(e.response?.data?.message ?? 'Failed to add item'),
  })
 
  const updateMut = useMutation({
    mutationFn: ({ id, dto }) => grnItemApi.update(id, dto),
    onSuccess: () => {
      qc.invalidateQueries(['grn-items', String(grn.grnID)])
      setModal(null)
      toast.success('GRN item updated')
    },
    onError: e => toast.error(e.response?.data?.message ?? 'Failed to update item'),
  })
 
  const deleteMut = useMutation({
    mutationFn: (id) => grnItemApi.delete(id),
    onSuccess: () => {
      qc.invalidateQueries(['grn-items', String(grn.grnID)])
      setModal(null)
      toast.success('GRN item deleted')
    },
    onError: e => toast.error(e.response?.data?.message ?? 'Failed to delete item'),
  })
 
  const openCreate = () => {
    reset({
      PoLineID:    '',
      ReceivedQty: '',
      AcceptedQty: '',
      RejectedQty: '',
      Reason:      '',
      Status:      'Pending',
    })
    setSelected(null)
    setModal('form')
  }
 
  const openEdit = (row) => {
    setSelected(row)
    reset({
      ReceivedQty: row.receivedQty ?? '',
      AcceptedQty: row.acceptedQty ?? '',
      RejectedQty: row.rejectedQty ?? '',
      Reason:      row.reason      ?? '',
      Status:      row.status      ?? 'Pending',
    })
    setModal('form')
  }
 
  const onSubmit = (d) => {
    if (selected) {
      // GrnItemUpdateDto — no IDs in body, uses route param
      updateMut.mutate({
        id: selected.grnItemID,
        dto: {
          ReceivedQty: d.ReceivedQty ? Number(d.ReceivedQty) : null,
          AcceptedQty: d.AcceptedQty ? Number(d.AcceptedQty) : null,
          RejectedQty: d.RejectedQty ? Number(d.RejectedQty) : null,
          Reason:      d.Reason      || null,
          Status:      d.Status,
        },
      })
    } else {
      createMut.mutate({
        GrnID:       grn.grnID,
        PoLineID:    Number(d.PoLineID),
        ReceivedQty: d.ReceivedQty ? Number(d.ReceivedQty) : null,
        AcceptedQty: d.AcceptedQty ? Number(d.AcceptedQty) : null,
        RejectedQty: d.RejectedQty ? Number(d.RejectedQty) : null,
        Reason:      d.Reason      || null,
        Status:      d.Status,
      })
    }
  }
 
  const isPending = createMut.isPending || updateMut.isPending
 
  return (
    <div>
      <div className="flex justify-between items-center mb-3">
        <div>
          <h4 className="text-sm font-medium text-gray-700">
            GRN Items — {grnItems.length} line{grnItems.length !== 1 ? 's' : ''}
          </h4>
          <p className="text-xs text-gray-400 mt-0.5">
            Record received, accepted and rejected quantities per PO line
          </p>
        </div>
        {canWrite && (
          <button className="btn btn-primary btn-sm" onClick={openCreate}>
            + Add Item
          </button>
        )}
      </div>
 
      <div className="sh-card p-0 overflow-hidden">
        {grnItems.length === 0 ? (
          <div className="text-center py-8">
            <p className="text-sm text-gray-400 mb-3">No items recorded yet.</p>
            {canWrite && (
              <button className="btn btn-primary btn-sm" onClick={openCreate}>
                + Add First Item
              </button>
            )}
          </div>
        ) : (
          <table className="sh-table">
            <thead>
              <tr>
                <th>Sr. No.</th>
                <th>Item ID</th>
                <th>PO Line ID</th>
                <th>Received Qty</th>
                <th>Accepted Qty</th>
                <th>Rejected Qty</th>
                <th>Reason</th>
                <th>Status</th>
                {canWrite && <th>Actions</th>}
              </tr>
            </thead>
            <tbody>
              {grnItems.map((item, i) => (
                <tr key={item.grnItemID}>
                  <td className="text-gray-400 text-xs">{i + 1}</td>
                  <td className="text-gray-400 text-xs">{item.grnItemID}</td>
                  <td className="font-medium">#{item.poLineID}</td>
                  <td className="font-medium">{item.receivedQty ?? '—'}</td>
                  <td className="text-green-700 font-medium">{item.acceptedQty ?? '—'}</td>
                  <td className={`font-medium ${(item.rejectedQty ?? 0) > 0 ? 'text-red-600' : ''}`}>
                    {item.rejectedQty ?? '—'}
                  </td>
                  <td className="text-xs text-gray-500 max-w-xs truncate">
                    {item.reason ?? '—'}
                  </td>
                  <td><span className={statusColor(item.status)}>{item.status}</span></td>
                  {canWrite && (
                    <td>
                      <div className="flex gap-1">
                        <button
                          className="btn btn-ghost btn-sm"
                          onClick={() => openEdit(item)}
                        >
                          Edit
                        </button>
                        {isAdmin && (
                          <button
                            className="btn btn-danger btn-sm"
                            onClick={() => { setSelected(item); setModal('delete') }}
                          >
                            Del
                          </button>
                        )}
                      </div>
                    </td>
                  )}
                </tr>
              ))}
            </tbody>
          </table>
        )}
      </div>
 
      {/* Summary row */}
      {grnItems.length > 0 && (
        <div className="mt-3 flex gap-6 text-sm">
          <div className="sh-card py-2 px-4 flex gap-6">
            <span className="text-gray-500">
              Total Received:{' '}
              <strong className="text-gray-800">
                {grnItems.reduce((s, i) => s + (i.receivedQty ?? 0), 0)}
              </strong>
            </span>
            <span className="text-gray-500">
              Accepted:{' '}
              <strong className="text-green-700">
                {grnItems.reduce((s, i) => s + (i.acceptedQty ?? 0), 0)}
              </strong>
            </span>
            <span className="text-gray-500">
              Rejected:{' '}
              <strong className="text-red-600">
                {grnItems.reduce((s, i) => s + (i.rejectedQty ?? 0), 0)}
              </strong>
            </span>
          </div>
        </div>
      )}
 
      {/* Create / Edit Modal */}
      {modal === 'form' && (
        <Modal
          title={selected ? `Edit Item #${selected.grnItemID}` : 'Add GRN Item'}
          onClose={() => setModal(null)}
          footer={
            <>
              <button className="btn btn-secondary btn-sm" onClick={() => setModal(null)}>
                Cancel
              </button>
              <button
                className="btn btn-primary btn-sm"
                onClick={handleSubmit(onSubmit)}
                disabled={isPending}
              >
                {isPending ? 'Saving…' : selected ? 'Update' : 'Add Item'}
              </button>
            </>
          }
        >
          <form className="space-y-3" noValidate>
            {!selected && (
              <div>
                <label className="sh-label">PO Line ID *</label>
                <input
                  type="number"
                  className="sh-input"
                  placeholder="Enter PO Line ID"
                  {...register('PoLineID', { required: 'PO Line ID is required' })}
                />
                {errors.PoLineID && (
                  <p className="text-red-500 text-xs mt-1">{errors.PoLineID.message}</p>
                )}
              </div>
            )}
 
            {/* Quantity row */}
            <div className="grid grid-cols-3 gap-3">
              <div>
                <label className="sh-label">Received Qty</label>
                <input type="number" step="0.01" min="0" className="sh-input"
                  {...register('ReceivedQty')} />
              </div>
              <div>
                <label className="sh-label">Accepted Qty</label>
                <input type="number" step="0.01" min="0" className="sh-input"
                  {...register('AcceptedQty')} />
              </div>
              <div>
                <label className="sh-label">Rejected Qty</label>
                <input type="number" step="0.01" min="0" className="sh-input"
                  {...register('RejectedQty')} />
              </div>
            </div>
 
            <div>
              <label className="sh-label">Reason for Rejection</label>
              <input
                className="sh-input"
                placeholder="e.g. Damaged packaging, Wrong item"
                {...register('Reason')}
              />
            </div>
 
            <div>
              <label className="sh-label">Status *</label>
              <select
                className="sh-select"
                {...register('Status', { required: true })}
              >
                {ITEM_STATUSES.map(s => <option key={s}>{s}</option>)}
              </select>
            </div>
          </form>
        </Modal>
      )}
 
      {modal === 'delete' && (
        <ConfirmDialog
          message={`Delete GRN item #${selected?.grnItemID}? Its inspections and NCRs will also be removed.`}
          onConfirm={() => deleteMut.mutate(selected.grnItemID)}
          onCancel={() => setModal(null)}
          loading={deleteMut.isPending}
        />
      )}
    </div>
  )
}
 
// ─────────────────────────────────────────────────────────
// INSPECTIONS TAB
// ─────────────────────────────────────────────────────────
function InspectionsTab({ grnItems, canWrite }) {
  const qc = useQueryClient()
  const { user } = useAuthStore()
  const [modal, setModal]       = useState(null)
  const [selected, setSelected] = useState(null)
  const [filterItem, setFilterItem] = useState('all')
 
  // Fetch inspections for each GRN item
  const itemIDs = grnItems.map(i => i.grnItemID)
 
  const { data, isLoading } = useQuery({
    queryKey: ['inspections-all'],
    queryFn:  inspectionApi.getAll,
    select: (d) => {
      const all = d?.data ?? d ?? []
      return all.filter(insp => itemIDs.includes(insp.grnItemID))
    },
    enabled: itemIDs.length > 0,
  })
  const allInspections = data ?? []
  const inspections = filterItem === 'all'
    ? allInspections
    : allInspections.filter(i => i.grnItemID === Number(filterItem))
 
  const { register, handleSubmit, reset, formState: { errors } } = useForm()
 
  const createMut = useMutation({
    mutationFn: inspectionApi.create,
    onSuccess: () => {
      qc.invalidateQueries(['inspections-all'])
      setModal(null)
      toast.success('Inspection recorded')
    },
    onError: e => toast.error(e.response?.data?.message ?? 'Failed'),
  })
 
  const updateMut = useMutation({
    mutationFn: ({ id, dto }) => inspectionApi.update(id, dto),
    onSuccess: () => {
      qc.invalidateQueries(['inspections-all'])
      setModal(null)
      toast.success('Inspection updated')
    },
    onError: e => toast.error(e.response?.data?.message ?? 'Failed'),
  })
 
  const deleteMut = useMutation({
    mutationFn: (id) => inspectionApi.delete(id),
    onSuccess: () => {
      qc.invalidateQueries(['inspections-all'])
      setModal(null)
      toast.success('Inspection deleted')
    },
    onError: e => toast.error(e.response?.data?.message ?? 'Failed'),
  })
 
  const openCreate = () => {
    reset({
      GrnItemID:   '',
      Result:      'Pass',
      InspectorID: user?.userId ?? '',
      InspDate:    new Date().toISOString().split('T')[0],
      Status:      'Completed',
    })
    setSelected(null)
    setModal('form')
  }
 
  const openEdit = (row) => {
    setSelected(row)
    reset({
      // InspectionUpdateDto still requires GrnItemID
      GrnItemID:   row.grnItemID   ?? '',
      Result:      row.result      ?? 'Pass',
      InspectorID: row.inspectorID ?? '',
      InspDate:    row.inspDate?.split('T')[0] ?? '',
      Status:      row.status      ?? 'Completed',
    })
    setModal('form')
  }
 
  const onSubmit = (d) => {
    const payload = {
      GrnItemID:   Number(d.GrnItemID),
      Result:      d.Result      || null,
      InspectorID: d.InspectorID ? Number(d.InspectorID) : null,
      InspDate:    d.InspDate    || null,
      Status:      d.Status,
    }
    if (selected) {
      updateMut.mutate({ id: selected.inspID, dto: payload })
    } else {
      createMut.mutate(payload)
    }
  }
 
  const isPending = createMut.isPending || updateMut.isPending
 
  const passCount = allInspections.filter(i => i.result === 'Pass').length
  const failCount = allInspections.filter(i => i.result === 'Fail').length
 
  return (
    <div>
      <div className="flex justify-between items-center mb-3">
        <div>
          <h4 className="text-sm font-medium text-gray-700">Quality Inspections</h4>
          <p className="text-xs text-gray-400 mt-0.5">
            {allInspections.length} inspection{allInspections.length !== 1 ? 's' : ''} &nbsp;·&nbsp;
            <span className="text-green-600">{passCount} passed</span> &nbsp;·&nbsp;
            <span className="text-red-600">{failCount} failed</span>
          </p>
        </div>
        <div className="flex gap-2 items-center">
          {/* Filter by GRN item */}
          <select
            className="sh-select text-xs"
            style={{ width: 180 }}
            value={filterItem}
            onChange={e => setFilterItem(e.target.value)}
          >
            <option value="all">All Items</option>
            {grnItems.map(i => (
              <option key={i.grnItemID} value={i.grnItemID}>
                Item #{i.grnItemID} (Line {i.poLineID})
              </option>
            ))}
          </select>
          {canWrite && (
            <button className="btn btn-primary btn-sm" onClick={openCreate}>
              + Add Inspection
            </button>
          )}
        </div>
      </div>
 
      <div className="sh-card p-0 overflow-hidden">
        {isLoading ? (
          <div className="flex justify-center py-8"><Spinner /></div>
        ) : inspections.length === 0 ? (
          <div className="text-center py-8">
            <p className="text-sm text-gray-400 mb-3">No inspections recorded yet.</p>
            {canWrite && grnItems.length === 0 && (
              <p className="text-xs text-amber-600">
                ⚠ Add GRN items first before recording inspections.
              </p>
            )}
            {canWrite && grnItems.length > 0 && (
              <button className="btn btn-primary btn-sm" onClick={openCreate}>
                + Record First Inspection
              </button>
            )}
          </div>
        ) : (
          <table className="sh-table">
            <thead>
              <tr>
                <th>Sr. No.</th>
                <th>Insp ID</th>
                <th>GRN Item</th>
                <th>Result</th>
                <th>Inspector ID</th>
                <th>Insp Date</th>
                <th>Status</th>
                {canWrite && <th>Actions</th>}
              </tr>
            </thead>
            <tbody>
              {inspections.map((insp, i) => (
                <tr key={insp.inspID}>
                  <td className="text-gray-400 text-xs">{i + 1}</td>
                  <td className="text-gray-400 text-xs">{insp.inspID}</td>
                  <td className="font-medium">Item #{insp.grnItemID}</td>
                  <td>
                    <span className={statusColor(insp.result ?? '')}>
                      {insp.result ?? '—'}
                    </span>
                  </td>
                  <td className="text-gray-500">{insp.inspectorID ?? '—'}</td>
                  <td className="text-xs text-gray-500">
                    {insp.inspDate ? new Date(insp.inspDate).toLocaleDateString() : '—'}
                  </td>
                  <td><span className={statusColor(insp.status)}>{insp.status}</span></td>
                  {canWrite && (
                    <td>
                      <div className="flex gap-1">
                        <button className="btn btn-ghost btn-sm"
                          onClick={() => openEdit(insp)}>Edit</button>
                        <button className="btn btn-danger btn-sm"
                          onClick={() => { setSelected(insp); setModal('delete') }}>Del</button>
                      </div>
                    </td>
                  )}
                </tr>
              ))}
            </tbody>
          </table>
        )}
      </div>
 
      {modal === 'form' && (
        <Modal
          title={selected ? `Edit Inspection #${selected.inspID}` : 'Record Inspection'}
          onClose={() => setModal(null)}
          footer={
            <>
              <button className="btn btn-secondary btn-sm" onClick={() => setModal(null)}>Cancel</button>
              <button className="btn btn-primary btn-sm"
                onClick={handleSubmit(onSubmit)} disabled={isPending}>
                {isPending ? 'Saving…' : selected ? 'Update' : 'Record'}
              </button>
            </>
          }
        >
          <form className="space-y-3" noValidate>
            {/* GrnItemID — required even in update */}
            <div>
              <label className="sh-label">GRN Item *</label>
              <select
                className="sh-select"
                {...register('GrnItemID', { required: 'Select a GRN item' })}
              >
                <option value="">— Select GRN item —</option>
                {grnItems.map(i => (
                  <option key={i.grnItemID} value={i.grnItemID}>
                    Item #{i.grnItemID} — PO Line {i.poLineID}
                    {i.receivedQty ? ` (${i.receivedQty} received)` : ''}
                  </option>
                ))}
              </select>
              {errors.GrnItemID && (
                <p className="text-red-500 text-xs mt-1">{errors.GrnItemID.message}</p>
              )}
            </div>
 
            {/* Result */}
            <div>
              <label className="sh-label">Result *</label>
              <div className="flex gap-3">
                {INSPECTION_RESULTS.map(r => (
                  <label key={r} className="flex items-center gap-2 cursor-pointer">
                    <input type="radio" value={r} {...register('Result', { required: true })} />
                    <span className={`pill ${r === 'Pass' ? 'pill-green' : 'pill-red'}`}>{r}</span>
                  </label>
                ))}
              </div>
            </div>
 
            <div className="grid grid-cols-2 gap-3">
              <div>
                <label className="sh-label">Inspector ID</label>
                <input type="number" className="sh-input" {...register('InspectorID')} />
              </div>
              <div>
                <label className="sh-label">Inspection Date</label>
                <input type="date" className="sh-input" {...register('InspDate')} />
              </div>
            </div>
 
            <div>
              <label className="sh-label">Status *</label>
              <select className="sh-select" {...register('Status', { required: true })}>
                {INSPECTION_STATUS.map(s => <option key={s}>{s}</option>)}
              </select>
            </div>
          </form>
        </Modal>
      )}
 
      {modal === 'delete' && (
        <ConfirmDialog
          message={`Delete inspection #${selected?.inspID}?`}
          onConfirm={() => deleteMut.mutate(selected.inspID)}
          onCancel={() => setModal(null)}
          loading={deleteMut.isPending}
        />
      )}
    </div>
  )
}
 
// ─────────────────────────────────────────────────────────
// NCRs TAB
// ─────────────────────────────────────────────────────────
function NCRsTab({ grnItems, canWrite }) {
  const qc = useQueryClient()
  const [modal, setModal]       = useState(null)
  const [selected, setSelected] = useState(null)
  const [filterItem, setFilterItem] = useState('all')
 
  const itemIDs = grnItems.map(i => i.grnItemID)
 
  const { data, isLoading } = useQuery({
    queryKey: ['ncrs-all'],
    queryFn:  ncrApi.getAll,
    select: (d) => {
      const all = d?.data ?? d ?? []
      return all.filter(n => itemIDs.includes(n.grnItemID))
    },
    enabled: itemIDs.length > 0,
  })
  const allNcrs = data ?? []
  const ncrs = filterItem === 'all'
    ? allNcrs
    : allNcrs.filter(n => n.grnItemID === Number(filterItem))
 
  const { register, handleSubmit, reset, formState: { errors } } = useForm()
 
  const createMut = useMutation({
    mutationFn: ncrApi.create,
    onSuccess: () => {
      qc.invalidateQueries(['ncrs-all'])
      setModal(null)
      toast.success('NCR raised successfully')
    },
    onError: e => toast.error(e.response?.data?.message ?? 'Failed'),
  })
 
  const updateMut = useMutation({
    mutationFn: ({ id, dto }) => ncrApi.update(id, dto),
    onSuccess: () => {
      qc.invalidateQueries(['ncrs-all'])
      setModal(null)
      toast.success('NCR updated')
    },
    onError: e => toast.error(e.response?.data?.message ?? 'Failed'),
  })
 
  const deleteMut = useMutation({
    mutationFn: (id) => ncrApi.delete(id),
    onSuccess: () => {
      qc.invalidateQueries(['ncrs-all'])
      setModal(null)
      toast.success('NCR deleted')
    },
    onError: e => toast.error(e.response?.data?.message ?? 'Failed'),
  })
 
  const openCreate = () => {
    reset({
      GrnItemID:  '',
      DefectType: '',
      Severity:   'Minor',
      Status:     'Open',
    })
    setSelected(null)
    setModal('form')
  }
 
  const openEdit = (row) => {
    setSelected(row)
    reset({
      GrnItemID:  row.grnItemID  ?? '',
      DefectType: row.defectType ?? '',
      Severity:   row.severity   ?? 'Minor',
      Status:     row.status     ?? 'Open',
    })
    setModal('form')
  }
 
  const onSubmit = (d) => {
    if (selected) {
      // NcrUpdateDto — no IDs in body
      updateMut.mutate({
        id: selected.ncrID,
        dto: {
          DefectType: d.DefectType || null,
          Severity:   d.Severity   || null,
          Status:     d.Status,
        },
      })
    } else {
      createMut.mutate({
        GrnItemID:  Number(d.GrnItemID),
        DefectType: d.DefectType || null,
        Severity:   d.Severity   || null,
        Status:     d.Status,
      })
    }
  }
 
  const isPending = createMut.isPending || updateMut.isPending
 
  const openCount   = allNcrs.filter(n => n.status === 'Open').length
  const closedCount = allNcrs.filter(n => n.status === 'Closed').length
 
  return (
    <div>
      <div className="flex justify-between items-center mb-3">
        <div>
          <h4 className="text-sm font-medium text-gray-700">Non-Conformance Reports (NCR)</h4>
          <p className="text-xs text-gray-400 mt-0.5">
            {allNcrs.length} NCR{allNcrs.length !== 1 ? 's' : ''} &nbsp;·&nbsp;
            <span className="text-red-600">{openCount} open</span> &nbsp;·&nbsp;
            <span className="text-gray-500">{closedCount} closed</span>
          </p>
        </div>
        <div className="flex gap-2 items-center">
          <select
            className="sh-select text-xs"
            style={{ width: 180 }}
            value={filterItem}
            onChange={e => setFilterItem(e.target.value)}
          >
            <option value="all">All Items</option>
            {grnItems.map(i => (
              <option key={i.grnItemID} value={i.grnItemID}>
                Item #{i.grnItemID} (Line {i.poLineID})
              </option>
            ))}
          </select>
          {canWrite && (
            <button className="btn btn-danger btn-sm" onClick={openCreate}>
              + Raise NCR
            </button>
          )}
        </div>
      </div>
 
      <div className="sh-card p-0 overflow-hidden">
        {isLoading ? (
          <div className="flex justify-center py-8"><Spinner /></div>
        ) : ncrs.length === 0 ? (
          <div className="text-center py-8">
            <p className="text-sm text-gray-400 mb-1">No NCRs raised.</p>
            <p className="text-xs text-gray-400">
              Raise an NCR when received goods fail quality inspection.
            </p>
          </div>
        ) : (
          <table className="sh-table">
            <thead>
              <tr>
                <th>Sr. No.</th>
                <th>NCR ID</th>
                <th>GRN Item</th>
                <th>Defect Type</th>
                <th>Severity</th>
                <th>Status</th>
                <th>Created</th>
                {canWrite && <th>Actions</th>}
              </tr>
            </thead>
            <tbody>
              {ncrs.map((ncr, i) => (
                <tr key={ncr.ncrID}>
                  <td className="text-gray-400 text-xs">{i + 1}</td>
                  <td className="font-medium">NCR-{ncr.ncrID}</td>
                  <td className="text-gray-500">Item #{ncr.grnItemID}</td>
                  <td className="font-medium">{ncr.defectType ?? '—'}</td>
                  <td>
                    <span className={statusColor(ncr.severity ?? '')}>
                      {ncr.severity ?? '—'}
                    </span>
                  </td>
                  <td>
                    <span className={statusColor(ncr.status)}>
                      {ncr.status}
                    </span>
                  </td>
                  <td className="text-xs text-gray-500">
                    {ncr.createdOn ? new Date(ncr.createdOn).toLocaleDateString() : '—'}
                  </td>
                  {canWrite && (
                    <td>
                      <div className="flex gap-1">
                        <button className="btn btn-ghost btn-sm"
                          onClick={() => openEdit(ncr)}>Edit</button>
                        <button className="btn btn-danger btn-sm"
                          onClick={() => { setSelected(ncr); setModal('delete') }}>Del</button>
                      </div>
                    </td>
                  )}
                </tr>
              ))}
            </tbody>
          </table>
        )}
      </div>
 
      {modal === 'form' && (
        <Modal
          title={selected ? `Edit NCR-${selected.ncrID}` : 'Raise NCR'}
          onClose={() => setModal(null)}
          footer={
            <>
              <button className="btn btn-secondary btn-sm" onClick={() => setModal(null)}>Cancel</button>
              <button className="btn btn-primary btn-sm"
                onClick={handleSubmit(onSubmit)} disabled={isPending}>
                {isPending ? 'Saving…' : selected ? 'Update' : 'Raise NCR'}
              </button>
            </>
          }
        >
          <form className="space-y-3" noValidate>
            {/* GRN Item — always show for context */}
            <div>
              <label className="sh-label">GRN Item *</label>
              <select
                className="sh-select"
                {...register('GrnItemID', { required: 'Select a GRN item' })}
                disabled={!!selected}
              >
                <option value="">— Select GRN item —</option>
                {grnItems.map(i => (
                  <option key={i.grnItemID} value={i.grnItemID}>
                    Item #{i.grnItemID} — PO Line {i.poLineID}
                    {i.rejectedQty ? ` (${i.rejectedQty} rejected)` : ''}
                  </option>
                ))}
              </select>
              {errors.GrnItemID && (
                <p className="text-red-500 text-xs mt-1">{errors.GrnItemID.message}</p>
              )}
            </div>
 
            <div>
              <label className="sh-label">Defect Type</label>
              <input
                className="sh-input"
                placeholder="e.g. Dimensional, Surface Finish, Wrong Item"
                {...register('DefectType')}
              />
            </div>
 
            {/* Severity */}
            <div>
              <label className="sh-label">Severity *</label>
              <div className="flex gap-3">
                {NCR_SEVERITIES.map(s => (
                  <label key={s} className="flex items-center gap-2 cursor-pointer">
                    <input type="radio" value={s} {...register('Severity', { required: true })} />
                    <span className={statusColor(s)}>{s}</span>
                  </label>
                ))}
              </div>
            </div>
 
            <div>
              <label className="sh-label">Status *</label>
              <select className="sh-select" {...register('Status', { required: true })}>
                {NCR_STATUSES.map(s => <option key={s}>{s}</option>)}
              </select>
            </div>
 
            {/* Disposition note */}
            <div className="bg-amber-50 border border-amber-200 rounded-lg px-3 py-2 text-xs text-amber-700">
              💡 Disposition options: <strong>UseAsIs</strong> — <strong>Rework</strong> — <strong>Reject</strong> — <strong>Return</strong>
              <br />Disposition field can be added to NcrUpdateDto in the backend if needed.
            </div>
          </form>
        </Modal>
      )}
 
      {modal === 'delete' && (
        <ConfirmDialog
          message={`Delete NCR-${selected?.ncrID}? This cannot be undone.`}
          onConfirm={() => deleteMut.mutate(selected.ncrID)}
          onCancel={() => setModal(null)}
          loading={deleteMut.isPending}
        />
      )}
    </div>
  )
}