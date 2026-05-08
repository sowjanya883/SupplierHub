import { useState } from 'react'
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query'
import { useNavigate } from 'react-router-dom'
import { useForm } from 'react-hook-form'
import toast from 'react-hot-toast'
import { invoicesApi } from '../../api/operations.api'
import { StatusPill } from '../../components/ui/StatusPill'
import { PageHeader, Spinner, EmptyState } from '../../components/ui/index'
import Modal from '../../components/ui/Modal'

export default function InvoicesPage() {
  const navigate = useNavigate()
  const qc = useQueryClient()
  const [showCreate, setShowCreate] = useState(false)
  const [search, setSearch] = useState('')

  const { data, isLoading } = useQuery({ queryKey: ['invoices'], queryFn: invoicesApi.getAll })
  const rows = (data?.data ?? data ?? []).filter(i =>
    (i.invoiceNo ?? '').toLowerCase().includes(search.toLowerCase()) ||
    (i.status ?? '').toLowerCase().includes(search.toLowerCase())
  )

  const { register, handleSubmit, reset, formState: { errors } } = useForm()
  const createMut = useMutation({
    mutationFn: (form) => invoicesApi.create({
      ...form,
      invoiceDate: form.invoiceDate ? new Date(form.invoiceDate).toISOString() : new Date().toISOString(),
    }),
    onSuccess: () => { qc.invalidateQueries(['invoices']); setShowCreate(false); reset(); toast.success('Invoice created') },
    onError: e => toast.error(extractMsg(e) ?? 'Error creating invoice'),
  })

  return (
    <div>
      <PageHeader
        title="Invoices"
        subtitle="Supplier invoices and 3-way match references"
        action={<button className="btn btn-primary" onClick={() => { reset(); setShowCreate(true) }}>+ New Invoice</button>}
      />
      <div className="mb-4">
        <input className="sh-input max-w-xs" placeholder="Search by invoice no or status…"
          value={search} onChange={e => setSearch(e.target.value)} />
      </div>
      <div className="sh-card p-0 overflow-hidden">
        {isLoading ? <div className="flex justify-center py-12"><Spinner /></div>
          : rows.length === 0 ? <EmptyState />
          : (
          <table className="sh-table">
            <thead><tr><th>Invoice ID</th><th>Invoice No</th><th>Supplier ID</th><th>PO ID</th><th>Date</th><th>Total</th><th>Status</th><th></th></tr></thead>
            <tbody>
              {rows.map(inv => (
                <tr key={inv.invoiceID}>
                  <td className="font-mono text-xs text-gray-500">#{inv.invoiceID}</td>
                  <td className="font-medium cursor-pointer text-blue-700 hover:underline"
                    onClick={() => navigate(`/invoices/${inv.invoiceID}`)}>
                    {inv.invoiceNo}
                  </td>
                  <td>{inv.supplierID}</td>
                  <td>{inv.poID}</td>
                  <td className="text-xs text-gray-500">{inv.invoiceDate ? new Date(inv.invoiceDate).toLocaleDateString() : '—'}</td>
                  <td className="font-medium">{inv.totalAmount}</td>
                  <td><StatusPill status={inv.status} /></td>
                  <td><button className="btn btn-ghost btn-sm" onClick={() => navigate(`/invoices/${inv.invoiceID}`)}>View →</button></td>
                </tr>
              ))}
            </tbody>
          </table>
        )}
      </div>

      {showCreate && (
        <Modal title="New Invoice" onClose={() => setShowCreate(false)}
          footer={
            <>
              <button className="btn btn-secondary btn-sm" onClick={() => setShowCreate(false)}>Cancel</button>
              <button className="btn btn-primary btn-sm" onClick={handleSubmit(d => createMut.mutate(d))}
                disabled={createMut.isPending}>{createMut.isPending ? 'Saving…' : 'Create'}</button>
            </>
          }
        >
          <form className="space-y-3" noValidate>
            <div><label className="sh-label">Supplier ID *</label>
              <input type="number" className="sh-input" {...register('supplierID', { required: true, valueAsNumber: true })} /></div>
            <div><label className="sh-label">PO ID</label>
              <input type="number" className="sh-input" {...register('poID', { valueAsNumber: true })} /></div>
            <div><label className="sh-label">Invoice No *</label>
              <input className="sh-input" {...register('invoiceNo', { required: true })} /></div>
            <div><label className="sh-label">Invoice Date *</label>
              <input type="date" className="sh-input"
                defaultValue={new Date().toISOString().slice(0, 10)}
                {...register('invoiceDate', { required: true })} /></div>
            <div><label className="sh-label">Total Amount</label>
              <input type="number" step="0.01" className="sh-input" {...register('totalAmount', { valueAsNumber: true })} /></div>
            <div><label className="sh-label">Currency</label>
              <input className="sh-input" defaultValue="USD" {...register('currency')} /></div>
            <div><label className="sh-label">Status</label>
              <select className="sh-select" {...register('status')}>
                {['Submitted','Hold','Approved','Rejected','Paid'].map(s => <option key={s}>{s}</option>)}
              </select></div>
          </form>
        </Modal>
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
