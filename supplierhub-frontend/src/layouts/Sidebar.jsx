import { NavLink, useNavigate } from 'react-router-dom'
import useAuthStore from '../store/auth.store'

const NAV = [
  { section: 'Main',
    links: [
      { to: '/dashboard',       label: 'Dashboard',       dot: '#378add' },
      { to: '/organizations',   label: 'Organizations',   dot: '#1d9e75', roles: ['Admin'] },
      { to: '/suppliers',       label: 'Suppliers',       dot: '#1d9e75', roles: ['Admin','CategoryManager'] },
    ]
  },
  { section: 'Catalog',
    links: [
      { to: '/categories',  label: 'Categories',   dot: '#7f77dd' },
      { to: '/items',       label: 'Items',        dot: '#7f77dd' },
      { to: '/catalogs',    label: 'Catalogs',     dot: '#7f77dd' },
      { to: '/contracts',   label: 'Contracts',    dot: '#7f77dd' },
    ]
  },
  { section: 'Procurement',
    links: [
      { to: '/rfx',            label: 'RFx Events',     dot: '#ef9f27' },
      { to: '/requisitions',   label: 'Requisitions (PR)', dot: '#ef9f27' },
      { to: '/purchase-orders',label: 'Purchase Orders', dot: '#ef9f27' },
    ]
  },
  { section: 'Operations',
    links: [
      { to: '/shipping',    label: 'Shipments / ASN', dot: '#639922' },
      { to: '/grn',         label: 'GRN & Receiving', dot: '#639922' },
      { to: '/invoices',    label: 'Invoices',         dot: '#d85a30' },
      { to: '/ncr',         label: 'NCR / Quality',   dot: '#d4537e' },
    ]
  },
  { section: 'Compliance & Admin',
    links: [
      { to: '/compliance-docs', label: 'Compliance Docs', dot: '#888780' },
      { to: '/performance',     label: 'Performance',      dot: '#1d9e75' },
      { to: '/notifications',   label: 'Notifications',    dot: '#378add' },
      { to: '/users',           label: 'Users',            dot: '#888780', roles: ['Admin'] },
      { to: '/admin',           label: 'Admin Config',     dot: '#888780', roles: ['Admin'] },
      { to: '/audit-logs',      label: 'Audit Logs',       dot: '#888780', roles: ['Admin'] },
    ]
  },
]

export default function Sidebar() {
  const { user, logout } = useAuthStore()
  const navigate = useNavigate()
  const userRoles = user?.roles ?? []

  const canSee = (link) =>
    !link.roles || link.roles.some(r => userRoles.includes(r))

  const initials = user?.name
    ? user.name.split(' ').map(w => w[0]).join('').slice(0, 2).toUpperCase()
    : 'U'

  return (
    <aside
      style={{ width: 'var(--sidebar-w)', background: 'var(--navy-800)' }}
      className="flex flex-col h-full flex-shrink-0 overflow-y-auto"
    >
      {/* Logo */}
      <div className="px-4 py-4 border-b border-white/10">
        <div className="text-white font-semibold text-[15px] tracking-tight">SupplierHub</div>
        <div className="text-[10px] text-white/40 mt-0.5">Procurement Portal</div>
      </div>

      {/* Nav */}
      <nav className="flex-1 py-2 overflow-y-auto">
        {NAV.map(({ section, links }) => {
          const visible = links.filter(canSee)
          if (!visible.length) return null
          return (
            <div key={section} className="mb-1">
              <div className="px-4 pt-3 pb-1 text-[9.5px] uppercase tracking-widest text-white/30 font-semibold">
                {section}
              </div>
              {visible.map(link => (
                <NavLink
                  key={link.to}
                  to={link.to}
                  className={({ isActive }) =>
                    `flex items-center gap-2.5 px-4 py-[7px] text-[12.5px] transition-all border-l-[2.5px] border-transparent
                    ${isActive ? 'nav-active' : 'text-white/55 hover:text-white/85 hover:bg-white/5'}`
                  }
                >
                  <span
                    className="w-[7px] h-[7px] rounded-full flex-shrink-0"
                    style={{ background: link.dot }}
                  />
                  {link.label}
                </NavLink>
              ))}
            </div>
          )
        })}
      </nav>

      {/* User area */}
      <div className="border-t border-white/10 px-4 py-3 flex items-center gap-2.5">
        <div
          className="w-7 h-7 rounded-full flex items-center justify-center text-[11px] font-semibold flex-shrink-0"
          style={{ background: '#185fa5', color: '#b5d4f4' }}
        >
          {initials}
        </div>
        <div className="flex-1 min-w-0">
          <div className="text-white/80 text-[12px] font-medium truncate">{user?.name ?? 'User'}</div>
          <div className="text-white/35 text-[10px] truncate">{user?.roles?.[0] ?? ''}</div>
        </div>
        <button
          onClick={() => { logout(); navigate('/login') }}
          className="text-white/30 hover:text-white/70 transition-colors text-[10px] flex-shrink-0"
          title="Log out"
        >
          ⏻
        </button>
      </div>
    </aside>
  )
}
