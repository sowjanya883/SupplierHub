import { useState } from 'react'
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query'
import { useForm } from 'react-hook-form'
import toast from 'react-hot-toast'
import { adminApi } from '../../api/admin.api'
import { PageHeader, Spinner, EmptyState } from '../../components/ui/index'
import Modal from '../../components/ui/Modal'
import { StatusPill } from '../../components/ui/StatusPill'

const TABS = ['System Config', 'Approval Rules', 'Role Assignment']

export default function AdminPage() {
  const [tab, setTab] = useState('System Config')

  return (
    <div>
      <PageHeader title="Admin Configuration" subtitle="System settings, approval rules, and role management" />
      <div className="flex gap-1 mb-5 border-b border-gray-200">
        {TABS.map(t => (
          <button key={t} onClick={() => setTab(t)}
            className={`px-4 py-2 text-sm font-medium border-b-2 transition-colors ${
              tab === t ? 'border-blue-600 text-blue-700' : 'border-transparent text-gray-500 hover:text-gray-700'
            }`}>
            {t}
          </button>
        ))}
      </div>
      {tab === 'System Config'   && <SystemConfigTab />}
      {tab === 'Approval Rules'  && <ApprovalRulesTab />}
      {tab === 'Role Assignment' && <RoleAssignmentTab />}
    </div>
  )
}

/* ─── System Config ─────────────────────────────────────── */
function SystemConfigTab() {
  const qc = useQueryClient()
  const [showCreate, setShowCreate] = useState(false)
  const [editing, setEditing] = useState(null)

  const { data, isLoading } = useQuery({
    queryKey: ['sys-configs'],
    queryFn: adminApi.getAllSystemConfigs,
  })
  const configs = data?.data ?? data ?? []

  const { register, handleSubmit, reset, setValue } = useForm()

  const createMut = useMutation({
    mutationFn: adminApi.createSystemConfig,
    onSuccess: () => { qc.invalidateQueries(['sys-configs']); setShowCreate(false); toast.success('Config created') },
    onError: e => toast.error(e.response?.data?.message ?? 'Failed to create config'),
  })
  const updateMut = useMutation({
    mutationFn: adminApi.updateSystemConfig,
    onSuccess: () => { qc.invalidateQueries(['sys-configs']); setEditing(null); toast.success('Config updated') },
    onError: e => toast.error(e.response?.data?.message ?? 'Failed to update config'),
  })

  const openEdit = (c) => {
    setEditing(c)
    setValue('key', c.key)
    setValue('value', c.value)
    setValue('scope', c.scope)
  }

  const onSubmit = (d) => {
    if (editing) updateMut.mutate({ ...d, configID: editing.configID })
    else createMut.mutate(d)
  }

  const isPending = createMut.isPending || updateMut.isPending

  return (
    <div>
      <div className="flex justify-end mb-3">
        <button className="btn btn-primary btn-sm" onClick={() => { reset(); setEditing(null); setShowCreate(true) }}>
          + Add Config
        </button>
      </div>

      <div className="sh-card p-0 overflow-hidden">
        {isLoading ? <div className="flex justify-center py-8"><Spinner /></div>
          : configs.length === 0 ? <EmptyState message="No system configurations found." />
          : (
          <table className="sh-table">
            <thead>
              <tr><th>ID</th><th>Key</th><th>Value</th><th>Scope</th><th>Updated By</th><th>Updated</th><th>Actions</th></tr>
            </thead>
            <tbody>
              {configs.map(c => (
                <tr key={c.configID}>
                  <td className="text-gray-400 text-xs">{c.configID}</td>
                  <td className="font-mono text-xs font-medium">{c.key}</td>
                  <td className="max-w-xs truncate text-sm">{c.value}</td>
                  <td><span className="pill pill-gray">{c.scope}</span></td>
                  <td className="text-xs text-gray-500">{c.updatedBy || '—'}</td>
                  <td className="text-xs text-gray-500">
                    {c.updatedDate ? new Date(c.updatedDate).toLocaleDateString() : '—'}
                  </td>
                  <td>
                    <button className="btn btn-ghost btn-sm" onClick={() => { openEdit(c); setShowCreate(true) }}>
                      Edit
                    </button>
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
        )}
      </div>

      {showCreate && (
        <Modal
          title={editing ? `Edit Config: ${editing.key}` : 'New System Config'}
          onClose={() => { setShowCreate(false); setEditing(null) }}
          footer={
            <>
              <button className="btn btn-secondary btn-sm" onClick={() => { setShowCreate(false); setEditing(null) }}>Cancel</button>
              <button className="btn btn-primary btn-sm" onClick={handleSubmit(onSubmit)} disabled={isPending}>
                {isPending ? 'Saving…' : editing ? 'Update' : 'Create'}
              </button>
            </>
          }
        >
          <form className="space-y-3" noValidate>
            <div>
              <label className="sh-label">Config Key *</label>
              <input className="sh-input font-mono" placeholder="e.g. MAX_PO_AMOUNT" {...register('key', { required: 'Required' })} />
            </div>
            <div>
              <label className="sh-label">Value *</label>
              <input className="sh-input" {...register('value', { required: 'Required' })} />
            </div>
            <div>
              <label className="sh-label">Scope</label>
              <select className="sh-select" {...register('scope')}>
                <option>Global</option><option>Org</option><option>Site</option>
              </select>
            </div>
          </form>
        </Modal>
      )}
    </div>
  )
}

/* ─── Approval Rules ────────────────────────────────────── */
function ApprovalRulesTab() {
  const qc = useQueryClient()
  const [showCreate, setShowCreate] = useState(false)
  const [editing, setEditing] = useState(null)

  const { data, isLoading } = useQuery({
    queryKey: ['approval-rules'],
    queryFn: adminApi.getAllApprovalRules,
  })
  const rules = data?.data ?? data ?? []

  const { register, handleSubmit, reset, setValue } = useForm()

  const createMut = useMutation({
    mutationFn: adminApi.createApprovalRule,
    onSuccess: () => { qc.invalidateQueries(['approval-rules']); setShowCreate(false); toast.success('Approval rule created') },
    onError: e => toast.error(e.response?.data?.message ?? 'Error'),
  })
  const updateMut = useMutation({
    mutationFn: adminApi.updateApprovalRule,
    onSuccess: () => { qc.invalidateQueries(['approval-rules']); setEditing(null); setShowCreate(false); toast.success('Rule updated') },
    onError: e => toast.error(e.response?.data?.message ?? 'Error'),
  })

  const openEdit = (r) => {
    setEditing(r)
    setValue('scope', r.scope)
    setValue('severity', r.severity)
    setShowCreate(true)
  }

  const onSubmit = (d) => {
    if (editing) updateMut.mutate({ ...d, ruleID: editing.ruleID })
    else createMut.mutate(d)
  }

  const isPending = createMut.isPending || updateMut.isPending

  return (
    <div>
      <div className="flex justify-end mb-3">
        <button className="btn btn-primary btn-sm" onClick={() => { reset(); setEditing(null); setShowCreate(true) }}>
          + Add Rule
        </button>
      </div>

      <div className="sh-card p-0 overflow-hidden">
        {isLoading ? <div className="flex justify-center py-8"><Spinner /></div>
          : rules.length === 0 ? <EmptyState message="No approval rules configured." />
          : (
          <table className="sh-table">
            <thead>
              <tr><th>Rule ID</th><th>Scope</th><th>Severity</th><th>State</th><th>Created</th><th>Updated</th><th>Actions</th></tr>
            </thead>
            <tbody>
              {rules.map(r => (
                <tr key={r.ruleID}>
                  <td className="text-gray-400 text-xs">{r.ruleID}</td>
                  <td><span className="pill pill-blue">{r.scope}</span></td>
                  <td><span className={`pill ${r.severity === 'Block' ? 'pill-red' : 'pill-amber'}`}>{r.severity}</span></td>
                  <td><StatusPill status={r.isDeleted ? 'Deleted' : 'Active'} /></td>
                  <td className="text-xs text-gray-500">{r.createdOn ? new Date(r.createdOn).toLocaleDateString() : '—'}</td>
                  <td className="text-xs text-gray-500">{r.updatedOn ? new Date(r.updatedOn).toLocaleDateString() : '—'}</td>
                  <td>
                    <button className="btn btn-ghost btn-sm" onClick={() => openEdit(r)}>Edit</button>
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
        )}
      </div>

      {showCreate && (
        <Modal
          title={editing ? `Edit Rule #${editing.ruleID}` : 'New Approval Rule'}
          onClose={() => { setShowCreate(false); setEditing(null) }}
          footer={
            <>
              <button className="btn btn-secondary btn-sm" onClick={() => { setShowCreate(false); setEditing(null) }}>Cancel</button>
              <button className="btn btn-primary btn-sm" onClick={handleSubmit(onSubmit)} disabled={isPending}>
                {isPending ? 'Saving…' : editing ? 'Update' : 'Create'}
              </button>
            </>
          }
        >
          <form className="space-y-3" noValidate>
            <div>
              <label className="sh-label">Scope * <span className="text-gray-400 font-normal">(max 30 chars)</span></label>
              <select className="sh-select" {...register('scope', { required: 'Required' })}>
                <option>PR</option><option>PO</option><option>RFx</option><option>Invoice</option>
              </select>
            </div>
            <div>
              <label className="sh-label">Severity *</label>
              <select className="sh-select" {...register('severity', { required: 'Required' })}>
                <option>Info</option><option>Block</option>
              </select>
            </div>
            {editing && (
              <div>
                <label className="sh-label">Soft Delete</label>
                <select className="sh-select" {...register('isDeleted')}>
                  <option value={false}>Active</option>
                  <option value={true}>Deleted</option>
                </select>
              </div>
            )}
          </form>
        </Modal>
      )}
    </div>
  )
}

/* ─── Role Assignment ───────────────────────────────────── */
function RoleAssignmentTab() {
  const { register: regAssign, handleSubmit: hsAssign, reset: resetAssign } = useForm()
  const { register: regDelete, handleSubmit: hsDelete, reset: resetDelete } = useForm()

  const assignMut = useMutation({
    mutationFn: adminApi.assignRole,
    onSuccess: () => { resetAssign(); toast.success('Role assigned successfully') },
    onError: e => toast.error(e.response?.data?.message ?? 'Failed to assign role'),
  })
  const removeMut = useMutation({
    mutationFn: adminApi.deleteRole,
    onSuccess: () => { resetDelete(); toast.success('Role removed successfully') },
    onError: e => toast.error(e.response?.data?.message ?? 'Failed to remove role'),
  })

  const ROLE_OPTIONS = ['Admin','Buyer','CategoryManager','SupplierUser','ComplianceOfficer','AccountsPayable','ReceivingUser']

  return (
    <div className="grid grid-cols-1 gap-5 md:grid-cols-2">
      {/* Assign */}
      <div className="sh-card">
        <h3 className="font-semibold text-gray-800 text-sm mb-4">Assign Role to User</h3>
        <form className="space-y-3" onSubmit={hsAssign(d => assignMut.mutate({ userId: Number(d.userId), role: d.role }))} noValidate>
          <div>
            <label className="sh-label">User ID *</label>
            <input type="number" className="sh-input" placeholder="Enter user ID"
              {...regAssign('userId', { required: true })} />
          </div>
          <div>
            <label className="sh-label">Role *</label>
            <select className="sh-select" {...regAssign('role', { required: true })}>
              {ROLE_OPTIONS.map(r => <option key={r}>{r}</option>)}
            </select>
          </div>
          <button type="submit" className="btn btn-primary w-full justify-center" disabled={assignMut.isPending}>
            {assignMut.isPending ? 'Assigning…' : 'Assign Role'}
          </button>
        </form>
      </div>

      {/* Remove */}
      <div className="sh-card">
        <h3 className="font-semibold text-gray-800 text-sm mb-4">Remove Role from User</h3>
        <form className="space-y-3" onSubmit={hsDelete(d => removeMut.mutate({ userId: Number(d.userId), role: d.role }))} noValidate>
          <div>
            <label className="sh-label">User ID *</label>
            <input type="number" className="sh-input" placeholder="Enter user ID"
              {...regDelete('userId', { required: true })} />
          </div>
          <div>
            <label className="sh-label">Role to Remove *</label>
            <select className="sh-select" {...regDelete('role', { required: true })}>
              {ROLE_OPTIONS.map(r => <option key={r}>{r}</option>)}
            </select>
          </div>
          <button type="submit" className="btn btn-danger w-full justify-center" disabled={removeMut.isPending}>
            {removeMut.isPending ? 'Removing…' : 'Remove Role'}
          </button>
        </form>
      </div>
    </div>
  )
}
