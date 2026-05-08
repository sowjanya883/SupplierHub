import { useState } from 'react'
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query'
import { useForm } from 'react-hook-form'
import toast from 'react-hot-toast'
import { adminApi } from '../../api/admin.api'
import { permissionsApi, rolePermissionsApi, rolesApi } from '../../api/operations.api'
import { PageHeader, Spinner, EmptyState } from '../../components/ui/index'
import Modal from '../../components/ui/Modal'
import { StatusPill } from '../../components/ui/StatusPill'

const TABS = ['System Config', 'Approval Rules', 'Role Assignment', 'Permissions', 'Role-Permissions']

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
      {tab === 'System Config'    && <SystemConfigTab />}
      {tab === 'Approval Rules'   && <ApprovalRulesTab />}
      {tab === 'Role Assignment'  && <RoleAssignmentTab />}
      {tab === 'Permissions'      && <PermissionsTab />}
      {tab === 'Role-Permissions' && <RolePermissionsTab />}
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
    setValue('configKey', c.configKey ?? c.key)
    setValue('value', c.value ?? '')
    setValue('scope', c.scope ?? 'Global')
    setValue('status', c.status ?? 'Active')
  }

  const onSubmit = (d) => {
    const payload = {
      configKey: d.configKey?.trim(),
      value: d.value?.trim() ?? '',
      scope: d.scope || 'Global',
      status: d.status || 'Active',
    }
    if (editing) {
      updateMut.mutate({
        configID: editing.configID,
        value: payload.value,
        status: payload.status,
      })
    } else {
      createMut.mutate(payload)
    }
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
                  <td className="font-mono text-xs font-medium">{c.configKey ?? c.key}</td>
                  <td className="max-w-xs truncate text-sm">{c.value}</td>
                  <td><span className="pill pill-gray">{c.scope}</span></td>
                  <td className="text-xs text-gray-500">{c.updatedBy || '—'}</td>
                  <td className="text-xs text-gray-500">
                    {c.updatedOn ? new Date(c.updatedOn).toLocaleDateString() : '—'}
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
              <input
                className="sh-input font-mono"
                placeholder="e.g. MAX_PO_AMOUNT"
                disabled={!!editing}
                {...register('configKey', { required: 'Required' })}
              />
            </div>
            <div>
              <label className="sh-label">Value</label>
              <input className="sh-input" {...register('value')} />
            </div>
            <div>
              <label className="sh-label">Scope *</label>
              <select className="sh-select" {...register('scope', { required: 'Required' })}>
                <option>Global</option><option>Org</option><option>Site</option>
              </select>
            </div>
            <div>
              <label className="sh-label">Status *</label>
              <select className="sh-select" {...register('status', { required: 'Required' })}>
                <option>Active</option><option>Inactive</option>
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
    setValue('isDeleted', r.isDeleted ? 'true' : 'false')
    setShowCreate(true)
  }

  const onSubmit = (d) => {
    const payload = {
      scope: d.scope,
      severity: d.severity,
      isDeleted: d.isDeleted === 'true' || d.isDeleted === true,
    }
    if (editing) updateMut.mutate({ ...payload, ruleID: editing.ruleID })
    else createMut.mutate({ scope: payload.scope, severity: payload.severity })
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
                  <option value="false">Active</option>
                  <option value="true">Deleted</option>
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
// Mirrors backend SupplierHub.Constants.RoleType enum (Constants/Enum/Role.cs).
// Roles table is seeded with RoleID = (long)RoleType, so these IDs are stable.
const ROLE_OPTIONS = [
  { id: 1, name: 'Admin' },
  { id: 2, name: 'Buyer' },
  { id: 3, name: 'CategoryManager' },
  { id: 4, name: 'SupplierUser' },
  { id: 5, name: 'ReceivingUser' },
  { id: 6, name: 'AccountsPayable' },
  { id: 7, name: 'WarehouseManager' },
  { id: 8, name: 'ComplianceOfficer' },
]

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

  const submitAssign = hsAssign(d =>
    assignMut.mutate({ userID: Number(d.userID), roleID: Number(d.roleID) })
  )
  const submitDelete = hsDelete(d =>
    removeMut.mutate({ userID: Number(d.userID), roleID: Number(d.roleID) })
  )

  return (
    <div className="grid grid-cols-1 gap-5 md:grid-cols-2">
      {/* Assign */}
      <div className="sh-card">
        <h3 className="font-semibold text-gray-800 text-sm mb-4">Assign Role to User</h3>
        <form className="space-y-3" onSubmit={submitAssign} noValidate>
          <div>
            <label className="sh-label">User ID *</label>
            <input type="number" className="sh-input" placeholder="Enter user ID"
              {...regAssign('userID', { required: true, valueAsNumber: true })} />
          </div>
          <div>
            <label className="sh-label">Role ID *</label>
            <select className="sh-select" defaultValue=""
              {...regAssign('roleID', { required: true, valueAsNumber: true })}>
              <option value="" disabled>Select a role…</option>
              {ROLE_OPTIONS.map(r => (
                <option key={r.id} value={r.id}>{r.id} — {r.name}</option>
              ))}
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
        <form className="space-y-3" onSubmit={submitDelete} noValidate>
          <div>
            <label className="sh-label">User ID *</label>
            <input type="number" className="sh-input" placeholder="Enter user ID"
              {...regDelete('userID', { required: true, valueAsNumber: true })} />
          </div>
          <div>
            <label className="sh-label">Role ID to Remove *</label>
            <select className="sh-select" defaultValue=""
              {...regDelete('roleID', { required: true, valueAsNumber: true })}>
              <option value="" disabled>Select a role…</option>
              {ROLE_OPTIONS.map(r => (
                <option key={r.id} value={r.id}>{r.id} — {r.name}</option>
              ))}
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

/* ─── Permissions CRUD ──────────────────────────────────── */
function PermissionsTab() {
  const qc = useQueryClient()
  const [modalOpen, setModalOpen] = useState(false)
  const [editing, setEditing] = useState(null)
  const [confirmDelete, setConfirmDelete] = useState(null)

  const { data, isLoading } = useQuery({ queryKey: ['permissions'], queryFn: () => permissionsApi.getAll(true) })
  const rows = data?.data ?? data ?? []

  const { register, handleSubmit, reset, setValue } = useForm()

  const createMut = useMutation({
    mutationFn: permissionsApi.create,
    onSuccess: () => { qc.invalidateQueries(['permissions']); close(); toast.success('Permission created') },
    onError: e => toast.error(extractErr(e) ?? 'Failed to create permission'),
  })
  const updateMut = useMutation({
    mutationFn: ({ id, dto }) => permissionsApi.update(id, dto),
    onSuccess: () => { qc.invalidateQueries(['permissions']); close(); toast.success('Permission updated') },
    onError: e => toast.error(extractErr(e) ?? 'Failed to update permission'),
  })
  const deleteMut = useMutation({
    mutationFn: permissionsApi.delete,
    onSuccess: () => { qc.invalidateQueries(['permissions']); setConfirmDelete(null); toast.success('Permission deleted') },
    onError: e => toast.error(extractErr(e) ?? 'Failed to delete permission'),
  })

  const open = (p) => {
    setEditing(p ?? null)
    if (p) {
      setValue('code', p.code)
      setValue('permissionName', p.permissionName)
      setValue('status', p.status ?? 'Active')
    } else {
      reset({ code: '', permissionName: '', status: 'Active' })
    }
    setModalOpen(true)
  }
  const close = () => { setModalOpen(false); setEditing(null); reset() }

  const onSubmit = (form) => {
    const dto = { code: form.code.trim().toUpperCase(), permissionName: form.permissionName.trim(), status: form.status }
    if (editing) updateMut.mutate({ id: editing.permissionID, dto: { ...dto, isDeleted: false } })
    else createMut.mutate(dto)
  }

  const isPending = createMut.isPending || updateMut.isPending

  return (
    <div>
      <div className="flex justify-end mb-3">
        <button className="btn btn-primary btn-sm" onClick={() => open(null)}>+ New Permission</button>
      </div>

      <div className="sh-card p-0 overflow-hidden">
        {isLoading ? <div className="flex justify-center py-8"><Spinner /></div>
          : rows.length === 0 ? <EmptyState message="No permissions defined." />
          : (
          <table className="sh-table">
            <thead><tr><th>ID</th><th>Code</th><th>Name</th><th>Status</th><th>State</th><th>Actions</th></tr></thead>
            <tbody>
              {rows.map(p => (
                <tr key={p.permissionID}>
                  <td className="text-gray-400 text-xs">{p.permissionID}</td>
                  <td className="font-mono text-xs font-medium">{p.code}</td>
                  <td>{p.permissionName}</td>
                  <td><StatusPill status={p.status} /></td>
                  <td>{p.isDeleted
                    ? <span className="pill pill-red">Deleted</span>
                    : <span className="pill pill-green">Active</span>}</td>
                  <td>
                    <button className="btn btn-ghost btn-sm" onClick={() => open(p)}>Edit</button>
                    {!p.isDeleted && (
                      <button className="btn btn-ghost btn-sm" onClick={() => setConfirmDelete(p)}>Delete</button>
                    )}
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
        )}
      </div>

      {modalOpen && (
        <Modal title={editing ? `Edit Permission: ${editing.code}` : 'New Permission'} onClose={close}
          footer={
            <>
              <button className="btn btn-secondary btn-sm" onClick={close}>Cancel</button>
              <button className="btn btn-primary btn-sm" onClick={handleSubmit(onSubmit)} disabled={isPending}>
                {isPending ? 'Saving…' : editing ? 'Update' : 'Create'}
              </button>
            </>
          }>
          <form className="space-y-3" noValidate>
            <div>
              <label className="sh-label">Code * <span className="text-gray-400 font-normal">(unique, uppercase)</span></label>
              <input className="sh-input font-mono" placeholder="e.g. PR_APPROVE" {...register('code', { required: true })} />
            </div>
            <div>
              <label className="sh-label">Permission Name *</label>
              <input className="sh-input" {...register('permissionName', { required: true })} />
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
        <Modal title="Delete Permission" onClose={() => setConfirmDelete(null)}
          footer={
            <>
              <button className="btn btn-secondary btn-sm" onClick={() => setConfirmDelete(null)}>Cancel</button>
              <button className="btn btn-danger btn-sm" onClick={() => deleteMut.mutate(confirmDelete.permissionID)} disabled={deleteMut.isPending}>
                {deleteMut.isPending ? 'Deleting…' : 'Delete'}
              </button>
            </>
          }>
          <p className="text-sm text-gray-600">Soft-delete <strong>{confirmDelete.code}</strong>?</p>
        </Modal>
      )}
    </div>
  )
}

/* ─── Role-Permissions matrix ───────────────────────────── */
function RolePermissionsTab() {
  const qc = useQueryClient()
  const [selectedRoleId, setSelectedRoleId] = useState(null)

  const { data: rolesData } = useQuery({ queryKey: ['roles'], queryFn: () => rolesApi.getAll(false) })
  const { data: permsData } = useQuery({ queryKey: ['permissions'], queryFn: () => permissionsApi.getAll(false) })
  const { data: mappingsData, isLoading } = useQuery({
    queryKey: ['role-permissions', selectedRoleId],
    queryFn: () => rolePermissionsApi.getByRole(selectedRoleId),
    enabled: !!selectedRoleId,
  })

  const roles    = rolesData?.data ?? rolesData ?? []
  const perms    = permsData?.data ?? permsData ?? []
  const mappings = (mappingsData?.data ?? mappingsData ?? []).filter(m => !m.isDeleted)

  const grantedPermIds = new Set(mappings.map(m => m.permissionID))

  const grantMut = useMutation({
    mutationFn: rolePermissionsApi.create,
    onSuccess: () => { qc.invalidateQueries(['role-permissions', selectedRoleId]); toast.success('Permission granted') },
    onError: e => toast.error(extractErr(e) ?? 'Failed to grant permission'),
  })
  const revokeMut = useMutation({
    mutationFn: ({ roleId, permId }) => rolePermissionsApi.delete(roleId, permId),
    onSuccess: () => { qc.invalidateQueries(['role-permissions', selectedRoleId]); toast.success('Permission revoked') },
    onError: e => toast.error(extractErr(e) ?? 'Failed to revoke permission'),
  })

  const togglePerm = (permId, granted) => {
    if (granted) revokeMut.mutate({ roleId: selectedRoleId, permId })
    else grantMut.mutate({ roleID: selectedRoleId, permissionID: permId, status: 'Active' })
  }

  return (
    <div>
      <div className="sh-card mb-4">
        <label className="sh-label">Role</label>
        <select className="sh-select max-w-md" value={selectedRoleId ?? ''} onChange={e => setSelectedRoleId(Number(e.target.value))}>
          <option value="" disabled>Select a role…</option>
          {roles.filter(r => !r.isDeleted).map(r => (
            <option key={r.roleID} value={r.roleID}>{r.roleID} — {r.roleName}</option>
          ))}
        </select>
      </div>

      {!selectedRoleId
        ? <p className="text-sm text-gray-500">Pick a role to view and edit its permissions.</p>
        : isLoading ? <div className="flex justify-center py-8"><Spinner /></div>
        : perms.length === 0 ? <EmptyState message="No permissions defined yet — create some on the Permissions tab first." />
        : (
        <div className="sh-card p-0 overflow-hidden">
          <table className="sh-table">
            <thead>
              <tr><th>Code</th><th>Name</th><th>Granted</th><th>Status</th></tr>
            </thead>
            <tbody>
              {perms.map(p => {
                const granted = grantedPermIds.has(p.permissionID)
                const busy = (grantMut.isPending || revokeMut.isPending)
                return (
                  <tr key={p.permissionID}>
                    <td className="font-mono text-xs">{p.code}</td>
                    <td>{p.permissionName}</td>
                    <td>
                      <input
                        type="checkbox"
                        checked={granted}
                        disabled={busy}
                        onChange={() => togglePerm(p.permissionID, granted)}
                        className="w-4 h-4 cursor-pointer"
                      />
                    </td>
                    <td>{granted
                      ? <span className="pill pill-green">Granted</span>
                      : <span className="pill pill-gray">Not granted</span>}</td>
                  </tr>
                )
              })}
            </tbody>
          </table>
        </div>
      )}
    </div>
  )
}

function extractErr(err) {
  const data = err?.response?.data
  if (!data) return null
  const firstModelError = data.errors ? Object.values(data.errors).flat().find(Boolean) : null
  return firstModelError ?? data.message ?? (typeof data === 'string' ? data : null)
}
