import { useState } from 'react'
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query'
import { useForm } from 'react-hook-form'
import toast from 'react-hot-toast'
import { usersApi } from '../../api/operations.api'
import { PageHeader, Spinner, EmptyState } from '../../components/ui/index'
import Modal from '../../components/ui/Modal'
import { StatusPill } from '../../components/ui/StatusPill'

export default function UsersPage() {
  const qc = useQueryClient()
  const [modalOpen, setModalOpen] = useState(false)
  const [editing, setEditing] = useState(null)
  const [confirmDelete, setConfirmDelete] = useState(null)

  const { data, isLoading } = useQuery({
    queryKey: ['users'],
    queryFn: () => usersApi.getAll(true),
  })
  const rows = data?.data ?? data ?? []

  const { register, handleSubmit, reset, setValue, formState: { errors } } = useForm()

  const createMut = useMutation({
    mutationFn: usersApi.create,
    onSuccess: () => { qc.invalidateQueries(['users']); closeModal(); toast.success('User created') },
    onError: e => toast.error(extractError(e) ?? 'Failed to create user'),
  })
  const updateMut = useMutation({
    mutationFn: ({ id, dto }) => usersApi.update(id, dto),
    onSuccess: () => { qc.invalidateQueries(['users']); closeModal(); toast.success('User updated') },
    onError: e => toast.error(extractError(e) ?? 'Failed to update user'),
  })
  const deleteMut = useMutation({
    mutationFn: usersApi.delete,
    onSuccess: () => { qc.invalidateQueries(['users']); setConfirmDelete(null); toast.success('User deactivated') },
    onError: e => toast.error(extractError(e) ?? 'Failed to delete user'),
  })

  const openCreate = () => {
    reset({ userName: '', email: '', phone: '', password: '', status: 'Active' })
    setEditing(null)
    setModalOpen(true)
  }
  const openEdit = (u) => {
    setEditing(u)
    setValue('userName', u.userName ?? '')
    setValue('email', u.email ?? '')
    setValue('phone', u.phone ?? '')
    setValue('password', '')
    setValue('status', u.status ?? 'Active')
    setModalOpen(true)
  }
  const closeModal = () => { setModalOpen(false); setEditing(null); reset() }

  const onSubmit = (form) => {
    const payload = {
      userName: form.userName.trim(),
      email: form.email.trim().toLowerCase(),
      phone: form.phone?.trim() || null,
      status: form.status,
    }
    if (form.password) payload.password = form.password

    if (editing) {
      updateMut.mutate({ id: editing.userID ?? editing.userId, dto: payload })
    } else {
      if (!form.password) { toast.error('Password required for new user'); return }
      createMut.mutate(payload)
    }
  }

  const isPending = createMut.isPending || updateMut.isPending

  return (
    <div>
      <PageHeader
        title="Users"
        subtitle="User accounts and access"
        action={
          <button className="btn btn-primary btn-sm" onClick={openCreate}>+ Add User</button>
        }
      />

      <div className="sh-card p-0 overflow-hidden">
        {isLoading ? (
          <div className="flex justify-center py-12"><Spinner /></div>
        ) : rows.length === 0 ? (
          <EmptyState message="No users found." />
        ) : (
          <div className="overflow-x-auto">
            <table className="sh-table">
              <thead>
                <tr>
                  <th>ID</th>
                  <th>Name</th>
                  <th>Email</th>
                  <th>Phone</th>
                  <th>Status</th>
                  <th>State</th>
                  <th>Actions</th>
                </tr>
              </thead>
              <tbody>
                {rows.map(u => {
                  const id = u.userID ?? u.userId
                  return (
                    <tr key={id}>
                      <td className="text-gray-400 text-xs">{id}</td>
                      <td className="font-medium">{u.userName}</td>
                      <td>{u.email}</td>
                      <td className="text-gray-500">{u.phone ?? '—'}</td>
                      <td><StatusPill status={u.status} /></td>
                      <td>
                        {u.isDeleted
                          ? <span className="pill pill-red">Deleted</span>
                          : <span className="pill pill-green">Active</span>}
                      </td>
                      <td>
                        <button className="btn btn-ghost btn-sm" onClick={() => openEdit(u)}>Edit</button>
                        {!u.isDeleted && (
                          <button className="btn btn-ghost btn-sm" onClick={() => setConfirmDelete(u)}>
                            Delete
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

      {modalOpen && (
        <Modal
          title={editing ? `Edit User: ${editing.userName}` : 'New User'}
          onClose={closeModal}
          footer={
            <>
              <button className="btn btn-secondary btn-sm" onClick={closeModal}>Cancel</button>
              <button className="btn btn-primary btn-sm" onClick={handleSubmit(onSubmit)} disabled={isPending}>
                {isPending ? 'Saving…' : editing ? 'Update' : 'Create'}
              </button>
            </>
          }
        >
          <form className="space-y-3" noValidate>
            <div>
              <label className="sh-label">Full Name *</label>
              <input className="sh-input" {...register('userName', { required: 'Required' })} />
              {errors.userName && <p className="text-red-500 text-xs mt-1">{errors.userName.message}</p>}
            </div>
            <div>
              <label className="sh-label">Email *</label>
              <input type="email" className="sh-input" {...register('email', {
                required: 'Required',
                pattern: { value: /^[^\s@]+@[^\s@]+\.[^\s@]+$/, message: 'Invalid email' },
              })} />
              {errors.email && <p className="text-red-500 text-xs mt-1">{errors.email.message}</p>}
            </div>
            <div>
              <label className="sh-label">Phone</label>
              <input className="sh-input" {...register('phone')} />
            </div>
            <div>
              <label className="sh-label">
                Password {editing ? <span className="text-gray-400 font-normal">(leave blank to keep current)</span> : '*'}
              </label>
              <input type="password" className="sh-input"
                {...register('password', editing ? {} : { required: 'Required', minLength: { value: 8, message: 'Min 8 chars' } })} />
              {errors.password && <p className="text-red-500 text-xs mt-1">{errors.password.message}</p>}
            </div>
            <div>
              <label className="sh-label">Status</label>
              <select className="sh-select" {...register('status')}>
                <option>Active</option>
                <option>Inactive</option>
                <option>Suspended</option>
              </select>
            </div>
          </form>
        </Modal>
      )}

      {confirmDelete && (
        <Modal
          title="Confirm Deactivation"
          onClose={() => setConfirmDelete(null)}
          footer={
            <>
              <button className="btn btn-secondary btn-sm" onClick={() => setConfirmDelete(null)}>Cancel</button>
              <button className="btn btn-danger btn-sm"
                onClick={() => deleteMut.mutate(confirmDelete.userID ?? confirmDelete.userId)}
                disabled={deleteMut.isPending}>
                {deleteMut.isPending ? 'Deactivating…' : 'Deactivate'}
              </button>
            </>
          }
        >
          <p className="text-sm text-gray-600">
            This will soft-delete <strong>{confirmDelete.userName}</strong> ({confirmDelete.email}).
            They will no longer be able to log in.
          </p>
        </Modal>
      )}
    </div>
  )
}

function extractError(err) {
  const data = err?.response?.data
  if (!data) return null
  const firstModelError = data.errors
    ? Object.values(data.errors).flat().find(Boolean)
    : null
  return firstModelError ?? data.message ?? (typeof data === 'string' ? data : null)
}
