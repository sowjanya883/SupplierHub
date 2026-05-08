// StatusPill.jsx
export function StatusPill({ status }) {
  const map = {
    open: 'pill-blue', active: 'pill-green', closed: 'pill-gray',
    approved: 'pill-green', rejected: 'pill-red', pending: 'pill-amber',
    submitted: 'pill-blue', hold: 'pill-amber', paid: 'pill-green',
    cancelled: 'pill-red', acknowledged: 'pill-teal', shipped: 'pill-purple',
    delivered: 'pill-green', draft: 'pill-gray', converted: 'pill-teal',
    valid: 'pill-green', expired: 'pill-red', suspended: 'pill-red',
    matched: 'pill-green', mismatch: 'pill-red', unread: 'pill-blue',
    read: 'pill-gray', dismissed: 'pill-gray', pass: 'pill-green', fail: 'pill-red',
  }
  const cls = map[(status ?? '').toLowerCase()] ?? 'pill-gray'
  return <span className={`pill ${cls}`}>{status}</span>
}
