import { create } from 'zustand'
import { persist } from 'zustand/middleware'

const useAuthStore = create(
  persist(
    (set) => ({
      token: null,
      user: null,      // { userId, name, email, roles[] }

      setAuth: (token, user) => set({ token, user }),
      logout: () => set({ token: null, user: null }),

      hasRole: (role) => {
        // selector helper used in components
      },
    }),
    { name: 'sh-auth' }
  )
)

// Helper: check if logged-in user has one of the given roles
export const selectHasRole = (roles) => (state) =>
  state.user?.roles?.some((r) => roles.includes(r)) ?? false

export default useAuthStore
