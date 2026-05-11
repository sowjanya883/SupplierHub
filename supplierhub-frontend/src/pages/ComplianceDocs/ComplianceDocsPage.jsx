import { useState } from 'react'
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query'
import { useForm } from 'react-hook-form'
import toast from 'react-hot-toast'
import { complianceDocsApi } from '../../api/procurement.api'
import { suppliersApi } from '../../api/suppliers.api'
import { PageHeader, Spinner, EmptyState, ConfirmDialog } from '../../components/ui/index'
import Modal from '../../components/ui/Modal'
import useAuthStore from '../../store/auth.store'

// ComplianceDocStatus enum: Active=1, Inactive=2, Expired=3
const DOC_STATUSES = ['Active', 'Inactive', 'Expired']

// Common compliance document types
const DOC_TYPES = [
  'ISO9001', 'ISO14001', 'ISO45001', 'ISO27001',
  'GSTIN', 'PAN', 'TradeLicense', 'FactoryLicense',
  'InsuranceCertificate', 'BankGuarantee',
  'MSMECertificate', 'StartupCertificate',
  'EnvironmentalClearance', 'FireNOC',
  'ExportLicense', 'ImportLicense',
  'Other',
]

// ── Expiry helpers ─────────────────────────────────────
const getDaysToExpiry = (expiryDate) => {
  if (!expiryDate) return null
  const diff = new Date(expiryDate) - new Date()
  return Math.ceil(diff / (1000 * 60 * 60 * 24))
}

const getExpiryStyle = (days, status) => {
  if (status === 'Expired' || (days !== null && days <= 0)) {
    return { pill: 'pill pill-red', row: 'bg-red-50', label: 'Expired' }
  }
  if (days !== null && days <= 30) {
    return { pill: 'pill pill-orange', row: 'bg-orange-50', label: `${days}d left` }
  }
  if (days !== null && days <= 90) {
    return { pill: 'pill pill-amber', row: '', label: `${days}d left` }
  }
  if (status === 'Active') {
    return { pill: 'pill pill-green', row: '', label: 'Valid' }
  }
  return { pill: 'pill pill-gray', row: '', label: status }
}

// ─────────────────────────────────────────────────────────
export default function ComplianceDocsPage() {
  const qc       = useQueryClient()
  const { user } = useAuthStore()

  const isAdmin             = user?.roles?.includes('Admin')
  const isComplianceOfficer = user?.roles?.includes('ComplianceOfficer')
  const isSupplier          = user?.roles?.includes('SupplierUser')
  const canWrite            = isAdmin || isComplianceOfficer || isSupplier

  const [modal, setModal]         = useState(null)   // 'form' | 'delete'
  const [selected, setSelected]   = useState(null)
  const [search, setSearch]       = useState('')
  const [statusFilter, setStatusFilter]     = useState('all')
  const [supplierFilter, setSupplierFilter] = useState('all')
  const [expiryFilter, setExpiryFilter]     = useState('all') // all | expiring | expired
  const [loadingEdit, setLoadingEdit]       = useState(false)

  // ── Fetch all docs ─────────────────────────────────────
  // Response wrapped: { message, data: [...] }
  const { data: rawData, isLoading } = useQuery({
    queryKey: ['compliance-docs'],
    queryFn:  complianceDocsApi.getAll,
  })
  const allDocs = rawData?.data ?? rawData ?? []

  // ── Fetch suppliers for dropdown + name lookup ─────────
  const { data: suppData } = useQuery({
    queryKey: ['suppliers'],
    queryFn:  suppliersApi.getAll,
  })
  const suppliers = suppData?.data ?? suppData ?? []
  const getSupplierName = (id) =>
    suppliers.find(s => s.supplierID === id)?.legalName ?? `Supplier #${id}`

  // ── Apply filters ──────────────────────────────────────
  const rows = allDocs
    .filter(d =>
      search === '' ||
      String(d.docID       ?? '').includes(search) ||
      (d.docType ?? '').toLowerCase().includes(search.toLowerCase()) ||
      getSupplierName(d.supplierID).toLowerCase().includes(search.toLowerCase())
    )
    .filter(d => statusFilter   === 'all' || d.status === statusFilter)
    .filter(d => supplierFilter === 'all' || String(d.supplierID) === supplierFilter)
    .filter(d => {
      if (expiryFilter === 'all')      return true
      const days = getDaysToExpiry(d.expiryDate)
      if (expiryFilter === 'expired')  return d.status === 'Expired' || (days !== null && days <= 0)
      if (expiryFilter === 'expiring') return days !== null && days > 0 && days <= 30
      return true
    })

  // ── Form ───────────────────────────────────────────────
  const { register, handleSubmit, reset, watch, formState: { errors } } = useForm()
  const watchDocType = watch('DocType')

  // ── Mutations ──────────────────────────────────────────
  const createMut = useMutation({
    mutationFn: complianceDocsApi.create,
    onSuccess: (res) => {
      qc.invalidateQueries(['compliance-docs'])
      setModal(null)
      const id = res?.data?.docID ?? res?.docID
      toast.success(`Document #${id} added successfully`)
    },
    onError: e => toast.error(e.response?.data?.message ?? 'Failed to add document'),
  })

  const updateMut = useMutation({
    mutationFn: ({ id, dto }) => complianceDocsApi.update(id, dto),
    onSuccess: () => {
      qc.invalidateQueries(['compliance-docs'])
      setModal(null)
      toast.success('Document updated successfully')
    },
    onError: e => toast.error(e.response?.data?.message ?? 'Failed to update document'),
  })

  const deleteMut = useMutation({
    mutationFn: (dto) => complianceDocsApi.delete(dto),
    onSuccess: () => {
      qc.invalidateQueries(['compliance-docs'])
      setModal(null)
      toast.success('Document deleted successfully')
    },
    onError: e => toast.error(e.response?.data?.message ?? 'Failed to delete document'),
  })

  // ── Handlers ───────────────────────────────────────────
  const openCreate = () => {
    reset({
      SupplierID:  '',
      DocType:     '',
      CustomType:  '',
      FileUri:     '',
      IssueDate:   '',
      ExpiryDate:  '',
      Status:      'Active',
    })
    setSelected(null)
    setModal('form')
  }

  // GetAll only returns 4 fields — need GetById for edit
  const openEdit = async (row) => {
    setLoadingEdit(true)
    try {
      const res  = await complianceDocsApi.getById(row.docID)
      const full = res?.data ?? res
      setSelected(full)
      reset({
        SupplierID: full.supplierID ?? '',
        DocType:    DOC_TYPES.includes(full.docType) ? full.docType : 'Other',
        CustomType: DOC_TYPES.includes(full.docType) ? '' : full.docType,
        FileUri:    full.fileUri   ?? '',
        IssueDate:  full.issueDate?.split('T')[0]  ?? '',
        ExpiryDate: full.expiryDate?.split('T')[0] ?? '',
        Status:     full.status    ?? 'Active',
      })
      setModal('form')
    } catch {
      toast.error('Failed to load document details')
    } finally {
      setLoadingEdit(false)
    }
  }

  const onSubmit = (d) => {
    // If DocType is "Other", use the custom type input
    const finalDocType = d.DocType === 'Other'
      ? (d.CustomType?.trim() || 'Other')
      : d.DocType

    const payload = {
      SupplierID: Number(d.SupplierID),
      DocType:    finalDocType,
      FileUri:    d.FileUri   || null,
      IssueDate:  d.IssueDate  || null,
      ExpiryDate: d.ExpiryDate || null,
      Status:     d.Status,
    }

    if (selected) {
      // ComplianceDocUpdateDto requires DocID in body matching route param
      updateMut.mutate({
        id:  selected.docID,
        dto: { ...payload, DocID: selected.docID },
      })
    } else {
      createMut.mutate(payload)
    }
  }

  const isPending = createMut.isPending || updateMut.isPending

  // ── Stats ──────────────────────────────────────────────
  const stats = {
    total:    allDocs.length,
    active:   allDocs.filter(d => d.status === 'Active').length,
    expired:  allDocs.filter(d => d.status === 'Expired').length,
    inactive: allDocs.filter(d => d.status === 'Inactive').length,
    expiring: allDocs.filter(d => {
      const days = getDaysToExpiry(d.expiryDate)
      return days !== null && days > 0 && days <= 30 && d.status === 'Active'
    }).length,
  }

  // ── Render ─────────────────────────────────────────────
  return (
    <div>
      <PageHeader
        title="Compliance Documents"
        subtitle="Supplier certifications, licenses and regulatory documents — track validity and expiry"
        action={
          canWrite && (
            <button className="btn btn-primary" onClick={openCreate}>
              + Add Document
            </button>
          )
        }
      />

      {/* ── Stats bar ─────────────────────────────────────── */}
      <div className="grid grid-cols-2 gap-3 mb-5 md:grid-cols-5">
        <StatCard label="Total Docs"     value={stats.total}    color="text-gray-800"   bg="bg-white" />
        <StatCard label="Active"         value={stats.active}   color="text-green-700"  bg="bg-green-50" />
        <StatCard label="Expiring Soon"  value={stats.expiring} color="text-orange-700" bg="bg-orange-50" />
        <StatCard label="Expired"        value={stats.expired}  color="text-red-700"    bg="bg-red-50" />
        <StatCard label="Inactive"       value={stats.inactive} color="text-gray-500"   bg="bg-gray-50" />
      </div>

      {/* Expiring soon banner */}
      {stats.expiring > 0 && (
        <div className="mb-4 bg-orange-50 border border-orange-200 rounded-lg px-4 py-3 flex items-center gap-3">
          <span className="text-2xl">⏰</span>
          <div>
            <p className="text-orange-800 font-semibold text-sm">
              {stats.expiring} document{stats.expiring > 1 ? 's' : ''} expiring within 30 days
            </p>
            <p className="text-orange-600 text-xs">
              Contact suppliers to renew these documents before they expire
            </p>
          </div>
          <button
            className="ml-auto btn btn-sm bg-orange-500 text-white border-0 hover:bg-orange-600"
            onClick={() => setExpiryFilter('expiring')}
          >
            View Expiring
          </button>
        </div>
      )}

      {/* Expired banner */}
      {stats.expired > 0 && (
        <div className="mb-4 bg-red-50 border border-red-200 rounded-lg px-4 py-3 flex items-center gap-3">
          <span className="text-2xl">🚨</span>
          <div>
            <p className="text-red-800 font-semibold text-sm">
              {stats.expired} document{stats.expired > 1 ? 's' : ''} expired
            </p>
            <p className="text-red-600 text-xs">
              Expired documents may block supplier transactions — request renewals immediately
            </p>
          </div>
          <button
            className="ml-auto btn btn-sm bg-red-500 text-white border-0 hover:bg-red-600"
            onClick={() => setStatusFilter('Expired')}
          >
            View Expired
          </button>
        </div>
      )}

      {/* ── Filters ───────────────────────────────────────── */}
      <div className="flex gap-3 mb-4 flex-wrap items-center">
        <input
          className="sh-input max-w-xs"
          placeholder="Search by ID, doc type or supplier…"
          value={search}
          onChange={e => setSearch(e.target.value)}
        />

        {/* Supplier filter */}
        <select
          className="sh-select"
          style={{ width: 180 }}
          value={supplierFilter}
          onChange={e => setSupplierFilter(e.target.value)}
        >
          <option value="all">All Suppliers</option>
          {suppliers.map(s => (
            <option key={s.supplierID} value={String(s.supplierID)}>
              {s.legalName}
            </option>
          ))}
        </select>

        {/* Status filter */}
        <select
          className="sh-select"
          style={{ width: 140 }}
          value={statusFilter}
          onChange={e => setStatusFilter(e.target.value)}
        >
          <option value="all">All Status</option>
          {DOC_STATUSES.map(s => (
            <option key={s} value={s}>{s}</option>
          ))}
        </select>

        {/* Expiry filter */}
        <select
          className="sh-select"
          style={{ width: 160 }}
          value={expiryFilter}
          onChange={e => setExpiryFilter(e.target.value)}
        >
          <option value="all">All Expiry</option>
          <option value="expiring">Expiring in 30 days</option>
          <option value="expired">Already Expired</option>
        </select>

        {/* Clear filters */}
        {(search || statusFilter !== 'all' || supplierFilter !== 'all' || expiryFilter !== 'all') && (
          <button
            className="btn btn-ghost btn-sm text-gray-400"
            onClick={() => {
              setSearch('')
              setStatusFilter('all')
              setSupplierFilter('all')
              setExpiryFilter('all')
            }}
          >
            ✕ Clear
          </button>
        )}

        <span className="ml-auto text-xs text-gray-400">
          {rows.length} of {allDocs.length} document{allDocs.length !== 1 ? 's' : ''}
        </span>
      </div>

      {/* ── Table ─────────────────────────────────────────── */}
      <div className="sh-card p-0 overflow-hidden">
        {isLoading ? (
          <div className="flex justify-center py-12"><Spinner /></div>
        ) : rows.length === 0 ? (
          <EmptyState
            message={
              allDocs.length === 0
                ? 'No compliance documents yet.'
                : 'No documents match your filters.'
            }
          />
        ) : (
          <table className="sh-table">
            <thead>
              <tr>
                <th>Sr. No.</th>
                <th>Doc ID</th>
                <th>Supplier</th>
                <th>Document Type</th>
                <th>Issue Date</th>
                <th>Expiry Date</th>
                <th>Validity</th>
                <th>File</th>
                <th>Status</th>
                {canWrite && <th>Actions</th>}
              </tr>
            </thead>
            <tbody>
              {rows.map((doc, index) => {
                const days   = getDaysToExpiry(doc.expiryDate)
                const expiry = getExpiryStyle(days, doc.status)
                return (
                  <tr key={doc.docID} className={expiry.row}>
                    <td className="text-gray-400 text-xs">{index + 1}</td>
                    <td className="text-gray-400 text-xs">{doc.docID}</td>
                    <td className="font-medium">{getSupplierName(doc.supplierID)}</td>
                    <td>
                      <span className="font-medium text-sm">{doc.docType}</span>
                    </td>
                    {/* IssueDate / ExpiryDate — only available after backend fix */}
                    <td className="text-xs text-gray-500">
                      {doc.issueDate
                        ? new Date(doc.issueDate).toLocaleDateString()
                        : <span className="text-gray-300">—</span>}
                    </td>
                    <td className="text-xs text-gray-500">
                      {doc.expiryDate
                        ? new Date(doc.expiryDate).toLocaleDateString()
                        : <span className="text-gray-300">—</span>}
                    </td>
                    <td>
                      <span className={expiry.pill}>{expiry.label}</span>
                    </td>
                    <td>
                      {doc.fileUri ? (
                        <a
                          href={doc.fileUri}
                          target="_blank"
                          rel="noopener noreferrer"
                          className="text-blue-600 hover:underline text-xs flex items-center gap-1"
                        >
                          📄 View File
                        </a>
                      ) : (
                        <span className="text-gray-300 text-xs">No file</span>
                      )}
                    </td>
                    <td>
                      <span className={`pill ${
                        doc.status === 'Active'   ? 'pill-green'
                        : doc.status === 'Expired'  ? 'pill-red'
                        : 'pill-gray'
                      }`}>
                        {doc.status}
                      </span>
                    </td>
                    {canWrite && (
                      <td>
                        <div className="flex gap-1">
                          <button
                            className="btn btn-ghost btn-sm"
                            onClick={() => openEdit(doc)}
                            disabled={loadingEdit}
                          >
                            {loadingEdit ? '…' : 'Edit'}
                          </button>
                          <button
                            className="btn btn-danger btn-sm"
                            onClick={() => { setSelected(doc); setModal('delete') }}
                          >
                            Del
                          </button>
                        </div>
                      </td>
                    )}
                  </tr>
                )
              })}
            </tbody>
          </table>
        )}
      </div>

      {/* Backend fix note */}
      <p className="text-xs text-gray-400 mt-2">
        * Issue Date, Expiry Date and File columns require adding those fields to{' '}
        <code className="bg-gray-100 px-1 rounded">ComplianceDocGetAllDto.cs</code>.
        Until then, Edit loads them via GetById.
      </p>

      {/* ── Create / Edit Modal ──────────────────────────── */}
      {modal === 'form' && (
        <Modal
          title={selected ? `Edit Document #${selected.docID}` : 'Add Compliance Document'}
          onClose={() => setModal(null)}
          footer={
            <>
              <button className="btn btn-secondary btn-sm" onClick={() => setModal(null)}>
                Cancel
              </button>
              <button
                className="btn btn-primary btn-sm"
                onClick={handleSubmit(onSubmit)}
                disabled={isPending}
              >
                {isPending ? 'Saving…' : selected ? 'Update' : 'Add Document'}
              </button>
            </>
          }
        >
          <form className="space-y-3" noValidate>

            {/* Supplier */}
            <div>
              <label className="sh-label">Supplier *</label>
              <select
                className="sh-select"
                {...register('SupplierID', { required: 'Supplier is required' })}
              >
                <option value="">— Select supplier —</option>
                {suppliers.map(s => (
                  <option key={s.supplierID} value={s.supplierID}>
                    {s.legalName}
                  </option>
                ))}
              </select>
              {errors.SupplierID && (
                <p className="text-red-500 text-xs mt-1">{errors.SupplierID.message}</p>
              )}
            </div>

            {/* Document Type */}
            <div>
              <label className="sh-label">Document Type *</label>
              <select
                className="sh-select"
                {...register('DocType', { required: 'Document type is required' })}
              >
                <option value="">— Select type —</option>
                {DOC_TYPES.map(t => (
                  <option key={t} value={t}>{t}</option>
                ))}
              </select>
              {errors.DocType && (
                <p className="text-red-500 text-xs mt-1">{errors.DocType.message}</p>
              )}
            </div>

            {/* Custom type if "Other" selected */}
            {watchDocType === 'Other' && (
              <div>
                <label className="sh-label">Custom Document Type *</label>
                <input
                  className="sh-input"
                  placeholder="Enter document type name"
                  {...register('CustomType', { required: 'Please specify the document type' })}
                />
                {errors.CustomType && (
                  <p className="text-red-500 text-xs mt-1">{errors.CustomType.message}</p>
                )}
              </div>
            )}

            {/* File URI */}
            <div>
              <label className="sh-label">
                File URL
                <span className="text-gray-400 font-normal ml-1">(optional)</span>
              </label>
              <input
                className="sh-input"
                placeholder="https://docs.example.com/iso9001.pdf"
                {...register('FileUri')}
              />
              <p className="text-xs text-gray-400 mt-1">
                Link to the document file (cloud storage URL, SharePoint, etc.)
              </p>
            </div>

            {/* Issue + Expiry dates */}
            <div className="grid grid-cols-2 gap-3">
              <div>
                <label className="sh-label">Issue Date</label>
                <input
                  type="date"
                  className="sh-input"
                  {...register('IssueDate')}
                />
              </div>
              <div>
                <label className="sh-label">Expiry Date</label>
                <input
                  type="date"
                  className="sh-input"
                  {...register('ExpiryDate')}
                />
                <p className="text-xs text-gray-400 mt-1">
                  System will alert 30 days before expiry
                </p>
              </div>
            </div>

            {/* Status */}
            <div>
              <label className="sh-label">Status *</label>
              <select
                className="sh-select"
                {...register('Status', { required: true })}
              >
                {DOC_STATUSES.map(s => <option key={s}>{s}</option>)}
              </select>
            </div>
          </form>
        </Modal>
      )}

      {/* ── Delete Confirm ───────────────────────────────── */}
      {modal === 'delete' && (
        <ConfirmDialog
          message={`Delete "${selected?.docType}" document for ${getSupplierName(selected?.supplierID)}? This cannot be undone.`}
          onConfirm={() => deleteMut.mutate({ DocID: selected.docID })}
          onCancel={() => setModal(null)}
          loading={deleteMut.isPending}
        />
      )}
    </div>
  )
}

// ── Stat card ──────────────────────────────────────────
function StatCard({ label, value, color, bg }) {
  return (
    <div className={`sh-card py-3 text-center ${bg}`}>
      <p className={`text-2xl font-bold ${color}`}>{value}</p>
      <p className="text-xs text-gray-500 mt-0.5">{label}</p>
    </div>
  )
}