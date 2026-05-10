import { useState } from 'react'
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query'
import { useForm } from 'react-hook-form'
import toast from 'react-hot-toast'
import { supplierKpiApi, scorecardsApi } from '../../api/supplier.extras.api'
import { PageHeader, Spinner, EmptyState } from '../../components/ui/index'
import Modal from '../../components/ui/Modal'

const TABS = ['KPIs','Scorecards']

export default function PerformancePage() {
  const [tab, setTab] = useState('KPIs')

  return (
    <div>
      <PageHeader title="Supplier Performance" subtitle="OTIF, NCR, responsiveness and rankings" />
      <div className="flex gap-1 mb-4 border-b border-gray-200">
        {TABS.map(t => (
          <button key={t} onClick={() => setTab(t)}
            className={`px-4 py-2 text-sm font-medium border-b-2 transition-colors ${
              tab === t ? 'border-blue-600 text-blue-700' : 'border-transparent text-gray-500 hover:text-gray-700'}`}>
            {t}
          </button>
        ))}
      </div>

      {tab === 'KPIs'       && <KpisTab />}
      {tab === 'Scorecards' && <ScorecardsTab />}
    </div>
  )
}

/* ─── KPIs CRUD ─── */
function KpisTab() {
  const qc = useQueryClient()
  const [open, setOpen] = useState(false)
  const [editing, setEditing] = useState(null)
  const [confirmDelete, setConfirmDelete] = useState(null)

  const { data, isLoading } = useQuery({ queryKey: ['kpis'], queryFn: supplierKpiApi.getAll })
  const rows = (data?.data ?? data ?? []).filter(k => !k.isDeleted)

  const { register, handleSubmit, reset, setValue } = useForm()

  const createMut = useMutation({
    mutationFn: supplierKpiApi.create,
    onSuccess: () => { qc.invalidateQueries(['kpis']); close(); toast.success('KPI added') },
    onError: e => toast.error(extract(e) ?? 'Failed'),
  })
  const updateMut = useMutation({
    mutationFn: ({ id, dto }) => supplierKpiApi.update(id, dto),
    onSuccess: () => { qc.invalidateQueries(['kpis']); close(); toast.success('KPI updated') },
    onError: e => toast.error(extract(e) ?? 'Failed'),
  })
  const deleteMut = useMutation({
    mutationFn: supplierKpiApi.delete,
    onSuccess: () => { qc.invalidateQueries(['kpis']); setConfirmDelete(null); toast.success('KPI deleted') },
    onError: e => toast.error(extract(e) ?? 'Failed'),
  })

  const openModal = (k) => {
    setEditing(k ?? null)
    if (k) {
      setValue('supplierID',         k.supplierID)
      setValue('period',             k.period ?? '')
      setValue('otifPct',            k.otifPct ?? '')
      setValue('ncrRatePPM',         k.ncrRatePPM ?? '')
      setValue('avgAckTimeHrs',      k.avgAckTimeHrs ?? '')
      setValue('priceAdherencePct',  k.priceAdherencePct ?? '')
      setValue('score',              k.score ?? '')
    } else {
      reset({ supplierID: '', period: '', otifPct: '', ncrRatePPM: '', avgAckTimeHrs: '', priceAdherencePct: '', score: '' })
    }
    setOpen(true)
  }
  const close = () => { setOpen(false); setEditing(null); reset() }

  const onSubmit = (form) => {
    const num = (v) => v === '' ? null : Number(v)
    const dto = {
      supplierID:         Number(form.supplierID),
      period:             form.period.trim(),
      otifPct:            num(form.otifPct),
      ncrRatePPM:         num(form.ncrRatePPM),
      avgAckTimeHrs:      num(form.avgAckTimeHrs),
      priceAdherencePct:  num(form.priceAdherencePct),
      score:              num(form.score),
      generatedDate:      new Date().toISOString(),
    }
    if (editing) updateMut.mutate({ id: editing.kpiid ?? editing.kpiID, dto: { ...dto, kpiid: editing.kpiid ?? editing.kpiID } })
    else createMut.mutate(dto)
  }

  const isPending = createMut.isPending || updateMut.isPending

  return (
    <div>
      <div className="flex justify-end mb-3">
        <button className="btn btn-primary btn-sm" onClick={() => openModal(null)}>+ Add KPI</button>
      </div>

      <div className="sh-card p-0 overflow-hidden">
        {isLoading ? <div className="flex justify-center py-8"><Spinner /></div>
          : rows.length === 0 ? <EmptyState message="No KPIs recorded." />
          : (
            <table className="sh-table">
              <thead><tr><th>ID</th><th>Supplier</th><th>Period</th><th>OTIF %</th><th>NCR PPM</th><th>Ack Hrs</th><th>Price %</th><th>Score</th><th>Actions</th></tr></thead>
              <tbody>
                {rows.map(k => (
                  <tr key={k.kpiid ?? k.kpiID}>
                    <td className="text-gray-400 text-xs">{k.kpiid ?? k.kpiID}</td>
                    <td>{k.supplierID}</td>
                    <td>{k.period}</td>
                    <td>{k.otifPct ?? '—'}</td>
                    <td>{k.ncrRatePPM ?? '—'}</td>
                    <td>{k.avgAckTimeHrs ?? '—'}</td>
                    <td>{k.priceAdherencePct ?? '—'}</td>
                    <td className="font-medium">{k.score ?? '—'}</td>
                    <td>
                      <button className="btn btn-ghost btn-sm" onClick={() => openModal(k)}>Edit</button>
                      <button className="btn btn-ghost btn-sm" onClick={() => setConfirmDelete(k)}>Delete</button>
                    </td>
                  </tr>
                ))}
              </tbody>
            </table>
          )}
      </div>

      {open && (
        <Modal title={editing ? `Edit KPI #${editing.kpiid ?? editing.kpiID}` : 'New KPI'} onClose={close}
          footer={
            <>
              <button className="btn btn-secondary btn-sm" onClick={close}>Cancel</button>
              <button className="btn btn-primary btn-sm" onClick={handleSubmit(onSubmit)} disabled={isPending}>
                {isPending ? 'Saving…' : editing ? 'Update' : 'Create'}
              </button>
            </>
          }>
          <form className="space-y-3" noValidate>
            <div className="grid grid-cols-2 gap-3">
              <div><label className="sh-label">Supplier ID *</label><input type="number" className="sh-input" {...register('supplierID', { required: true })} /></div>
              <div><label className="sh-label">Period *</label><input className="sh-input" placeholder="2026-Q1" {...register('period', { required: true })} /></div>
            </div>
            <div className="grid grid-cols-2 gap-3">
              <div><label className="sh-label">OTIF %</label><input type="number" step="0.1" className="sh-input" {...register('otifPct')} /></div>
              <div><label className="sh-label">NCR Rate PPM</label><input type="number" className="sh-input" {...register('ncrRatePPM')} /></div>
              <div><label className="sh-label">Avg Ack Time (hrs)</label><input type="number" step="0.1" className="sh-input" {...register('avgAckTimeHrs')} /></div>
              <div><label className="sh-label">Price Adherence %</label><input type="number" step="0.1" className="sh-input" {...register('priceAdherencePct')} /></div>
            </div>
            <div><label className="sh-label">Score</label><input type="number" step="0.1" className="sh-input" {...register('score')} /></div>
          </form>
        </Modal>
      )}

      {confirmDelete && (
        <Modal title="Delete KPI" onClose={() => setConfirmDelete(null)}
          footer={<><button className="btn btn-secondary btn-sm" onClick={() => setConfirmDelete(null)}>Cancel</button>
            <button className="btn btn-danger btn-sm" onClick={() => deleteMut.mutate(confirmDelete.kpiid ?? confirmDelete.kpiID)} disabled={deleteMut.isPending}>{deleteMut.isPending ? 'Deleting…' : 'Delete'}</button></>}>
          <p className="text-sm text-gray-600">Soft-delete KPI <strong>#{confirmDelete.kpiid ?? confirmDelete.kpiID}</strong>?</p>
        </Modal>
      )}
    </div>
  )
}

/* ─── Scorecards CRUD ─── */
function ScorecardsTab() {
  const qc = useQueryClient()
  const [open, setOpen] = useState(false)
  const [editing, setEditing] = useState(null)
  const [confirmDelete, setConfirmDelete] = useState(null)

  const { data, isLoading } = useQuery({ queryKey: ['scorecards'], queryFn: scorecardsApi.getAll })
  const rows = (data?.data ?? data ?? []).filter(s => !s.isDeleted)

  const { register, handleSubmit, reset, setValue } = useForm()

  const createMut = useMutation({
    mutationFn: scorecardsApi.create,
    onSuccess: () => { qc.invalidateQueries(['scorecards']); close(); toast.success('Scorecard saved') },
    onError: e => toast.error(extract(e) ?? 'Failed'),
  })
  const updateMut = useMutation({
    mutationFn: ({ id, dto }) => scorecardsApi.update(id, dto),
    onSuccess: () => { qc.invalidateQueries(['scorecards']); close(); toast.success('Scorecard updated') },
    onError: e => toast.error(extract(e) ?? 'Failed'),
  })
  const deleteMut = useMutation({
    mutationFn: scorecardsApi.delete,
    onSuccess: () => { qc.invalidateQueries(['scorecards']); setConfirmDelete(null); toast.success('Scorecard deleted') },
    onError: e => toast.error(extract(e) ?? 'Failed'),
  })

  const openModal = (s) => {
    setEditing(s ?? null)
    if (s) {
      setValue('supplierID',  s.supplierID)
      setValue('period',      s.period ?? '')
      setValue('rank',        s.rank ?? '')
      setValue('metricsJSON', s.metricsJSON ?? '{}')
      setValue('notes',       s.notes ?? '')
    } else {
      reset({ supplierID: '', period: '', rank: '', metricsJSON: '{}', notes: '' })
    }
    setOpen(true)
  }
  const close = () => { setOpen(false); setEditing(null); reset() }

  const onSubmit = (form) => {
    const dto = {
      supplierID:  Number(form.supplierID),
      period:      form.period.trim(),
      rank:        form.rank === '' ? null : Number(form.rank),
      metricsJSON: form.metricsJSON?.trim() || '{}',
      notes:       form.notes ?? '',
    }
    if (editing) updateMut.mutate({ id: editing.scorecardID, dto: { ...dto, scorecardID: editing.scorecardID } })
    else createMut.mutate(dto)
  }

  const isPending = createMut.isPending || updateMut.isPending

  return (
    <div>
      <div className="flex justify-end mb-3">
        <button className="btn btn-primary btn-sm" onClick={() => openModal(null)}>+ Add Scorecard</button>
      </div>

      <div className="sh-card p-0 overflow-hidden">
        {isLoading ? <div className="flex justify-center py-8"><Spinner /></div>
          : rows.length === 0 ? <EmptyState message="No scorecards yet." />
          : (
            <table className="sh-table">
              <thead><tr><th>ID</th><th>Supplier</th><th>Period</th><th>Rank</th><th>Notes</th><th>Actions</th></tr></thead>
              <tbody>
                {rows.map(s => (
                  <tr key={s.scorecardID}>
                    <td className="text-gray-400 text-xs">{s.scorecardID}</td>
                    <td>{s.supplierID}</td>
                    <td>{s.period}</td>
                    <td className="font-medium">{s.rank ?? '—'}</td>
                    <td className="max-w-xs truncate text-sm">{s.notes ?? '—'}</td>
                    <td>
                      <button className="btn btn-ghost btn-sm" onClick={() => openModal(s)}>Edit</button>
                      <button className="btn btn-ghost btn-sm" onClick={() => setConfirmDelete(s)}>Delete</button>
                    </td>
                  </tr>
                ))}
              </tbody>
            </table>
          )}
      </div>

      {open && (
        <Modal title={editing ? `Edit Scorecard #${editing.scorecardID}` : 'New Scorecard'} onClose={close}
          footer={
            <>
              <button className="btn btn-secondary btn-sm" onClick={close}>Cancel</button>
              <button className="btn btn-primary btn-sm" onClick={handleSubmit(onSubmit)} disabled={isPending}>
                {isPending ? 'Saving…' : editing ? 'Update' : 'Create'}
              </button>
            </>
          }>
          <form className="space-y-3" noValidate>
            <div className="grid grid-cols-2 gap-3">
              <div><label className="sh-label">Supplier ID *</label><input type="number" className="sh-input" {...register('supplierID', { required: true })} /></div>
              <div><label className="sh-label">Period *</label><input className="sh-input" placeholder="2026-Q1" {...register('period', { required: true })} /></div>
            </div>
            <div><label className="sh-label">Rank</label><input type="number" className="sh-input" {...register('rank')} /></div>
            <div><label className="sh-label">Metrics (JSON)</label><textarea rows={3} className="sh-input font-mono text-xs" {...register('metricsJSON')} /></div>
            <div><label className="sh-label">Notes</label><textarea rows={2} className="sh-input" {...register('notes')} /></div>
          </form>
        </Modal>
      )}

      {confirmDelete && (
        <Modal title="Delete Scorecard" onClose={() => setConfirmDelete(null)}
          footer={<><button className="btn btn-secondary btn-sm" onClick={() => setConfirmDelete(null)}>Cancel</button>
            <button className="btn btn-danger btn-sm" onClick={() => deleteMut.mutate(confirmDelete.scorecardID)} disabled={deleteMut.isPending}>{deleteMut.isPending ? 'Deleting…' : 'Delete'}</button></>}>
          <p className="text-sm text-gray-600">Soft-delete scorecard <strong>#{confirmDelete.scorecardID}</strong>?</p>
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
