import { useState } from 'react'
import { useParams, useNavigate } from 'react-router-dom'
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query'
import { useForm } from 'react-hook-form'
import toast from 'react-hot-toast'
import { grnApi, grnItemApi, inspectionApi, ncrApi } from '../../api/operations.api'
import { StatusPill } from '../../components/ui/StatusPill'
import { Spinner, EmptyState } from '../../components/ui/index'
import Modal from '../../components/ui/Modal'
import useAuthStore from '../../store/auth.store'

const TABS = ['Items', 'Inspections', 'NCRs']

export default function GRNDetailPage() {
  const { id } = useParams()
  const navigate = useNavigate()
  const user = useAuthStore(s => s.user)
  const roles = user?.roles ?? []
  // Only the receiving team and admin can record items / inspections / NCRs.
  // Buyer / AccountsPayable / SupplierUser can view but not modify.
  const canEdit = roles.some(r => ['Admin','ReceivingUser','WarehouseManager'].includes(r))

  const [tab, setTab] = useState('Items')
  const [selectedItemId, setSelectedItemId] = useState(null)

  const { data, isLoading } = useQuery({ queryKey: ['grn', id], queryFn: () => grnApi.getById(id) })
  const { data: itemsData } = useQuery({ queryKey: ['grn-items', id], queryFn: () => grnItemApi.getByGrnId(id) })

  const items = itemsData?.data ?? itemsData ?? []

  if (isLoading) return <div className="flex justify-center py-16"><Spinner /></div>
  const grn = data?.data ?? data
  if (!grn) return <p>GRN not found.</p>

  return (
    <div>
      <div className="page-header">
        <div>
          <button onClick={() => navigate(-1)} className="text-sm text-blue-600 hover:underline mb-1 block">← Back</button>
          <h1 className="page-title">GRN #{grn.grnID}</h1>
        </div>
        <StatusPill status={grn.status} />
      </div>

      <div className="sh-card mb-4">
        <div className="grid grid-cols-3 gap-4 text-sm">
          <div><span className="text-gray-500">PO ID</span><p className="font-medium mt-0.5">{grn.poID}</p></div>
          <div><span className="text-gray-500">ASN ID</span><p className="font-medium mt-0.5">{grn.asnID || '—'}</p></div>
          <div><span className="text-gray-500">Received Date</span><p className="font-medium mt-0.5">{grn.receivedDate ? new Date(grn.receivedDate).toLocaleDateString() : '—'}</p></div>
          <div><span className="text-gray-500">Received By</span><p className="font-medium mt-0.5">{grn.receivedBy || '—'}</p></div>
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

      {tab === 'Items' && (
        <ItemsSection grnId={Number(id)} items={items} onSelect={setSelectedItemId} selectedItemId={selectedItemId} canEdit={canEdit} />
      )}

      {tab === 'Inspections' && (
        <InspectionsSection items={items} selectedItemId={selectedItemId} setSelectedItemId={setSelectedItemId} canEdit={canEdit} />
      )}

      {tab === 'NCRs' && (
        <NcrsSection items={items} selectedItemId={selectedItemId} setSelectedItemId={setSelectedItemId} canEdit={canEdit} />
      )}
    </div>
  )
}

/* ─── Items: list + add (click row to select for Inspection/NCR tabs) ─── */
function ItemsSection({ grnId, items, onSelect, selectedItemId, canEdit = false }) {
  const qc = useQueryClient()
  const [open, setOpen] = useState(false)
  const { register, handleSubmit, reset } = useForm()

  const addMut = useMutation({
    mutationFn: grnItemApi.create,
    onSuccess: () => { qc.invalidateQueries(['grn-items', String(grnId)]); qc.invalidateQueries(['grn-items', grnId]); setOpen(false); reset(); toast.success('GRN item added') },
    onError: e => toast.error(e.response?.data?.message ?? 'Failed to add GRN item'),
  })

  const onSubmit = (form) => {
    const received = Number(form.receivedQty)
    const accepted = form.acceptedQty === '' ? received : Number(form.acceptedQty)
    addMut.mutate({
      grnID:       grnId,
      poLineID:    Number(form.poLineID),
      receivedQty: received,
      acceptedQty: accepted,
      rejectedQty: received - accepted,
      reason:      form.reason ?? '',
    })
  }

  return (
    <div>
      <div className="flex items-center justify-between mb-3">
        <div className="text-sm text-gray-600">Click a row to select for Inspections / NCR tabs</div>
        {canEdit && (
          <button className="btn btn-primary btn-sm"
            onClick={() => { reset({ poLineID: '', receivedQty: 1, acceptedQty: '', reason: '' }); setOpen(true) }}>
            + Add Item
          </button>
        )}
      </div>

      <div className="sh-card p-0 overflow-hidden">
        {items.length === 0 ? <EmptyState message="No GRN items yet." /> : (
          <table className="sh-table">
            <thead><tr><th>Item ID</th><th>PO Line ID</th><th>Received Qty</th><th>Accepted Qty</th><th>Rejected Qty</th><th>Reason</th></tr></thead>
            <tbody>
              {items.map(item => (
                <tr key={item.grnItemID}
                    onClick={() => onSelect(item.grnItemID)}
                    className={`cursor-pointer ${selectedItemId === item.grnItemID ? 'bg-blue-50' : ''}`}>
                  <td>{item.grnItemID}</td><td>{item.poLineID}</td>
                  <td>{item.receivedQty}</td><td>{item.acceptedQty}</td>
                  <td className={item.rejectedQty > 0 ? 'text-red-600 font-medium' : ''}>{item.rejectedQty}</td>
                  <td>{item.reason || '—'}</td>
                </tr>
              ))}
            </tbody>
          </table>
        )}
      </div>

      {open && (
        <Modal title="Add GRN Item" onClose={() => setOpen(false)}
          footer={<><button className="btn btn-secondary btn-sm" onClick={() => setOpen(false)}>Cancel</button>
            <button className="btn btn-primary btn-sm" onClick={handleSubmit(onSubmit)} disabled={addMut.isPending}>{addMut.isPending ? 'Adding…' : 'Add'}</button></>}>
          <form className="space-y-3" noValidate>
            <div><label className="sh-label">PO Line ID *</label><input type="number" className="sh-input" {...register('poLineID', { required: true })} /></div>
            <div className="grid grid-cols-2 gap-3">
              <div><label className="sh-label">Received Qty *</label><input type="number" step="0.01" className="sh-input" {...register('receivedQty', { required: true })} /></div>
              <div><label className="sh-label">Accepted Qty</label><input type="number" step="0.01" className="sh-input" placeholder="defaults to received" {...register('acceptedQty')} /></div>
            </div>
            <div><label className="sh-label">Reason (if rejected)</label><textarea rows={2} className="sh-input" {...register('reason')} /></div>
          </form>
        </Modal>
      )}
    </div>
  )
}

/* ─── Inspections CRUD ─── */
function InspectionsSection({ items, selectedItemId, setSelectedItemId, canEdit = false }) {
  const qc = useQueryClient()
  const [modalOpen, setModalOpen] = useState(false)
  const [editing, setEditing] = useState(null)
  const [confirmDelete, setConfirmDelete] = useState(null)

  const { data, isLoading } = useQuery({
    queryKey: ['inspections', selectedItemId],
    queryFn: () => inspectionApi.getByItemId(selectedItemId),
    enabled: !!selectedItemId,
  })
  const inspections = (data?.data ?? data ?? []).filter(i => !i.isDeleted)

  const { register, handleSubmit, reset, setValue } = useForm()

  const createMut = useMutation({
    mutationFn: inspectionApi.create,
    onSuccess: () => { qc.invalidateQueries(['inspections', selectedItemId]); close(); toast.success('Inspection created') },
    onError: e => toast.error(e.response?.data?.message ?? 'Failed to create inspection'),
  })
  const updateMut = useMutation({
    mutationFn: ({ id, dto }) => inspectionApi.update(id, dto),
    onSuccess: () => { qc.invalidateQueries(['inspections', selectedItemId]); close(); toast.success('Inspection updated') },
    onError: e => toast.error(e.response?.data?.message ?? 'Failed to update inspection'),
  })
  const deleteMut = useMutation({
    mutationFn: inspectionApi.delete,
    onSuccess: () => { qc.invalidateQueries(['inspections', selectedItemId]); setConfirmDelete(null); toast.success('Inspection deleted') },
    onError: e => toast.error(e.response?.data?.message ?? 'Failed to delete inspection'),
  })

  const open = (insp) => {
    setEditing(insp ?? null)
    if (insp) {
      setValue('result',      insp.result)
      setValue('inspectorID', insp.inspectorID ?? '')
      setValue('inspDate',    insp.inspDate ? insp.inspDate.slice(0, 10) : '')
      setValue('status',      insp.status ?? 'Active')
    } else {
      reset({ result: 'Pass', inspectorID: '', inspDate: new Date().toISOString().slice(0, 10), status: 'Active' })
    }
    setModalOpen(true)
  }
  const close = () => { setModalOpen(false); setEditing(null); reset() }

  const onSubmit = (form) => {
    const dto = {
      grnItemID:   selectedItemId,
      result:      form.result,
      inspectorID: form.inspectorID ? Number(form.inspectorID) : null,
      inspDate:    form.inspDate ? new Date(form.inspDate).toISOString() : null,
      status:      form.status,
    }
    if (editing) updateMut.mutate({ id: editing.inspID, dto })
    else createMut.mutate(dto)
  }

  if (!selectedItemId) {
    return (
      <div className="sh-card">
        <p className="text-sm text-gray-500 mb-3">Select a GRN item from the Items tab to manage its inspections.</p>
        {items.length > 0 && (
          <select className="sh-select max-w-xs" onChange={e => setSelectedItemId(Number(e.target.value))} defaultValue="">
            <option value="" disabled>Or pick one here…</option>
            {items.map(i => <option key={i.grnItemID} value={i.grnItemID}>Item #{i.grnItemID} (PO Line {i.poLineID})</option>)}
          </select>
        )}
      </div>
    )
  }

  return (
    <div>
      <div className="flex items-center justify-between mb-3">
        <div className="text-sm text-gray-600">Inspections for GRN Item <span className="font-mono font-medium">#{selectedItemId}</span></div>
        {canEdit && (
          <button className="btn btn-primary btn-sm" onClick={() => open(null)}>+ New Inspection</button>
        )}
      </div>

      <div className="sh-card p-0 overflow-hidden">
        {isLoading ? <div className="py-12 flex justify-center"><Spinner /></div>
          : inspections.length === 0 ? <EmptyState message="No inspections recorded." />
          : (
            <table className="sh-table">
              <thead><tr><th>ID</th><th>Result</th><th>Inspector</th><th>Date</th><th>Status</th><th>Actions</th></tr></thead>
              <tbody>
                {inspections.map(ins => (
                  <tr key={ins.inspID}>
                    <td className="text-gray-400 text-xs">{ins.inspID}</td>
                    <td><span className={`pill ${ins.result === 'Pass' ? 'pill-green' : 'pill-red'}`}>{ins.result}</span></td>
                    <td>{ins.inspectorID ?? '—'}</td>
                    <td className="text-xs text-gray-500">{ins.inspDate ? new Date(ins.inspDate).toLocaleDateString() : '—'}</td>
                    <td><StatusPill status={ins.status} /></td>
                    <td>
                      {canEdit ? (
                        <>
                          <button className="btn btn-ghost btn-sm" onClick={() => open(ins)}>Edit</button>
                          <button className="btn btn-ghost btn-sm" onClick={() => setConfirmDelete(ins)}>Delete</button>
                        </>
                      ) : <span className="text-xs text-gray-300">—</span>}
                    </td>
                  </tr>
                ))}
              </tbody>
            </table>
          )}
      </div>

      {modalOpen && (
        <Modal title={editing ? `Edit Inspection #${editing.inspID}` : 'New Inspection'} onClose={close}
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
              <label className="sh-label">Result *</label>
              <select className="sh-select" {...register('result', { required: true })}>
                <option>Pass</option><option>Fail</option>
              </select>
            </div>
            <div>
              <label className="sh-label">Inspector User ID</label>
              <input type="number" className="sh-input" {...register('inspectorID')} />
            </div>
            <div>
              <label className="sh-label">Inspection Date</label>
              <input type="date" className="sh-input" {...register('inspDate')} />
            </div>
            <div>
              <label className="sh-label">Status</label>
              <select className="sh-select" {...register('status')}>
                <option>Active</option><option>Inactive</option>
              </select>
            </div>
          </form>
        </Modal>
      )}

      {confirmDelete && (
        <Modal title="Delete Inspection" onClose={() => setConfirmDelete(null)}
          footer={
            <>
              <button className="btn btn-secondary btn-sm" onClick={() => setConfirmDelete(null)}>Cancel</button>
              <button className="btn btn-danger btn-sm" onClick={() => deleteMut.mutate(confirmDelete.inspID)} disabled={deleteMut.isPending}>
                {deleteMut.isPending ? 'Deleting…' : 'Delete'}
              </button>
            </>
          }>
          <p className="text-sm text-gray-600">Delete inspection <strong>#{confirmDelete.inspID}</strong>? This cannot be undone.</p>
        </Modal>
      )}
    </div>
  )
}

/* ─── NCRs (read + create against selected item) ─── */
function NcrsSection({ items, selectedItemId, setSelectedItemId, canEdit = false }) {
  const qc = useQueryClient()
  const [modalOpen, setModalOpen] = useState(false)
  const { register, handleSubmit, reset } = useForm()

  const { data, isLoading } = useQuery({
    queryKey: ['ncrs', selectedItemId],
    queryFn: () => ncrApi.getByItemId(selectedItemId),
    enabled: !!selectedItemId,
  })
  const ncrs = (data?.data ?? data ?? []).filter(n => !n.isDeleted)

  const createMut = useMutation({
    mutationFn: ncrApi.create,
    onSuccess: () => { qc.invalidateQueries(['ncrs', selectedItemId]); setModalOpen(false); reset(); toast.success('NCR raised') },
    onError: e => toast.error(e.response?.data?.message ?? 'Failed to raise NCR'),
  })

  const onSubmit = (form) => {
    createMut.mutate({
      grnItemID:   selectedItemId,
      defectType:  form.defectType,
      severity:    form.severity,
      disposition: form.disposition,
      notes:       form.notes,
      status:      'Open',
    })
  }

  if (!selectedItemId) {
    return (
      <div className="sh-card">
        <p className="text-sm text-gray-500 mb-3">Select a GRN item to raise/view NCRs.</p>
        {items.length > 0 && (
          <select className="sh-select max-w-xs" onChange={e => setSelectedItemId(Number(e.target.value))} defaultValue="">
            <option value="" disabled>Pick item…</option>
            {items.map(i => <option key={i.grnItemID} value={i.grnItemID}>Item #{i.grnItemID}</option>)}
          </select>
        )}
      </div>
    )
  }

  return (
    <div>
      <div className="flex items-center justify-between mb-3">
        <div className="text-sm text-gray-600">NCRs for GRN Item <span className="font-mono font-medium">#{selectedItemId}</span></div>
        {canEdit && (
          <button className="btn btn-primary btn-sm" onClick={() => { reset({ severity: 'Minor', disposition: 'Rework' }); setModalOpen(true) }}>+ Raise NCR</button>
        )}
      </div>

      <div className="sh-card p-0 overflow-hidden">
        {isLoading ? <div className="py-12 flex justify-center"><Spinner /></div>
          : ncrs.length === 0 ? <EmptyState message="No NCRs raised." />
          : (
            <table className="sh-table">
              <thead><tr><th>ID</th><th>Defect</th><th>Severity</th><th>Disposition</th><th>Status</th><th>Notes</th></tr></thead>
              <tbody>
                {ncrs.map(n => (
                  <tr key={n.ncrid ?? n.ncrID}>
                    <td className="text-gray-400 text-xs">{n.ncrid ?? n.ncrID}</td>
                    <td>{n.defectType}</td>
                    <td><span className={`pill ${n.severity === 'Critical' ? 'pill-red' : n.severity === 'Major' ? 'pill-amber' : 'pill-gray'}`}>{n.severity}</span></td>
                    <td>{n.disposition}</td>
                    <td><StatusPill status={n.status} /></td>
                    <td className="max-w-xs truncate text-sm">{n.notes ?? '—'}</td>
                  </tr>
                ))}
              </tbody>
            </table>
          )}
      </div>

      {modalOpen && (
        <Modal title="Raise NCR" onClose={() => setModalOpen(false)}
          footer={
            <>
              <button className="btn btn-secondary btn-sm" onClick={() => setModalOpen(false)}>Cancel</button>
              <button className="btn btn-primary btn-sm" onClick={handleSubmit(onSubmit)} disabled={createMut.isPending}>
                {createMut.isPending ? 'Saving…' : 'Create'}
              </button>
            </>
          }>
          <form className="space-y-3" noValidate>
            <div>
              <label className="sh-label">Defect Type *</label>
              <input className="sh-input" {...register('defectType', { required: true })} />
            </div>
            <div>
              <label className="sh-label">Severity *</label>
              <select className="sh-select" {...register('severity', { required: true })}>
                <option>Minor</option><option>Major</option><option>Critical</option>
              </select>
            </div>
            <div>
              <label className="sh-label">Disposition *</label>
              <select className="sh-select" {...register('disposition', { required: true })}>
                <option>UseAsIs</option><option>Rework</option><option>Reject</option><option>Return</option>
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
