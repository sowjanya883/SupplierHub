import { useState } from 'react'

/**
 * Simple state helper for CRUD pages.
 * Returns modal state and helpers.
 */
export function useCrudPage() {
  const [modal, setModal] = useState(null)   // null | 'create' | 'edit' | 'delete'
  const [selected, setSelected] = useState(null)

  const openCreate = () => { setSelected(null); setModal('create') }
  const openEdit   = (row) => { setSelected(row); setModal('edit') }
  const openDelete = (row) => { setSelected(row); setModal('delete') }
  const closeModal = () => { setModal(null); setSelected(null) }

  return { modal, selected, openCreate, openEdit, openDelete, closeModal }
}
