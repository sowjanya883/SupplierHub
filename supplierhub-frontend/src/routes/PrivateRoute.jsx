import { Navigate, Outlet } from 'react-router-dom'
import useAuthStore from '../store/auth.store'

export default function PrivateRoute({ roles }) {
  const { token, user } = useAuthStore()

  if (!token) return <Navigate to="/login" replace />

  if (roles && roles.length > 0) {
    const hasRole = user?.roles?.some(r => roles.includes(r))
    if (!hasRole) return <Navigate to="/dashboard" replace />
  }

  return <Outlet />
}
