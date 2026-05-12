import { useState, useRef, useEffect } from 'react'
import { useNavigate, useLocation } from 'react-router-dom'
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query'
import { Bell, Check, CheckCheck, ExternalLink, LogOut } from 'lucide-react'
import { notificationsApi } from '../api/operations.api'
import useAuthStore from '../store/auth.store'

const ROUTE_LABELS = {
    '/dashboard': 'Dashboard',
    '/organizations': 'Organizations',
    '/suppliers': 'Suppliers',
    '/categories': 'Categories',
    '/items': 'Items',
    '/catalogs': 'Catalogs',
    '/contracts': 'Contracts',
    '/compliance-docs': 'Compliance Documents',
    '/rfx': 'RFx Events',
    '/requisitions': 'Requisitions (PR)',
    '/purchase-orders': 'Purchase Orders',
    '/shipping': 'Shipments & ASN',
    '/grn': 'GRN & Receiving',
    '/invoices': 'Invoices',
    '/ncr': 'NCR / Quality',
    '/performance': 'Supplier Performance',
    '/notifications': 'Notifications',
    '/users': 'Users',
    '/admin': 'Admin Configuration',
    '/audit-logs': 'Audit Logs',
}

const CAT_COLORS = {
    System: '#6366F1',
    Supplier: '#7C3AED',
    RFx: '#D97706',
    Requisition: '#2563EB',
    PurchaseOrder: '#0D9488',
    Invoice: '#EA580C',
    NCR: '#DC2626',
    GRN: '#16A34A',
    Shipment: '#0891B2',
    Compliance: '#B45309',
}

// Role badge colour
const ROLE_COLORS = {
    Admin: { bg: '#EEF2FF', color: '#4338CA' },
    Buyer: { bg: '#F0FDF4', color: '#15803D' },
    SupplierUser: { bg: '#F0FDFA', color: '#0F766E' },
    CategoryManager: { bg: '#FFF7ED', color: '#C2410C' },
    AccountsPayable: { bg: '#FFFBEB', color: '#92400E' },
    ComplianceOfficer: { bg: '#F5F3FF', color: '#6D28D9' },
    ReceivingUser: { bg: '#EFF6FF', color: '#1D4ED8' },
}

const toUtc = (d) =>
    d?.endsWith('Z') || d?.includes('+') ? d : (d ? d + 'Z' : d)

const relTime = (date) => {
    if (!date) return ''
    const diff = Date.now() - new Date(toUtc(date)).getTime()
    const mins = Math.floor(diff / 60000)
    if (mins < 1) return 'just now'
    if (mins < 60) return `${mins}m ago`
    const hrs = Math.floor(mins / 60)
    if (hrs < 24) return `${hrs}h ago`
    return `${Math.floor(hrs / 24)}d ago`
}

export default function Topbar() {
    const location = useLocation()
    const navigate = useNavigate()
    const qc = useQueryClient()
    const { user, logout } = useAuthStore()
    const [bellOpen, setBellOpen] = useState(false)
    const bellRef = useRef(null)

    const base = '/' + location.pathname.split('/')[1]
    const title = ROUTE_LABELS[base] ?? 'SupplierHub'

    // User display
    const initials = user?.name
        ? user.name.split(' ').map(w => w[0]).join('').slice(0, 2).toUpperCase()
        : 'U'
    const primaryRole = user?.roles?.[0] ?? ''
    const roleStyle = ROLE_COLORS[primaryRole] ?? { bg: '#F1F5F9', color: '#475569' }

    // ── Notifications ──────────────────────────────────
    const { data: raw } = useQuery({
        queryKey: ['notifications'],
        queryFn: notificationsApi.getAll,
        refetchInterval: 15_000,
    })

    const allNotifs = raw?.data ?? raw ?? []
    const unread = allNotifs.filter(n => n.status === 'Unread')
    const unreadCount = unread.length
    const preview = [...unread]
        .sort((a, b) => new Date(toUtc(b.createdOn)) - new Date(toUtc(a.createdOn)))
        .slice(0, 5)

    // ── Mark read mutations ────────────────────────────
    const markMut = useMutation({
        mutationFn: (id) => notificationsApi.updateStatus(id, { Status: 'Read' }),
        onSuccess: () => qc.invalidateQueries(['notifications']),
    })
    const markAllMut = useMutation({
        mutationFn: notificationsApi.markAllRead,
        onSuccess: () => { qc.invalidateQueries(['notifications']); setBellOpen(false) },
    })

    // ── Close bell on outside click ────────────────────
    useEffect(() => {
        const handler = (e) => {
            if (bellRef.current && !bellRef.current.contains(e.target))
                setBellOpen(false)
        }
        document.addEventListener('mousedown', handler)
        return () => document.removeEventListener('mousedown', handler)
    }, [])

    // ─────────────────────────────────────────────────
    return (
        <header
            style={{ height: 'var(--topbar-h)' }}
            className="bg-white border-b border-gray-200 flex items-center px-5 gap-3 flex-shrink-0 z-10"
        >
            {/* ── Page title ────────────────────────────── */}
            <div>
                <div className="font-semibold text-[15px] text-gray-900 leading-tight">
                    {title}
                </div>
            </div>

            {/* ── Right section ─────────────────────────── */}
            <div className="ml-auto flex items-center gap-3">

                {/* ── Bell ────────────────────────────────── */}
                <div ref={bellRef} style={{ position: 'relative' }}>
                    <button
                        onClick={() => setBellOpen(o => !o)}
                        style={{
                            width: 34, height: 34, borderRadius: 9,
                            border: bellOpen ? '1.5px solid #6366F1' : '1px solid #E2E8F0',
                            background: bellOpen ? '#EEF2FF' : '#fff',
                            display: 'flex', alignItems: 'center',
                            justifyContent: 'center', cursor: 'pointer',
                            position: 'relative', transition: 'all .15s',
                        }}
                    >
                        <Bell size={15}
                            color={bellOpen ? '#6366F1' : '#6B7280'}
                            strokeWidth={2} />
                        {unreadCount > 0 && (
                            <span style={{
                                position: 'absolute', top: -5, right: -5,
                                minWidth: 16, height: 16,
                                background: '#EF4444', color: '#fff',
                                fontSize: 9, fontWeight: 700,
                                borderRadius: 20, padding: '0 4px',
                                display: 'flex', alignItems: 'center',
                                justifyContent: 'center',
                                border: '1.5px solid #fff',
                            }}>
                                {unreadCount > 99 ? '99+' : unreadCount}
                            </span>
                        )}
                    </button>

                    {/* Bell dropdown */}
                    {bellOpen && (
                        <div style={{
                            position: 'absolute', top: 42, right: 0,
                            width: 340,
                            background: '#fff',
                            border: '1px solid #E8ECF4',
                            borderRadius: 14,
                            boxShadow: '0 8px 32px rgba(0,0,0,0.12)',
                            zIndex: 100,
                            overflow: 'hidden',
                        }}>
                            {/* Dropdown header */}
                            <div style={{
                                padding: '12px 16px',
                                borderBottom: '1px solid #F1F5F9',
                                display: 'flex', alignItems: 'center',
                                justifyContent: 'space-between',
                            }}>
                                <div>
                                    <p style={{ fontSize: 13.5, fontWeight: 600, color: '#0F172A' }}>
                                        Notifications
                                    </p>
                                    <p style={{ fontSize: 11, color: '#94A3B8', marginTop: 1 }}>
                                        {unreadCount > 0
                                            ? `${unreadCount} unread`
                                            : 'All caught up'}
                                    </p>
                                </div>
                                {unreadCount > 0 && (
                                    <button
                                        onClick={() => markAllMut.mutate()}
                                        disabled={markAllMut.isPending}
                                        style={{
                                            fontSize: 11.5, fontWeight: 500,
                                            color: '#6366F1', background: '#EEF2FF',
                                            border: 'none', borderRadius: 7,
                                            padding: '5px 10px', cursor: 'pointer',
                                            display: 'flex', alignItems: 'center', gap: 4,
                                        }}
                                    >
                                        <CheckCheck size={12} />
                                        {markAllMut.isPending ? '…' : 'Mark all read'}
                                    </button>
                                )}
                            </div>

                            {/* Notification items */}
                            <div style={{ maxHeight: 320, overflowY: 'auto' }}>
                                {preview.length === 0 ? (
                                    <div style={{ padding: '24px 16px', textAlign: 'center' }}>
                                        <Bell size={22} color="#CBD5E1"
                                            style={{ margin: '0 auto 8px' }} />
                                        <p style={{ fontSize: 13, color: '#94A3B8' }}>
                                            No new notifications
                                        </p>
                                    </div>
                                ) : (
                                    preview.map((n, i) => {
                                        const color = CAT_COLORS[n.category] ?? '#64748B'
                                        return (
                                            <div key={n.notificationID} style={{
                                                display: 'flex', alignItems: 'flex-start',
                                                gap: 10, padding: '11px 16px',
                                                borderBottom: i < preview.length - 1
                                                    ? '1px solid #F8FAFC' : 'none',
                                                background: '#FAFBFF',
                                            }}>
                                                <div style={{
                                                    width: 7, height: 7, borderRadius: '50%',
                                                    background: color, flexShrink: 0, marginTop: 6,
                                                }} />
                                                <div style={{ flex: 1, minWidth: 0 }}>
                                                    <p style={{
                                                        fontSize: 12.5, color: '#1E293B',
                                                        fontWeight: 500, lineHeight: 1.45,
                                                    }}>
                                                        {n.message}
                                                    </p>
                                                    <div style={{
                                                        display: 'flex', alignItems: 'center',
                                                        gap: 6, marginTop: 4,
                                                    }}>
                                                        <span style={{
                                                            fontSize: 9.5, fontWeight: 600,
                                                            color, background: color + '18',
                                                            padding: '1px 6px', borderRadius: 20,
                                                        }}>
                                                            {n.category ?? 'General'}
                                                        </span>
                                                        <span style={{ fontSize: 10.5, color: '#94A3B8' }}>
                                                            {relTime(n.createdOn)}
                                                        </span>
                                                    </div>
                                                </div>
                                                <button
                                                    title="Mark as read"
                                                    onClick={() => markMut.mutate(n.notificationID)}
                                                    style={{
                                                        width: 24, height: 24, borderRadius: 6,
                                                        background: 'rgba(99,102,241,0.1)',
                                                        border: 'none', cursor: 'pointer',
                                                        display: 'flex', alignItems: 'center',
                                                        justifyContent: 'center', flexShrink: 0,
                                                    }}
                                                >
                                                    <Check size={11} color="#6366F1" strokeWidth={2.5} />
                                                </button>
                                            </div>
                                        )
                                    })
                                )}
                            </div>

                            {/* Dropdown footer */}
                            <div style={{
                                padding: '10px 16px',
                                borderTop: '1px solid #F1F5F9',
                                textAlign: 'center',
                            }}>
                                <button
                                    onClick={() => { setBellOpen(false); navigate('/notifications') }}
                                    style={{
                                        fontSize: 12, fontWeight: 500,
                                        color: '#6366F1', background: 'none',
                                        border: 'none', cursor: 'pointer',
                                        display: 'inline-flex', alignItems: 'center', gap: 4,
                                    }}
                                >
                                    View all notifications
                                    <ExternalLink size={11} />
                                </button>
                            </div>
                        </div>
                    )}
                </div>

                {/* ── Divider ───────────────────────────────── */}
                <div style={{
                    width: 1, height: 22,
                    background: '#E2E8F0',
                }} />

                {/* ── User profile ──────────────────────────── */}
                <div style={{
                    display: 'flex', alignItems: 'center', gap: 9,
                }}>
                    {/* Avatar */}
                    <div style={{
                        width: 30, height: 30, borderRadius: '50%',
                        background: '#EEF2FF',
                        border: '1.5px solid #C7D2FE',
                        color: '#4338CA',
                        fontSize: 11, fontWeight: 700,
                        display: 'flex', alignItems: 'center',
                        justifyContent: 'center', flexShrink: 0,
                    }}>
                        {initials}
                    </div>

                    {/* Name + role */}
                    <div style={{ lineHeight: 1.3 }}>
                        <p style={{
                            fontSize: 12.5, fontWeight: 600,
                            color: '#1E293B',
                            maxWidth: 120,
                            overflow: 'hidden',
                            textOverflow: 'ellipsis',
                            whiteSpace: 'nowrap',
                        }}>
                            {user?.name ?? 'User'}
                        </p>
                        <span style={{
                            fontSize: 10, fontWeight: 600,
                            background: roleStyle.bg,
                            color: roleStyle.color,
                            padding: '1px 6px',
                            borderRadius: 20,
                            display: 'inline-block',
                            marginTop: 1,
                        }}>
                            {primaryRole}
                        </span>
                    </div>

                    {/* Logout button */}
                    <button
                        onClick={() => { logout(); navigate('/login') }}
                        title="Log out"
                        style={{
                            width: 28, height: 28, borderRadius: 7,
                            background: '#F8FAFC',
                            border: '1px solid #E2E8F0',
                            display: 'flex', alignItems: 'center',
                            justifyContent: 'center', cursor: 'pointer',
                            transition: 'all .15s', flexShrink: 0,
                        }}
                        onMouseEnter={e => {
                            e.currentTarget.style.background = '#FEF2F2'
                            e.currentTarget.style.borderColor = '#FECACA'
                        }}
                        onMouseLeave={e => {
                            e.currentTarget.style.background = '#F8FAFC'
                            e.currentTarget.style.borderColor = '#E2E8F0'
                        }}
                    >
                        <LogOut size={13} color="#94A3B8" strokeWidth={2} />
                    </button>
                </div>

            </div>
        </header>
    )
}