import { useNavigate, useLocation } from 'react-router-dom'
import { useQuery, useQueryClient } from '@tanstack/react-query'
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
  '/erp-exports': 'ERP Exports',
}

export default function Topbar() {
  const location = useLocation()
  const navigate = useNavigate()
  const qc = useQueryClient()
  const user = useAuthStore(s => s.user)
  const logout = useAuthStore(s => s.logout)
  const userId = user?.userId

  const base = '/' + location.pathname.split('/')[1]
  const title = ROUTE_LABELS[base] ?? 'SupplierHub'

  const hardLogout = () => {
    logout()
    qc.clear()
    try { localStorage.removeItem('sh-auth') } catch (_) { /* noop */ }
    navigate('/login', { replace: true })
  }

  const { data } = useQuery({
    queryKey: ['notifications', userId],
    queryFn: () => notificationsApi.getAll(userId),
    enabled: !!userId,
    refetchInterval: 60_000,
    select: (d) => {
      const list = d?.data ?? d ?? []
      return list.filter(n => n.status === 'Unread').length
    },
  })
  const unread = data ?? 0

  const initials = user?.name
    ? user.name.split(' ').map(w => w[0]).join('').slice(0, 2).toUpperCase()
    : 'U'
  const allRoles = user?.roles ?? []
  const rolesLabel = allRoles.length === 0 ? 'No role' : allRoles.join(', ')

  return (
    <header
      style={{ height: 'var(--topbar-h)' }}
      className="bg-white border-b border-gray-200 flex items-center px-5 gap-3 flex-shrink-0 z-10"
    >
      <div>
        <div className="font-semibold text-[15px] text-gray-900 leading-tight">{title}</div>
      </div>
      <div className="ml-auto flex items-center gap-3">
        {/* Notification bell */}
        <button
          onClick={() => navigate('/notifications')}
          className="relative w-8 h-8 flex items-center justify-center rounded-lg border border-gray-200 hover:bg-gray-50 transition-colors"
        >
          <svg width="15" height="15" viewBox="0 0 16 16" fill="none">
            <path d="M8 1a5 5 0 00-5 5v2.5L1.5 10h13L13 8.5V6a5 5 0 00-5-5z"
              stroke="#6b7280" strokeWidth="1.2" fill="none"/>
            <path d="M6.5 13a1.5 1.5 0 003 0" stroke="#6b7280" strokeWidth="1.2" fill="none"/>
          </svg>
          {unread > 0 && (
            <span className="absolute -top-1 -right-1 w-4 h-4 rounded-full text-[9px] font-semibold flex items-center justify-center"
              style={{ background: '#d85a30', color: '#fff' }}>
              {unread > 9 ? '9+' : unread}
            </span>
          )}
        </button>

        {/* User identity + roles + hard logout */}
        <div className="flex items-center gap-2 pl-3 border-l border-gray-200">
          <div className="w-7 h-7 rounded-full flex items-center justify-center text-[11px] font-semibold flex-shrink-0"
            style={{ background: '#185fa5', color: '#fff' }}>
            {initials}
          </div>
          <div className="text-right leading-tight max-w-[220px]">
            <div className="text-[12.5px] font-medium text-gray-800 truncate">{user?.name ?? 'User'}</div>
            <div className="text-[10px] text-gray-500 truncate" title={rolesLabel}>
              Roles: {rolesLabel}
            </div>
          </div>
          <button
            onClick={hardLogout}
            title="Log out (clears cached auth)"
            className="ml-2 text-gray-400 hover:text-red-500 transition-colors text-[18px] leading-none"
          >
            ⏻
          </button>
        </div>
      </div>
    </header>
  )
}
