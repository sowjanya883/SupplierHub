// ConfirmDialog.jsx
export function ConfirmDialog({ message, onConfirm, onCancel, loading }) {
  return (
    <div className="modal-overlay">
      <div className="modal-box" style={{ maxWidth: 400 }}>
        <div className="modal-header">
          <span className="font-semibold text-gray-900">Confirm Action</span>
        </div>
        <div className="modal-body">
          <p className="text-sm text-gray-600">{message}</p>
        </div>
        <div className="modal-footer">
          <button className="btn btn-secondary btn-sm" onClick={onCancel} disabled={loading}>Cancel</button>
          <button className="btn btn-danger btn-sm" onClick={onConfirm} disabled={loading}>
            {loading ? 'Deleting...' : 'Delete'}
          </button>
        </div>
      </div>
    </div>
  )
}

// Spinner.jsx
export function Spinner({ size = 20 }) {
  return (
    <svg width={size} height={size} viewBox="0 0 24 24" fill="none"
      className="animate-spin" style={{ color: '#378add' }}>
      <circle cx="12" cy="12" r="10" stroke="currentColor" strokeWidth="3" strokeOpacity=".25"/>
      <path d="M12 2a10 10 0 0110 10" stroke="currentColor" strokeWidth="3" strokeLinecap="round"/>
    </svg>
  )
}

// EmptyState.jsx
export function EmptyState({ message = 'No records found.' }) {
  return (
    <div className="flex flex-col items-center justify-center py-16 text-gray-400">
      <svg width="40" height="40" fill="none" viewBox="0 0 24 24" className="mb-3 opacity-40">
        <rect x="3" y="3" width="18" height="18" rx="3" stroke="currentColor" strokeWidth="1.5"/>
        <path d="M9 9h6M9 12h4" stroke="currentColor" strokeWidth="1.5" strokeLinecap="round"/>
      </svg>
      <p className="text-sm">{message}</p>
    </div>
  )
}

// FormField.jsx
export function FormField({ label, error, children }) {
  return (
    <div className="mb-4">
      {label && <label className="sh-label">{label}</label>}
      {children}
      {error && <p className="text-red-500 text-xs mt-1">{error}</p>}
    </div>
  )
}

// PageHeader.jsx
export function PageHeader({ title, subtitle, action }) {
  return (
    <div className="page-header">
      <div>
        <h1 className="page-title">{title}</h1>
        {subtitle && <p className="page-subtitle">{subtitle}</p>}
      </div>
      {action && <div>{action}</div>}
    </div>
  )
}
