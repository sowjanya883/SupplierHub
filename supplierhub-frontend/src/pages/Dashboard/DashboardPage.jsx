import { useQuery } from '@tanstack/react-query'
import { useNavigate } from 'react-router-dom'
import { purchaseOrdersApi } from '../../api/procurement.api'
import { invoicesApi } from '../../api/operations.api'
import { notificationsApi } from '../../api/operations.api'
import { suppliersApi } from '../../api/suppliers.api'
import { StatusPill } from '../../components/ui/StatusPill'
import { Spinner } from '../../components/ui/index'

function MetricCard({ label, value, sub, subColor = 'text-gray-400' }) {
  return (
    <div className="sh-card">
      <div className="text-xs text-gray-500 mb-2 font-medium">{label}</div>
      <div className="text-2xl font-semibold text-gray-900 leading-none">{value ?? '—'}</div>
      {sub && <div className={`text-xs mt-1.5 ${subColor}`}>{sub}</div>}
    </div>
  )
}

export default function DashboardPage() {
  const navigate = useNavigate()

  const { data: poData }   = useQuery({ queryKey: ['pos'], queryFn: purchaseOrdersApi.getAll })
  const { data: invData }  = useQuery({ queryKey: ['invoices'], queryFn: invoicesApi.getAll })
  const { data: notifData }= useQuery({ queryKey: ['notifications'], queryFn: notificationsApi.getAll })
  const { data: suppData } = useQuery({ queryKey: ['suppliers'], queryFn: suppliersApi.getAll })

  const pos       = poData?.data    ?? poData    ?? []
  const invoices  = invData?.data   ?? invData   ?? []
  const notifs    = notifData?.data ?? notifData ?? []
  const suppliers = suppData?.data  ?? suppData  ?? []

  const openPOs     = pos.filter(p => p.status === 'Open' || p.status === 'Acknowledged').length
  const holdInv     = invoices.filter(i => i.status === 'Hold').length
  const unreadNotif = notifs.filter(n => n.status === 'Unread').length
  const activeSupp  = suppliers.filter(s => s.status === 'Active').length

  const recentPOs    = [...pos].slice(-5).reverse()
  const recentNotifs = [...notifs].slice(0, 5)

  return (
    <div>
      <div className="page-header">
        <div>
          <h1 className="page-title">Dashboard</h1>
          <p className="page-subtitle">Overview of your procurement activity</p>
        </div>
      </div>

      {/* Metric cards */}
      <div className="grid grid-cols-2 gap-4 mb-5 md:grid-cols-4">
        <MetricCard label="Open Purchase Orders" value={openPOs} sub="Active in system" />
        <MetricCard label="Invoices on Hold"     value={holdInv} sub="Needs review" subColor="text-amber-600" />
        <MetricCard label="Active Suppliers"     value={activeSupp} sub="Registered" />
        <MetricCard label="Unread Notifications" value={unreadNotif} sub="Pending action" subColor="text-blue-600" />
      </div>

      {/* Two column */}
      <div className="grid grid-cols-1 gap-4 lg:grid-cols-2">
        {/* Recent POs */}
        <div className="sh-card">
          <div className="flex items-center justify-between mb-4">
            <h3 className="font-semibold text-gray-900 text-sm">Recent Purchase Orders</h3>
            <button onClick={() => navigate('/purchase-orders')} className="text-xs text-blue-600 hover:underline">
              View all →
            </button>
          </div>
          {recentPOs.length === 0 ? (
            <p className="text-sm text-gray-400 text-center py-6">No purchase orders yet.</p>
          ) : (
            <table className="sh-table">
              <thead><tr><th>PO ID</th><th>Supplier</th><th>Status</th></tr></thead>
              <tbody>
                {recentPOs.map(po => (
                  <tr key={po.poID ?? po.poid} className="cursor-pointer"
                    onClick={() => navigate(`/purchase-orders/${po.poID ?? po.poid}`)}>
                    <td className="font-medium">{po.poID ?? po.poid}</td>
                    <td className="text-gray-500">{po.supplierName ?? `Supplier #${po.supplierID}`}</td>
                    <td><StatusPill status={po.status} /></td>
                  </tr>
                ))}
              </tbody>
            </table>
          )}
        </div>

        {/* Notifications */}
        <div className="sh-card">
          <div className="flex items-center justify-between mb-4">
            <h3 className="font-semibold text-gray-900 text-sm">Recent Notifications</h3>
            <button onClick={() => navigate('/notifications')} className="text-xs text-blue-600 hover:underline">
              View all →
            </button>
          </div>
          {recentNotifs.length === 0 ? (
            <p className="text-sm text-gray-400 text-center py-6">No notifications.</p>
          ) : (
            <div className="divide-y divide-gray-100">
              {recentNotifs.map(n => (
                <div key={n.notificationID} className="py-2.5 flex gap-3 items-start">
                  <span className={`mt-1 w-2 h-2 rounded-full flex-shrink-0 ${
                    n.status === 'Unread' ? 'bg-blue-500' : 'bg-gray-300'
                  }`} />
                  <div>
                    <p className="text-sm text-gray-800 leading-snug">{n.message}</p>
                    <p className="text-[11px] text-gray-400 mt-0.5">{n.category}</p>
                  </div>
                </div>
              ))}
            </div>
          )}
        </div>
      </div>
    </div>
  )
}
