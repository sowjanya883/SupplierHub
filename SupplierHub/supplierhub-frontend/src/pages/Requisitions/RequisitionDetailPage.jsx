import { useState } from 'react'
import { useParams, useNavigate } from 'react-router-dom'
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query'
import { useForm } from 'react-hook-form'
import toast from 'react-hot-toast'
import { requisitionsApi } from '../../api/procurement.api'
import { StatusPill } from '../../components/ui/StatusPill'
import { Spinner, EmptyState } from '../../components/ui/index'
import Modal from '../../components/ui/Modal'
import useAuthStore from '../../store/auth.store'

const ALL_TABS = ['Lines', 'Approvals']

export default function RequisitionDetailPage() {
  const { id } = useParams()
  const navigate = useNavigate()
  const qc = useQueryClient()
  const user = useAuthStore(s => s.user)
  const roles = user?.roles ?? []
  const isSupplier = roles.includes('SupplierUser')
  const isAdmin    = roles.includes('Admin')

  // SupplierUsers only see PR Lines (read-only); no Approvals tab (internal).
  const visibleTabs = isSupplier && !isAdmin ? ['Lines'] : ALL_TABS
  const [tab, setTab] = useState(visibleTabs[0])

  const { data, isLoading } = useQuery({ queryKey: ['requisition', id], queryFn: () => requisitionsApi.getById(id) })

  const statusMut = useMutation({
    mutationFn: (status) => requisitionsApi.updateStatus(Number(id), status),
    onSuccess: (_, status) => {
      qc.invalidateQueries({ queryKey: ['requisition', id] })
      qc.invalidateQueries({ queryKey: ['requisitions'] })
      toast.success(`Marked as ${status}`)
    },
    onError: e => toast.error(extract(e) ?? 'Failed to update status'),
  })

  if (isLoading) return <div className="flex justify-center py-16"><Spinner /></div>
  const pr = data?.data ?? data
  if (!pr) return <p>Requisition not found.</p>

  const prId = Number(id)
  const currentStatus = (pr.status ?? '').toLowerCase()
  // Supplier can act once internal approval is done (status 'Approved'), or on
  // legacy DRAFT PRs. Don't allow further changes once already Accepted/Declined.
  const supplierCanAct = isSupplier
    && currentStatus !== 'accepted'
    && currentStatus !== 'declined'

  return (
    <div>
      <div className="page-header">
        <div>
          <button onClick={() => navigate(-1)} className="text-sm text-blue-600 hover:underline mb-1 block">← Back</button>
          <h1 className="page-title">PR #{pr.prID}</h1>
        </div>
        <StatusPill status={pr.status} />
      </div>

      <div className="sh-card mb-4">
        <div className="grid grid-cols-3 gap-4 text-sm">
          <div><span className="text-gray-500">Requester</span><p className="font-medium mt-0.5">{pr.requesterID}</p></div>
          <div><span className="text-gray-500">Organization</span><p className="font-medium mt-0.5">{pr.orgID}</p></div>
          <div><span className="text-gray-500">Cost Center</span><p className="font-medium mt-0.5">{pr.costCenter || '—'}</p></div>
          <div><span className="text-gray-500">Requested</span><p className="font-medium mt-0.5">{pr.requestedDate ? new Date(pr.requestedDate).toLocaleDateString() : '—'}</p></div>
          <div><span className="text-gray-500">Needed By</span><p className="font-medium mt-0.5">{pr.neededByDate ? new Date(pr.neededByDate).toLocaleDateString() : '—'}</p></div>
          <div><span className="text-gray-500">Justification</span><p className="font-medium mt-0.5">{pr.justification || '—'}</p></div>
        </div>

        {isSupplier && (
          <div className="mt-4 pt-4 border-t border-gray-100 flex items-center gap-3">
            <p className="text-sm text-gray-600 flex-1">
              {supplierCanAct
                ? 'Respond to this requisition — accept to indicate you can fulfil it, decline if you cannot.'
                : `You have already marked this PR as ${pr.status}. Buyer is notified.`}
            </p>
            {supplierCanAct && (
              <>
                <button
                  className="btn btn-primary btn-sm"
                  onClick={() => statusMut.mutate('Accepted')}
                  disabled={statusMut.isPending}
                >
                  {statusMut.isPending ? '…' : 'Accept'}
                </button>
                <button
                  className="btn btn-danger btn-sm"
                  onClick={() => statusMut.mutate('Declined')}
                  disabled={statusMut.isPending}
                >
                  {statusMut.isPending ? '…' : 'Decline'}
                </button>
              </>
            )}
          </div>
        )}
      </div>

      <div className="flex gap-1 mb-4 border-b border-gray-200">
        {visibleTabs.map(t => (
          <button key={t} onClick={() => setTab(t)}
            className={`px-4 py-2 text-sm font-medium border-b-2 transition-colors ${
              tab === t ? 'border-blue-600 text-blue-700' : 'border-transparent text-gray-500 hover:text-gray-700'}`}>
            {t}
          </button>
        ))}
      </div>

      {tab === 'Lines'     && <LinesSection prId={prId} />}
      {tab === 'Approvals' && <ApprovalsSection prId={prId} />}
    </div>
  )
}

/* ─── PR Lines ─── (Buyer / Admin add; CategoryManager / Approver view only) */
function LinesSection({ prId }) {
  const qc = useQueryClient()
  const user = useAuthStore(s => s.user)
  const roles = user?.roles ?? []
  const canAdd = roles.some(r => ['Admin','Buyer'].includes(r))

  const [modalOpen, setModalOpen] = useState(false)
  const { register, handleSubmit, reset } = useForm()

  const { data, isLoading } = useQuery({
    queryKey: ['pr-lines', prId],
    queryFn: () => requisitionsApi.getLines(prId),
  })
  const rows = (data?.data ?? data ?? []).filter(l => !l.isDeleted)

  const addMut = useMutation({
    mutationFn: requisitionsApi.addLine,
    onSuccess: () => { qc.invalidateQueries({ queryKey: ['pr-lines', prId] }); setModalOpen(false); reset(); toast.success('Line added') },
    onError: e => toast.error(extract(e) ?? 'Failed to add line'),
  })

  const onSubmit = (form) => {
    addMut.mutate({
      prID:                prId,
      itemID:              form.itemID === '' ? null : Number(form.itemID),
      description:         form.description?.trim() || '',
      qty:                 form.qty === '' ? null : Number(form.qty),
      uom:                 form.uom?.trim() || '',
      targetPrice:         form.targetPrice === '' ? null : Number(form.targetPrice),
      currency:            form.currency?.trim() || 'USD',
      supplierPreferredID: form.supplierPreferredID === '' ? null : Number(form.supplierPreferredID),
      notes:               form.notes ?? '',
      status:              'Active',
    })
  }

  return (
    <div>
      <div className="flex items-center justify-between mb-3">
        <div className="text-sm text-gray-600">Line items on this requisition</div>
        {canAdd && (
          <button className="btn btn-primary btn-sm" onClick={() => {
            reset({ itemID: '', description: '', qty: 1, uom: 'EA', targetPrice: '', currency: 'USD', supplierPreferredID: '', notes: '' })
            setModalOpen(true)
          }}>+ Add Line</button>
        )}
      </div>

      <div className="sh-card p-0 overflow-hidden">
        {isLoading ? <div className="py-12 flex justify-center"><Spinner /></div>
          : rows.length === 0 ? <EmptyState message="No line items." />
          : (
          <table className="sh-table">
            <thead><tr><th>Line ID</th><th>Item</th><th>Description</th><th>Qty</th><th>UoM</th><th>Target Price</th><th>Pref. Supplier</th></tr></thead>
            <tbody>
              {rows.map(l => (
                <tr key={l.prLineID}>
                  <td className="text-gray-400 text-xs">{l.prLineID}</td>
                  <td>{l.itemID ?? '—'}</td>
                  <td className="max-w-xs truncate text-sm">{l.description ?? '—'}</td>
                  <td>{l.qty ?? '—'}</td>
                  <td>{l.uom ?? '—'}</td>
                  <td>{l.targetPrice != null ? `${l.targetPrice} ${l.currency ?? ''}` : '—'}</td>
                  <td>{l.supplierPreferredID ?? '—'}</td>
                </tr>
              ))}
            </tbody>
          </table>
        )}
      </div>

      {modalOpen && (
        <Modal title="Add PR Line" onClose={() => setModalOpen(false)}
          footer={
            <>
              <button className="btn btn-secondary btn-sm" onClick={() => setModalOpen(false)}>Cancel</button>
              <button className="btn btn-primary btn-sm" onClick={handleSubmit(onSubmit)} disabled={addMut.isPending}>
                {addMut.isPending ? 'Adding…' : 'Add'}
              </button>
            </>
          }>
          <form className="space-y-3" noValidate>
            <div className="grid grid-cols-2 gap-3">
              <div>
                <label className="sh-label">Item ID</label>
                <input type="number" className="sh-input" {...register('itemID')} />
              </div>
              <div>
                <label className="sh-label">UoM</label>
                <input className="sh-input" placeholder="EA, KG…" {...register('uom')} />
              </div>
            </div>
            <div>
              <label className="sh-label">Description</label>
              <input className="sh-input" {...register('description')} />
            </div>
            <div className="grid grid-cols-3 gap-3">
              <div>
                <label className="sh-label">Qty</label>
                <input type="number" step="0.01" className="sh-input" {...register('qty')} />
              </div>
              <div>
                <label className="sh-label">Target Price</label>
                <input type="number" step="0.01" className="sh-input" {...register('targetPrice')} />
              </div>
              <div>
                <label className="sh-label">Currency</label>
                <input className="sh-input" {...register('currency')} />
              </div>
            </div>
            <div>
              <label className="sh-label">Preferred Supplier ID</label>
              <input type="number" className="sh-input" {...register('supplierPreferredID')} />
            </div>
            <div>
              <label className="sh-label">Notes</label>
              <textarea rows={2} className="sh-input" {...register('notes')} />
            </div>
          </form>
        </Modal>
      )}
    </div>
  )
}

/* ─── Approval steps ─── (Requester adds approver; Admin/CategoryManager decide) */
function ApprovalsSection({ prId }) {
  const qc = useQueryClient()
  const user = useAuthStore(s => s.user)
  const roles = user?.roles ?? []
  const canAddApprover = roles.some(r => ['Admin','Buyer'].includes(r))
  const canDecide      = roles.some(r => ['Admin','CategoryManager'].includes(r))

  const [modalOpen, setModalOpen] = useState(false)
  const [decisionOn, setDecisionOn] = useState(null)
  const { register, handleSubmit, reset } = useForm()
  const decisionForm = useForm()

  const { data, isLoading } = useQuery({
    queryKey: ['pr-approvals', prId],
    queryFn: () => requisitionsApi.getApprovals(prId),
  })
  const rows = (data?.data ?? data ?? []).filter(s => !s.isDeleted)

  const createMut = useMutation({
    mutationFn: requisitionsApi.createApprovalStep,
    onSuccess: () => { qc.invalidateQueries({ queryKey: ['pr-approvals', prId] }); setModalOpen(false); reset(); toast.success('Approval step added') },
    onError: e => toast.error(extract(e) ?? 'Failed to add approval'),
  })
  const updateMut = useMutation({
    mutationFn: ({ stepId, dto }) => requisitionsApi.updateApproval(stepId, dto),
    onSuccess: () => { qc.invalidateQueries({ queryKey: ['pr-approvals', prId] }); setDecisionOn(null); decisionForm.reset(); toast.success('Decision recorded') },
    onError: e => toast.error(extract(e) ?? 'Failed to record decision'),
  })

  const onAdd = (form) => {
    createMut.mutate({
      prID:       prId,
      approverID: Number(form.approverID || user?.userId || 0),
      decision:   'PENDING',
      status:     'Active',
      remarks:    form.remarks ?? '',
    })
  }

  const onDecide = (form) => {
    updateMut.mutate({
      stepId: decisionOn.stepID,
      dto: {
        decision: form.decision,
        decisionDate: new Date().toISOString(),
        remarks: form.remarks ?? '',
        status: 'Active',
      },
    })
  }

  return (
    <div>
      <div className="flex items-center justify-between mb-3">
        <div className="text-sm text-gray-600">Approval workflow chain</div>
        {canAddApprover && (
          <button className="btn btn-primary btn-sm" onClick={() => {
            reset({ approverID: user?.userId ?? '', remarks: '' })
            setModalOpen(true)
          }}>+ Add Approver</button>
        )}
      </div>

      <div className="sh-card p-0 overflow-hidden">
        {isLoading ? <div className="py-12 flex justify-center"><Spinner /></div>
          : rows.length === 0 ? <EmptyState message="No approval steps yet — add an approver to start the workflow." />
          : (
          <table className="sh-table">
            <thead><tr><th>Step</th><th>Approver</th><th>Decision</th><th>Decided</th><th>Remarks</th><th>Actions</th></tr></thead>
            <tbody>
              {rows.map(s => {
                const isPending = (s.decision ?? '').toUpperCase() === 'PENDING'
                return (
                  <tr key={s.stepID}>
                    <td className="text-gray-400 text-xs">#{s.stepID}</td>
                    <td>{s.approverID}</td>
                    <td>
                      <span className={`pill ${
                        isPending ? 'pill-amber'
                        : s.decision === 'Approved' ? 'pill-green'
                        : s.decision === 'Rejected' ? 'pill-red'
                        : 'pill-gray'
                      }`}>{s.decision}</span>
                    </td>
                    <td className="text-xs text-gray-500">{s.decisionDate ? new Date(s.decisionDate).toLocaleString() : '—'}</td>
                    <td className="max-w-xs truncate text-sm">{s.remarks || '—'}</td>
                    <td>
                      {isPending && canDecide ? (
                        <button className="btn btn-ghost btn-sm" onClick={() => {
                          decisionForm.reset({ decision: 'Approved', remarks: '' })
                          setDecisionOn(s)
                        }}>Decide</button>
                      ) : isPending ? (
                        <span className="text-xs text-gray-400">Awaiting approver</span>
                      ) : null}
                    </td>
                  </tr>
                )
              })}
            </tbody>
          </table>
        )}
      </div>

      {modalOpen && (
        <Modal title="Add Approval Step" onClose={() => setModalOpen(false)}
          footer={
            <>
              <button className="btn btn-secondary btn-sm" onClick={() => setModalOpen(false)}>Cancel</button>
              <button className="btn btn-primary btn-sm" onClick={handleSubmit(onAdd)} disabled={createMut.isPending}>
                {createMut.isPending ? 'Adding…' : 'Add'}
              </button>
            </>
          }>
          <form className="space-y-3" noValidate>
            <div>
              <label className="sh-label">Approver User ID *</label>
              <input type="number" className="sh-input" {...register('approverID', { required: true })} />
            </div>
            <div>
              <label className="sh-label">Initial Remarks</label>
              <textarea rows={3} className="sh-input" {...register('remarks')} />
            </div>
          </form>
        </Modal>
      )}

      {decisionOn && (
        <Modal title={`Decide on Step #${decisionOn.stepID}`} onClose={() => setDecisionOn(null)}
          footer={
            <>
              <button className="btn btn-secondary btn-sm" onClick={() => setDecisionOn(null)}>Cancel</button>
              <button className="btn btn-primary btn-sm" onClick={decisionForm.handleSubmit(onDecide)} disabled={updateMut.isPending}>
                {updateMut.isPending ? 'Saving…' : 'Submit Decision'}
              </button>
            </>
          }>
          <form className="space-y-3" noValidate>
            <div>
              <label className="sh-label">Decision *</label>
              <select className="sh-select" {...decisionForm.register('decision', { required: true })}>
                <option>Approved</option><option>Rejected</option>
              </select>
            </div>
            <div>
              <label className="sh-label">Remarks</label>
              <textarea rows={3} className="sh-input" {...decisionForm.register('remarks')} />
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
