import { useState } from 'react'
import { useParams, useNavigate } from 'react-router-dom'
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query'
import { useForm } from 'react-hook-form'
import toast from 'react-hot-toast'
import {
    invoicesApi,
    invoiceLinesApi,
    matchRefsApi,
} from '../../api/operations.api'
import { suppliersApi } from '../../api/suppliers.api'
import { purchaseOrdersApi, poLinesApi } from '../../api/procurement.api'
import { grnApi } from '../../api/operations.api'
import { Spinner, EmptyState } from '../../components/ui/index'
import Modal from '../../components/ui/Modal'
import useAuthStore from '../../store/auth.store'

// ── Enums ──────────────────────────────────────────────
const INVOICE_STATUSES = ['Submitted', 'Pending_Match', 'On_Hold', 'Approved', 'Paid', 'Rejected']
const MATCH_RESULTS = ['Matched', 'Mismatch', 'Partial', 'Pending']
const MATCH_REF_STATUSES = ['Active', 'Inactive', 'Closed']
const LINE_MATCH_STATUS = ['Active', 'Inactive', 'Archive']

const statusColor = (s) => {
    const m = {
        Submitted: 'pill-blue', Pending_Match: 'pill-amber',
        On_Hold: 'pill-orange', Approved: 'pill-teal',
        Paid: 'pill-green', Rejected: 'pill-red',
        Matched: 'pill-green', Mismatch: 'pill-red',
        Partial: 'pill-amber', Pending: 'pill-gray',
        Active: 'pill-blue', Inactive: 'pill-gray', Closed: 'pill-green',
        Archive: 'pill-gray',
    }
    return `pill ${m[s] ?? 'pill-gray'}`
}

const TABS = ['Invoice Lines', 'Match References']

// ─────────────────────────────────────────────────────────
export default function InvoiceDetailPage() {
    const { id } = useParams()
    const navigate = useNavigate()
    const { user } = useAuthStore()
    const [tab, setTab] = useState('Invoice Lines')

    const isAdmin = user?.roles?.includes('Admin')
    const isAccountsPayable = user?.roles?.includes('AccountsPayable')
    const isSupplier = user?.roles?.includes('SupplierUser')
    const canEditLines = isAdmin || isAccountsPayable || isSupplier
    const canManageMatch = isAdmin || isAccountsPayable || user?.roles?.includes('Buyer') || user?.roles?.includes('ReceivingUser')

    // ── Fetch invoice header ───────────────────────────────
    const { data: invData, isLoading } = useQuery({
        queryKey: ['invoice', id],
        queryFn: () => invoicesApi.getById(id),
    })
    const inv = invData?.data ?? invData

    // ── Suppliers for name lookup ──────────────────────────
    const { data: suppData } = useQuery({
        queryKey: ['suppliers'],
        queryFn: suppliersApi.getAll,
    })
    const suppliers = suppData?.data ?? suppData ?? []
    const getSupplierName = (sid) =>
        suppliers.find(s => s.supplierID === sid)?.legalName ?? `Supplier #${sid}`

    if (isLoading) return <div className="flex justify-center py-16"><Spinner /></div>
    if (!inv) return <p className="text-gray-500 p-4">Invoice not found.</p>

    const invStatusColor = (s) => {
        const m = {
            Submitted: 'pill-blue', Pending_Match: 'pill-amber',
            On_Hold: 'pill-orange', Approved: 'pill-teal',
            Paid: 'pill-green', Rejected: 'pill-red',
        }
        return `pill ${m[s] ?? 'pill-gray'}`
    }

    return (
        <div>
            {/* ── Header ────────────────────────────────────────── */}
            <div className="mb-5">
                <button
                    className="text-sm text-blue-600 hover:underline mb-2 block"
                    onClick={() => navigate(-1)}
                >
                    ← Back to Invoices
                </button>

                <div className="sh-card">
                    <div className="flex items-start justify-between">
                        <div>
                            <h1 className="text-xl font-semibold text-gray-900">
                                INV-{inv.invoiceID}
                                {inv.invoiceNo && (
                                    <span className="ml-2 text-sm font-normal text-gray-500 font-mono">
                                        ({inv.invoiceNo})
                                    </span>
                                )}
                            </h1>
                            <p className="text-sm text-gray-500 mt-1">
                                {getSupplierName(inv.supplierID)}
                                {inv.poID && <> &nbsp;·&nbsp; PO-{inv.poID}</>}
                            </p>
                        </div>
                        <span className={invStatusColor(inv.status)}>
                            {inv.status?.replace('_', ' ')}
                        </span>
                    </div>

                    <div className="grid grid-cols-2 gap-4 mt-4 text-sm md:grid-cols-5">
                        <Detail label="Invoice ID" value={`INV-${inv.invoiceID}`} />
                        <Detail label="Invoice No." value={inv.invoiceNo ?? '—'} />
                        <Detail label="Supplier" value={getSupplierName(inv.supplierID)} />
                        <Detail label="PO" value={inv.poID ? `PO-${inv.poID}` : '—'} />
                        <Detail label="Invoice Date"
                            value={inv.invoiceDate
                                ? new Date(inv.invoiceDate).toLocaleDateString()
                                : '—'} />
                        <Detail label="Total Amount"
                            value={inv.totalAmount != null
                                ? `${inv.totalAmount.toLocaleString()} ${inv.currency ?? ''}`
                                : '—'} />
                        <Detail label="Currency" value={inv.currency ?? '—'} />
                        <Detail label="Created"
                            value={inv.createdOn
                                ? new Date(inv.createdOn).toLocaleDateString()
                                : '—'} />
                        {inv.taxJson && (
                            <div className="col-span-2 md:col-span-2">
                                <p className="text-gray-500 text-xs font-medium">Tax JSON</p>
                                <p className="text-gray-800 font-mono text-xs mt-0.5">{inv.taxJson}</p>
                            </div>
                        )}
                    </div>
                </div>
            </div>

            {/* ── Tabs ──────────────────────────────────────────── */}
            <div className="flex gap-1 mb-4 border-b border-gray-200">
                {TABS.map(t => (
                    <button key={t} onClick={() => setTab(t)}
                        className={`px-4 py-2 text-sm font-medium border-b-2 transition-colors ${tab === t
                                ? 'border-blue-600 text-blue-700'
                                : 'border-transparent text-gray-500 hover:text-gray-700'
                            }`}>
                        {t}
                    </button>
                ))}
            </div>

            {tab === 'Invoice Lines' && <InvoiceLinesTab inv={inv} canEdit={canEditLines} />}
            {tab === 'Match References' && <MatchRefsTab inv={inv} canManage={canManageMatch} />}
        </div>
    )
}

function Detail({ label, value }) {
    return (
        <div>
            <p className="text-gray-500 text-xs font-medium">{label}</p>
            <p className="text-gray-800 font-medium mt-0.5 text-sm">{value}</p>
        </div>
    )
}

// ─────────────────────────────────────────────────────────
// INVOICE LINES TAB
// ─────────────────────────────────────────────────────────
function InvoiceLinesTab({ inv, canEdit }) {
    const qc = useQueryClient()
    const [modal, setModal] = useState(null)
    const [selected, setSelected] = useState(null)

    const { data, isLoading } = useQuery({
        queryKey: ['invoice-lines', inv.invoiceID],
        queryFn: () => invoiceLinesApi.getByInvoiceId(inv.invoiceID),
    })
    const lines = data?.data ?? data ?? []

    // PO Lines for dropdown — only if invoice is linked to a PO
    const { data: poLinesData } = useQuery({
        queryKey: ['po-lines', inv.poID],
        queryFn: () => poLinesApi.getByPoId(inv.poID),
        enabled: !!inv.poID,
    })
    const poLines = poLinesData?.data ?? poLinesData ?? []

    const { register, handleSubmit, reset, formState: { errors } } = useForm()

    const createMut = useMutation({
        mutationFn: invoiceLinesApi.create,
        onSuccess: () => {
            qc.invalidateQueries(['invoice-lines', inv.invoiceID])
            setModal(null)
            toast.success('Invoice line added')
        },
        onError: e => toast.error(e.response?.data?.message ?? 'Failed to add line'),
    })

    const updateMut = useMutation({
        mutationFn: ({ id, dto }) => invoiceLinesApi.update(id, dto),
        onSuccess: () => {
            qc.invalidateQueries(['invoice-lines', inv.invoiceID])
            setModal(null)
            toast.success('Invoice line updated')
        },
        onError: e => toast.error(e.response?.data?.message ?? 'Failed to update line'),
    })

    const openCreate = () => {
        reset({
            PoLineID: '',
            Qty: '',
            UnitPrice: '',
            LineTotal: '',
            TaxJson: '',
            MatchStatus: 'Active',
        })
        setSelected(null)
        setModal('form')
    }

    const openEdit = (row) => {
        setSelected(row)
        reset({
            PoLineID: row.poLineID ?? '',
            Qty: row.qty ?? '',
            UnitPrice: row.unitPrice ?? '',
            LineTotal: row.lineTotal ?? '',
            TaxJson: row.taxJson ?? '',
            MatchStatus: row.matchStatus ?? 'Active',
        })
        setModal('form')
    }

    const onSubmit = (d) => {
        if (selected) {
            // InvoiceLineUpdateDto requires InvLineID + InvoiceID
            updateMut.mutate({
                id: selected.invLineID,
                dto: {
                    InvLineID: selected.invLineID,
                    InvoiceID: inv.invoiceID,
                    PoLineID: d.PoLineID ? Number(d.PoLineID) : null,
                    Qty: d.Qty ? Number(d.Qty) : null,
                    UnitPrice: d.UnitPrice ? Number(d.UnitPrice) : null,
                    LineTotal: d.LineTotal ? Number(d.LineTotal) : null,
                    TaxJson: d.TaxJson || null,
                    MatchStatus: d.MatchStatus || null,
                },
            })
        } else {
            createMut.mutate({
                InvoiceID: inv.invoiceID,
                PoLineID: d.PoLineID ? Number(d.PoLineID) : null,
                Qty: d.Qty ? Number(d.Qty) : null,
                UnitPrice: d.UnitPrice ? Number(d.UnitPrice) : null,
                LineTotal: d.LineTotal ? Number(d.LineTotal) : null,
                TaxJson: d.TaxJson || null,
                MatchStatus: d.MatchStatus || null,
            })
        }
    }

    const isPending = createMut.isPending || updateMut.isPending

    // ── Totals ─────────────────────────────────────────────
    const totalBilled = lines.reduce((sum, l) => sum + (l.lineTotal ?? 0), 0)

    return (
        <div>
            <div className="flex justify-between items-center mb-3">
                <div>
                    <h4 className="text-sm font-medium text-gray-700">
                        Invoice Lines — {lines.length} line{lines.length !== 1 ? 's' : ''}
                    </h4>
                    <p className="text-xs text-gray-400 mt-0.5">
                        Individual items billed in this invoice
                    </p>
                </div>
                {canEdit && (
                    <button className="btn btn-primary btn-sm" onClick={openCreate}>
                        + Add Line
                    </button>
                )}
            </div>

            <div className="sh-card p-0 overflow-hidden">
                {isLoading ? (
                    <div className="flex justify-center py-8"><Spinner /></div>
                ) : lines.length === 0 ? (
                    <div className="text-center py-8">
                        <p className="text-sm text-gray-400 mb-3">No lines added yet.</p>
                        {canEdit && (
                            <button className="btn btn-primary btn-sm" onClick={openCreate}>
                                + Add First Line
                            </button>
                        )}
                    </div>
                ) : (
                    <>
                        <table className="sh-table">
                            <thead>
                                <tr>
                                    <th>Sr. No.</th>
                                    <th>Line ID</th>
                                    <th>PO Line</th>
                                    <th>Qty</th>
                                    <th>Unit Price</th>
                                    <th>Line Total</th>
                                    <th>Tax</th>
                                    <th>Match Status</th>
                                    {canEdit && <th>Actions</th>}
                                </tr>
                            </thead>
                            <tbody>
                                {lines.map((l, i) => (
                                    <tr key={l.invLineID}>
                                        <td className="text-gray-400 text-xs">{i + 1}</td>
                                        <td className="text-gray-400 text-xs">{l.invLineID}</td>
                                        <td>{l.poLineID ? `#${l.poLineID}` : '—'}</td>
                                        <td>{l.qty ?? '—'}</td>
                                        <td className="text-gray-700">
                                            {l.unitPrice?.toLocaleString() ?? '—'}
                                        </td>
                                        <td className="font-semibold text-green-700">
                                            {l.lineTotal?.toLocaleString() ?? '—'}
                                        </td>
                                        <td className="font-mono text-xs text-gray-500">
                                            {l.taxJson ?? '—'}
                                        </td>
                                        <td>
                                            {l.matchStatus ? (
                                                <span className={statusColor(l.matchStatus)}>
                                                    {l.matchStatus}
                                                </span>
                                            ) : '—'}
                                        </td>
                                        {canEdit && (
                                            <td>
                                                <button
                                                    className="btn btn-ghost btn-sm"
                                                    onClick={() => openEdit(l)}
                                                >
                                                    Edit
                                                </button>
                                            </td>
                                        )}
                                    </tr>
                                ))}
                            </tbody>
                        </table>

                        {/* Totals bar */}
                        <div className="px-4 py-3 border-t border-gray-100 bg-gray-50 flex justify-between items-center">
                            <span className="text-xs text-gray-500">
                                {lines.length} line{lines.length !== 1 ? 's' : ''}
                            </span>
                            <div className="flex gap-6 text-sm">
                                <span className="text-gray-500">
                                    Invoice Total:{' '}
                                    <strong className="text-gray-800">
                                        {inv.totalAmount?.toLocaleString() ?? '—'} {inv.currency}
                                    </strong>
                                </span>
                                <span className={`font-semibold ${totalBilled !== (inv.totalAmount ?? 0) ? 'text-red-600' : 'text-green-700'
                                    }`}>
                                    Lines Total: {totalBilled.toLocaleString()} {inv.currency}
                                    {totalBilled !== (inv.totalAmount ?? 0) && (
                                        <span className="ml-1 text-xs font-normal text-red-500">
                                            ⚠ Mismatch with header
                                        </span>
                                    )}
                                </span>
                            </div>
                        </div>
                    </>
                )}
            </div>

            {/* Form Modal */}
            {modal === 'form' && (
                <Modal
                    title={selected ? `Edit Line #${selected.invLineID}` : 'Add Invoice Line'}
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
                                {isPending ? 'Saving…' : selected ? 'Update' : 'Add Line'}
                            </button>
                        </>
                    }
                >
                    <form className="space-y-3" noValidate>
                        {/* PO Line dropdown if PO is linked */}
                        {inv.poID ? (
                            <div>
                                <label className="sh-label">
                                    PO Line
                                    <span className="text-gray-400 font-normal ml-1">(optional)</span>
                                </label>
                                <select className="sh-select" {...register('PoLineID')}>
                                    <option value="">— Select PO line —</option>
                                    {poLines.map(l => (
                                        <option key={l.poLineId} value={l.poLineId}>
                                            Line #{l.poLineId}
                                            {l.description ? ` — ${l.description}` : ''}
                                            {l.qty ? ` (${l.qty} ${l.uoM ?? ''})` : ''}
                                        </option>
                                    ))}
                                </select>
                            </div>
                        ) : (
                            <div>
                                <label className="sh-label">PO Line ID (optional)</label>
                                <input type="number" className="sh-input" {...register('PoLineID')} />
                                <p className="text-xs text-gray-400 mt-1">
                                    Link invoice line to a PO line for 3-way match
                                </p>
                            </div>
                        )}

                        <div className="grid grid-cols-3 gap-3">
                            <div>
                                <label className="sh-label">Qty</label>
                                <input type="number" step="0.01" className="sh-input"
                                    {...register('Qty')} />
                            </div>
                            <div>
                                <label className="sh-label">Unit Price</label>
                                <input type="number" step="0.01" className="sh-input"
                                    {...register('UnitPrice')} />
                            </div>
                            <div>
                                <label className="sh-label">Line Total</label>
                                <input type="number" step="0.01" className="sh-input"
                                    {...register('LineTotal')} />
                            </div>
                        </div>

                        <div>
                            <label className="sh-label">Tax (JSON)</label>
                            <input className="sh-input font-mono text-xs"
                                placeholder='{"gst":"18%","amount":1800}'
                                {...register('TaxJson')} />
                        </div>

                        <div>
                            <label className="sh-label">Match Status</label>
                            <select className="sh-select" {...register('MatchStatus')}>
                                {LINE_MATCH_STATUS.map(s => <option key={s}>{s}</option>)}
                            </select>
                        </div>
                    </form>
                </Modal>
            )}
        </div>
    )
}

// ─────────────────────────────────────────────────────────
// MATCH REFERENCES TAB — 3-way match
// ─────────────────────────────────────────────────────────
function MatchRefsTab({ inv, canManage }) {
    const qc = useQueryClient()
    const [modal, setModal] = useState(null)
    const [selected, setSelected] = useState(null)

    const { data, isLoading } = useQuery({
        queryKey: ['match-refs', inv.invoiceID],
        queryFn: () => matchRefsApi.getByInvoiceId(inv.invoiceID),
    })
    const matches = data?.data ?? data ?? []

    // GRNs for dropdown
    const { data: grnData } = useQuery({
        queryKey: ['grns'],
        queryFn: grnApi.getAll,
    })
    const grns = (grnData?.data ?? grnData ?? []).filter(g =>
        inv.poID ? g.poID === inv.poID : true
    )

    // POs for dropdown
    const { data: poData } = useQuery({
        queryKey: ['pos'],
        queryFn: purchaseOrdersApi.getAll,
    })
    const pos = poData?.data ?? poData ?? []

    const { register, handleSubmit, reset, formState: { errors } } = useForm()

    const createMut = useMutation({
        mutationFn: matchRefsApi.create,
        onSuccess: () => {
            qc.invalidateQueries(['match-refs', inv.invoiceID])
            setModal(null)
            toast.success('Match reference created')
        },
        onError: e => toast.error(e.response?.data?.message ?? 'Failed to create match'),
    })

    const updateMut = useMutation({
        mutationFn: ({ id, dto }) => matchRefsApi.update(id, dto),
        onSuccess: () => {
            qc.invalidateQueries(['match-refs', inv.invoiceID])
            setModal(null)
            toast.success('Match reference updated')
        },
        onError: e => toast.error(e.response?.data?.message ?? 'Failed to update match'),
    })

    const openCreate = () => {
        reset({
            PoID: inv.poID ?? '',
            GrnID: '',
            Result: 'Pending',
            Notes: '',
            Status: 'Active',
        })
        setSelected(null)
        setModal('form')
    }

    const openEdit = (row) => {
        setSelected(row)
        reset({
            PoID: row.poID ?? '',
            GrnID: row.grnID ?? '',
            Result: row.result ?? 'Pending',
            Notes: row.notes ?? '',
            Status: row.status ?? 'Active',
        })
        setModal('form')
    }

    const onSubmit = (d) => {
        const payload = {
            InvoiceID: inv.invoiceID,
            PoID: d.PoID ? Number(d.PoID) : null,
            GrnID: d.GrnID ? Number(d.GrnID) : null,
            Result: d.Result || null,
            Notes: d.Notes || null,
            Status: d.Status || null,
        }
        if (selected) {
            // MatchRefUpdateDto requires MatchID + InvoiceID
            updateMut.mutate({
                id: selected.matchID,
                dto: { ...payload, MatchID: selected.matchID },
            })
        } else {
            createMut.mutate(payload)
        }
    }

    const isPending = createMut.isPending || updateMut.isPending

    const matchedCount = matches.filter(m => m.result === 'Matched').length
    const mismatchCount = matches.filter(m => m.result === 'Mismatch').length

    return (
        <div>
            <div className="flex justify-between items-center mb-3">
                <div>
                    <h4 className="text-sm font-medium text-gray-700">3-Way Match References</h4>
                    <p className="text-xs text-gray-400 mt-0.5">
                        Match invoice against PO and GRN to verify quantities and amounts
                        {matches.length > 0 && (
                            <> &nbsp;·&nbsp;
                                <span className="text-green-600">{matchedCount} matched</span>
                                {mismatchCount > 0 && (
                                    <> &nbsp;·&nbsp; <span className="text-red-600">{mismatchCount} mismatch</span></>
                                )}
                            </>
                        )}
                    </p>
                </div>
                {canManage && (
                    <button className="btn btn-primary btn-sm" onClick={openCreate}>
                        + Run Match
                    </button>
                )}
            </div>

            {/* 3-way match explainer */}
            <div className="sh-card mb-4 bg-blue-50 border border-blue-100">
                <div className="flex gap-4 text-xs text-blue-800">
                    <div className="flex-1 text-center">
                        <div className="text-2xl mb-1">📋</div>
                        <div className="font-semibold">Purchase Order</div>
                        <div className="text-blue-600">What was ordered</div>
                        {inv.poID && (
                            <div className="mt-1 font-mono">PO-{inv.poID}</div>
                        )}
                    </div>
                    <div className="flex items-center text-blue-400 text-lg">↔</div>
                    <div className="flex-1 text-center">
                        <div className="text-2xl mb-1">📦</div>
                        <div className="font-semibold">GRN</div>
                        <div className="text-blue-600">What was received</div>
                    </div>
                    <div className="flex items-center text-blue-400 text-lg">↔</div>
                    <div className="flex-1 text-center">
                        <div className="text-2xl mb-1">🧾</div>
                        <div className="font-semibold">Invoice</div>
                        <div className="text-blue-600">What is being billed</div>
                        <div className="mt-1 font-mono">INV-{inv.invoiceID}</div>
                    </div>
                </div>
            </div>

            <div className="sh-card p-0 overflow-hidden">
                {isLoading ? (
                    <div className="flex justify-center py-8"><Spinner /></div>
                ) : matches.length === 0 ? (
                    <div className="text-center py-8">
                        <p className="text-sm text-gray-400 mb-1">No match references yet.</p>
                        <p className="text-xs text-gray-400 mb-3">
                            Run a 3-way match to compare invoice against PO and GRN.
                        </p>
                        {canManage && (
                            <button className="btn btn-primary btn-sm" onClick={openCreate}>
                                + Run First Match
                            </button>
                        )}
                    </div>
                ) : (
                    <table className="sh-table">
                        <thead>
                            <tr>
                                <th>Sr. No.</th>
                                <th>Match ID</th>
                                <th>PO</th>
                                <th>GRN</th>
                                <th>Result</th>
                                <th>Notes</th>
                                <th>Status</th>
                                <th>Created</th>
                                {canManage && <th>Actions</th>}
                            </tr>
                        </thead>
                        <tbody>
                            {matches.map((m, i) => (
                                <tr key={m.matchID}>
                                    <td className="text-gray-400 text-xs">{i + 1}</td>
                                    <td className="text-gray-400 text-xs">{m.matchID}</td>
                                    <td>{m.poID ? `PO-${m.poID}` : '—'}</td>
                                    <td>{m.grnID ? `GRN-${m.grnID}` : '—'}</td>
                                    <td>
                                        <span className={statusColor(m.result ?? 'Pending')}>
                                            {m.result ?? 'Pending'}
                                        </span>
                                    </td>
                                    <td className="text-xs text-gray-500 max-w-xs truncate">
                                        {m.notes ?? '—'}
                                    </td>
                                    <td>
                                        <span className={statusColor(m.status ?? '')}>
                                            {m.status ?? '—'}
                                        </span>
                                    </td>
                                    <td className="text-xs text-gray-500">
                                        {m.createdOn ? new Date(m.createdOn).toLocaleDateString() : '—'}
                                    </td>
                                    {canManage && (
                                        <td>
                                            <button
                                                className="btn btn-ghost btn-sm"
                                                onClick={() => openEdit(m)}
                                            >
                                                Update
                                            </button>
                                        </td>
                                    )}
                                </tr>
                            ))}
                        </tbody>
                    </table>
                )}
            </div>

            {/* Create / Edit Modal */}
            {modal === 'form' && (
                <Modal
                    title={selected ? `Update Match #${selected.matchID}` : 'Run 3-Way Match'}
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
                                {isPending ? 'Saving…' : selected ? 'Update' : 'Run Match'}
                            </button>
                        </>
                    }
                >
                    <form className="space-y-3" noValidate>
                        {/* PO dropdown */}
                        <div>
                            <label className="sh-label">
                                Purchase Order
                                <span className="text-gray-400 font-normal ml-1">(optional)</span>
                            </label>
                            <select className="sh-select" {...register('PoID')}>
                                <option value="">— Select PO —</option>
                                {pos.map(p => (
                                    <option key={p.poID} value={p.poID}>
                                        PO-{p.poID} ({p.status})
                                    </option>
                                ))}
                            </select>
                        </div>

                        {/* GRN dropdown — filtered to matching PO */}
                        <div>
                            <label className="sh-label">
                                GRN (Goods Receipt)
                                <span className="text-gray-400 font-normal ml-1">(optional)</span>
                            </label>
                            <select className="sh-select" {...register('GrnID')}>
                                <option value="">— Select GRN —</option>
                                {grns.map(g => (
                                    <option key={g.grnID} value={g.grnID}>
                                        GRN-{g.grnID} — PO-{g.poID} ({g.status})
                                    </option>
                                ))}
                            </select>
                        </div>

                        {/* Match Result */}
                        <div>
                            <label className="sh-label">Match Result *</label>
                            <div className="grid grid-cols-2 gap-2">
                                {MATCH_RESULTS.map(r => (
                                    <label key={r}
                                        className="flex items-center gap-2 p-2 border rounded-lg cursor-pointer hover:bg-gray-50">
                                        <input type="radio" value={r}
                                            {...register('Result', { required: true })} />
                                        <span className={statusColor(r)}>{r}</span>
                                    </label>
                                ))}
                            </div>
                        </div>

                        <div>
                            <label className="sh-label">Notes</label>
                            <input
                                className="sh-input"
                                placeholder="Describe any discrepancies found…"
                                {...register('Notes')}
                            />
                        </div>

                        <div>
                            <label className="sh-label">Status</label>
                            <select className="sh-select" {...register('Status')}>
                                {MATCH_REF_STATUSES.map(s => <option key={s}>{s}</option>)}
                            </select>
                        </div>
                    </form>
                </Modal>
            )}
        </div>
    )
}