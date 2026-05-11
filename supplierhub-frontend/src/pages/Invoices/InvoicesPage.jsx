import { useState } from 'react'
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query'
import { useNavigate } from 'react-router-dom'
import { useForm } from 'react-hook-form'
import toast from 'react-hot-toast'
import { invoicesApi } from '../../api/operations.api'
import { suppliersApi } from '../../api/suppliers.api'
import { purchaseOrdersApi } from '../../api/procurement.api'
import { StatusPill } from '../../components/ui/StatusPill'
import { PageHeader, Spinner, EmptyState } from '../../components/ui/index'
import Modal from '../../components/ui/Modal'
import useAuthStore from '../../store/auth.store'
 
// InvoiceStatus enum: Submitted=1, Pending_Match=2, On_Hold=3, Approved=4, Paid=5, Rejected=6
const INVOICE_STATUSES = [
  'Submitted',
  'Pending_Match',
  'On_Hold',
  'Approved',
  'Paid',
  'Rejected',
]
 
const statusColor = (s) => {
  const m = {
    Submitted:     'pill-blue',
    Pending_Match: 'pill-amber',
    On_Hold:       'pill-orange',
    Approved:      'pill-teal',
    Paid:          'pill-green',
    Rejected:      'pill-red',
  }
  return `pill ${m[s] ?? 'pill-gray'}`
}
 
export default function InvoicesPage() {
  const navigate   = useNavigate()
  const qc         = useQueryClient()
  const { user }   = useAuthStore()
 
  const isAdmin          = user?.roles?.includes('Admin')
  const isSupplier       = user?.roles?.includes('SupplierUser')
  const isAccountsPayable = user?.roles?.includes('AccountsPayable')
  const isBuyer          = user?.roles?.includes('Buyer')
 
  const canCreate = isAdmin || isSupplier || isAccountsPayable || isBuyer
  const canEdit   = isAdmin || isAccountsPayable
 
  const [modal, setModal]       = useState(null)   // 'create' | 'edit'
  const [selected, setSelected] = useState(null)
  const [search, setSearch]     = useState('')
  const [statusFilter, setStatusFilter] = useState('all')
 
  // ── Fetch invoices ─────────────────────────────────────
  const { data, isLoading } = useQuery({
    queryKey: ['invoices'],
    queryFn:  invoicesApi.getAll,
  })
 
  const allRows = data?.data ?? data ?? []
  const rows = allRows
    .filter(inv =>
      String(inv.invoiceID  ?? '').includes(search) ||
      (inv.invoiceNo  ?? '').toLowerCase().includes(search.toLowerCase()) ||
      String(inv.supplierID ?? '').includes(search)
    )
    .filter(inv => statusFilter === 'all' || inv.status === statusFilter)
 
  // ── Dropdowns ──────────────────────────────────────────
  const { data: suppData } = useQuery({
    queryKey: ['suppliers'],
    queryFn:  suppliersApi.getAll,
  })
  const suppliers = suppData?.data ?? suppData ?? []
 
  const { data: poData } = useQuery({
    queryKey: ['pos'],
    queryFn:  purchaseOrdersApi.getAll,
  })
  const pos = poData?.data ?? poData ?? []
 
  // ── Forms ──────────────────────────────────────────────
  const { register, handleSubmit, reset, formState: { errors } } = useForm()
 
  // ── Mutations ──────────────────────────────────────────
  const createMut = useMutation({
    mutationFn: invoicesApi.create,
    onSuccess: (res) => {
      qc.invalidateQueries(['invoices'])
      setModal(null)
      const id = res?.invoiceID ?? res?.data?.invoiceID
      toast.success(`Invoice #${id} submitted — status: Submitted`)
    },
    onError: e => toast.error(e.response?.data?.message ?? 'Failed to create invoice'),
  })
 
  const updateMut = useMutation({
    mutationFn: ({ id, dto }) => invoicesApi.update(id, dto),
    onSuccess: () => {
      qc.invalidateQueries(['invoices'])
      setModal(null)
      toast.success('Invoice updated successfully')
    },
    onError: e => toast.error(e.response?.data?.message ?? 'Failed to update invoice'),
  })
 
  // ── Handlers ───────────────────────────────────────────
  const openCreate = () => {
    reset({
      SupplierID:  '',
      PoID:        '',
      InvoiceNo:   '',
      InvoiceDate: new Date().toISOString().split('T')[0],
      Currency:    'INR',
      TotalAmount: '',
      TaxJson:     '',
    })
    setSelected(null)
    setModal('create')
  }
 
  const openEdit = async (inv) => {
    setSelected(inv)
    reset({
      SupplierID:  inv.supplierID  ?? '',
      PoID:        inv.poID        ?? '',
      InvoiceNo:   inv.invoiceNo   ?? '',
      InvoiceDate: inv.invoiceDate?.split('T')[0] ?? '',
      Currency:    inv.currency    ?? 'INR',
      TotalAmount: inv.totalAmount ?? '',
      TaxJson:     inv.taxJson     ?? '',
      Status:      inv.status      ?? 'Submitted',
    })
    setModal('edit')
  }
 
  const onSubmitCreate = (d) => {
    // Status is NOT sent — service always sets "Submitted"
    createMut.mutate({
      SupplierID:  Number(d.SupplierID),
      PoID:        d.PoID        ? Number(d.PoID)        : null,
      InvoiceNo:   d.InvoiceNo   || null,
      InvoiceDate: d.InvoiceDate || null,
      Currency:    d.Currency    || null,
      TotalAmount: d.TotalAmount ? Number(d.TotalAmount) : null,
      TaxJson:     d.TaxJson     || null,
    })
  }
 
  const onSubmitEdit = (d) => {
    // InvoiceUpdateDto requires InvoiceID + SupplierID in body
    updateMut.mutate({
      id: selected.invoiceID,
      dto: {
        InvoiceID:   selected.invoiceID,
        SupplierID:  Number(d.SupplierID),
        PoID:        d.PoID        ? Number(d.PoID)        : null,
        InvoiceNo:   d.InvoiceNo   || null,
        InvoiceDate: d.InvoiceDate || null,
        Currency:    d.Currency    || null,
        TotalAmount: d.TotalAmount ? Number(d.TotalAmount) : null,
        TaxJson:     d.TaxJson     || null,
        Status:      d.Status,
      },
    })
  }
 
  const isPending = createMut.isPending || updateMut.isPending
 
  const getSupplierName = (id) =>
    suppliers.find(s => s.supplierID === id)?.legalName ?? `Supplier #${id}`
 
  // ── Summary stats ──────────────────────────────────────
  const stats = {
    total:       allRows.length,
    submitted:   allRows.filter(i => i.status === 'Submitted').length,
    onHold:      allRows.filter(i => i.status === 'On_Hold').length,
    approved:    allRows.filter(i => i.status === 'Approved').length,
    paid:        allRows.filter(i => i.status === 'Paid').length,
  }
 
  return (
    <div>
      <PageHeader
        title="Invoices"
        subtitle="Supplier invoices — submit, match against PO & GRN, approve and pay"
        action={
          canCreate && (
            <button className="btn btn-primary" onClick={openCreate}>
              + Submit Invoice
            </button>
          )
        }
      />
 
      {/* ── Stats bar ─────────────────────────────────────── */}
      <div className="grid grid-cols-2 gap-3 mb-5 md:grid-cols-5">
        {[
          { label: 'Total',     value: stats.total,     color: 'text-gray-800' },
          { label: 'Submitted', value: stats.submitted, color: 'text-blue-600' },
          { label: 'On Hold',   value: stats.onHold,    color: 'text-orange-600' },
          { label: 'Approved',  value: stats.approved,  color: 'text-teal-600' },
          { label: 'Paid',      value: stats.paid,      color: 'text-green-600' },
        ].map(s => (
          <div key={s.label} className="sh-card py-3 text-center">
            <p className={`text-2xl font-bold ${s.color}`}>{s.value}</p>
            <p className="text-xs text-gray-500 mt-0.5">{s.label}</p>
          </div>
        ))}
      </div>
 
      {/* ── Filters ───────────────────────────────────────── */}
      <div className="flex gap-3 mb-4 flex-wrap">
        <input
          className="sh-input max-w-xs"
          placeholder="Search by ID, invoice no or supplier…"
          value={search}
          onChange={e => setSearch(e.target.value)}
        />
        <select
          className="sh-select"
          style={{ width: 160 }}
          value={statusFilter}
          onChange={e => setStatusFilter(e.target.value)}
        >
          <option value="all">All Statuses</option>
          {INVOICE_STATUSES.map(s => (
            <option key={s} value={s}>{s.replace('_', ' ')}</option>
          ))}
        </select>
      </div>
 
      {/* ── Table ─────────────────────────────────────────── */}
      <div className="sh-card p-0 overflow-hidden">
        {isLoading ? (
          <div className="flex justify-center py-12"><Spinner /></div>
        ) : rows.length === 0 ? (
          <EmptyState message="No invoices found." />
        ) : (
          <table className="sh-table">
            <thead>
              <tr>
                <th>Sr. No.</th>
                <th>Invoice ID</th>
                <th>Invoice No.</th>
                <th>Supplier</th>
                <th>PO ID</th>
                <th>Invoice Date</th>
                <th>Total Amount</th>
                <th>Currency</th>
                <th>Status</th>
                <th>Actions</th>
              </tr>
            </thead>
            <tbody>
              {rows.map((inv, index) => (
                <tr key={inv.invoiceID}>
                  <td className="text-gray-400 text-xs">{index + 1}</td>
                  <td
                    className="font-medium text-blue-700 cursor-pointer hover:underline"
                    onClick={() => navigate(`/invoices/${inv.invoiceID}`)}
                  >
                    INV-{inv.invoiceID}
                  </td>
                  <td className="font-mono text-xs">{inv.invoiceNo ?? '—'}</td>
                  <td className="font-medium">{getSupplierName(inv.supplierID)}</td>
                  <td>{inv.poID ? `PO-${inv.poID}` : '—'}</td>
                  <td className="text-xs text-gray-500">
                    {inv.invoiceDate
                      ? new Date(inv.invoiceDate).toLocaleDateString()
                      : '—'}
                  </td>
                  <td className="font-semibold text-green-700">
                    {inv.totalAmount != null
                      ? inv.totalAmount.toLocaleString()
                      : '—'}
                  </td>
                  <td>{inv.currency ?? '—'}</td>
                  <td>
                    <span className={statusColor(inv.status)}>
                      {inv.status?.replace('_', ' ')}
                    </span>
                  </td>
                  <td>
                    <div className="flex gap-1">
                      <button
                        className="btn btn-secondary btn-sm"
                        onClick={() => navigate(`/invoices/${inv.invoiceID}`)}
                      >
                        View
                      </button>
                      {canEdit && (
                        <button
                          className="btn btn-ghost btn-sm"
                          onClick={() => openEdit(inv)}
                        >
                          Edit
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
 
      {/* ── Create Modal ─────────────────────────────────── */}
      {modal === 'create' && (
        <Modal
          title="Submit New Invoice"
          onClose={() => setModal(null)}
          footer={
            <>
              <button className="btn btn-secondary btn-sm" onClick={() => setModal(null)}>
                Cancel
              </button>
              <button
                className="btn btn-primary btn-sm"
                onClick={handleSubmit(onSubmitCreate)}
                disabled={isPending}
              >
                {isPending ? 'Submitting…' : 'Submit Invoice'}
              </button>
            </>
          }
        >
          <form className="space-y-3" noValidate>
            <div className="bg-blue-50 rounded-lg px-3 py-2 text-xs text-blue-700">
              New invoices are automatically set to <strong>Submitted</strong> status.
              The AP team will review and process.
            </div>
 
            <div>
              <label className="sh-label">Supplier *</label>
              <select
                className="sh-select"
                {...register('SupplierID', { required: 'Supplier is required' })}
              >
                <option value="">— Select supplier —</option>
                {suppliers.map(s => (
                  <option key={s.supplierID} value={s.supplierID}>
                    {s.legalName}
                  </option>
                ))}
              </select>
              {errors.SupplierID && (
                <p className="text-red-500 text-xs mt-1">{errors.SupplierID.message}</p>
              )}
            </div>
 
            <div>
              <label className="sh-label">
                Purchase Order
                <span className="text-gray-400 font-normal ml-1">(optional)</span>
              </label>
              <select className="sh-select" {...register('PoID')}>
                <option value="">— Select PO (if applicable) —</option>
                {pos.map(p => (
                  <option key={p.poID} value={p.poID}>
                    PO-{p.poID} — {p.status}
                  </option>
                ))}
              </select>
            </div>
 
            <div className="grid grid-cols-2 gap-3">
              <div>
                <label className="sh-label">Invoice Number</label>
                <input
                  className="sh-input font-mono"
                  placeholder="e.g. INV-2025-001"
                  {...register('InvoiceNo')}
                />
              </div>
              <div>
                <label className="sh-label">Invoice Date</label>
                <input type="date" className="sh-input" {...register('InvoiceDate')} />
              </div>
            </div>
 
            <div className="grid grid-cols-2 gap-3">
              <div>
                <label className="sh-label">Total Amount</label>
                <input
                  type="number"
                  step="0.01"
                  min="0"
                  className="sh-input"
                  placeholder="e.g. 150000.00"
                  {...register('TotalAmount')}
                />
              </div>
              <div>
                <label className="sh-label">Currency</label>
                <select className="sh-select" {...register('Currency')}>
                  <option>INR</option>
                  <option>USD</option>
                  <option>EUR</option>
                  <option>GBP</option>
                </select>
              </div>
            </div>
 
            <div>
              <label className="sh-label">
                Tax (JSON)
                <span className="text-gray-400 font-normal ml-1">(optional)</span>
              </label>
              <input
                className="sh-input font-mono text-xs"
                placeholder='{"gst":"18%","amount":27000}'
                {...register('TaxJson')}
              />
            </div>
          </form>
        </Modal>
      )}
 
      {/* ── Edit Modal ───────────────────────────────────── */}
      {modal === 'edit' && (
        <Modal
          title={`Edit INV-${selected?.invoiceID}`}
          onClose={() => setModal(null)}
          footer={
            <>
              <button className="btn btn-secondary btn-sm" onClick={() => setModal(null)}>
                Cancel
              </button>
              <button
                className="btn btn-primary btn-sm"
                onClick={handleSubmit(onSubmitEdit)}
                disabled={isPending}
              >
                {isPending ? 'Saving…' : 'Update Invoice'}
              </button>
            </>
          }
        >
          <form className="space-y-3" noValidate>
            <div>
              <label className="sh-label">Supplier *</label>
              <select
                className="sh-select"
                {...register('SupplierID', { required: true })}
              >
                <option value="">— Select supplier —</option>
                {suppliers.map(s => (
                  <option key={s.supplierID} value={s.supplierID}>
                    {s.legalName}
                  </option>
                ))}
              </select>
            </div>
 
            <div>
              <label className="sh-label">Purchase Order</label>
              <select className="sh-select" {...register('PoID')}>
                <option value="">— None —</option>
                {pos.map(p => (
                  <option key={p.poID} value={p.poID}>PO-{p.poID}</option>
                ))}
              </select>
            </div>
 
            <div className="grid grid-cols-2 gap-3">
              <div>
                <label className="sh-label">Invoice Number</label>
                <input className="sh-input font-mono" {...register('InvoiceNo')} />
              </div>
              <div>
                <label className="sh-label">Invoice Date</label>
                <input type="date" className="sh-input" {...register('InvoiceDate')} />
              </div>
            </div>
 
            <div className="grid grid-cols-2 gap-3">
              <div>
                <label className="sh-label">Total Amount</label>
                <input type="number" step="0.01" className="sh-input"
                  {...register('TotalAmount')} />
              </div>
              <div>
                <label className="sh-label">Currency</label>
                <select className="sh-select" {...register('Currency')}>
                  <option>INR</option><option>USD</option>
                  <option>EUR</option><option>GBP</option>
                </select>
              </div>
            </div>
 
            <div>
              <label className="sh-label">Tax (JSON)</label>
              <input className="sh-input font-mono text-xs" {...register('TaxJson')} />
            </div>
 
            {/* Status — only on edit, AP/Admin only */}
            {canEdit && (
              <div>
                <label className="sh-label">Status</label>
                <select className="sh-select" {...register('Status')}>
                  {INVOICE_STATUSES.map(s => (
                    <option key={s} value={s}>{s.replace('_', ' ')}</option>
                  ))}
                </select>
                <p className="text-xs text-gray-400 mt-1">
                  Move invoice through the approval workflow
                </p>
              </div>
            )}
          </form>
        </Modal>
      )}
    </div>
  )
}