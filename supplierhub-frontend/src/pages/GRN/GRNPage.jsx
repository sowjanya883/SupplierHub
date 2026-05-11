import { useState } from 'react'
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query'
import { useNavigate } from 'react-router-dom'
import { useForm } from 'react-hook-form'
import toast from 'react-hot-toast'
import { grnApi } from '../../api/operations.api'
import { purchaseOrdersApi } from '../../api/procurement.api'
import { StatusPill } from '../../components/ui/StatusPill'
import { PageHeader, Spinner, EmptyState, ConfirmDialog } from '../../components/ui/index'
import Modal from '../../components/ui/Modal'
import useAuthStore from '../../store/auth.store'
 
// GrnStatus enum: Open=1, Posted=2
const GRN_STATUSES = ['Pending', 'Open', 'Posted']
 
const statusColor = (s) => {
  const m = {
    Pending: 'pill-amber',
    Open:    'pill-blue',
    Posted:  'pill-green',
  }
  return `pill ${m[s] ?? 'pill-gray'}`
}
 
export default function GRNPage() {
  const navigate   = useNavigate()
  const qc         = useQueryClient()
  const { user }   = useAuthStore()
 
  const isAdmin    = user?.roles?.includes('Admin')
  const isSupplier = user?.roles?.includes('SupplierUser')
  const canWrite   = isAdmin || isSupplier
 
  const [modal, setModal]       = useState(null)   // 'form' | 'delete'
  const [selected, setSelected] = useState(null)
  const [search, setSearch]     = useState('')
 
  // ── Fetch GRNs ─────────────────────────────────────────
  const { data, isLoading } = useQuery({
    queryKey: ['grns'],
    queryFn:  grnApi.getAll,
  })
  const rows = (data?.data ?? data ?? []).filter(g =>
    String(g.grnID   ?? '').includes(search) ||
    String(g.poID    ?? '').includes(search) ||
    (g.status ?? '').toLowerCase().includes(search.toLowerCase())
  )
 
  // ── Fetch POs for dropdown ─────────────────────────────
  const { data: poData } = useQuery({
    queryKey: ['pos'],
    queryFn:  purchaseOrdersApi.getAll,
  })
  const pos = poData?.data ?? poData ?? []
 
  // ── Form ───────────────────────────────────────────────
  const { register, handleSubmit, reset, formState: { errors } } = useForm()
 
  // ── Mutations ──────────────────────────────────────────
  const createMut = useMutation({
    mutationFn: grnApi.create,
    onSuccess: (res) => {
      qc.invalidateQueries(['grns'])
      setModal(null)
      const id = res?.data?.grnID ?? res?.grnID
      toast.success(`GRN #${id} created successfully`)
    },
    onError: e => toast.error(e.response?.data?.message ?? 'Failed to create GRN'),
  })
 
  const updateMut = useMutation({
    mutationFn: ({ id, dto }) => grnApi.update(id, dto),
    onSuccess: () => {
      qc.invalidateQueries(['grns'])
      setModal(null)
      toast.success('GRN updated successfully')
    },
    onError: e => toast.error(e.response?.data?.message ?? 'Failed to update GRN'),
  })
 
  const deleteMut = useMutation({
    mutationFn: (id) => grnApi.delete(id),
    onSuccess: () => {
      qc.invalidateQueries(['grns'])
      setModal(null)
      toast.success('GRN deleted successfully')
    },
    onError: e => toast.error(e.response?.data?.message ?? 'Failed to delete GRN'),
  })
 
  // ── Handlers ───────────────────────────────────────────
  const openCreate = () => {
    reset({
      PoID:         '',
      AsnID:        '',
      ReceivedDate: new Date().toISOString().split('T')[0],
      ReceivedBy:   user?.userId ?? '',
      Status:       'Pending',
    })
    setSelected(null)
    setModal('form')
  }
 
  const openEdit = (row) => {
    setSelected(row)
    reset({
      PoID:         row.poID        ?? '',
      AsnID:        row.asnID       ?? '',
      ReceivedDate: row.receivedDate?.split('T')[0] ?? '',
      ReceivedBy:   row.receivedBy  ?? '',
      Status:       row.status      ?? 'Pending',
    })
    setModal('form')
  }
 
  const onSubmit = (d) => {
    const payload = {
      // GrnUpdateDto requires PoID even on update
      PoID:         Number(d.PoID),
      AsnID:        d.AsnID        ? Number(d.AsnID)       : null,
      ReceivedDate: d.ReceivedDate || null,
      ReceivedBy:   d.ReceivedBy   ? Number(d.ReceivedBy)  : null,
      Status:       d.Status,
    }
    if (selected) {
      updateMut.mutate({ id: selected.grnID, dto: payload })
    } else {
      createMut.mutate(payload)
    }
  }
 
  const isPending = createMut.isPending || updateMut.isPending
 
  return (
    <div>
      <PageHeader
        title="GRN & Receiving"
        subtitle="Goods receipt notes — record received quantities, inspect quality and raise NCRs"
        action={
          canWrite && (
            <button className="btn btn-primary" onClick={openCreate}>
              + New GRN
            </button>
          )
        }
      />
 
      {/* Search */}
      <div className="mb-4">
        <input
          className="sh-input max-w-xs"
          placeholder="Search by GRN ID, PO ID or status…"
          value={search}
          onChange={e => setSearch(e.target.value)}
        />
      </div>
 
      {/* Table */}
      <div className="sh-card p-0 overflow-hidden">
        {isLoading ? (
          <div className="flex justify-center py-12"><Spinner /></div>
        ) : rows.length === 0 ? (
          <EmptyState message="No GRNs found." />
        ) : (
          <table className="sh-table">
            <thead>
              <tr>
                <th>Sr. No.</th>
                <th>GRN ID</th>
                <th>PO ID</th>
                <th>ASN ID</th>
                <th>Received Date</th>
                <th>Received By</th>
                <th>Status</th>
                <th>Actions</th>
              </tr>
            </thead>
            <tbody>
              {rows.map((g, index) => (
                <tr key={g.grnID}>
                  <td className="text-gray-400 text-xs">{index + 1}</td>
                  <td
                    className="font-medium text-blue-700 cursor-pointer hover:underline"
                    onClick={() => navigate(`/grn/${g.grnID}`)}
                  >
                    GRN-{g.grnID}
                  </td>
                  <td>PO-{g.poID}</td>
                  <td className="text-gray-500">{g.asnID ? `ASN-${g.asnID}` : '—'}</td>
                  <td className="text-xs text-gray-500">
                    {g.receivedDate ? new Date(g.receivedDate).toLocaleDateString() : '—'}
                  </td>
                  <td className="text-gray-500">{g.receivedBy ?? '—'}</td>
                  <td><span className={statusColor(g.status)}>{g.status}</span></td>
                  <td>
                    <div className="flex gap-1">
                      <button
                        className="btn btn-secondary btn-sm"
                        onClick={() => navigate(`/grn/${g.grnID}`)}
                      >
                        Open
                      </button>
                      {canWrite && (
                        <button
                          className="btn btn-ghost btn-sm"
                          onClick={() => openEdit(g)}
                        >
                          Edit
                        </button>
                      )}
                      {isAdmin && (
                        <button
                          className="btn btn-danger btn-sm"
                          onClick={() => { setSelected(g); setModal('delete') }}
                        >
                          Delete
                        </button>
                      )}
                    </div>
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
        )}
      </div>
 
      {/* Create / Edit Modal */}
      {modal === 'form' && (
        <Modal
          title={selected ? `Edit GRN-${selected.grnID}` : 'New Goods Receipt Note'}
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
                {isPending ? 'Saving…' : selected ? 'Update' : 'Create GRN'}
              </button>
            </>
          }
        >
          <form className="space-y-3" noValidate>
            {/* PO dropdown */}
            <div>
              <label className="sh-label">Purchase Order *</label>
              <select
                className="sh-select"
                {...register('PoID', { required: 'PO is required' })}
              >
                <option value="">— Select PO —</option>
                {pos.map(p => (
                  <option key={p.poID} value={p.poID}>
                    PO-{p.poID} ({p.status})
                  </option>
                ))}
              </select>
              {errors.PoID && (
                <p className="text-red-500 text-xs mt-1">{errors.PoID.message}</p>
              )}
            </div>
 
            {/* ASN ID */}
            <div>
              <label className="sh-label">
                ASN ID
                <span className="text-gray-400 font-normal ml-1">(optional)</span>
              </label>
              <input
                type="number"
                className="sh-input"
                placeholder="Link to ASN if available"
                {...register('AsnID')}
              />
            </div>
 
            {/* Received Date */}
            <div>
              <label className="sh-label">Received Date</label>
              <input type="date" className="sh-input" {...register('ReceivedDate')} />
            </div>
 
            {/* Received By */}
            <div>
              <label className="sh-label">Received By (User ID)</label>
              <input
                type="number"
                className="sh-input"
                placeholder="User ID of receiving staff"
                {...register('ReceivedBy')}
              />
            </div>
 
            {/* Status */}
            <div>
              <label className="sh-label">Status *</label>
              <select
                className="sh-select"
                {...register('Status', { required: true })}
              >
                {GRN_STATUSES.map(s => <option key={s}>{s}</option>)}
              </select>
            </div>
          </form>
        </Modal>
      )}
 
      {/* Delete Confirm */}
      {modal === 'delete' && (
        <ConfirmDialog
          message={`Delete GRN-${selected?.grnID}? All items, inspections and NCRs will also be removed.`}
          onConfirm={() => deleteMut.mutate(selected.grnID)}
          onCancel={() => setModal(null)}
          loading={deleteMut.isPending}
        />
      )}
    </div>
  )
}