import { useState } from 'react'
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query'
import { useNavigate } from 'react-router-dom'
import { useForm } from 'react-hook-form'
import toast from 'react-hot-toast'
import { purchaseOrdersApi } from '../../api/procurement.api'
import { suppliersApi } from '../../api/suppliers.api'
import { StatusPill } from '../../components/ui/StatusPill'
import { PageHeader, Spinner, EmptyState, ConfirmDialog } from '../../components/ui/index'
import Modal from '../../components/ui/Modal'
import useAuthStore from '../../store/auth.store'

// From PurchaseOrderStatus enum
const PO_STATUSES = [
    'Open',
    'Acknowledged',
    'Partially_Shipped',
    'Shipped',
    'Closed',
    'Cancelled',
]

export default function PurchaseOrdersPage() {
    const navigate = useNavigate()
    const qc = useQueryClient()
    const { user } = useAuthStore()

    const isAdmin = user?.roles?.includes('Admin')
    const isCategoryManager = user?.roles?.includes('CategoryManager')
    const isBuyer = user?.roles?.includes('Buyer')
    const canCreate = isAdmin || isCategoryManager || isBuyer
    const canDelete = isAdmin

    const [modal, setModal] = useState(null)   // 'form' | 'delete'
    const [selected, setSelected] = useState(null)
    const [search, setSearch] = useState('')
    const [loadingEdit, setLoadingEdit] = useState(false)

    // ── Fetch all POs ──────────────────────────────────────
    const { data, isLoading } = useQuery({
        queryKey: ['pos'],
        queryFn: purchaseOrdersApi.getAll,
    })
    const rows = (data?.data ?? data ?? []).filter(p =>
        String(p.poID ?? '').includes(search) ||
        (p.status ?? '').toLowerCase().includes(search.toLowerCase())
    )

    // ── Fetch suppliers for dropdown ───────────────────────
    const { data: suppData } = useQuery({
        queryKey: ['suppliers'],
        queryFn: suppliersApi.getAll,
    })
    const suppliers = suppData?.data ?? suppData ?? []

    // ── Form ───────────────────────────────────────────────
    const { register, handleSubmit, reset, formState: { errors } } = useForm()

    // ── Mutations ──────────────────────────────────────────
    const createMut = useMutation({
        mutationFn: purchaseOrdersApi.create,
        onSuccess: (res) => {
            qc.invalidateQueries(['pos'])
            setModal(null)
            const id = res?.poID ?? res?.data?.poID
            toast.success(`PO #${id} created successfully`)
        },
        onError: e => toast.error(e.response?.data?.message ?? 'Failed to create PO'),
    })

    const updateMut = useMutation({
        mutationFn: ({ id, dto }) => purchaseOrdersApi.update(id, dto),
        onSuccess: () => {
            qc.invalidateQueries(['pos'])
            setModal(null)
            toast.success('Purchase order updated successfully')
        },
        onError: e => toast.error(e.response?.data?.message ?? 'Failed to update PO'),
    })

    const deleteMut = useMutation({
        mutationFn: (id) => purchaseOrdersApi.delete(id),
        onSuccess: () => {
            qc.invalidateQueries(['pos'])
            setModal(null)
            toast.success('Purchase order deleted successfully')
        },
        onError: e => toast.error(e.response?.data?.message ?? 'Failed to delete PO'),
    })

    // ── Handlers ───────────────────────────────────────────
    const openCreate = () => {
        reset({
            OrgID: '',
            SupplierID: '',
            PoDate: '',
            Currency: 'INR',
            Incoterms: '',
            PaymentTerms: '',
            Status: 'Open',
        })
        setSelected(null)
        setModal('form')
    }

    const openEdit = async (row) => {
        setLoadingEdit(true)
        try {
            const res = await purchaseOrdersApi.getById(row.poID)
            const full = res?.data ?? res
            setSelected(full)
            reset({
                OrgID: full.orgID ?? '',
                SupplierID: full.supplierID ?? '',
                PoDate: full.poDate?.split('T')[0] ?? '',
                Currency: full.currency ?? 'INR',
                Incoterms: full.incoterms ?? '',
                PaymentTerms: full.paymentTerms ?? '',
                Status: full.status ?? 'Open',
            })
            setModal('form')
        } catch {
            toast.error('Failed to load PO details')
        } finally {
            setLoadingEdit(false)
        }
    }

    const onSubmit = (d) => {
        const payload = {
            OrgID: Number(d.OrgID),
            SupplierID: Number(d.SupplierID),
            PoDate: d.PoDate || null,
            Currency: d.Currency || null,
            Incoterms: d.Incoterms || null,
            PaymentTerms: d.PaymentTerms || null,
            Status: d.Status,
        }
        if (selected) {
            updateMut.mutate({
                id: selected.poID,
                dto: { ...payload, PoID: selected.poID },
            })
        } else {
            createMut.mutate(payload)
        }
    }

    const isPending = createMut.isPending || updateMut.isPending

    const getSupplierName = (id) =>
        suppliers.find(s => s.supplierID === id)?.legalName ?? `Supplier #${id}`

    const statusColor = (s) => {
        const m = {
            Open: 'pill-blue',
            Acknowledged: 'pill-teal',
            Partially_Shipped: 'pill-amber',
            Shipped: 'pill-purple',
            Closed: 'pill-green',
            Cancelled: 'pill-red',
        }
        return `pill ${m[s] ?? 'pill-gray'}`
    }

    // ── Render ─────────────────────────────────────────────
    return (
        <div>
            <PageHeader
                title="Purchase Orders"
                subtitle="Create and manage purchase orders with supplier acknowledgements"
                action={
                    canCreate && (
                        <button className="btn btn-primary" onClick={openCreate}>
                            + New PO
                        </button>
                    )
                }
            />

            {/* Search */}
            <div className="mb-4">
                <input
                    className="sh-input max-w-xs"
                    placeholder="Search by PO ID or status…"
                    value={search}
                    onChange={e => setSearch(e.target.value)}
                />
            </div>

            {/* Table */}
            <div className="sh-card p-0 overflow-hidden">
                {isLoading ? (
                    <div className="flex justify-center py-12"><Spinner /></div>
                ) : rows.length === 0 ? (
                    <EmptyState message="No purchase orders found." />
                ) : (
                    <table className="sh-table">
                        <thead>
                            <tr>
                                <th>Sr. No.</th>
                                <th>PO ID</th>
                                <th>Supplier</th>
                                <th>Org ID</th>
                                <th>PO Date</th>
                                <th>Currency</th>
                                <th>Payment Terms</th>
                                <th>Status</th>
                                <th>Actions</th>
                            </tr>
                        </thead>
                        <tbody>
                            {rows.map((po, index) => (
                                <tr key={po.poID}>
                                    <td className="text-gray-400 text-xs">{index + 1}</td>
                                    <td
                                        className="font-medium text-blue-700 cursor-pointer hover:underline"
                                        onClick={() => navigate(`/purchase-orders/${po.poID}`)}
                                    >
                                        PO-{po.poID}
                                    </td>
                                    <td className="font-medium">{getSupplierName(po.supplierID)}</td>
                                    <td className="text-gray-500">{po.orgID}</td>
                                    <td className="text-xs text-gray-500">
                                        {po.poDate ? new Date(po.poDate).toLocaleDateString() : '—'}
                                    </td>
                                    <td>{po.currency ?? '—'}</td>
                                    <td className="text-xs text-gray-500 max-w-xs truncate">
                                        {po.paymentTerms ?? '—'}
                                    </td>
                                    <td>
                                        <span className={statusColor(po.status)}>{po.status}</span>
                                    </td>
                                    <td>
                                        <div className="flex gap-1">
                                            {/* View detail */}
                                            <button
                                                className="btn btn-secondary btn-sm"
                                                onClick={() => navigate(`/purchase-orders/${po.poID}`)}
                                            >
                                                View
                                            </button>

                                            {/* Edit */}
                                            <button
                                                className="btn btn-ghost btn-sm"
                                                onClick={() => openEdit(po)}
                                                disabled={loadingEdit}
                                            >
                                                {loadingEdit ? '…' : 'Edit'}
                                            </button>

                                            {/* Delete — Admin only */}
                                            {canDelete && (
                                                <button
                                                    className="btn btn-danger btn-sm"
                                                    onClick={() => { setSelected(po); setModal('delete') }}
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

            {/* ── Create / Edit Modal ──────────────────────────── */}
            {modal === 'form' && (
                <Modal
                    title={selected ? `Edit PO-${selected.poID}` : 'New Purchase Order'}
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
                                {isPending ? 'Saving…' : selected ? 'Update' : 'Create PO'}
                            </button>
                        </>
                    }
                >
                    <form className="space-y-3" noValidate>
                        {/* Supplier */}
                        <div>
                            <label className="sh-label">Supplier *</label>
                            <select
                                className="sh-select"
                                {...register('SupplierID', { required: 'Supplier is required' })}
                            >
                                <option value="">— Select a supplier —</option>
                                {suppliers.map(s => (
                                    <option key={s.supplierID} value={s.supplierID}>{s.legalName}</option>
                                ))}
                            </select>
                            {errors.SupplierID && (
                                <p className="text-red-500 text-xs mt-1">{errors.SupplierID.message}</p>
                            )}
                        </div>

                        {/* Org ID */}
                        <div>
                            <label className="sh-label">Organization ID *</label>
                            <input
                                type="number"
                                className="sh-input"
                                {...register('OrgID', { required: 'Organization ID is required' })}
                            />
                            {errors.OrgID && (
                                <p className="text-red-500 text-xs mt-1">{errors.OrgID.message}</p>
                            )}
                        </div>

                        {/* PO Date */}
                        <div>
                            <label className="sh-label">PO Date</label>
                            <input type="date" className="sh-input" {...register('PoDate')} />
                        </div>

                        {/* Currency + Incoterms */}
                        <div className="grid grid-cols-2 gap-3">
                            <div>
                                <label className="sh-label">Currency</label>
                                <select className="sh-select" {...register('Currency')}>
                                    <option>INR</option>
                                    <option>USD</option>
                                    <option>EUR</option>
                                    <option>GBP</option>
                                </select>
                            </div>
                            <div>
                                <label className="sh-label">Incoterms</label>
                                <select className="sh-select" {...register('Incoterms')}>
                                    <option value="">— Select —</option>
                                    <option>EXW</option>
                                    <option>FOB</option>
                                    <option>CIF</option>
                                    <option>DDP</option>
                                    <option>DAP</option>
                                    <option>FCA</option>
                                </select>
                            </div>
                        </div>

                        {/* Payment Terms */}
                        <div>
                            <label className="sh-label">Payment Terms</label>
                            <input
                                className="sh-input"
                                placeholder="e.g. Net 30, 50% advance + 50% on delivery"
                                {...register('PaymentTerms')}
                            />
                        </div>

                        {/* Status */}
                        <div>
                            <label className="sh-label">Status *</label>
                            <select
                                className="sh-select"
                                {...register('Status', { required: true })}
                            >
                                {PO_STATUSES.map(s => <option key={s}>{s}</option>)}
                            </select>
                        </div>
                    </form>
                </Modal>
            )}

            {/* ── Delete Confirm ───────────────────────────────── */}
            {modal === 'delete' && (
                <ConfirmDialog
                    message={`Delete PO-${selected?.poID}? This cannot be undone. All linked lines, acknowledgements and revisions will also be removed.`}
                    onConfirm={() => deleteMut.mutate(selected.poID)}
                    onCancel={() => setModal(null)}
                    loading={deleteMut.isPending}
                />
            )}
        </div>
    )
}