import { create } from 'zustand'

const useUiStore = create((set) => ({
  sidebarOpen: true,
  toggleSidebar: () => set((s) => ({ sidebarOpen: !s.sidebarOpen })),

  modal: null,          // { type: string, data: any }
  openModal: (type, data = null) => set({ modal: { type, data } }),
  closeModal: () => set({ modal: null }),
}))

export default useUiStore
