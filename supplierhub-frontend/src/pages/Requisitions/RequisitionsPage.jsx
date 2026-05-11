import { useState } from 'react'
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query'
import { useForm } from 'react-hook-form'
import toast from 'react-hot-toast'
import api from '../../api/axiosClient'
import { itemsApi } from '../../api/catalog.api'
import { suppliersApi } from '../../api/suppliers.api'
import { StatusPill } from '../../components/ui/StatusPill'
import { PageHeader, Spinner, EmptyState } from '../../components/ui/index'
import Modal from '../../components/ui/Modal'
import useAuthStore from '../../store/auth.store'

// ── Inline API ─────────────────────────────────────────
const reqApi = {
  getAll:             ()            => api.get('/api/requisitions').then(r => r.data),
  getById:            (id)          => api.get(`/api/requisitions/${id}`).then(r => r.data),
  create:             (dto)         => api.post('/api/requisitions', dto).then(r => r.data),
  getLinesByPrId:     (id)          => api.get(`/api/requisitions/${id}/lines`).then(r => r.data),
  addLine:            (dto)         => api.post('/api/requisitions/lines', dto).then(r => r.data),
  getApprovalsByPrId: (id)          => api.get(`/api/requisitions/${id}/approvals`).then(r => r.data),
  createApproval:     (dto)         => api.post('/api/requisitions/approvals', dto).then(r => r.data),
  updateApproval:     (stepId, dto) => api.put(`/api/requisitions/approvals/${stepId}`, dto).then(r => r.data),
}

const statusClass = (s) => {
  const m = {
    DRAFT: 'pill pill-gray', SUBMITTED: 'pill pill-blue',
    Approved: 'pill pill-green', Rejected: 'pill pill-red',
    Converted: 'pill pill-teal', PENDING: 'pill pill-amber',
    ACTIVE: 'pill pill-blue', COMPLETED: 'pill pill-green',
    REJECTED: 'pill pill-red',
  }
  return m[s] ?? 'pill pill-gray'
}

export default function RequisitionsPage() {
  const qc = useQueryClient()
  const { user } = useAuthStore()
  const [activePR, setActivePR]     = useState(null)
  const [search, setSearch]         = useState('')
  const [showCreate, setShowCreate] = useState(false)

  const { data, isLoading } = useQuery({
    queryKey: ['requisitions'],
    queryFn:  reqApi.getAll,
  })
  const rows = (data?.data ?? data ?? []).filter(r =>
    String(r.prID ?? '').includes(search) ||
    (r.status ?? '').toLowerCase().includes(search.toLowerCase()) ||
    (r.costCenter ?? '').toLowerCase().includes(search.toLowerCase())
  )

  const { register, handleSubmit, reset, formState: { errors } } = useForm()

  const createMut = useMutation({
    mutationFn: reqApi.create,
    onSuccess: (res) => {
      qc.invalidateQueries(['requisitions'])
      setShowCreate(false)
      const id = res?.prID ?? res?.data?.prID
      toast.success(`PR-${id} created successfully`)
    },
    onError: e => toast.error(e.response?.data?.message ?? 'Failed to create PR'),
  })

  const onSubmit = (d) => {
    createMut.mutate({
      RequesterID:     user?.userId ?? 1,
      RequesterUserID: user?.userId ?? 1,
      OrgID:           Number(d.OrgID),
      CostCenter:      d.CostCenter    || null,
      Justification:   d.Justification || null,
      RequestedDate:   d.RequestedDate || null,
      NeededByDate:    d.NeededByDate  || null,
    })
  }

  return (
    <div>
      <PageHeader
        title="Requisitions (PR)"
        subtitle="Purchase requisitions — raise, approve and convert to purchase orders"
        action={
          <button className="btn btn-primary"
            onClick={() => { reset(); setShowCreate(true) }}>
            + New Requisition
          </button>
        }
      />

      <div className="mb-4">
        <input className="sh-input max-w-xs"
          placeholder="Search by PR ID, status or cost center…"
          value={search} onChange={e => setSearch(e.target.value)} />
      </div>

      <div className="sh-card p-0 overflow-hidden mb-5">
        {isLoading ? <div className="flex justify-center py-12"><Spinner /></div>
          : rows.length === 0 ? <EmptyState message="No requisitions found." />
          : (
          <table className="sh-table">
            <thead>
              <tr>
                <th>Sr. No.</th><th>PR ID</th><th>Org ID</th>
                <th>Cost Center</th><th>Needed By</th>
                <th>Status</th><th>Created</th><th>Actions</th>
              </tr>
            </thead>
            <tbody>
              {rows.map((r, index) => (
                <tr key={r.prID}
                  className={activePR?.prID === r.prID ? 'bg-blue-50' : ''}>
                  <td className="text-gray-400 text-xs">{index + 1}</td>
                  <td className="font-medium">PR-{r.prID}</td>
                  <td>{r.orgID}</td>
                  <td className="text-gray-500">{r.costCenter ?? '—'}</td>
                  <td className="text-xs text-gray-500">
                    {r.neededByDate ? new Date(r.neededByDate).toLocaleDateString() : '—'}
                  </td>
                  <td><span className={statusClass(r.status)}>{r.status}</span></td>
                  <td className="text-xs text-gray-500">
                    {r.createdOn ? new Date(r.createdOn).toLocaleDateString() : '—'}
                  </td>
                  <td>
                    <button
                      className={`btn btn-sm ${activePR?.prID === r.prID ? 'btn-primary' : 'btn-secondary'}`}
                      onClick={() => setActivePR(activePR?.prID === r.prID ? null : r)}>
                      {activePR?.prID === r.prID ? 'Close' : 'Open'}
                    </button>
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
        )}
      </div>

      {activePR && (
        <PRDetailPanel
          pr={activePR}
          onClose={() => setActivePR(null)}
          onStatusChange={() => qc.invalidateQueries(['requisitions'])}
        />
      )}

      {showCreate && (
        <Modal title="New Purchase Requisition" onClose={() => setShowCreate(false)}
          footer={
            <>
              <button className="btn btn-secondary btn-sm" onClick={() => setShowCreate(false)}>Cancel</button>
              <button className="btn btn-primary btn-sm"
                onClick={handleSubmit(onSubmit)} disabled={createMut.isPending}>
                {createMut.isPending ? 'Creating…' : 'Create PR'}
              </button>
            </>
          }>
          <form className="space-y-3" noValidate>
            <div className="bg-blue-50 rounded-lg px-3 py-2 text-xs text-blue-700">
              PR will be created as <strong>DRAFT</strong> — add lines then submit for approval.
            </div>
            <div>
              <label className="sh-label">Organization ID *</label>
              <input type="number" className="sh-input"
                {...register('OrgID', { required: 'Organization ID is required' })} />
              {errors.OrgID && <p className="text-red-500 text-xs mt-1">{errors.OrgID.message}</p>}
            </div>
            <div>
              <label className="sh-label">Cost Center</label>
              <input className="sh-input" placeholder="e.g. IT-OPS, PROCUREMENT"
                {...register('CostCenter')} />
            </div>
            <div>
              <label className="sh-label">Justification</label>
              <input className="sh-input" placeholder="Business reason for this purchase"
                {...register('Justification')} />
            </div>
            <div className="grid grid-cols-2 gap-3">
              <div>
                <label className="sh-label">Requested Date</label>
                <input type="date" className="sh-input" {...register('RequestedDate')} />
              </div>
              <div>
                <label className="sh-label">Needed By Date</label>
                <input type="date" className="sh-input" {...register('NeededByDate')} />
              </div>
            </div>
          </form>
        </Modal>
      )}
    </div>
  )
}

function PRDetailPanel({ pr, onClose, onStatusChange }) {
  const [tab, setTab] = useState('Lines')
  return (
    <div className="sh-card p-0 overflow-hidden">
      <div className="flex items-center justify-between px-5 py-3 border-b border-gray-100 bg-blue-50">
        <div>
          <h3 className="font-semibold text-gray-900 text-sm">PR-{pr.prID}</h3>
          <p className="text-xs text-gray-500 mt-0.5">
            Org: {pr.orgID}
            {pr.costCenter && <> · Cost Center: {pr.costCenter}</>}
            {pr.justification && <> · {pr.justification}</>}
            &nbsp;·&nbsp; Status: <strong>{pr.status}</strong>
          </p>
        </div>
        <button className="btn btn-ghost btn-sm text-gray-400" onClick={onClose}>✕ Close</button>
      </div>
      <div className="flex gap-1 px-4 border-b border-gray-100">
        {['Lines', 'Approvals'].map(t => (
          <button key={t} onClick={() => setTab(t)}
            className={`px-4 py-2.5 text-sm font-medium border-b-2 transition-colors ${
              tab === t ? 'border-blue-600 text-blue-700' : 'border-transparent text-gray-500 hover:text-gray-700'}`}>
            {t}
          </button>
        ))}
      </div>
      <div className="p-4">
        {tab === 'Lines'     && <PRLinesTab pr={pr} />}
        {tab === 'Approvals' && <PRApprovalsTab pr={pr} onStatusChange={onStatusChange} />}
      </div>
    </div>
  )
}

function PRLinesTab({ pr }) {
  const qc = useQueryClient()
  const [modal, setModal] = useState(false)
  const { data, isLoading } = useQuery({
    queryKey: ['pr-lines', pr.prID],
    queryFn:  () => reqApi.getLinesByPrId(pr.prID),
  })
  const lines = data?.data ?? data ?? []
  const { data: itemsData } = useQuery({ queryKey: ['items'], queryFn: itemsApi.getAll })
  const items = itemsData?.data ?? itemsData ?? []
  const { data: suppData } = useQuery({ queryKey: ['suppliers'], queryFn: suppliersApi.getAll })
  const suppliers = suppData?.data ?? suppData ?? []
  const { register, handleSubmit, reset } = useForm()

  const addMut = useMutation({
    mutationFn: reqApi.addLine,
    onSuccess: () => { qc.invalidateQueries(['pr-lines', pr.prID]); setModal(false); toast.success('Line added') },
    onError: e => toast.error(e.response?.data?.message ?? 'Failed'),
  })

  const onSubmit = (d) => {
    addMut.mutate({
      PrID: pr.prID,
      ItemID: d.ItemID ? Number(d.ItemID) : null,
      Description: d.Description || null,
      Qty: d.Qty ? Number(d.Qty) : null,
      Uom: d.Uom || null,
      TargetPrice: d.TargetPrice ? Number(d.TargetPrice) : null,
      Currency: d.Currency || null,
      SupplierPreferredID: d.SupplierPreferredID ? Number(d.SupplierPreferredID) : null,
      Notes: d.Notes || null,
      Status: 'PENDING',
    })
  }

  return (
    <div>
      <div className="flex justify-between items-center mb-3">
        <h4 className="text-sm font-medium text-gray-700">PR Lines</h4>
        <button className="btn btn-primary btn-sm" onClick={() => { reset(); setModal(true) }}>+ Add Line</button>
      </div>
      {isLoading ? <div className="flex justify-center py-8"><Spinner /></div>
        : lines.length === 0 ? <EmptyState message="No lines added yet." />
        : (
        <table className="sh-table">
          <thead>
            <tr><th>Sr. No.</th><th>Line ID</th><th>Item</th><th>Description</th>
              <th>Qty</th><th>UoM</th><th>Target Price</th><th>Currency</th>
              <th>Preferred Supplier</th><th>Status</th></tr>
          </thead>
          <tbody>
            {lines.map((l, i) => {
              const itemSku  = items.find(it => it.itemID === l.itemID)?.sku
              const suppName = suppliers.find(s => s.supplierID === l.supplierPreferredID)?.legalName
              return (
                <tr key={l.prLineID}>
                  <td className="text-gray-400 text-xs">{i + 1}</td>
                  <td className="text-gray-400 text-xs">{l.prLineID}</td>
                  <td className="font-medium font-mono text-xs">{itemSku ?? (l.itemID ? `#${l.itemID}` : '—')}</td>
                  <td className="text-gray-500 text-xs max-w-xs truncate">{l.description ?? '—'}</td>
                  <td>{l.qty ?? '—'}</td>
                  <td>{l.uom ?? '—'}</td>
                  <td className="font-medium text-green-700">{l.targetPrice?.toLocaleString() ?? '—'}</td>
                  <td>{l.currency ?? '—'}</td>
                  <td className="text-xs text-gray-500">{suppName ?? (l.supplierPreferredID ? `#${l.supplierPreferredID}` : '—')}</td>
                  <td><span className={statusClass(l.status)}>{l.status}</span></td>
                </tr>
              )
            })}
          </tbody>
        </table>
      )}
      {modal && (
        <Modal title="Add PR Line" onClose={() => setModal(false)}
          footer={
            <>
              <button className="btn btn-secondary btn-sm" onClick={() => setModal(false)}>Cancel</button>
              <button className="btn btn-primary btn-sm" onClick={handleSubmit(onSubmit)} disabled={addMut.isPending}>
                {addMut.isPending ? 'Adding…' : 'Add Line'}
              </button>
            </>
          }>
          <form className="space-y-3" noValidate>
            <div>
              <label className="sh-label">Item from Master <span className="text-gray-400 font-normal">(optional)</span></label>
              <select className="sh-select" {...register('ItemID')}>
                <option value="">— Free-text item —</option>
                {items.map(i => <option key={i.itemID} value={i.itemID}>{i.sku}</option>)}
              </select>
            </div>
            <div>
              <label className="sh-label">Description</label>
              <input className="sh-input" placeholder="Item description if no item selected" {...register('Description')} />
            </div>
            <div className="grid grid-cols-2 gap-3">
              <div><label className="sh-label">Quantity</label>
                <input type="number" step="0.01" className="sh-input" {...register('Qty')} /></div>
              <div><label className="sh-label">Unit of Measure</label>
                <input className="sh-input" placeholder="KG, PCS…" {...register('Uom')} /></div>
            </div>
            <div className="grid grid-cols-2 gap-3">
              <div><label className="sh-label">Target Price</label>
                <input type="number" step="0.01" className="sh-input" {...register('TargetPrice')} /></div>
              <div><label className="sh-label">Currency</label>
                <select className="sh-select" {...register('Currency')}>
                  <option>INR</option><option>USD</option><option>EUR</option>
                </select></div>
            </div>
            <div>
              <label className="sh-label">Preferred Supplier <span className="text-gray-400 font-normal">(optional)</span></label>
              <select className="sh-select" {...register('SupplierPreferredID')}>
                <option value="">— No preference —</option>
                {suppliers.map(s => <option key={s.supplierID} value={s.supplierID}>{s.legalName}</option>)}
              </select>
            </div>
            <div><label className="sh-label">Notes</label>
              <input className="sh-input" {...register('Notes')} /></div>
          </form>
        </Modal>
      )}
    </div>
  )
}

function PRApprovalsTab({ pr, onStatusChange }) {
  const qc = useQueryClient()
  const [createModal, setCreateModal]     = useState(false)
  const [decisionModal, setDecisionModal] = useState(null)

  const { data, isLoading } = useQuery({
    queryKey: ['pr-approvals', pr.prID],
    queryFn:  () => reqApi.getApprovalsByPrId(pr.prID),
  })
  const steps = data?.data ?? data ?? []

  const { data: usersData } = useQuery({
    queryKey: ['users'],
    queryFn:  () => api.get('/api/user').then(r => r.data),
  })
  const users = usersData?.data ?? usersData ?? []

  const { register: regC, handleSubmit: hsC, reset: resetC } = useForm()
  const { register: regD, handleSubmit: hsD, reset: resetD } = useForm()

  const createMut = useMutation({
    mutationFn: reqApi.createApproval,
    onSuccess: () => { qc.invalidateQueries(['pr-approvals', pr.prID]); setCreateModal(false); toast.success('Approver added') },
    onError: e => toast.error(e.response?.data?.message ?? 'Failed'),
  })
  const updateMut = useMutation({
    mutationFn: ({ stepId, dto }) => reqApi.updateApproval(stepId, dto),
    onSuccess: () => {
      qc.invalidateQueries(['pr-approvals', pr.prID])
      onStatusChange()
      setDecisionModal(null)
      toast.success('Decision recorded')
    },
    onError: e => toast.error(e.response?.data?.message ?? 'Failed'),
  })

  return (
    <div>
      <div className="flex justify-between items-center mb-3">
        <h4 className="text-sm font-medium text-gray-700">Approval Workflow</h4>
        <button className="btn btn-primary btn-sm" onClick={() => { resetC({ ApproverID: '', Remarks: '' }); setCreateModal(true) }}>
          + Add Approver
        </button>
      </div>

      {isLoading ? <div className="flex justify-center py-8"><Spinner /></div>
        : steps.length === 0 ? <EmptyState message="No approval steps yet." />
        : (
        <div className="space-y-3">
          {steps.map((step, i) => (
            <div key={step.stepID}
              className={`flex gap-4 items-start p-4 rounded-lg border ${
                step.decision === 'Approved'  ? 'bg-green-50 border-green-200'
                : step.decision === 'Rejected' ? 'bg-red-50 border-red-200'
                : 'bg-gray-50 border-gray-200'}`}>
              <div className={`w-8 h-8 rounded-full flex items-center justify-center text-xs font-bold flex-shrink-0 ${
                step.decision === 'Approved'  ? 'bg-green-500 text-white'
                : step.decision === 'Rejected' ? 'bg-red-500 text-white'
                : 'bg-gray-300 text-gray-700'}`}>
                {i + 1}
              </div>
              <div className="flex-1 min-w-0">
                <div className="flex items-center gap-2 flex-wrap">
                  <span className="font-medium text-sm text-gray-800">
                    {users.find(u => u.userID === step.approverID)?.userName ?? `User #${step.approverID}`}
                  </span>
                  <span className={statusClass(step.decision)}>{step.decision}</span>
                  <span className={statusClass(step.status)}>{step.status}</span>
                </div>
                {step.decisionDate && (
                  <p className="text-xs text-gray-500 mt-0.5">Decided: {new Date(step.decisionDate).toLocaleDateString()}</p>
                )}
                {step.remarks && <p className="text-xs text-gray-600 mt-1 italic">"{step.remarks}"</p>}
              </div>
              {step.decision === 'PENDING' && (
                <button className="btn btn-secondary btn-sm flex-shrink-0"
                  onClick={() => { setDecisionModal(step); resetD({ Decision: 'PENDING', Remarks: '' }) }}>
                  Record Decision
                </button>
              )}
            </div>
          ))}
        </div>
      )}

      {createModal && (
        <Modal title="Add Approval Step" onClose={() => setCreateModal(false)}
          footer={
            <>
              <button className="btn btn-secondary btn-sm" onClick={() => setCreateModal(false)}>Cancel</button>
              <button className="btn btn-primary btn-sm"
                onClick={hsC(d => createMut.mutate({ PrID: pr.prID, ApproverID: Number(d.ApproverID), Decision: 'PENDING', Status: 'ACTIVE', Remarks: d.Remarks || null }))}
                disabled={createMut.isPending}>
                {createMut.isPending ? 'Adding…' : 'Add Approver'}
              </button>
            </>
          }>
          <form className="space-y-3" noValidate>
            <div>
              <label className="sh-label">Approver *</label>
              <select className="sh-select" {...regC('ApproverID', { required: true })}>
                <option value="">— Select approver —</option>
                {users.map(u => <option key={u.userID} value={u.userID}>{u.userName} ({u.email})</option>)}
              </select>
            </div>
            <div><label className="sh-label">Remarks</label>
              <input className="sh-input" placeholder="Instructions for the approver" {...regC('Remarks')} /></div>
          </form>
        </Modal>
      )}

      {decisionModal && (
        <Modal title={`Record Decision — Step #${decisionModal.stepID}`} onClose={() => setDecisionModal(null)}
          footer={
            <>
              <button className="btn btn-secondary btn-sm" onClick={() => setDecisionModal(null)}>Cancel</button>
              <button className="btn btn-primary btn-sm"
                onClick={hsD(d => updateMut.mutate({ stepId: decisionModal.stepID, dto: { Decision: d.Decision, DecisionDate: new Date().toISOString(), Remarks: d.Remarks || null, Status: d.Decision === 'Approved' ? 'COMPLETED' : d.Decision === 'Rejected' ? 'REJECTED' : 'ACTIVE' } }))}
                disabled={updateMut.isPending}>
                {updateMut.isPending ? 'Saving…' : 'Record Decision'}
              </button>
            </>
          }>
          <form className="space-y-3" noValidate>
            <div>
              <label className="sh-label">Decision *</label>
              <select className="sh-select" {...regD('Decision', { required: true })}>
                <option value="PENDING">Pending</option>
                <option value="Approved">Approved</option>
                <option value="Rejected">Rejected</option>
              </select>
            </div>
            <div><label className="sh-label">Remarks</label>
              <input className="sh-input" placeholder="Reason for decision" {...regD('Remarks')} /></div>
          </form>
        </Modal>
      )}
    </div>
  )
}