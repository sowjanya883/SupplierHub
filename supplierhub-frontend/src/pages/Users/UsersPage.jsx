import { useQuery } from '@tanstack/react-query'
import { usersApi } from '../../api/operations.api'
import { StatusPill } from '../../components/ui/StatusPill'
import { PageHeader, Spinner, EmptyState } from '../../components/ui/index'

export default function UsersPage() {
  const { data, isLoading } = useQuery({
    queryKey: ['Users'],
    queryFn: usersApi.getAll,
  })
  const rows = data?.data ?? data ?? []

  return (
    <div>
      <PageHeader title="Users" subtitle="User accounts and access" />
      <div className="sh-card p-0 overflow-hidden">
        {isLoading ? (
          <div className="flex justify-center py-12"><Spinner /></div>
        ) : rows.length === 0 ? (
          <EmptyState />
        ) : (
          <div className="overflow-x-auto">
            <table className="sh-table">
              <thead>
                <tr>
                  {Object.keys(rows[0] ?? {}).slice(0, 7).map(k => <th key={k}>{k}</th>)}
                </tr>
              </thead>
              <tbody>
                {rows.map((row, i) => (
                  <tr key={i}>
                    {Object.values(row).slice(0, 6).map((v, j) => (
                      <td key={j}>{String(v ?? '—').slice(0, 50)}</td>
                    ))}
                    <td><StatusPill status={row.status} /></td>
                  </tr>
                ))}
              </tbody>
            </table>
          </div>
        )}
      </div>
    </div>
  )
}
