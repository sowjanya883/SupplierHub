import { useState } from 'react'
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query'
import { useForm } from 'react-hook-form'
import toast from 'react-hot-toast'
import { rfxApi } from '../../api/procurement.api'
import { suppliersApi } from '../../api/suppliers.api'
import { itemsApi, categoriesApi } from '../../api/catalog.api'
import { StatusPill } from '../../components/ui/StatusPill'
import { PageHeader, Spinner, EmptyState, ConfirmDialog } from '../../components/ui/index'
import Modal from '../../components/ui/Modal'

// ── Type badge colours ─────────────────────────────────
const typePill = (type) => {
  if (type === 'RFQ') return 'pill pill-blue'
  if (type === 'RFP') return 'pill pill-purple'
  return 'pill pill-teal'   // RFI
}

const RFX_STATUSES = ['Open', 'Closed', 'Awarded', 'Cancelled']

// ─────────────────────────────────────────────────────────
export default function RFxPage() {
  const qc = useQueryClient()

  const [activeRfx, setActiveRfx] = useState(null)
  const [search, setSearch]       = useState('')
  const [modal, setModal]         = useState(null)   // 'create' | 'edit'
  const [selected, setSelected]   = useState(null)

  // ── Fetch all RFx events ───────────────────────────────
  const { data, isLoading } = useQuery({
    queryKey: ['rfx'],
    queryFn: rfxApi.getAllRfx,
  })
  const rows = (data?.data ?? data ?? []).filter(r =>
    (r.title ?? '').toLowerCase().includes(search.toLowerCase()) ||
    (r.type  ?? '').toLowerCase().includes(search.toLowerCase())
  )

  // ── Form ───────────────────────────────────────────────
  const { register, handleSubmit, reset, formState: { errors } } = useForm()

  // ── Mutations ──────────────────────────────────────────
  const createMut = useMutation({
    mutationFn: rfxApi.createRfx,
    onSuccess: () => {
      qc.invalidateQueries(['rfx'])
      setModal(null)
      toast.success('RFx event created')
    },
    onError: e => toast.error(e.response?.data?.message ?? 'Failed to create'),
  })

  // Update uses PUT with no ID in URL — RfxID must be in body
  const updateMut = useMutation({
    mutationFn: rfxApi.updateRfx,
    onSuccess: () => {
      qc.invalidateQueries(['rfx'])
      setModal(null)
      toast.success('RFx event updated')
    },
    onError: e => toast.error(e.response?.data?.message ?? 'Failed to update'),
  })

  const openCreate = () => {
    reset({ Type: 'RFQ', Title: '', CategoryID: '', OpenDate: '', CloseDate: '' })
    setSelected(null)
    setModal('create')
  }

  const openEdit = (row) => {
    setSelected(row)
    reset({
      Title:     row.title     ?? '',
      CloseDate: row.closeDate?.split('T')[0] ?? '',
      Status:    row.status    ?? 'Open',
    })
    setModal('edit')
  }

  const onSubmit = (d) => {
    if (selected) {
      // Update — only Title, CloseDate, Status can be changed
      updateMut.mutate({
        RfxID:     selected.rfxID,
        Title:     d.Title,
        CloseDate: d.CloseDate || null,
        Status:    d.Status,
      })
    } else {
      // Create
      createMut.mutate({
        Type:       d.Type,
        Title:      d.Title,
        CategoryID: d.CategoryID ? Number(d.CategoryID) : null,
        OpenDate:   d.OpenDate   || null,
        CloseDate:  d.CloseDate  || null,
      })
    }
  }

  const isPending = createMut.isPending || updateMut.isPending

  // ── Render ─────────────────────────────────────────────
  return (
    <div>
      <PageHeader
        title="RFx Events"
        subtitle="RFI / RFP / RFQ sourcing events — invite suppliers, collect bids, award contracts"
        action={
          <button className="btn btn-primary" onClick={openCreate}>
            + New RFx Event
          </button>
        }
      />

      {/* Search */}
      <div className="mb-4">
        <input
          className="sh-input max-w-xs"
          placeholder="Search by title or type…"
          value={search}
          onChange={e => setSearch(e.target.value)}
        />
      </div>

      {/* ── Events Table ────────────────────────────────── */}
      <div className="sh-card p-0 overflow-hidden mb-5">
        {isLoading ? (
          <div className="flex justify-center py-12"><Spinner /></div>
        ) : rows.length === 0 ? (
          <EmptyState message="No RFx events found." />
        ) : (
          <table className="sh-table">
            <thead>
              <tr>
                <th>Sr. No.</th>
                <th>ID</th>
                <th>Type</th>
                <th>Title</th>
                <th>Close Date</th>
                <th>Status</th>
                <th>Actions</th>
              </tr>
            </thead>
            <tbody>
              {rows.map((r, index) => (
                <tr
                  key={r.rfxID}
                  className={activeRfx?.rfxID === r.rfxID ? 'bg-blue-50' : ''}
                >
                  <td className="text-gray-400 text-xs">{index + 1}</td>
                  <td className="text-gray-400 text-xs">{r.rfxID}</td>
                  <td>
                    <span className={typePill(r.type)}>{r.type}</span>
                  </td>
                  <td className="font-medium">{r.title}</td>
                  <td className="text-xs text-gray-500">
                    {r.closeDate ? new Date(r.closeDate).toLocaleDateString() : '—'}
                  </td>
                  <td><StatusPill status={r.status} /></td>
                  <td>
                    <div className="flex gap-1">
                      {/* Open detail panel */}
                      <button
                        className={`btn btn-sm ${
                          activeRfx?.rfxID === r.rfxID ? 'btn-primary' : 'btn-secondary'
                        }`}
                        onClick={() =>
                          setActiveRfx(
                            activeRfx?.rfxID === r.rfxID ? null : r
                          )
                        }
                      >
                        {activeRfx?.rfxID === r.rfxID ? 'Close' : 'Open'}
                      </button>
                      <button
                        className="btn btn-ghost btn-sm"
                        onClick={() => openEdit(r)}
                      >
                        Edit
                      </button>
                    </div>
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
        )}
      </div>

      {/* ── Detail Panel ────────────────────────────────── */}
      {activeRfx && (
        <RfxDetailPanel
          rfx={activeRfx}
          onClose={() => setActiveRfx(null)}
        />
      )}

      {/* ── Create Modal ─────────────────────────────────── */}
      {modal === 'create' && (
        <Modal
          title="New RFx Event"
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
                {isPending ? 'Creating…' : 'Create'}
              </button>
            </>
          }
        >
          <form className="space-y-3" noValidate>
            <div>
              <label className="sh-label">Type *</label>
              <select className="sh-select" {...register('Type', { required: true })}>
                <option>RFQ</option>
                <option>RFP</option>
                <option>RFI</option>
              </select>
            </div>
            <div>
              <label className="sh-label">Title *</label>
              <input
                className="sh-input"
                placeholder="e.g. Steel Coil Q3 Procurement"
                {...register('Title', { required: 'Title is required' })}
              />
              {errors.Title && (
                <p className="text-red-500 text-xs mt-1">{errors.Title.message}</p>
              )}
            </div>
            <div>
              <label className="sh-label">Category ID</label>
              <input
                type="number"
                className="sh-input"
                placeholder="Optional"
                {...register('CategoryID')}
              />
            </div>
            <div className="grid grid-cols-2 gap-3">
              <div>
                <label className="sh-label">Open Date</label>
                <input type="date" className="sh-input" {...register('OpenDate')} />
              </div>
              <div>
                <label className="sh-label">Close Date</label>
                <input type="date" className="sh-input" {...register('CloseDate')} />
              </div>
            </div>
          </form>
        </Modal>
      )}

      {/* ── Edit Modal ───────────────────────────────────── */}
      {modal === 'edit' && (
        <Modal
          title={`Edit — ${selected?.title}`}
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
                {isPending ? 'Saving…' : 'Update'}
              </button>
            </>
          }
        >
          <form className="space-y-3" noValidate>
            <div>
              <label className="sh-label">Title *</label>
              <input
                className="sh-input"
                {...register('Title', { required: 'Title is required' })}
              />
              {errors.Title && (
                <p className="text-red-500 text-xs mt-1">{errors.Title.message}</p>
              )}
            </div>
            <div>
              <label className="sh-label">Close Date</label>
              <input type="date" className="sh-input" {...register('CloseDate')} />
            </div>
            <div>
              <label className="sh-label">Status</label>
              <select className="sh-select" {...register('Status')}>
                {RFX_STATUSES.map(s => <option key={s}>{s}</option>)}
              </select>
            </div>
            <p className="text-xs text-gray-400">
              Note: Type and Category cannot be changed after creation.
            </p>
          </form>
        </Modal>
      )}
    </div>
  )
}

// ─────────────────────────────────────────────────────────
// RFx Detail Panel — shows tabs for the selected event
// ─────────────────────────────────────────────────────────
function RfxDetailPanel({ rfx, onClose }) {
  const [tab, setTab] = useState('Lines')
  const TABS = ['Lines', 'Invites', 'Bids', 'Awards']

  return (
    <div className="sh-card p-0 overflow-hidden">
      {/* Panel header */}
      <div className="flex items-center justify-between px-5 py-3 border-b border-gray-100 bg-blue-50">
        <div className="flex items-center gap-3">
          <span className={typePill(rfx.type)}>{rfx.type}</span>
          <div>
            <h3 className="font-semibold text-gray-900 text-sm">{rfx.title}</h3>
            <p className="text-xs text-gray-500 mt-0.5">
              RFx ID: {rfx.rfxID} &nbsp;·&nbsp;
              Status: <span className="font-medium">{rfx.status}</span>
              {rfx.closeDate && (
                <> &nbsp;·&nbsp; Closes: {new Date(rfx.closeDate).toLocaleDateString()}</>
              )}
            </p>
          </div>
        </div>
        <button className="btn btn-ghost btn-sm text-gray-400" onClick={onClose}>
          ✕ Close
        </button>
      </div>

      {/* Tabs */}
      <div className="flex gap-1 px-4 border-b border-gray-100">
        {TABS.map(t => (
          <button
            key={t}
            onClick={() => setTab(t)}
            className={`px-4 py-2.5 text-sm font-medium border-b-2 transition-colors ${
              tab === t
                ? 'border-blue-600 text-blue-700'
                : 'border-transparent text-gray-500 hover:text-gray-700'
            }`}
          >
            {t}
          </button>
        ))}
      </div>

      {/* Tab content */}
      <div className="p-4">
        {tab === 'Lines'   && <LinesTab   rfx={rfx} />}
        {tab === 'Invites' && <InvitesTab rfx={rfx} />}
        {tab === 'Bids'    && <BidsTab    rfx={rfx} />}
        {tab === 'Awards'  && <AwardsTab  rfx={rfx} />}
      </div>
    </div>
  )
}

// ─────────────────────────────────────────────────────────
// LINES TAB
// ─────────────────────────────────────────────────────────
function LinesTab({ rfx }) {
  const qc = useQueryClient()
  const [modal, setModal]       = useState(null)
  const [selected, setSelected] = useState(null)

  const { data, isLoading } = useQuery({
    queryKey: ['rfx-lines', rfx.rfxID],
    queryFn:  () => rfxApi.getRfxLines(rfx.rfxID),
  })
  const lines = data?.data ?? data ?? []

  const { data: itemsData } = useQuery({
    queryKey: ['items'],
    queryFn:  itemsApi.getAll,
  })
  const items = itemsData?.data ?? itemsData ?? []

  const { register, handleSubmit, reset } = useForm()

  const addMut = useMutation({
    mutationFn: rfxApi.addRfxLine,
    onSuccess: () => { qc.invalidateQueries(['rfx-lines', rfx.rfxID]); setModal(null); toast.success('Line added') },
    onError: e => toast.error(e.response?.data?.message ?? 'Failed'),
  })
  const updateMut = useMutation({
    mutationFn: rfxApi.updateRfxLine,
    onSuccess: () => { qc.invalidateQueries(['rfx-lines', rfx.rfxID]); setModal(null); toast.success('Line updated') },
    onError: e => toast.error(e.response?.data?.message ?? 'Failed'),
  })

  const openAdd = () => {
    reset({ ItemID: '', Qty: '', Uom: '', TargetPrice: '', Notes: '' })
    setSelected(null); setModal('form')
  }
  const openEdit = (row) => {
    setSelected(row)
    reset({ Qty: row.qty ?? '', Uom: row.uom ?? '', TargetPrice: row.targetPrice ?? '', Notes: row.notes ?? '', Status: row.status ?? '' })
    setModal('form')
  }
  const onSubmit = (d) => {
    if (selected) {
      updateMut.mutate({ RfxLineID: selected.rfxLineID, Qty: d.Qty ? Number(d.Qty) : null, Uom: d.Uom || null, TargetPrice: d.TargetPrice ? Number(d.TargetPrice) : null, Notes: d.Notes || null, Status: d.Status || null })
    } else {
      addMut.mutate({ RfxID: rfx.rfxID, ItemID: d.ItemID ? Number(d.ItemID) : null, Qty: d.Qty ? Number(d.Qty) : null, Uom: d.Uom || null, TargetPrice: d.TargetPrice ? Number(d.TargetPrice) : null, Notes: d.Notes || null })
    }
  }
  const isPending = addMut.isPending || updateMut.isPending

  return (
    <div>
      <div className="flex justify-between items-center mb-3">
        <h4 className="text-sm font-medium text-gray-700">RFx Lines — items being requested</h4>
        <button className="btn btn-primary btn-sm" onClick={openAdd}>+ Add Line</button>
      </div>

      {isLoading ? <div className="flex justify-center py-8"><Spinner /></div>
        : lines.length === 0 ? <EmptyState message="No lines added yet." />
        : (
        <table className="sh-table">
          <thead>
            <tr>
              <th>Sr. No.</th><th>Line ID</th><th>Item</th><th>Qty</th><th>UoM</th><th>Target Price</th><th>Status</th><th>Actions</th>
            </tr>
          </thead>
          <tbody>
            {lines.map((l, i) => (
              <tr key={l.rfxLineID}>
                <td className="text-gray-400 text-xs">{i + 1}</td>
                <td className="text-gray-400 text-xs">{l.rfxLineID}</td>
                <td className="font-medium">{l.itemName ?? (l.itemID ? `Item #${l.itemID}` : '—')}</td>
                <td>{l.qty ?? '—'}</td>
                <td>{l.uom ?? '—'}</td>
                <td className="font-medium text-green-700">{l.targetPrice ?? '—'}</td>
                <td><StatusPill status={l.status} /></td>
                <td>
                  <button className="btn btn-ghost btn-sm" onClick={() => openEdit(l)}>Edit</button>
                </td>
              </tr>
            ))}
          </tbody>
        </table>
      )}

      {modal === 'form' && (
        <Modal
          title={selected ? 'Edit Line' : 'Add RFx Line'}
          onClose={() => setModal(null)}
          footer={
            <>
              <button className="btn btn-secondary btn-sm" onClick={() => setModal(null)}>Cancel</button>
              <button className="btn btn-primary btn-sm" onClick={handleSubmit(onSubmit)} disabled={isPending}>
                {isPending ? 'Saving…' : selected ? 'Update' : 'Add'}
              </button>
            </>
          }
        >
          <form className="space-y-3" noValidate>
            {!selected && (
              <div>
                <label className="sh-label">Item <span className="text-gray-400 font-normal">(optional)</span></label>
                <select className="sh-select" {...register('ItemID')}>
                  <option value="">— No specific item —</option>
                  {items.map(i => <option key={i.itemID} value={i.itemID}>{i.sku}</option>)}
                </select>
              </div>
            )}
            <div className="grid grid-cols-2 gap-3">
              <div>
                <label className="sh-label">Quantity</label>
                <input type="number" step="0.01" className="sh-input" {...register('Qty')} />
              </div>
              <div>
                <label className="sh-label">UoM</label>
                <input className="sh-input" placeholder="KG, PCS…" {...register('Uom')} />
              </div>
            </div>
            <div>
              <label className="sh-label">Target Price</label>
              <input type="number" step="0.01" className="sh-input" {...register('TargetPrice')} />
            </div>
            <div>
              <label className="sh-label">Notes</label>
              <input className="sh-input" {...register('Notes')} />
            </div>
            {selected && (
              <div>
                <label className="sh-label">Status</label>
                <select className="sh-select" {...register('Status')}>
                  <option>Active</option><option>Inactive</option>
                </select>
              </div>
            )}
          </form>
        </Modal>
      )}
    </div>
  )
}

// ─────────────────────────────────────────────────────────
// INVITES TAB
// ─────────────────────────────────────────────────────────
function InvitesTab({ rfx }) {
  const qc = useQueryClient()
  const [modal, setModal]       = useState(null)
  const [selected, setSelected] = useState(null)

  const { data, isLoading } = useQuery({
    queryKey: ['rfx-invites', rfx.rfxID],
    queryFn:  () => rfxApi.getInvites(rfx.rfxID),
  })
  const invites = data?.data ?? data ?? []

  const { data: suppData } = useQuery({
    queryKey: ['suppliers'],
    queryFn:  suppliersApi.getAll,
  })
  const suppliers = suppData?.data ?? suppData ?? []

  const { register, handleSubmit, reset } = useForm()

  const addMut = useMutation({
    mutationFn: rfxApi.addInvite,
    onSuccess: () => { qc.invalidateQueries(['rfx-invites', rfx.rfxID]); setModal(null); toast.success('Supplier invited') },
    onError: e => toast.error(e.response?.data?.message ?? 'Failed'),
  })
  const updateMut = useMutation({
    mutationFn: rfxApi.updateInvite,
    onSuccess: () => { qc.invalidateQueries(['rfx-invites', rfx.rfxID]); setModal(null); toast.success('Invite updated') },
    onError: e => toast.error(e.response?.data?.message ?? 'Failed'),
  })

  const openAdd = () => {
    reset({ SupplierID: '', InvitedDate: '', Status: 'Invited' })
    setSelected(null); setModal('form')
  }
  const openEdit = (row) => {
    setSelected(row)
    reset({ Status: row.status ?? 'Invited' })
    setModal('form')
  }
  const onSubmit = (d) => {
    if (selected) {
      updateMut.mutate({ InviteID: selected.inviteID, Status: d.Status })
    } else {
      addMut.mutate({ RfxID: rfx.rfxID, SupplierID: Number(d.SupplierID), InvitedDate: d.InvitedDate || null, Status: d.Status })
    }
  }
  const isPending = addMut.isPending || updateMut.isPending

  const getSupplierName = (id) => suppliers.find(s => s.supplierID === id)?.legalName ?? `Supplier #${id}`

  return (
    <div>
      <div className="flex justify-between items-center mb-3">
        <h4 className="text-sm font-medium text-gray-700">Invited Suppliers</h4>
        <button className="btn btn-primary btn-sm" onClick={openAdd}>+ Invite Supplier</button>
      </div>

      {isLoading ? <div className="flex justify-center py-8"><Spinner /></div>
        : invites.length === 0 ? <EmptyState message="No suppliers invited yet." />
        : (
        <table className="sh-table">
          <thead>
            <tr><th>Sr. No.</th><th>Invite ID</th><th>Supplier</th><th>Invited Date</th><th>Status</th><th>Actions</th></tr>
          </thead>
          <tbody>
            {invites.map((inv, i) => (
              <tr key={inv.inviteID}>
                <td className="text-gray-400 text-xs">{i + 1}</td>
                <td className="text-gray-400 text-xs">{inv.inviteID}</td>
                <td className="font-medium">{getSupplierName(inv.supplierID)}</td>
                <td className="text-xs text-gray-500">{inv.invitedDate ? new Date(inv.invitedDate).toLocaleDateString() : '—'}</td>
                <td><StatusPill status={inv.status} /></td>
                <td>
                  <button className="btn btn-ghost btn-sm" onClick={() => openEdit(inv)}>Update Status</button>
                </td>
              </tr>
            ))}
          </tbody>
        </table>
      )}

      {modal === 'form' && (
        <Modal
          title={selected ? 'Update Invite Status' : 'Invite Supplier'}
          onClose={() => setModal(null)}
          footer={
            <>
              <button className="btn btn-secondary btn-sm" onClick={() => setModal(null)}>Cancel</button>
              <button className="btn btn-primary btn-sm" onClick={handleSubmit(onSubmit)} disabled={isPending}>
                {isPending ? 'Saving…' : selected ? 'Update' : 'Invite'}
              </button>
            </>
          }
        >
          <form className="space-y-3" noValidate>
            {!selected && (
              <>
                <div>
                  <label className="sh-label">Supplier *</label>
                  <select className="sh-select" {...register('SupplierID', { required: true })}>
                    <option value="">— Select supplier —</option>
                    {suppliers.map(s => <option key={s.supplierID} value={s.supplierID}>{s.legalName}</option>)}
                  </select>
                </div>
                <div>
                  <label className="sh-label">Invited Date</label>
                  <input type="date" className="sh-input" {...register('InvitedDate')} />
                </div>
              </>
            )}
            <div>
              <label className="sh-label">Status *</label>
              <select className="sh-select" {...register('Status', { required: true })}>
                <option>Invited</option>
                <option>Accepted</option>
                <option>Declined</option>
              </select>
            </div>
          </form>
        </Modal>
      )}
    </div>
  )
}

// ─────────────────────────────────────────────────────────
// BIDS TAB
// ─────────────────────────────────────────────────────────
function BidsTab({ rfx }) {
  const qc = useQueryClient()
  const [modal, setModal]         = useState(null)
  const [selected, setSelected]   = useState(null)
  const [activeBid, setActiveBid] = useState(null) // for viewing bid lines

  const { data, isLoading } = useQuery({
    queryKey: ['rfx-bids', rfx.rfxID],
    queryFn:  () => rfxApi.getBids(rfx.rfxID),
  })
  const bids = data?.data ?? data ?? []

  const { data: suppData } = useQuery({ queryKey: ['suppliers'], queryFn: suppliersApi.getAll })
  const suppliers = suppData?.data ?? suppData ?? []

  const { register, handleSubmit, reset } = useForm()

  const addMut = useMutation({
    mutationFn: rfxApi.addBid,
    onSuccess: () => { qc.invalidateQueries(['rfx-bids', rfx.rfxID]); setModal(null); toast.success('Bid submitted') },
    onError: e => toast.error(e.response?.data?.message ?? 'Failed'),
  })
  const updateMut = useMutation({
    mutationFn: rfxApi.updateBid,
    onSuccess: () => { qc.invalidateQueries(['rfx-bids', rfx.rfxID]); setModal(null); toast.success('Bid updated') },
    onError: e => toast.error(e.response?.data?.message ?? 'Failed'),
  })

  const openAdd = () => {
    reset({ SupplierID: '', BidDate: '', TotalValue: '', Currency: 'INR', Status: 'Submitted' })
    setSelected(null); setModal('form')
  }
  const openEdit = (row) => {
    setSelected(row)
    reset({ BidDate: row.bidDate?.split('T')[0] ?? '', TotalValue: row.totalValue ?? '', Currency: row.currency ?? 'INR', Status: row.status ?? 'Submitted' })
    setModal('form')
  }
  const onSubmit = (d) => {
    if (selected) {
      updateMut.mutate({ BidID: selected.bidID, BidDate: d.BidDate || null, TotalValue: d.TotalValue ? Number(d.TotalValue) : null, Currency: d.Currency || null, Status: d.Status })
    } else {
      addMut.mutate({ RfxID: rfx.rfxID, SupplierID: Number(d.SupplierID), BidDate: d.BidDate || null, TotalValue: d.TotalValue ? Number(d.TotalValue) : null, Currency: d.Currency || null, Status: d.Status })
    }
  }
  const isPending = addMut.isPending || updateMut.isPending
  const getSupplierName = (id) => suppliers.find(s => s.supplierID === id)?.legalName ?? `Supplier #${id}`

  return (
    <div>
      <div className="flex justify-between items-center mb-3">
        <h4 className="text-sm font-medium text-gray-700">Supplier Bids</h4>
        <button className="btn btn-primary btn-sm" onClick={openAdd}>+ Add Bid</button>
      </div>

      {isLoading ? <div className="flex justify-center py-8"><Spinner /></div>
        : bids.length === 0 ? <EmptyState message="No bids submitted yet." />
        : (
        <div>
          <table className="sh-table">
            <thead>
              <tr><th>Sr. No.</th><th>Bid ID</th><th>Supplier</th><th>Bid Date</th><th>Total Value</th><th>Currency</th><th>Status</th><th>Actions</th></tr>
            </thead>
            <tbody>
              {bids.map((b, i) => (
                <tr key={b.bidID} className={activeBid?.bidID === b.bidID ? 'bg-blue-50' : ''}>
                  <td className="text-gray-400 text-xs">{i + 1}</td>
                  <td className="text-gray-400 text-xs">{b.bidID}</td>
                  <td className="font-medium">{getSupplierName(b.supplierID)}</td>
                  <td className="text-xs text-gray-500">{b.bidDate ? new Date(b.bidDate).toLocaleDateString() : '—'}</td>
                  <td className="font-semibold text-green-700">{b.totalValue?.toLocaleString() ?? '—'}</td>
                  <td>{b.currency ?? '—'}</td>
                  <td><StatusPill status={b.status} /></td>
                  <td>
                    <div className="flex gap-1">
                      <button
                        className={`btn btn-sm ${activeBid?.bidID === b.bidID ? 'btn-primary' : 'btn-secondary'}`}
                        onClick={() => setActiveBid(activeBid?.bidID === b.bidID ? null : b)}
                      >
                        {activeBid?.bidID === b.bidID ? 'Hide Lines' : 'View Lines'}
                      </button>
                      <button className="btn btn-ghost btn-sm" onClick={() => openEdit(b)}>Edit</button>
                    </div>
                  </td>
                </tr>
              ))}
            </tbody>
          </table>

          {/* Bid Lines sub-section */}
          {activeBid && (
            <BidLinesSection bid={activeBid} rfx={rfx} />
          )}
        </div>
      )}

      {modal === 'form' && (
        <Modal
          title={selected ? `Edit Bid #${selected.bidID}` : 'Submit New Bid'}
          onClose={() => setModal(null)}
          footer={
            <>
              <button className="btn btn-secondary btn-sm" onClick={() => setModal(null)}>Cancel</button>
              <button className="btn btn-primary btn-sm" onClick={handleSubmit(onSubmit)} disabled={isPending}>
                {isPending ? 'Saving…' : selected ? 'Update' : 'Submit'}
              </button>
            </>
          }
        >
          <form className="space-y-3" noValidate>
            {!selected && (
              <div>
                <label className="sh-label">Supplier *</label>
                <select className="sh-select" {...register('SupplierID', { required: true })}>
                  <option value="">— Select supplier —</option>
                  {suppliers.map(s => <option key={s.supplierID} value={s.supplierID}>{s.legalName}</option>)}
                </select>
              </div>
            )}
            <div>
              <label className="sh-label">Bid Date</label>
              <input type="date" className="sh-input" {...register('BidDate')} />
            </div>
            <div className="grid grid-cols-2 gap-3">
              <div>
                <label className="sh-label">Total Value</label>
                <input type="number" step="0.01" className="sh-input" {...register('TotalValue')} />
              </div>
              <div>
                <label className="sh-label">Currency</label>
                <select className="sh-select" {...register('Currency')}>
                  <option>INR</option><option>USD</option><option>EUR</option>
                </select>
              </div>
            </div>
            <div>
              <label className="sh-label">Status *</label>
              <select className="sh-select" {...register('Status', { required: true })}>
                <option>Submitted</option><option>Withdrawn</option>
              </select>
            </div>
          </form>
        </Modal>
      )}
    </div>
  )
}

// ── Bid Lines sub-section ──────────────────────────────
function BidLinesSection({ bid, rfx }) {
  const qc = useQueryClient()
  const [modal, setModal]     = useState(null)
  const [selected, setSelected] = useState(null)

  const { data, isLoading } = useQuery({
    queryKey: ['bid-lines', bid.bidID],
    queryFn:  () => rfxApi.getBidLines(bid.bidID),
  })
  const lines = data?.data ?? data ?? []

  const { data: rfxLinesData } = useQuery({
    queryKey: ['rfx-lines', rfx.rfxID],
    queryFn:  () => rfxApi.getRfxLines(rfx.rfxID),
  })
  const rfxLines = rfxLinesData?.data ?? rfxLinesData ?? []

  const { register, handleSubmit, reset } = useForm()

  const addMut = useMutation({
    mutationFn: rfxApi.addBidLine,
    onSuccess: () => { qc.invalidateQueries(['bid-lines', bid.bidID]); setModal(null); toast.success('Bid line added') },
    onError: e => toast.error(e.response?.data?.message ?? 'Failed'),
  })
  const updateMut = useMutation({
    mutationFn: rfxApi.updateBidLine,
    onSuccess: () => { qc.invalidateQueries(['bid-lines', bid.bidID]); setModal(null); toast.success('Bid line updated') },
    onError: e => toast.error(e.response?.data?.message ?? 'Failed'),
  })

  const openAdd = () => {
    reset({ RfxLineID: '', UnitPrice: '', LeadTimeDays: '', Currency: 'INR', Notes: '', Status: 'Active' })
    setSelected(null); setModal('form')
  }
  const openEdit = (row) => {
    setSelected(row)
    reset({ UnitPrice: row.unitPrice ?? '', LeadTimeDays: row.leadTimeDays ?? '', Currency: row.currency ?? 'INR', Notes: row.notes ?? '', Status: row.status ?? 'Active' })
    setModal('form')
  }
  const onSubmit = (d) => {
    if (selected) {
      updateMut.mutate({ BidLineID: selected.bidLineID, UnitPrice: d.UnitPrice ? Number(d.UnitPrice) : null, LeadTimeDays: d.LeadTimeDays ? Number(d.LeadTimeDays) : null, Currency: d.Currency || null, Notes: d.Notes || null, Status: d.Status })
    } else {
      addMut.mutate({ BidID: bid.bidID, RfxLineID: Number(d.RfxLineID), UnitPrice: d.UnitPrice ? Number(d.UnitPrice) : null, LeadTimeDays: d.LeadTimeDays ? Number(d.LeadTimeDays) : null, Currency: d.Currency || null, Notes: d.Notes || null, Status: d.Status })
    }
  }
  const isPending = addMut.isPending || updateMut.isPending

  return (
    <div className="mt-3 bg-gray-50 border border-gray-200 rounded-lg p-4">
      <div className="flex justify-between items-center mb-3">
        <h5 className="text-xs font-semibold text-gray-600 uppercase tracking-wide">
          Bid Lines — {bid.bidID}
        </h5>
        <button className="btn btn-primary btn-sm" onClick={openAdd}>+ Add Line</button>
      </div>
      {isLoading ? <div className="flex justify-center py-4"><Spinner size={16} /></div>
        : lines.length === 0 ? <p className="text-xs text-gray-400 text-center py-4">No bid lines yet.</p>
        : (
        <table className="sh-table">
          <thead><tr><th>Line ID</th><th>RFx Line</th><th>Unit Price</th><th>Lead Days</th><th>Currency</th><th>Notes</th><th>Status</th><th></th></tr></thead>
          <tbody>
            {lines.map(l => (
              <tr key={l.bidLineID}>
                <td className="text-gray-400 text-xs">{l.bidLineID}</td>
                <td>{l.rfxLineID}</td>
                <td className="font-semibold text-green-700">{l.unitPrice?.toLocaleString() ?? '—'}</td>
                <td>{l.leadTimeDays ?? '—'}</td>
                <td>{l.currency ?? '—'}</td>
                <td className="text-xs text-gray-500 max-w-xs truncate">{l.notes ?? '—'}</td>
                <td><StatusPill status={l.status} /></td>
                <td><button className="btn btn-ghost btn-sm" onClick={() => openEdit(l)}>Edit</button></td>
              </tr>
            ))}
          </tbody>
        </table>
      )}

      {modal === 'form' && (
        <Modal
          title={selected ? 'Edit Bid Line' : 'Add Bid Line'}
          onClose={() => setModal(null)}
          footer={
            <>
              <button className="btn btn-secondary btn-sm" onClick={() => setModal(null)}>Cancel</button>
              <button className="btn btn-primary btn-sm" onClick={handleSubmit(onSubmit)} disabled={isPending}>
                {isPending ? 'Saving…' : selected ? 'Update' : 'Add'}
              </button>
            </>
          }
        >
          <form className="space-y-3" noValidate>
            {!selected && (
              <div>
                <label className="sh-label">RFx Line *</label>
                <select className="sh-select" {...register('RfxLineID', { required: true })}>
                  <option value="">— Select RFx line —</option>
                  {rfxLines.map(l => (
                    <option key={l.rfxLineID} value={l.rfxLineID}>
                      Line {l.rfxLineID} — {l.itemName ?? `Item #${l.itemID}`} ({l.qty} {l.uom})
                    </option>
                  ))}
                </select>
              </div>
            )}
            <div className="grid grid-cols-2 gap-3">
              <div>
                <label className="sh-label">Unit Price</label>
                <input type="number" step="0.01" className="sh-input" {...register('UnitPrice')} />
              </div>
              <div>
                <label className="sh-label">Lead Time (Days)</label>
                <input type="number" className="sh-input" {...register('LeadTimeDays')} />
              </div>
            </div>
            <div>
              <label className="sh-label">Currency</label>
              <select className="sh-select" {...register('Currency')}>
                <option>INR</option><option>USD</option><option>EUR</option>
              </select>
            </div>
            <div>
              <label className="sh-label">Notes</label>
              <input className="sh-input" {...register('Notes')} />
            </div>
            <div>
              <label className="sh-label">Status</label>
              <select className="sh-select" {...register('Status')}>
                <option>Active</option><option>Inactive</option>
              </select>
            </div>
          </form>
        </Modal>
      )}
    </div>
  )
}

// ─────────────────────────────────────────────────────────
// AWARDS TAB
// ─────────────────────────────────────────────────────────
function AwardsTab({ rfx }) {
  const qc = useQueryClient()
  const [modal, setModal]     = useState(null)
  const [selected, setSelected] = useState(null)

  const { data, isLoading } = useQuery({
    queryKey: ['rfx-awards', rfx.rfxID],
    queryFn:  () => rfxApi.getAwards(rfx.rfxID),
  })
  const awards = data?.data ?? data ?? []

  const { data: suppData } = useQuery({ queryKey: ['suppliers'], queryFn: suppliersApi.getAll })
  const suppliers = suppData?.data ?? suppData ?? []

  const { register, handleSubmit, reset } = useForm()

  const addMut = useMutation({
    mutationFn: rfxApi.addAward,
    onSuccess: () => { qc.invalidateQueries(['rfx-awards', rfx.rfxID]); setModal(null); toast.success('Award created') },
    onError: e => toast.error(e.response?.data?.message ?? 'Failed'),
  })
  const updateMut = useMutation({
    mutationFn: rfxApi.updateAward,
    onSuccess: () => { qc.invalidateQueries(['rfx-awards', rfx.rfxID]); setModal(null); toast.success('Award updated') },
    onError: e => toast.error(e.response?.data?.message ?? 'Failed'),
  })

  const openAdd = () => {
    reset({ SupplierID: '', AwardDate: '', AwardValue: '', Currency: 'INR', Notes: '', Status: 'Awarded' })
    setSelected(null); setModal('form')
  }
  const openEdit = (row) => {
    setSelected(row)
    reset({ AwardDate: row.awardDate?.split('T')[0] ?? '', AwardValue: row.awardValue ?? '', Currency: row.currency ?? 'INR', Notes: row.notes ?? '', Status: row.status ?? 'Awarded' })
    setModal('form')
  }
  const onSubmit = (d) => {
    if (selected) {
      updateMut.mutate({ AwardID: selected.awardID, AwardDate: d.AwardDate || null, AwardValue: d.AwardValue ? Number(d.AwardValue) : null, Currency: d.Currency || null, Notes: d.Notes || null, Status: d.Status })
    } else {
      addMut.mutate({ RfxID: rfx.rfxID, SupplierID: Number(d.SupplierID), AwardDate: d.AwardDate || null, AwardValue: d.AwardValue ? Number(d.AwardValue) : null, Currency: d.Currency || null, Notes: d.Notes || null, Status: d.Status })
    }
  }
  const isPending = addMut.isPending || updateMut.isPending
  const getSupplierName = (id) => suppliers.find(s => s.supplierID === id)?.legalName ?? `Supplier #${id}`

  return (
    <div>
      <div className="flex justify-between items-center mb-3">
        <h4 className="text-sm font-medium text-gray-700">Awards — winning supplier selection</h4>
        <button className="btn btn-primary btn-sm" onClick={openAdd}>+ Create Award</button>
      </div>

      {isLoading ? <div className="flex justify-center py-8"><Spinner /></div>
        : awards.length === 0 ? <EmptyState message="No awards created yet." />
        : (
        <table className="sh-table">
          <thead>
            <tr><th>Sr. No.</th><th>Award ID</th><th>Supplier</th><th>Award Date</th><th>Value</th><th>Currency</th><th>Notes</th><th>Status</th><th>Actions</th></tr>
          </thead>
          <tbody>
            {awards.map((a, i) => (
              <tr key={a.awardID}>
                <td className="text-gray-400 text-xs">{i + 1}</td>
                <td className="text-gray-400 text-xs">{a.awardID}</td>
                <td className="font-medium">{getSupplierName(a.supplierID)}</td>
                <td className="text-xs text-gray-500">{a.awardDate ? new Date(a.awardDate).toLocaleDateString() : '—'}</td>
                <td className="font-semibold text-green-700">{a.awardValue?.toLocaleString() ?? '—'}</td>
                <td>{a.currency ?? '—'}</td>
                <td className="text-xs text-gray-500 max-w-xs truncate">{a.notes ?? '—'}</td>
                <td><StatusPill status={a.status} /></td>
                <td>
                  <button className="btn btn-ghost btn-sm" onClick={() => openEdit(a)}>Edit</button>
                </td>
              </tr>
            ))}
          </tbody>
        </table>
      )}

      {modal === 'form' && (
        <Modal
          title={selected ? `Edit Award #${selected.awardID}` : 'Create Award'}
          onClose={() => setModal(null)}
          footer={
            <>
              <button className="btn btn-secondary btn-sm" onClick={() => setModal(null)}>Cancel</button>
              <button className="btn btn-primary btn-sm" onClick={handleSubmit(onSubmit)} disabled={isPending}>
                {isPending ? 'Saving…' : selected ? 'Update' : 'Create Award'}
              </button>
            </>
          }
        >
          <form className="space-y-3" noValidate>
            {!selected && (
              <div>
                <label className="sh-label">Winning Supplier *</label>
                <select className="sh-select" {...register('SupplierID', { required: true })}>
                  <option value="">— Select supplier —</option>
                  {suppliers.map(s => <option key={s.supplierID} value={s.supplierID}>{s.legalName}</option>)}
                </select>
              </div>
            )}
            <div>
              <label className="sh-label">Award Date</label>
              <input type="date" className="sh-input" {...register('AwardDate')} />
            </div>
            <div className="grid grid-cols-2 gap-3">
              <div>
                <label className="sh-label">Award Value</label>
                <input type="number" step="0.01" className="sh-input" {...register('AwardValue')} />
              </div>
              <div>
                <label className="sh-label">Currency</label>
                <select className="sh-select" {...register('Currency')}>
                  <option>INR</option><option>USD</option><option>EUR</option>
                </select>
              </div>
            </div>
            <div>
              <label className="sh-label">Notes</label>
              <input className="sh-input" placeholder="Reason for award, evaluation notes…" {...register('Notes')} />
            </div>
            <div>
              <label className="sh-label">Status *</label>
              <select className="sh-select" {...register('Status', { required: true })}>
                <option>Awarded</option><option>Cancelled</option>
              </select>
            </div>
          </form>
        </Modal>
      )}
    </div>
  )
}