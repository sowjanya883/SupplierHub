import { useState } from 'react'
import { useParams, useNavigate } from 'react-router-dom'
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query'
import { useForm } from 'react-hook-form'
import toast from 'react-hot-toast'
import { suppliersApi } from '../../api/suppliers.api'
import { supplierContactsApi, supplierRisksApi } from '../../api/supplier.extras.api'
import { StatusPill } from '../../components/ui/StatusPill'
import { Spinner, EmptyState } from '../../components/ui/index'
import Modal from '../../components/ui/Modal'

const TABS = ['Overview', 'Contacts', 'Risks']

function Section({ title, children, action }) {
  return (
    <div className="sh-card mb-4">
      <div className="flex items-center justify-between mb-3">
        <h3 className="font-semibold text-gray-800 text-sm">{title}</h3>
        {action}
      </div>
      {children}
    </div>
  )
}

export default function SupplierDetailPage() {
  const { id } = useParams()
  const navigate = useNavigate()
  const [tab, setTab] = useState('Overview')

  const { data, isLoading } = useQuery({
    queryKey: ['supplier', id],
    queryFn: () => suppliersApi.getById(id),
  })

  if (isLoading) return <div className="flex justify-center py-16"><Spinner /></div>

  const s = data?.data ?? data
  if (!s) return <p className="text-gray-500">Supplier not found.</p>

  const supplierId = Number(id)

  return (
    <div>
      <div className="page-header">
        <div>
          <button onClick={() => navigate(-1)} className="text-sm text-blue-600 hover:underline mb-1 block">
            ← Back
          </button>
          <h1 className="page-title">{s.legalName}</h1>
        </div>
        <StatusPill status={s.status} />
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

      {tab === 'Overview' && (
        <>
          <Section title="Supplier Details">
            <div className="grid grid-cols-2 gap-4 text-sm">
              <div><span className="text-gray-500">Supplier ID</span><p className="font-medium mt-0.5">#{s.supplierID}</p></div>
              <div><span className="text-gray-500">Tax ID</span><p className="font-medium mt-0.5">{s.taxID || '—'}</p></div>
              <div><span className="text-gray-500">DUNS / Reg No</span><p className="font-medium mt-0.5">{s.dunsOrRegNo || '—'}</p></div>
              <div><span className="text-gray-500">Status</span><p className="mt-0.5"><StatusPill status={s.status} /></p></div>
            </div>
          </Section>

          <Section title="Bank & Tax Info">
            <pre className="text-xs bg-gray-50 rounded-lg p-3 text-gray-700 overflow-auto">
              {s.bankInfoJSON ? JSON.stringify(safeParse(s.bankInfoJSON), null, 2) : 'No bank info'}
            </pre>
          </Section>
        </>
      )}

      {tab === 'Contacts' && <ContactsSection supplierId={supplierId} supplierName={s.legalName} />}
      {tab === 'Risks'    && <RisksSection    supplierId={supplierId} />}
    </div>
  )
}

function safeParse(s) { try { return JSON.parse(s) } catch { return s } }

/* ─── Contacts CRUD ─── */
function ContactsSection({ supplierId, supplierName }) {
  const qc = useQueryClient()
  const [modalOpen, setModalOpen] = useState(false)
  const [editing, setEditing] = useState(null)
  const [confirmDelete, setConfirmDelete] = useState(null)

  // Backend doesn't expose getBySupplierId — fetch all and filter client-side.
  const { data, isLoading } = useQuery({
    queryKey: ['supplier-contacts'],
    queryFn: supplierContactsApi.getAll,
  })
  const all = data?.data ?? data ?? []
  const rows = all.filter(c => c.supplierID === supplierId || c.supplierName === supplierName)

  const { register, handleSubmit, reset, setValue } = useForm()

  const createMut = useMutation({
    mutationFn: supplierContactsApi.create,
    onSuccess: () => { qc.invalidateQueries(['supplier-contacts']); close(); toast.success('Contact created') },
    onError: e => toast.error(extract(e) ?? 'Failed to create contact'),
  })
  const updateMut = useMutation({
    mutationFn: ({ id, dto }) => supplierContactsApi.update(id, dto),
    onSuccess: () => { qc.invalidateQueries(['supplier-contacts']); close(); toast.success('Contact updated') },
    onError: e => toast.error(extract(e) ?? 'Failed to update contact'),
  })
  const deleteMut = useMutation({
    mutationFn: supplierContactsApi.delete,
    onSuccess: () => { qc.invalidateQueries(['supplier-contacts']); setConfirmDelete(null); toast.success('Contact deleted') },
    onError: e => toast.error(extract(e) ?? 'Failed to delete contact'),
  })

  const open = (c) => {
    setEditing(c ?? null)
    if (c) {
      setValue('supplierName', c.supplierName ?? supplierName)
      setValue('email',  c.email ?? '')
      setValue('phone',  c.phone ?? '')
      setValue('role',   c.role ?? '')
      setValue('status', c.status ?? 'Active')
    } else {
      reset({ supplierName, email: '', phone: '', role: '', status: 'Active' })
    }
    setModalOpen(true)
  }
  const close = () => { setModalOpen(false); setEditing(null); reset() }

  const onSubmit = (form) => {
    const dto = {
      supplierID:   supplierId,
      supplierName: form.supplierName?.trim() || supplierName,
      email:        form.email?.trim().toLowerCase(),
      phone:        form.phone?.trim() || '',
      role:         form.role?.trim() || '',
      status:       form.status,
    }
    if (editing) {
      updateMut.mutate({
        id: editing.contactID,
        dto: { ...dto, contactID: editing.contactID },
      })
    } else {
      createMut.mutate(dto)
    }
  }

  return (
    <Section title={`Contacts (${rows.length})`}
      action={<button className="btn btn-primary btn-sm" onClick={() => open(null)}>+ New Contact</button>}>
      {isLoading ? <div className="py-8 flex justify-center"><Spinner /></div>
        : rows.length === 0 ? <EmptyState message="No contacts." />
        : (
        <table className="sh-table">
          <thead><tr><th>ID</th><th>Name/Email</th><th>Phone</th><th>Role</th><th>Status</th><th>Actions</th></tr></thead>
          <tbody>
            {rows.map(c => (
              <tr key={c.contactID}>
                <td className="text-gray-400 text-xs">{c.contactID}</td>
                <td>{c.email}</td>
                <td>{c.phone || '—'}</td>
                <td>{c.role || '—'}</td>
                <td><StatusPill status={c.status} /></td>
                <td>
                  <button className="btn btn-ghost btn-sm" onClick={() => open(c)}>Edit</button>
                  <button className="btn btn-ghost btn-sm" onClick={() => setConfirmDelete(c)}>Delete</button>
                </td>
              </tr>
            ))}
          </tbody>
        </table>
      )}

      {modalOpen && (
        <Modal title={editing ? `Edit Contact #${editing.contactID}` : 'New Contact'} onClose={close}
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
              <label className="sh-label">Display Name</label>
              <input className="sh-input" {...register('supplierName')} placeholder={supplierName} />
            </div>
            <div>
              <label className="sh-label">Email *</label>
              <input type="email" className="sh-input" {...register('email', { required: true })} />
            </div>
            <div>
              <label className="sh-label">Phone</label>
              <input className="sh-input" {...register('phone')} />
            </div>
            <div>
              <label className="sh-label">Role</label>
              <input className="sh-input" placeholder="e.g. Sales, AP" {...register('role')} />
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
        <Modal title="Delete Contact" onClose={() => setConfirmDelete(null)}
          footer={
            <>
              <button className="btn btn-secondary btn-sm" onClick={() => setConfirmDelete(null)}>Cancel</button>
              <button className="btn btn-danger btn-sm"
                onClick={() => deleteMut.mutate({ contactID: confirmDelete.contactID })}
                disabled={deleteMut.isPending}>
                {deleteMut.isPending ? 'Deleting…' : 'Delete'}
              </button>
            </>
          }>
          <p className="text-sm text-gray-600">Soft-delete contact <strong>{confirmDelete.email}</strong>?</p>
        </Modal>
      )}
    </Section>
  )
}

/* ─── Risks CRUD ─── */
function RisksSection({ supplierId }) {
  const qc = useQueryClient()
  const [modalOpen, setModalOpen] = useState(false)
  const [editing, setEditing] = useState(null)
  const [confirmDelete, setConfirmDelete] = useState(null)

  const { data, isLoading } = useQuery({
    queryKey: ['supplier-risks'],
    queryFn: supplierRisksApi.getAll,
  })
  const all = data?.data ?? data ?? []
  const rows = all.filter(r => r.supplierID === supplierId)

  const { register, handleSubmit, reset, setValue } = useForm()

  const createMut = useMutation({
    mutationFn: supplierRisksApi.create,
    onSuccess: () => { qc.invalidateQueries(['supplier-risks']); close(); toast.success('Risk recorded') },
    onError: e => toast.error(extract(e) ?? 'Failed to record risk'),
  })
  const updateMut = useMutation({
    mutationFn: ({ id, dto }) => supplierRisksApi.update(id, dto),
    onSuccess: () => { qc.invalidateQueries(['supplier-risks']); close(); toast.success('Risk updated') },
    onError: e => toast.error(extract(e) ?? 'Failed to update risk'),
  })
  const deleteMut = useMutation({
    mutationFn: supplierRisksApi.delete,
    onSuccess: () => { qc.invalidateQueries(['supplier-risks']); setConfirmDelete(null); toast.success('Risk deleted') },
    onError: e => toast.error(extract(e) ?? 'Failed to delete risk'),
  })

  const open = (r) => {
    setEditing(r ?? null)
    if (r) {
      setValue('riskType',     r.riskType ?? 'Financial')
      setValue('score',        r.score ?? '')
      setValue('assessedDate', r.assessedDate ? r.assessedDate.slice(0, 10) : '')
      setValue('notes',        r.notes ?? '')
      setValue('status',       r.status ?? 'Active')
    } else {
      reset({ riskType: 'Financial', score: '', assessedDate: new Date().toISOString().slice(0, 10), notes: '', status: 'Active' })
    }
    setModalOpen(true)
  }
  const close = () => { setModalOpen(false); setEditing(null); reset() }

  const onSubmit = (form) => {
    const dto = {
      supplierID:   supplierId,
      riskType:     form.riskType,
      score:        form.score === '' ? null : Number(form.score),
      assessedDate: form.assessedDate ? new Date(form.assessedDate).toISOString() : null,
      notes:        form.notes ?? '',
      status:       form.status,
    }
    if (editing) {
      updateMut.mutate({ id: editing.riskID, dto: { ...dto, riskID: editing.riskID } })
    } else {
      createMut.mutate(dto)
    }
  }

  return (
    <Section title={`Risk Assessments (${rows.length})`}
      action={<button className="btn btn-primary btn-sm" onClick={() => open(null)}>+ Add Risk</button>}>
      {isLoading ? <div className="py-8 flex justify-center"><Spinner /></div>
        : rows.length === 0 ? <EmptyState message="No risk assessments." />
        : (
        <table className="sh-table">
          <thead><tr><th>ID</th><th>Type</th><th>Score</th><th>Assessed</th><th>Notes</th><th>Status</th><th>Actions</th></tr></thead>
          <tbody>
            {rows.map(r => (
              <tr key={r.riskID}>
                <td className="text-gray-400 text-xs">{r.riskID}</td>
                <td><span className="pill pill-blue">{r.riskType}</span></td>
                <td className={Number(r.score) >= 7 ? 'text-red-600 font-medium' : ''}>{r.score ?? '—'}</td>
                <td className="text-xs text-gray-500">{r.assessedDate ? new Date(r.assessedDate).toLocaleDateString() : '—'}</td>
                <td className="max-w-xs truncate text-sm">{r.notes ?? '—'}</td>
                <td><StatusPill status={r.status} /></td>
                <td>
                  <button className="btn btn-ghost btn-sm" onClick={() => open(r)}>Edit</button>
                  <button className="btn btn-ghost btn-sm" onClick={() => setConfirmDelete(r)}>Delete</button>
                </td>
              </tr>
            ))}
          </tbody>
        </table>
      )}

      {modalOpen && (
        <Modal title={editing ? `Edit Risk #${editing.riskID}` : 'New Risk Assessment'} onClose={close}
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
              <label className="sh-label">Risk Type *</label>
              <select className="sh-select" {...register('riskType', { required: true })}>
                <option>Financial</option><option>Compliance</option><option>Delivery</option><option>Quality</option>
              </select>
            </div>
            <div>
              <label className="sh-label">Score (0–10)</label>
              <input type="number" step="0.1" min="0" max="10" className="sh-input" {...register('score')} />
            </div>
            <div>
              <label className="sh-label">Assessed Date</label>
              <input type="date" className="sh-input" {...register('assessedDate')} />
            </div>
            <div>
              <label className="sh-label">Notes</label>
              <textarea className="sh-input" rows={3} {...register('notes')} />
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
        <Modal title="Delete Risk" onClose={() => setConfirmDelete(null)}
          footer={
            <>
              <button className="btn btn-secondary btn-sm" onClick={() => setConfirmDelete(null)}>Cancel</button>
              <button className="btn btn-danger btn-sm"
                onClick={() => deleteMut.mutate({ riskID: confirmDelete.riskID })}
                disabled={deleteMut.isPending}>
                {deleteMut.isPending ? 'Deleting…' : 'Delete'}
              </button>
            </>
          }>
          <p className="text-sm text-gray-600">Soft-delete risk assessment <strong>#{confirmDelete.riskID}</strong>?</p>
        </Modal>
      )}
    </Section>
  )
}

function extract(err) {
  const data = err?.response?.data
  if (!data) return null
  const firstModelError = data.errors ? Object.values(data.errors).flat().find(Boolean) : null
  return firstModelError ?? data.message ?? (typeof data === 'string' ? data : null)
}
