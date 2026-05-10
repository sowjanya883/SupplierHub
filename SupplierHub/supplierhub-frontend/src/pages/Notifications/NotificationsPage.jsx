import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query'
import toast from 'react-hot-toast'
import { notificationsApi } from '../../api/operations.api'
import { StatusPill } from '../../components/ui/StatusPill'
import { PageHeader, Spinner, EmptyState } from '../../components/ui/index'
import useAuthStore from '../../store/auth.store'

export default function NotificationsPage() {
  const qc = useQueryClient()
  const user = useAuthStore(s => s.user)
  const userId = user?.userId

  const { data, isLoading } = useQuery({
    queryKey: ['notifications', userId],
    queryFn: () => notificationsApi.getAll(userId),
    enabled: !!userId,
  })
  const rows = (data?.data ?? data ?? []).filter(n => !userId || n.userID === userId || n.userId === userId)

  const markMut = useMutation({
    mutationFn: ({ id, status }) => notificationsApi.updateStatus(id, { status }),
    onSuccess: () => { qc.invalidateQueries(['notifications', userId]) },
    onError: e => toast.error(e.response?.data?.message ?? 'Failed to update notification'),
  })

  return (
    <div>
      <PageHeader title="Notifications" subtitle="Your in-app alerts" />
      <div className="sh-card p-0 overflow-hidden">
        {isLoading ? (
          <div className="flex justify-center py-12"><Spinner /></div>
        ) : rows.length === 0 ? (
          <EmptyState message="No notifications." />
        ) : (
          <div className="overflow-x-auto">
            <table className="sh-table">
              <thead>
                <tr>
                  <th>Message</th>
                  <th>Category</th>
                  <th>Status</th>
                  <th>Created</th>
                  <th>Actions</th>
                </tr>
              </thead>
              <tbody>
                {rows.map(n => {
                  const id = n.notificationID ?? n.notificationId
                  const status = n.status ?? 'Unread'
                  return (
                    <tr key={id}>
                      <td>{n.message}</td>
                      <td><span className="pill pill-gray">{n.category}</span></td>
                      <td><StatusPill status={status} /></td>
                      <td className="text-xs text-gray-500">
                        {n.createdDate ? new Date(n.createdDate).toLocaleString() : '—'}
                      </td>
                      <td>
                        {status !== 'Read' && (
                          <button
                            className="btn btn-ghost btn-sm"
                            onClick={() => markMut.mutate({ id, status: 'Read' })}
                            disabled={markMut.isPending}
                          >
                            Mark read
                          </button>
                        )}
                        {status !== 'Dismissed' && (
                          <button
                            className="btn btn-ghost btn-sm"
                            onClick={() => markMut.mutate({ id, status: 'Dismissed' })}
                            disabled={markMut.isPending}
                          >
                            Dismiss
                          </button>
                        )}
                      </td>
                    </tr>
                  )
                })}
              </tbody>
            </table>
          </div>
        )}
      </div>
    </div>
  )
}
