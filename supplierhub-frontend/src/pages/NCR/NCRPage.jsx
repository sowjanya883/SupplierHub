import { useState } from 'react'
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query'
import { useForm } from 'react-hook-form'
import toast from 'react-hot-toast'
import { ncrApi, grnItemApi } from '../../api/operations.api'
import { PageHeader, Spinner, EmptyState, ConfirmDialog } from '../../components/ui/index'
import Modal from '../../components/ui/Modal'
import useAuthStore from '../../store/auth.store'

// ── Enums ──────────────────────────────────────────────
const NCR_SEVERITIES = ['Minor', 'Major', 'Critical']
const NCR_STATUSES   = ['Open', 'Closed']

// ── Severity styling ───────────────────────────────────
const severityStyle = (s) => {
  const m = {
    Minor:    { pill: 'pill pill-amber',  badge: 'bg-amber-100 text-amber-800 border-amber-200' },
    Major:    { pill: 'pill pill-orange', badge: 'bg-orange-100 text-orange-800 border-orange-200' },
    Critical: { pill: 'pill pill-red',    badge: 'bg-red-100 text-red-800 border-red-200' },
  }
  return m[s] ?? { pill: 'pill pill-gray', badge: 'bg-gray-100 text-gray-600 border-gray-200' }
}

const statusStyle = (s) => s === 'Open' ? 'pill pill-blue' : 'pill pill-green'

// ─────────────────────────────────────────────────────────
export default function NCRPage() {
  const qc       = useQueryClient()
  const { user } = useAuthStore()

  const isAdmin    = user?.roles?.includes('Admin')
  const isSupplier = user?.roles?.includes('SupplierUser')
  const canWrite   = isAdmin || isSupplier

  const [modal, setModal]           = useState(null)   // 'form' | 'delete'
  const [selected, setSelected]     = useState(null)
  const [severityFilter, setSeverityFilter] = useState('all')
  const [statusFilter, setStatusFilter]     = useState('all')
  const [search, setSearch]         = useState('')

  // ── Fetch all NCRs ─────────────────────────────────────
  // NOTE: NCR controller wraps response as { message, data: [...] }
  const { data: rawData, isLoading } = useQuery({
    queryKey: ['ncrs'],
    queryFn:  ncrApi.getAll,
  })
  // Handle wrapped response: { message, data: [...] }
  const allNcrs = rawData?.data ?? rawData ?? []

  // ── Apply filters ──────────────────────────────────────
  const rows = allNcrs
    .filter(n =>
      search === '' ||
      String(n.ncrID     ?? '').includes(search) ||
      String(n.grnItemID ?? '').includes(search) ||
      (n.defectType ?? '').toLowerCase().includes(search.toLowerCase())
    )
    .filter(n => severityFilter === 'all' || n.severity === severityFilter)
    .filter(n => statusFilter   === 'all' || n.status   === statusFilter)

  // ── Form ───────────────────────────────────────────────
  const { register, handleSubmit, reset, formState: { errors } } = useForm()

  // ── Mutations ──────────────────────────────────────────
  const createMut = useMutation({
    mutationFn: ncrApi.create,
    onSuccess: (res) => {
      qc.invalidateQueries(['ncrs'])
      setModal(null)
      const id = res?.data?.ncrID ?? res?.ncrID
      toast.success(`NCR-${id} raised successfully`)
    },
    onError: e => toast.error(e.response?.data?.message ?? 'Failed to raise NCR'),
  })

  const updateMut = useMutation({
    mutationFn: ({ id, dto }) => ncrApi.update(id, dto),
    onSuccess: () => {
      qc.invalidateQueries(['ncrs'])
      setModal(null)
      toast.success('NCR updated successfully')
    },
    onError: e => toast.error(e.response?.data?.message ?? 'Failed to update NCR'),
  })

  const deleteMut = useMutation({
    mutationFn: ncrApi.delete,
    onSuccess: () => {
      qc.invalidateQueries(['ncrs'])
      setModal(null)
      toast.success('NCR deleted')
    },
    onError: e => toast.error(e.response?.data?.message ?? 'Failed to delete'),
  })

  // ── Quick close ────────────────────────────────────────
  const closeMut = useMutation({
    mutationFn: ({ id, ncr }) => ncrApi.update(id, {
      DefectType: ncr.defectType || null,
      Severity:   ncr.severity  || null,
      Status:     'Closed',
    }),
    onSuccess: () => {
      qc.invalidateQueries(['ncrs'])
      toast.success('NCR closed')
    },
    onError: e => toast.error(e.response?.data?.message ?? 'Failed to close'),
  })

  // ── Handlers ───────────────────────────────────────────
  const openCreate = () => {
    reset({ GrnItemID: '', DefectType: '', Severity: 'Minor', Status: 'Open' })
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
        id:  selected.ncrID,
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

  // ── Stats ──────────────────────────────────────────────
  const stats = {
    total:    allNcrs.length,
    open:     allNcrs.filter(n => n.status === 'Open').length,
    closed:   allNcrs.filter(n => n.status === 'Closed').length,
    minor:    allNcrs.filter(n => n.severity === 'Minor'    && n.status === 'Open').length,
    major:    allNcrs.filter(n => n.severity === 'Major'    && n.status === 'Open').length,
    critical: allNcrs.filter(n => n.severity === 'Critical' && n.status === 'Open').length,
  }

  // ── Render ─────────────────────────────────────────────
  return (
    <div>
      <PageHeader
        title="NCR Quality"
        subtitle="Non-conformance reports — track quality defects across all received goods"
        action={
          canWrite && (
            <button className="btn btn-primary" onClick={openCreate}>
              + Raise NCR
            </button>
          )
        }
      />

      {/* ── Stats Dashboard ───────────────────────────────── */}
      <div className="grid grid-cols-2 gap-3 mb-5 md:grid-cols-6">
        <StatCard label="Total NCRs"  value={stats.total}    color="text-gray-800"   bg="bg-white" />
        <StatCard label="Open"        value={stats.open}     color="text-blue-700"   bg="bg-blue-50" />
        <StatCard label="Closed"      value={stats.closed}   color="text-green-700"  bg="bg-green-50" />
        <StatCard label="Minor Open"  value={stats.minor}    color="text-amber-700"  bg="bg-amber-50" />
        <StatCard label="Major Open"  value={stats.major}    color="text-orange-700" bg="bg-orange-50" />
        <StatCard label="Critical 🔴" value={stats.critical} color="text-red-700"    bg="bg-red-50" />
      </div>

      {/* Critical open NCRs banner */}
      {stats.critical > 0 && (
        <div className="mb-4 bg-red-50 border border-red-200 rounded-lg px-4 py-3 flex items-center gap-3">
          <span className="text-red-600 text-xl">🚨</span>
          <div>
            <p className="text-red-800 font-semibold text-sm">
              {stats.critical} Critical NCR{stats.critical > 1 ? 's' : ''} open
            </p>
            <p className="text-red-600 text-xs">
              Requires immediate attention — critical defects block invoice payment
            </p>
          </div>
          <button
            className="ml-auto btn btn-sm"
            style={{ background: '#ef4444', color: 'white' }}
            onClick={() => { setSeverityFilter('Critical'); setStatusFilter('Open') }}
          >
            View Critical
          </button>
        </div>
      )}

      {/* ── Filters ───────────────────────────────────────── */}
      <div className="flex gap-3 mb-4 flex-wrap items-center">
        <input
          className="sh-input max-w-xs"
          placeholder="Search by NCR ID, item ID or defect…"
          value={search}
          onChange={e => setSearch(e.target.value)}
        />

        {/* Severity filter */}
        <div className="flex gap-1">
          <button
            onClick={() => setSeverityFilter('all')}
            className={`btn btn-sm ${severityFilter === 'all' ? 'btn-primary' : 'btn-secondary'}`}
          >
            All Severity
          </button>
          {NCR_SEVERITIES.map(s => (
            <button
              key={s}
              onClick={() => setSeverityFilter(severityFilter === s ? 'all' : s)}
              className={`btn btn-sm border ${
                severityFilter === s
                  ? s === 'Critical' ? 'bg-red-500 text-white border-red-500'
                    : s === 'Major'  ? 'bg-orange-500 text-white border-orange-500'
                    : 'bg-amber-500 text-white border-amber-500'
                  : 'bg-white border-gray-200 text-gray-600 hover:bg-gray-50'
              }`}
            >
              {s}
            </button>
          ))}
        </div>

        {/* Status filter */}
        <div className="flex gap-1">
          <button
            onClick={() => setStatusFilter('all')}
            className={`btn btn-sm ${statusFilter === 'all' ? 'btn-primary' : 'btn-secondary'}`}
          >
            All Status
          </button>
          {NCR_STATUSES.map(s => (
            <button
              key={s}
              onClick={() => setStatusFilter(statusFilter === s ? 'all' : s)}
              className={`btn btn-sm ${
                statusFilter === s
                  ? s === 'Open' ? 'btn-primary' : 'bg-green-500 text-white border-green-500'
                  : 'btn-secondary'
              }`}
            >
              {s}
            </button>
          ))}
        </div>

        {/* Clear filters */}
        {(severityFilter !== 'all' || statusFilter !== 'all' || search) && (
          <button
            className="btn btn-ghost btn-sm text-gray-400"
            onClick={() => {
              setSeverityFilter('all')
              setStatusFilter('all')
              setSearch('')
            }}
          >
            ✕ Clear filters
          </button>
        )}

        <span className="ml-auto text-xs text-gray-400">
          {rows.length} of {allNcrs.length} NCR{allNcrs.length !== 1 ? 's' : ''}
        </span>
      </div>

      {/* ── Table ─────────────────────────────────────────── */}
      <div className="sh-card p-0 overflow-hidden">
        {isLoading ? (
          <div className="flex justify-center py-12"><Spinner /></div>
        ) : rows.length === 0 ? (
          <EmptyState
            message={
              allNcrs.length === 0
                ? 'No NCRs raised yet.'
                : 'No NCRs match your filters.'
            }
          />
        ) : (
          <table className="sh-table">
            <thead>
              <tr>
                <th>Sr. No.</th>
                <th>NCR ID</th>
                <th>GRN Item ID</th>
                <th>Defect Type</th>
                <th>Severity</th>
                <th>Status</th>
                <th>Raised On</th>
                <th>Last Updated</th>
                {canWrite && <th>Actions</th>}
              </tr>
            </thead>
            <tbody>
              {rows.map((ncr, index) => (
                <tr
                  key={ncr.ncrID}
                  className={
                    ncr.severity === 'Critical' && ncr.status === 'Open'
                      ? 'bg-red-50'
                      : ncr.severity === 'Major' && ncr.status === 'Open'
                      ? 'bg-orange-50'
                      : ''
                  }
                >
                  <td className="text-gray-400 text-xs">{index + 1}</td>
                  <td className="font-medium">NCR-{ncr.ncrID}</td>
                  <td className="text-gray-500">#{ncr.grnItemID}</td>
                  <td className="font-medium">
                    {ncr.defectType ?? (
                      <span className="text-gray-400 italic text-xs">Not specified</span>
                    )}
                  </td>
                  <td>
                    {ncr.severity ? (
                      <span className={severityStyle(ncr.severity).pill}>
                        {ncr.severity}
                      </span>
                    ) : (
                      <span className="text-gray-400 text-xs">—</span>
                    )}
                  </td>
                  <td>
                    <span className={statusStyle(ncr.status)}>{ncr.status}</span>
                  </td>
                  <td className="text-xs text-gray-500">
                    {ncr.createdOn
                      ? new Date(ncr.createdOn).toLocaleDateString()
                      : '—'}
                  </td>
                  <td className="text-xs text-gray-500">
                    {ncr.updatedOn
                      ? new Date(ncr.updatedOn).toLocaleDateString()
                      : '—'}
                  </td>
                  {canWrite && (
                    <td>
                      <div className="flex gap-1">
                        {/* Quick close — only for open NCRs */}
                        {ncr.status === 'Open' && (
                          <button
                            className="btn btn-sm bg-green-500 text-white hover:bg-green-600 border-0"
                            onClick={() => closeMut.mutate({ id: ncr.ncrID, ncr })}
                            disabled={closeMut.isPending}
                            title="Close this NCR"
                          >
                            ✓ Close
                          </button>
                        )}
                        <button
                          className="btn btn-ghost btn-sm"
                          onClick={() => openEdit(ncr)}
                        >
                          Edit
                        </button>
                        {isAdmin && (
                          <button
                            className="btn btn-danger btn-sm"
                            onClick={() => { setSelected(ncr); setModal('delete') }}
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

      {/* ── Create / Edit Modal ──────────────────────────── */}
      {modal === 'form' && (
        <Modal
          title={selected ? `Edit NCR-${selected.ncrID}` : 'Raise New NCR'}
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
                {isPending
                  ? 'Saving…'
                  : selected ? 'Update NCR' : 'Raise NCR'}
              </button>
            </>
          }
        >
          <form className="space-y-4" noValidate>

            {/* GRN Item ID — only on create */}
            {!selected && (
              <div>
                <label className="sh-label">GRN Item ID *</label>
                <input
                  type="number"
                  className="sh-input"
                  placeholder="Enter GRN Item ID"
                  {...register('GrnItemID', { required: 'GRN Item ID is required' })}
                />
                {errors.GrnItemID && (
                  <p className="text-red-500 text-xs mt-1">{errors.GrnItemID.message}</p>
                )}
                <p className="text-xs text-gray-400 mt-1">
                  Find the GRN Item ID from the GRN detail page
                </p>
              </div>
            )}

            {/* Defect Type */}
            <div>
              <label className="sh-label">Defect Type</label>
              <input
                className="sh-input"
                placeholder="e.g. Dimensional, Surface Finish, Wrong Item, Damaged"
                {...register('DefectType')}
              />
            </div>

            {/* Severity — radio with visual pills */}
            <div>
              <label className="sh-label">Severity *</label>
              <div className="flex gap-3 mt-1">
                {NCR_SEVERITIES.map(s => (
                  <label
                    key={s}
                    className="flex items-center gap-2 cursor-pointer p-2 rounded-lg border hover:bg-gray-50"
                  >
                    <input
                      type="radio"
                      value={s}
                      {...register('Severity', { required: 'Severity is required' })}
                    />
                    <span className={severityStyle(s).pill}>{s}</span>
                  </label>
                ))}
              </div>
              {errors.Severity && (
                <p className="text-red-500 text-xs mt-1">{errors.Severity.message}</p>
              )}
              <div className="mt-2 text-xs text-gray-500 space-y-0.5">
                <p><strong>Minor</strong> — cosmetic or minor spec deviation, usable</p>
                <p><strong>Major</strong> — functional impact, rework may be needed</p>
                <p><strong>Critical</strong> — safety/regulatory risk, reject or return</p>
              </div>
            </div>

            {/* Status */}
            <div>
              <label className="sh-label">Status *</label>
              <select
                className="sh-select"
                {...register('Status', { required: true })}
              >
                {NCR_STATUSES.map(s => <option key={s}>{s}</option>)}
              </select>
            </div>

            {/* Disposition note */}
            <div className="bg-amber-50 border border-amber-200 rounded-lg px-3 py-2">
              <p className="text-xs text-amber-700">
                <strong>Disposition</strong> (UseAsIs / Rework / Reject / Return) is not yet
                available — the field exists in the system design but has not been added to the
                backend model. Ask your backend developer to add it to{' '}
                <code className="bg-amber-100 px-1 rounded">Ncr.cs</code> and{' '}
                <code className="bg-amber-100 px-1 rounded">NcrUpdateDto.cs</code>.
              </p>
            </div>
          </form>
        </Modal>
      )}

      {/* ── Delete Confirm ───────────────────────────────── */}
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

// ── Stat card component ────────────────────────────────
function StatCard({ label, value, color, bg }) {
  return (
    <div className={`sh-card py-3 text-center ${bg}`}>
      <p className={`text-2xl font-bold ${color}`}>{value}</p>
      <p className="text-xs text-gray-500 mt-0.5">{label}</p>
    </div>
  )
}