import { useState } from 'react'
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query'
import { useNavigate } from 'react-router-dom'
import { useForm } from 'react-hook-form'
import toast from 'react-hot-toast'
import { purchaseOrdersApi } from '../../api/procurement.api'
import { StatusPill } from '../../components/ui/StatusPill'
import { PageHeader, Spinner, EmptyState, ConfirmDialog } from '../../components/ui/index'
import Modal from '../../components/ui/Modal'

const STATUSES = ['Open','Acknowledged','PartiallyShipped','Shipped','Closed','Cancelled']

export default function PurchaseOrdersPage() {
  const navigate = useNavigate()
  const qc = useQueryClient()
  const [modal, setModal] = useState(null)
  const [selected, setSelected] = useState(null)
  const [search, setSearch] = useState('')

  const { data, isLoading } = useQuery({ queryKey: ['pos'], queryFn: purchaseOrdersApi.getAll })
  const rows = (data?.data ?? data ?? []).filter(p =>
    String(p.poID ?? p.poid ?? '').includes(search) ||
    (p.status ?? '').toLowerCase().includes(search.toLowerCase())
  )

  const { register, handleSubmit, reset, setValue, formState: { errors } } = useForm()

  const createMut = useMutation({
    mutationFn: purchaseOrdersApi.create,
    onSuccess: () => { qc.invalidateQueries(['pos']); setModal(null); toast.success('PO created') },
    onError: e => toast.error(extractMsg(e) ?? 'Error creating PO'),
  })
  const updateMut = useMutation({
    mutationFn: ({ id, dto }) => purchaseOrdersApi.update(id, dto),
    onSuccess: () => { qc.invalidateQueries(['pos']); setModal(null); toast.success('PO updated') },
    onError: e => toast.error(extractMsg(e) ?? 'Error updating PO'),
  })
  const deleteMut = useMutation({
    mutationFn: (id) => purchaseOrdersApi.delete(id),
    onSuccess: () => { qc.invalidateQueries(['pos']); setModal(null); toast.success('PO deleted') },
    onError: e => toast.error(extractMsg(e) ?? 'Error'),
  })

  const openCreate = () => { reset(); setSelected(null); setModal('form') }
  const openEdit = (row) => {
    setSelected(row)
    Object.entries(row).forEach(([k, v]) => setValue(k, v))
    setModal('form')
  }

  const onSubmit = (d) => {
    const payload = {
      ...d,
      poDate: d.poDate ? new Date(d.poDate).toISOString() : new Date().toISOString(),
    }
    if (selected) updateMut.mutate({ id: selected.poID ?? selected.poid, dto: { ...payload, poID: selected.poID ?? selected.poid } })
    else createMut.mutate(payload)
  }

  const isPending = createMut.isPending || updateMut.isPending

  return (
    <div>
      <PageHeader
        title="Purchase Orders"
        subtitle="Manage and track all purchase orders"
        action={<button className="btn btn-primary" onClick={openCreate}>+ New PO</button>}
      />

      <div className="mb-4">
        <input className="sh-input max-w-xs" placeholder="Search by ID or status…"
          value={search} onChange={e => setSearch(e.target.value)} />
      </div>

      <div className="sh-card p-0 overflow-hidden">
        {isLoading ? (
          <div className="flex justify-center py-12"><Spinner /></div>
        ) : rows.length === 0 ? <EmptyState /> : (
          <table className="sh-table">
            <thead>
              <tr><th>PO ID</th><th>Supplier ID</th><th>Org ID</th><th>Date</th><th>Currency</th><th>Status</th><th>Actions</th></tr>
            </thead>
            <tbody>
              {rows.map(po => {
                const id = po.poID ?? po.poid
                return (
                  <tr key={id}>
                    <td className="font-medium cursor-pointer text-blue-700 hover:underline"
                      onClick={() => navigate(`/purchase-orders/${id}`)}>{id}</td>
                    <td>{po.supplierID}</td>
                    <td>{po.orgID}</td>
                    <td className="text-gray-500 text-xs">{po.poDate ? new Date(po.poDate).toLocaleDateString() : '—'}</td>
                    <td>{po.currency}</td>
                    <td><StatusPill status={po.status} /></td>
                    <td>
                      <div className="flex gap-1">
                        <button className="btn btn-ghost btn-sm" onClick={() => openEdit(po)}>Edit</button>
                        <button className="btn btn-danger btn-sm" onClick={() => { setSelected(po); setModal('delete') }}>Del</button>
                      </div>
                    </td>
                  </tr>
                )
              })}
            </tbody>
          </table>
        )}
      </div>

      {modal === 'form' && (
        <Modal
          title={selected ? `Edit PO #${selected.poID ?? selected.poid}` : 'New Purchase Order'}
          onClose={() => setModal(null)}
          footer={
            <>
              <button className="btn btn-secondary btn-sm" onClick={() => setModal(null)}>Cancel</button>
              <button className="btn btn-primary btn-sm" onClick={handleSubmit(onSubmit)} disabled={isPending}>
                {isPending ? 'Saving…' : selected ? 'Update' : 'Create'}
              </button>
            </>
          }
        >
          <form noValidate className="space-y-3">
            <div>
              <label className="sh-label">Supplier ID *</label>
              <input type="number" className="sh-input" {...register('supplierID', { required: 'Required', valueAsNumber: true })} />
              {errors.supplierID && <p className="text-red-500 text-xs mt-1">{errors.supplierID.message}</p>}
            </div>
            <div>
              <label className="sh-label">Org ID *</label>
              <input type="number" className="sh-input" {...register('orgID', { required: 'Required', valueAsNumber: true })} />
            </div>
            <div>
              <label className="sh-label">PO Date *</label>
              <input type="date" className="sh-input"
                defaultValue={new Date().toISOString().slice(0, 10)}
                {...register('poDate', { required: 'Required' })} />
            </div>
            <div>
              <label className="sh-label">Currency</label>
              <input className="sh-input" defaultValue="USD" {...register('currency')} />
            </div>
            <div>
              <label className="sh-label">Incoterms</label>
              <input className="sh-input" {...register('incoterms')} />
            </div>
            <div>
              <label className="sh-label">Payment Terms</label>
              <input className="sh-input" {...register('paymentTerms')} />
            </div>
            <div>
              <label className="sh-label">Status</label>
              <select className="sh-select" {...register('status')}>
                {STATUSES.map(s => <option key={s}>{s}</option>)}
              </select>
            </div>
          </form>
        </Modal>
      )}

      {modal === 'delete' && (
        <ConfirmDialog
          message={`Delete PO #${selected?.poID ?? selected?.poid}? This cannot be undone.`}
          onConfirm={() => deleteMut.mutate(selected?.poID ?? selected?.poid)}
          onCancel={() => setModal(null)}
          loading={deleteMut.isPending}
        />
      )}
    </div>
  )
}

function extractMsg(err) {
  const data = err?.response?.data
  if (!data) return null
  const firstModelError = data.errors ? Object.values(data.errors).flat().find(Boolean) : null
  return firstModelError ?? data.message ?? data.detail ?? (typeof data === 'string' ? data : null)
}
