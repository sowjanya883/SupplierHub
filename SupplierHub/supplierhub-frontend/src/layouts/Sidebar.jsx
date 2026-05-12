import { useQuery } from '@tanstack/react-query'
import { NavLink, useNavigate } from 'react-router-dom'
import {
    LayoutDashboard, Building2, Users,
    Tag, Package, BookOpen, FileSignature,
    Megaphone, ClipboardList, ShoppingCart,
    Truck, PackageCheck, Receipt, AlertTriangle,
    ShieldCheck, BarChart2, Bell, UserCog,
    Settings, ScrollText, LogOut,
} from 'lucide-react'
import useAuthStore from '../store/auth.store'
import { notificationsApi, ncrApi, grnApi, invoicesApi } from '../api/operations.api'
import { purchaseOrdersApi, rfxApi } from '../api/procurement.api'

// ── Colour family per section ──────────────────────────
//  box bg (inactive)   box bg (active)   icon (inactive)   icon (active)   badge bg   badge text
const FAMILIES = {
    indigo: {
        box: 'rgba(139,140,251,0.18)',
        boxActive: 'rgba(139,140,251,0.38)',
        icon: '#a5b4fc',
        iconDim: 'rgba(165,180,252,0.45)',
        badge: 'rgba(139,140,251,0.22)',
        badgeTxt: '#c7d2fe',
    },
    amber: {
        box: 'rgba(251,191,36,0.15)',
        boxActive: 'rgba(251,191,36,0.32)',
        icon: '#fcd34d',
        iconDim: 'rgba(252,211,77,0.4)',
        badge: 'rgba(251,191,36,0.2)',
        badgeTxt: '#fde68a',
    },
    teal: {
        box: 'rgba(45,212,191,0.14)',
        boxActive: 'rgba(45,212,191,0.3)',
        icon: '#5eead4',
        iconDim: 'rgba(94,234,212,0.4)',
        badge: 'rgba(45,212,191,0.18)',
        badgeTxt: '#99f6e4',
    },
    rose: {
        box: 'rgba(251,113,133,0.14)',
        boxActive: 'rgba(251,113,133,0.3)',
        icon: '#fda4af',
        iconDim: 'rgba(253,164,175,0.4)',
        badge: 'rgba(248,113,113,0.22)',
        badgeTxt: '#fca5a5',
    },
    slate: {
        box: 'rgba(148,163,184,0.12)',
        boxActive: 'rgba(148,163,184,0.25)',
        icon: '#94a3b8',
        iconDim: 'rgba(148,163,184,0.38)',
        badge: 'rgba(148,163,184,0.18)',
        badgeTxt: '#cbd5e1',
    },
}

const NAV = [
    {
        section: 'Main',
        family: 'indigo',
        links: [
            { to: '/dashboard', label: 'Dashboard', icon: LayoutDashboard },
            { to: '/organizations', label: 'Organizations', icon: Building2, roles: ['Admin'] },
            { to: '/suppliers', label: 'Suppliers', icon: Users, roles: ['Admin', 'CategoryManager', 'ComplianceOfficer', 'Buyer', 'SupplierUser'] },
        ],
    },
    {
        section: 'Catalog',
        family: 'indigo',
        links: [
            { to: '/categories', label: 'Categories', icon: Tag, roles: ['Admin', 'CategoryManager', 'Buyer', 'SupplierUser'] },
            { to: '/items', label: 'Items', icon: Package, roles: ['Admin', 'CategoryManager', 'Buyer', 'SupplierUser'] },
            { to: '/catalogs', label: 'Catalogs', icon: BookOpen, roles: ['Admin', 'CategoryManager', 'Buyer', 'SupplierUser'] },
            { to: '/contracts', label: 'Contracts', icon: FileSignature, roles: ['Admin', 'CategoryManager', 'SupplierUser'] },
        ],
    },
    {
        section: 'Procurement',
        family: 'amber',
        links: [
            { to: '/rfx', label: 'RFx Events', icon: Megaphone, badgeKey: 'rfx', roles: ['Admin', 'Buyer', 'CategoryManager', 'SupplierUser'] },
            { to: '/requisitions', label: 'Requisitions', icon: ClipboardList, roles: ['Admin', 'Buyer', 'CategoryManager', 'SupplierUser'] },
            { to: '/purchase-orders', label: 'Purchase Orders', icon: ShoppingCart, badgeKey: 'po', roles: ['Admin', 'Buyer', 'CategoryManager', 'SupplierUser'] },
        ],
    },
    {
        section: 'Operations',
        family: 'teal',
        links: [
            { to: '/shipping', label: 'Shipments / ASN', icon: Truck, roles: ['Admin', 'SupplierUser', 'Buyer', 'ReceivingUser'] },
            { to: '/grn', label: 'GRN & Receiving', icon: PackageCheck, badgeKey: 'grn', roles: ['Admin', 'SupplierUser', 'ReceivingUser'] },
        ],
    },
    {
        section: 'Finance & Quality',
        family: 'rose',
        links: [
            { to: '/invoices', label: 'Invoices', icon: Receipt, badgeKey: 'invoices', roles: ['Admin', 'SupplierUser', 'AccountsPayable', 'Buyer'] },
            { to: '/ncr', label: 'NCR Quality', icon: AlertTriangle, badgeKey: 'ncr', roles: ['Admin', 'SupplierUser', 'ReceivingUser'] },
        ],
    },
    {
        section: 'Compliance & Admin',
        family: 'slate',
        links: [
            { to: '/compliance-docs', label: 'Compliance Docs', icon: ShieldCheck, roles: ['Admin', 'ComplianceOfficer', 'SupplierUser'] },
            { to: '/performance', label: 'Performance', icon: BarChart2 },
            { to: '/notifications', label: 'Notifications', icon: Bell, badgeKey: 'notifs' },
            { to: '/users', label: 'Users', icon: UserCog, roles: ['Admin'] },
            { to: '/admin', label: 'Admin Config', icon: Settings, roles: ['Admin'] },
            { to: '/audit-logs', label: 'Audit Logs', icon: ScrollText, roles: ['Admin'] },
        ],
    },
]

// ── Badge data hook ────────────────────────────────────
function useBadges() {
    const { data: notifsData } = useQuery({
        queryKey: ['notifications'],
        queryFn: notificationsApi.getAll,
        refetchInterval: 60_000,
        select: d => (d?.data ?? d ?? []).filter(n => n.status === 'Unread').length,
    })
    const { data: ncrData } = useQuery({
        queryKey: ['ncrs'],
        queryFn: ncrApi.getAll,
        select: d => (d?.data ?? d ?? []).filter(n => n.status === 'Open').length,
    })
    const { data: invoiceData } = useQuery({
        queryKey: ['invoices'],
        queryFn: invoicesApi.getAll,
        select: d => (d?.data ?? d ?? []).filter(i => i.status === 'On_Hold' || i.status === 'Submitted').length,
    })
    const { data: poData } = useQuery({
        queryKey: ['pos'],
        queryFn: purchaseOrdersApi.getAll,
        select: d => (d?.data ?? d ?? []).filter(p => p.status === 'Open').length,
    })
    const { data: grnData } = useQuery({
        queryKey: ['grns'],
        queryFn: grnApi.getAll,
        select: d => (d?.data ?? d ?? []).filter(g => g.status === 'Pending' || g.status === 'Open').length,
    })

    return {
        notifs: notifsData ?? 0,
        ncr: ncrData ?? 0,
        invoices: invoiceData ?? 0,
        po: poData ?? 0,
        grn: grnData ?? 0,
        rfx: 0,
    }
}

// ─────────────────────────────────────────────────────────
export default function Sidebar() {
    const { user, logout } = useAuthStore()
    const navigate = useNavigate()
    const userRoles = user?.roles ?? []
    const badges = useBadges()

    const canSee = (link) =>
        !link.roles || link.roles.some(r => userRoles.includes(r))

    const initials = user?.name
        ? user.name.split(' ').map(w => w[0]).join('').slice(0, 2).toUpperCase()
        : 'U'

    return (
        <aside
            style={{
                width: 'var(--sidebar-w)',
                background: '#182035',
                borderRight: '1px solid rgba(255,255,255,0.06)',
            }}
            className="flex flex-col h-full flex-shrink-0 overflow-y-auto"
        >
            {/* ── Logo ──────────────────────────────────────── */}
            <div style={{ borderBottom: '1px solid rgba(255,255,255,0.07)' }}
                className="px-4 py-4 flex items-center gap-2.5">
                <div style={{
                    width: 30, height: 30, borderRadius: 8,
                    background: '#4F46E5',
                    display: 'flex', alignItems: 'center', justifyContent: 'center',
                    fontSize: 13, fontWeight: 800, color: '#fff', flexShrink: 0,
                }}>
                    SH
                </div>
                <div>
                    <div style={{ color: '#fff', fontSize: 13, fontWeight: 700, letterSpacing: '-0.2px' }}>
                        SupplierHub
                    </div>
                    <div style={{ color: 'rgba(255,255,255,0.28)', fontSize: 9, letterSpacing: '0.4px' }}>
                        Procurement Portal
                    </div>
                </div>
            </div>

            {/* ── Nav ───────────────────────────────────────── */}
            <nav className="flex-1 overflow-y-auto py-2 px-2">
                {NAV.map(({ section, family, links }) => {
                    const f = FAMILIES[family]
                    const visible = links.filter(canSee)
                    if (!visible.length) return null
                    return (
                        <div key={section} className="mb-1">
                            {/* Section label */}
                            <div style={{
                                fontSize: 9, color: 'rgba(255,255,255,0.22)',
                                textTransform: 'uppercase', letterSpacing: '1.1px',
                                fontWeight: 700, padding: '10px 6px 4px',
                            }}>
                                {section}
                            </div>

                            {visible.map(({ to, label, icon: Icon, badgeKey }) => {
                                const count = badgeKey ? (badges[badgeKey] ?? 0) : 0
                                return (
                                    <NavLink key={to} to={to}>
                                        {({ isActive }) => (
                                            <div style={{
                                                display: 'flex',
                                                alignItems: 'center',
                                                gap: 8,
                                                padding: '6px 6px',
                                                borderRadius: 8,
                                                marginBottom: 1,
                                                cursor: 'pointer',
                                                background: isActive ? 'rgba(255,255,255,0.08)' : 'transparent',
                                                transition: 'all .15s',
                                            }}
                                                className={!isActive ? 'sidebar-item-hover' : ''}
                                            >
                                                {/* Coloured icon box */}
                                                <div style={{
                                                    width: 22, height: 22,
                                                    borderRadius: 6,
                                                    background: isActive ? f.boxActive : f.box,
                                                    display: 'flex', alignItems: 'center',
                                                    justifyContent: 'center',
                                                    flexShrink: 0,
                                                    transition: 'background .15s',
                                                }}>
                                                    <Icon
                                                        size={12}
                                                        color={isActive ? f.icon : f.iconDim}
                                                        strokeWidth={2.2}
                                                    />
                                                </div>

                                                {/* Label */}
                                                <span style={{
                                                    fontSize: 12,
                                                    color: isActive
                                                        ? 'rgba(255,255,255,0.92)'
                                                        : 'rgba(255,255,255,0.42)',
                                                    fontWeight: isActive ? 500 : 400,
                                                    flex: 1,
                                                    transition: 'color .15s',
                                                }}>
                                                    {label}
                                                </span>

                                                {/* Badge count OR active dot */}
                                                {count > 0 ? (
                                                    <span style={{
                                                        background: f.badge,
                                                        color: f.badgeTxt,
                                                        fontSize: 9,
                                                        fontWeight: 700,
                                                        padding: '1.5px 6px',
                                                        borderRadius: 20,
                                                        flexShrink: 0,
                                                    }}>
                                                        {count > 99 ? '99+' : count}
                                                    </span>
                                                ) : isActive ? (
                                                    <span style={{
                                                        width: 5, height: 5,
                                                        borderRadius: '50%',
                                                        background: '#6366F1',
                                                        flexShrink: 0,
                                                    }} />
                                                ) : null}
                                            </div>
                                        )}
                                    </NavLink>
                                )
                            })}
                        </div>
                    )
                })}
            </nav>

            {/* ── User area ─────────────────────────────────── */}
            <div style={{ borderTop: '1px solid rgba(255,255,255,0.07)' }}
                className="px-3 py-3 flex items-center gap-2.5">
                {/* Avatar */}
                <div style={{
                    width: 28, height: 28, borderRadius: '50%',
                    background: '#1e3a6e',
                    border: '1.5px solid rgba(99,102,241,0.45)',
                    color: '#a5b4fc',
                    fontSize: 10, fontWeight: 700,
                    display: 'flex', alignItems: 'center', justifyContent: 'center',
                    flexShrink: 0,
                }}>
                    {initials}
                </div>

                <div className="flex-1 min-w-0">
                    <div style={{ color: 'rgba(255,255,255,0.78)', fontSize: 12, fontWeight: 500 }}
                        className="truncate">
                        {user?.name ?? 'User'}
                    </div>
                    <div style={{ color: 'rgba(255,255,255,0.28)', fontSize: 10 }}
                        className="truncate">
                        {user?.roles?.[0] ?? ''}
                    </div>
                </div>

                {/* Logout */}
                <button
                    onClick={() => { logout(); navigate('/login') }}
                    title="Log out"
                    style={{
                        width: 24, height: 24, borderRadius: 6,
                        background: 'rgba(255,255,255,0.06)',
                        border: 'none', cursor: 'pointer',
                        display: 'flex', alignItems: 'center', justifyContent: 'center',
                        flexShrink: 0, transition: 'background .15s',
                    }}
                    onMouseEnter={e => e.currentTarget.style.background = 'rgba(244,63,94,0.2)'}
                    onMouseLeave={e => e.currentTarget.style.background = 'rgba(255,255,255,0.06)'}
                >
                    <LogOut size={12} color="rgba(255,255,255,0.4)" strokeWidth={2} />
                </button>
            </div>
        </aside>
    )
}