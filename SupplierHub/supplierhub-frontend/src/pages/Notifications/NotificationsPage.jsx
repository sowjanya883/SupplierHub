import { useState } from 'react'
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query'
import { useNavigate } from 'react-router-dom'
import toast from 'react-hot-toast'
import {
    Bell, BellOff, Check, CheckCheck, Trash2,
    ShoppingCart, Receipt, AlertTriangle, ShieldCheck,
    PackageCheck, Truck, ClipboardList, Users, Megaphone,
    Info, Monitor,
} from 'lucide-react'
import { notificationsApi } from '../../api/operations.api'
import { PageHeader, Spinner } from '../../components/ui/index'
import useAuthStore from '../../store/auth.store'

// ── Category config ────────────────────────────────────
const CATEGORY_CONFIG = {
    System: { icon: Monitor, color: '#6366F1', bg: '#EEF2FF', label: 'System' },
    Supplier: { icon: Users, color: '#7C3AED', bg: '#F5F3FF', label: 'Supplier' },
    RFx: { icon: Megaphone, color: '#D97706', bg: '#FFFBEB', label: 'RFx' },
    Requisition: { icon: ClipboardList, color: '#2563EB', bg: '#EFF6FF', label: 'Requisition' },
    PurchaseOrder: { icon: ShoppingCart, color: '#0D9488', bg: '#F0FDFA', label: 'Purchase Order' },
    Invoice: { icon: Receipt, color: '#EA580C', bg: '#FFF7ED', label: 'Invoice' },
    NCR: { icon: AlertTriangle, color: '#DC2626', bg: '#FEF2F2', label: 'NCR' },
    GRN: { icon: PackageCheck, color: '#16A34A', bg: '#F0FDF4', label: 'GRN' },
    Shipment: { icon: Truck, color: '#0891B2', bg: '#ECFEFF', label: 'Shipment' },
    Compliance: { icon: ShieldCheck, color: '#B45309', bg: '#FFFBEB', label: 'Compliance' },
}

const getCat = (cat) =>
    CATEGORY_CONFIG[cat] ?? { icon: Info, color: '#64748B', bg: '#F8FAFC', label: cat ?? 'General' }

// ── Role → which categories they actually receive ──────
const ROLE_CATEGORIES = {
    Admin: ['System', 'Supplier', 'RFx', 'Requisition', 'PurchaseOrder', 'Invoice', 'NCR', 'GRN', 'Shipment', 'Compliance'],
    Buyer: ['Supplier', 'RFx', 'PurchaseOrder', 'NCR', 'GRN'],
    SupplierUser: ['RFx', 'PurchaseOrder', 'Invoice', 'Compliance'],
    CategoryManager: ['Supplier', 'RFx'],
    AccountsPayable: ['Invoice'],
    ComplianceOfficer: ['Compliance'],
    ReceivingUser: ['GRN', 'NCR', 'Shipment'],
}

// ── Navigate path per category ─────────────────────────
const getNavPath = (category, refEntityID) => {
    const map = {
        System: null,
        Supplier: '/suppliers',
        RFx: '/rfx',
        Requisition: '/requisitions',
        PurchaseOrder: refEntityID ? `/purchase-orders/${refEntityID}` : '/purchase-orders',
        Invoice: refEntityID ? `/invoices/${refEntityID}` : '/invoices',
        NCR: '/ncr',
        GRN: refEntityID ? `/grn/${refEntityID}` : '/grn',
        Shipment: '/shipping',
        Compliance: '/compliance-docs',
    }
    return map[category] ?? null
}

// ── Relative time ──────────────────────────────────────
// Backend returns UTC without 'Z' — append it so JS parses correctly
const toUtc = (date) =>
    date.endsWith('Z') || date.includes('+') ? date : date + 'Z'

const relativeTime = (date) => {
    if (!date) return ''
    const diff = Date.now() - new Date(toUtc(date)).getTime()
    const mins = Math.floor(diff / 60000)
    if (mins < 1) return 'just now'
    if (mins < 60) return `${mins}m ago`
    const hrs = Math.floor(mins / 60)
    if (hrs < 24) return `${hrs}h ago`
    const days = Math.floor(hrs / 24)
    if (days < 7) return `${days}d ago`
    return new Date(toUtc(date)).toLocaleDateString()
}

// ─────────────────────────────────────────────────────────
export default function NotificationsPage() {
    const qc = useQueryClient()
    const navigate = useNavigate()
    const { user } = useAuthStore()

    const userRoles = user?.roles ?? []

    // ── Case-insensitive role matching ─────────────────────
    // JWT roles may come as 'SupplierUser', 'supplieruser', etc.
    const findRoleKey = (roles) => {
        if (!roles?.length) return null
        for (const role of roles) {
            const key = Object.keys(ROLE_CATEGORIES).find(
                k => k.toLowerCase() === role.toLowerCase()
            )
            if (key) return key
        }
        return null
    }

    const isAdmin = userRoles.some(r => r.toLowerCase() === 'admin')
    const matchedKey = findRoleKey(userRoles)
    const primaryRole = matchedKey ?? userRoles[0] ?? 'Buyer'

    // ── Figure out which categories this role sees ─────────
    const allowedCats = isAdmin
        ? ROLE_CATEGORIES['Admin']
        : (matchedKey ? ROLE_CATEGORIES[matchedKey] : Object.keys(CATEGORY_CONFIG))

    const [categoryFilter, setCategoryFilter] = useState('All')
    const [statusFilter, setStatusFilter] = useState('All')

    // ── Fetch ──────────────────────────────────────────────
    const { data: raw, isLoading } = useQuery({
        queryKey: ['notifications'],
        queryFn: notificationsApi.getAll,
        refetchInterval: 30_000,
    })

    const allNotifs = raw?.data ?? raw ?? []

    // Filter: only show categories relevant to this role
    const filtered = allNotifs
        .filter(n => allowedCats.includes(n.category) || !n.category)
        .filter(n => categoryFilter === 'All' || n.category === categoryFilter)
        .filter(n => statusFilter === 'All' || n.status === statusFilter)

    const unreadCount = allNotifs
        .filter(n => allowedCats.includes(n.category) || !n.category)
        .filter(n => n.status === 'Unread').length

    // ── Mutations ──────────────────────────────────────────
    const markReadMut = useMutation({
        mutationFn: (id) => notificationsApi.updateStatus(id, { Status: 'Read' }),
        onSuccess: () => qc.invalidateQueries(['notifications']),
        onError: (e) => {
            console.error('Mark read failed:', e)
            toast.error('Failed to mark as read')
        },
    })

    const markAllMut = useMutation({
        mutationFn: notificationsApi.markAllRead,
        onSuccess: () => {
            qc.invalidateQueries(['notifications'])
            toast.success('All notifications marked as read')
        },
        onError: () => toast.error('Failed to mark all as read'),
    })

    const deleteMut = useMutation({
        mutationFn: (id) => notificationsApi.delete(id),
        onSuccess: () => {
            qc.invalidateQueries(['notifications'])
            toast.success('Notification dismissed')
        },
    })

    // ── Click a notification ────────────────────────────────
    const handleClick = (notif) => {
        if (notif.status === 'Unread') markReadMut.mutate(notif.notificationID)
        const path = getNavPath(notif.category, notif.refEntityID)
        if (path) navigate(path)
    }

    // ── Role subtitle ───────────────────────────────────────
    const roleSubtitle = {
        Admin: 'You see all system notifications',
        Buyer: 'Supplier updates, POs, RFx events, NCRs and GRN alerts',
        SupplierUser: 'Your POs, RFx invites, invoice status and compliance alerts',
        CategoryManager: 'Supplier additions and RFx sourcing events',
        AccountsPayable: 'Invoice submissions and status changes',
        ComplianceOfficer: 'Compliance document expiry and renewal alerts',
        ReceivingUser: 'GRN receipts, inspections and NCR quality alerts',
    }[primaryRole] ?? 'Your notifications'

    // ─────────────────────────────────────────────────────────
    return (
        <div>
            <PageHeader
                title="Notifications"
                subtitle={
                    unreadCount > 0
                        ? `${unreadCount} unread · ${roleSubtitle}`
                        : roleSubtitle
                }
                action={
                    unreadCount > 0 && (
                        <button
                            className="btn btn-secondary"
                            onClick={() => markAllMut.mutate()}
                            disabled={markAllMut.isPending}
                            style={{ display: 'flex', alignItems: 'center', gap: 6 }}
                        >
                            <CheckCheck size={14} />
                            {markAllMut.isPending ? 'Marking…' : 'Mark all as read'}
                        </button>
                    )
                }
            />

            {/* ── Filter bar ────────────────────────────────── */}
            <div style={{
                background: '#fff',
                border: '1px solid #E8ECF4',
                borderRadius: 12,
                padding: '12px 16px',
                marginBottom: 16,
                display: 'flex',
                alignItems: 'center',
                gap: 12,
                flexWrap: 'wrap',
            }}>
                {/* Status toggle */}
                <div style={{ display: 'flex', gap: 4 }}>
                    {['All', 'Unread', 'Read'].map(s => (
                        <button
                            key={s}
                            onClick={() => setStatusFilter(s)}
                            style={{
                                fontSize: 12, fontWeight: 500,
                                padding: '5px 12px', borderRadius: 20,
                                border: 'none', cursor: 'pointer',
                                background: statusFilter === s ? '#4F46E5' : '#F1F5F9',
                                color: statusFilter === s ? '#fff' : '#64748B',
                                transition: 'all .15s',
                            }}
                        >
                            {s}
                            {s === 'Unread' && unreadCount > 0 && (
                                <span style={{
                                    marginLeft: 5,
                                    background: statusFilter === 'Unread'
                                        ? 'rgba(255,255,255,0.3)' : '#4F46E5',
                                    color: '#fff',
                                    fontSize: 9, fontWeight: 700,
                                    padding: '1px 5px', borderRadius: 20,
                                }}>
                                    {unreadCount}
                                </span>
                            )}
                        </button>
                    ))}
                </div>

                <div style={{ width: 1, height: 20, background: '#E2E8F0' }} />

                {/* Category filters — only show what this role receives */}
                <div style={{ display: 'flex', gap: 4, flexWrap: 'wrap' }}>
                    {/* All button */}
                    <button
                        onClick={() => setCategoryFilter('All')}
                        style={{
                            fontSize: 11.5, fontWeight: 500,
                            padding: '4px 10px', borderRadius: 20,
                            border: 'none', cursor: 'pointer',
                            background: categoryFilter === 'All' ? '#EEF2FF' : '#F8FAFC',
                            color: categoryFilter === 'All' ? '#4338CA' : '#94A3B8',
                            transition: 'all .15s',
                        }}
                    >
                        All
                    </button>

                    {/* Role-specific category buttons */}
                    {allowedCats.map(cat => {
                        const cfg = getCat(cat)
                        const isActive = categoryFilter === cat
                        return (
                            <button
                                key={cat}
                                onClick={() => setCategoryFilter(cat)}
                                style={{
                                    fontSize: 11.5, fontWeight: 500,
                                    padding: '4px 10px', borderRadius: 20,
                                    border: 'none', cursor: 'pointer',
                                    background: isActive ? cfg.bg : '#F8FAFC',
                                    color: isActive ? cfg.color : '#94A3B8',
                                    transition: 'all .15s',
                                }}
                            >
                                {cfg.label}
                            </button>
                        )
                    })}
                </div>

                <span style={{ marginLeft: 'auto', fontSize: 11.5, color: '#94A3B8' }}>
                    {filtered.length} of {allNotifs.length}
                </span>
            </div>

            {/* ── Role info banner ──────────────────────────── */}
            {!isAdmin && (
                <div style={{
                    background: '#F8FAFC',
                    border: '1px solid #E2E8F0',
                    borderRadius: 10,
                    padding: '9px 14px',
                    marginBottom: 14,
                    display: 'flex',
                    alignItems: 'center',
                    gap: 8,
                }}>
                    <div style={{ display: 'flex', gap: 6 }}>
                        {allowedCats.map(cat => {
                            const cfg = getCat(cat)
                            const Icon = cfg.icon
                            return (
                                <div key={cat} title={cfg.label} style={{
                                    width: 24, height: 24, borderRadius: 6,
                                    background: cfg.bg,
                                    display: 'flex', alignItems: 'center',
                                    justifyContent: 'center',
                                }}>
                                    <Icon size={12} color={cfg.color} strokeWidth={2} />
                                </div>
                            )
                        })}
                    </div>
                    <p style={{ fontSize: 11.5, color: '#64748B' }}>
                        As <strong>{primaryRole}</strong> you receive notifications for:{' '}
                        {allowedCats.join(', ')}.
                    </p>
                </div>
            )}

            {/* ── Notifications list ────────────────────────── */}
            <div style={{
                background: '#fff',
                border: '1px solid #E8ECF4',
                borderRadius: 12,
                overflow: 'hidden',
            }}>
                {isLoading ? (
                    <div className="flex justify-center py-12"><Spinner /></div>
                ) : filtered.length === 0 ? (
                    <div style={{ padding: '48px 24px', textAlign: 'center' }}>
                        <BellOff size={32} color="#CBD5E1"
                            style={{ margin: '0 auto 12px' }} />
                        <p style={{ fontSize: 14, color: '#94A3B8', marginBottom: 4 }}>
                            {allNotifs.length === 0
                                ? 'No notifications yet'
                                : 'No notifications match your filters'}
                        </p>
                        {(categoryFilter !== 'All' || statusFilter !== 'All') && (
                            <button
                                onClick={() => {
                                    setCategoryFilter('All')
                                    setStatusFilter('All')
                                }}
                                style={{
                                    fontSize: 12, color: '#4F46E5',
                                    background: 'none', border: 'none',
                                    cursor: 'pointer', marginTop: 8,
                                }}
                            >
                                Clear filters
                            </button>
                        )}
                    </div>
                ) : (
                    <div>
                        {filtered.map((notif, index) => {
                            const cat = getCat(notif.category)
                            const Icon = cat.icon
                            const isUnread = notif.status === 'Unread'
                            const hasPath = !!getNavPath(notif.category, notif.refEntityID)

                            return (
                                <div
                                    key={notif.notificationID}
                                    style={{
                                        display: 'flex',
                                        alignItems: 'flex-start',
                                        gap: 14,
                                        padding: '14px 18px',
                                        borderBottom: index < filtered.length - 1
                                            ? '1px solid #F1F5F9' : 'none',
                                        background: isUnread ? `${cat.bg}66` : '#fff',
                                        cursor: hasPath ? 'pointer' : 'default',
                                        transition: 'background .1s',
                                    }}
                                    onMouseEnter={e => {
                                        if (hasPath)
                                            e.currentTarget.style.background = '#F8FAFC'
                                    }}
                                    onMouseLeave={e => {
                                        e.currentTarget.style.background =
                                            isUnread ? `${cat.bg}66` : '#fff'
                                    }}
                                    onClick={() => handleClick(notif)}
                                >
                                    {/* Category icon */}
                                    <div style={{
                                        width: 36, height: 36, borderRadius: 10,
                                        background: cat.bg,
                                        display: 'flex', alignItems: 'center',
                                        justifyContent: 'center', flexShrink: 0,
                                        marginTop: 2,
                                    }}>
                                        <Icon size={16} color={cat.color} strokeWidth={2} />
                                    </div>

                                    {/* Message */}
                                    <div style={{ flex: 1, minWidth: 0 }}>
                                        <div style={{ display: 'flex', alignItems: 'flex-start', gap: 8 }}>
                                            <p style={{
                                                fontSize: 13.5,
                                                color: isUnread ? '#0F172A' : '#475569',
                                                fontWeight: isUnread ? 500 : 400,
                                                lineHeight: 1.5, flex: 1,
                                            }}>
                                                {notif.message}
                                            </p>
                                            {isUnread && (
                                                <span style={{
                                                    width: 7, height: 7, borderRadius: '50%',
                                                    background: cat.color,
                                                    flexShrink: 0, marginTop: 6,
                                                }} />
                                            )}
                                        </div>
                                        <div style={{
                                            display: 'flex', alignItems: 'center',
                                            gap: 8, marginTop: 5,
                                        }}>
                                            <span style={{
                                                fontSize: 10.5,
                                                background: cat.bg, color: cat.color,
                                                padding: '1.5px 7px', borderRadius: 20,
                                                fontWeight: 600,
                                            }}>
                                                {cat.label}
                                            </span>
                                            <span style={{ fontSize: 11, color: '#94A3B8' }}>
                                                {relativeTime(notif.createdOn)}
                                            </span>
                                            {notif.refEntityID && (
                                                <span style={{ fontSize: 11, color: '#B0BAC9' }}>
                                                    ref #{notif.refEntityID}
                                                </span>
                                            )}
                                        </div>
                                    </div>

                                    {/* Action buttons */}
                                    <div
                                        style={{ display: 'flex', gap: 4, flexShrink: 0 }}
                                        onClick={e => e.stopPropagation()}
                                    >
                                        {isUnread && (
                                            <button
                                                title="Mark as read"
                                                onClick={() => markReadMut.mutate(notif.notificationID)}
                                                style={{
                                                    width: 28, height: 28, borderRadius: 7,
                                                    background: 'rgba(99,102,241,0.08)',
                                                    border: 'none', cursor: 'pointer',
                                                    display: 'flex', alignItems: 'center',
                                                    justifyContent: 'center',
                                                }}
                                            >
                                                <Check size={13} color="#6366F1" strokeWidth={2.5} />
                                            </button>
                                        )}
                                        <button
                                            title="Dismiss"
                                            onClick={() => deleteMut.mutate(notif.notificationID)}
                                            style={{
                                                width: 28, height: 28, borderRadius: 7,
                                                background: 'rgba(239,68,68,0.07)',
                                                border: 'none', cursor: 'pointer',
                                                display: 'flex', alignItems: 'center',
                                                justifyContent: 'center',
                                            }}
                                        >
                                            <Trash2 size={12} color="#EF4444" strokeWidth={2} />
                                        </button>
                                    </div>
                                </div>
                            )
                        })}
                    </div>
                )}
            </div>

            <p style={{
                fontSize: 11.5, color: '#94A3B8',
                marginTop: 10, textAlign: 'center',
            }}>
                Showing notifications for your account only · Auto-refreshes every 30s
            </p>
        </div>
    )
}