import { useState } from 'react'
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query'
import { useForm } from 'react-hook-form'
import toast from 'react-hot-toast'
import { shippingApi } from '../../api/operations.api'
import { suppliersApi } from '../../api/suppliers.api'
import { purchaseOrdersApi } from '../../api/procurement.api'
import { StatusPill } from '../../components/ui/StatusPill'
import { PageHeader, Spinner, EmptyState, ConfirmDialog } from '../../components/ui/index'
import Modal from '../../components/ui/Modal'
import useAuthStore from '../../store/auth.store'

// ── Status options from enums ──────────────────────────
const SHIPMENT_STATUSES  = ['PENDING', 'Planned', 'Shipped', 'Delivered', 'Exception']
const ASN_STATUSES       = ['NEW', 'Open', 'Posted']
const ASN_ITEM_STATUSES  = ['SHIPPED', 'Partial', 'Returned']
const SLOT_STATUSES      = ['AVAILABLE', 'Open', 'Booked', 'Closed']

const statusColor = (s) => {
  const m = {
    PENDING: 'pill-amber', Planned: 'pill-blue',
    Shipped: 'pill-purple', Delivered: 'pill-green', Exception: 'pill-red',
    NEW: 'pill-gray', Open: 'pill-blue', Posted: 'pill-green',
    SHIPPED: 'pill-teal', Partial: 'pill-amber', Returned: 'pill-red',
    AVAILABLE: 'pill-green', Booked: 'pill-amber', Closed: 'pill-gray',
  }
  return `pill ${m[s] ?? 'pill-gray'}`
}

const TABS = ['Shipments', 'Delivery Slots']

// ─────────────────────────────────────────────────────────
export default function ShippingPage() {
  const [tab, setTab] = useState('Shipments')

  return (
    <div>
      <PageHeader
        title="Shipments & ASN"
        subtitle="Manage supplier shipments, advance shipping notices and delivery slots"
      />

      <div className="flex gap-1 mb-5 border-b border-gray-200">
        {TABS.map(t => (
          <button key={t} onClick={() => setTab(t)}
            className={`px-4 py-2 text-sm font-medium border-b-2 transition-colors ${
              tab === t
                ? 'border-blue-600 text-blue-700'
                : 'border-transparent text-gray-500 hover:text-gray-700'
            }`}>
            {t}
          </button>
        ))}
      </div>

      {tab === 'Shipments'      && <ShipmentsTab />}
      {tab === 'Delivery Slots' && <DeliverySlotsTab />}
    </div>
  )
}

// ─────────────────────────────────────────────────────────
// SHIPMENTS TAB
// ─────────────────────────────────────────────────────────
function ShipmentsTab() {
  const qc       = useQueryClient()
  const { user } = useAuthStore()

  const [modal, setModal]           = useState(null)
  const [selected, setSelected]     = useState(null)
  const [activeShipment, setActiveShipment] = useState(null)
  const [search, setSearch]         = useState('')

  const { data, isLoading } = useQuery({
    queryKey: ['shipments'],
    queryFn:  shippingApi.getAllShipments,
  })
  const rows = (data?.data ?? data ?? []).filter(s =>
    String(s.shipmentID ?? '').includes(search) ||
    String(s.poID       ?? '').includes(search) ||
    (s.status           ?? '').toLowerCase().includes(search.toLowerCase()) ||
    (s.carrier          ?? '').toLowerCase().includes(search.toLowerCase())
  )

  // Dropdowns
  const { data: suppData } = useQuery({ queryKey: ['suppliers'], queryFn: suppliersApi.getAll })
  const suppliers = suppData?.data ?? suppData ?? []

  const { data: poData } = useQuery({ queryKey: ['pos'], queryFn: purchaseOrdersApi.getAll })
  const pos = poData?.data ?? poData ?? []

  const { register, handleSubmit, reset, formState: { errors } } = useForm()

  const createMut = useMutation({
    mutationFn: shippingApi.createShipment,
    onSuccess: (res) => {
      qc.invalidateQueries(['shipments'])
      setModal(null)
      toast.success(`Shipment #${res?.shipmentID ?? res?.data?.shipmentID} created`)
    },
    onError: e => toast.error(e.response?.data?.message ?? 'Failed to create shipment'),
  })

  const updateMut = useMutation({
    mutationFn: ({ id, dto }) => shippingApi.updateShipment(id, dto),
    onSuccess: () => {
      qc.invalidateQueries(['shipments'])
      setModal(null)
      toast.success('Shipment updated')
    },
    onError: e => toast.error(e.response?.data?.message ?? 'Failed to update'),
  })

  const openCreate = () => {
    reset({
      PoID: '', SupplierID: '', ShipDate: '',
      Carrier: '', TrackingNo: '', Status: 'PENDING',
    })
    setSelected(null)
    setModal('form')
  }

  const openEdit = (row) => {
    setSelected(row)
    reset({
      ShipDate:   row.shipDate?.split('T')[0] ?? '',
      Carrier:    row.carrier    ?? '',
      TrackingNo: row.trackingNo ?? '',
      Status:     row.status     ?? 'PENDING',
    })
    setModal('form')
  }

  const onSubmit = (d) => {
    if (selected) {
      // ShipmentUpdateDto has no ID field — uses route param only
      updateMut.mutate({
        id: selected.shipmentID,
        dto: {
          ShipDate:   d.ShipDate   || null,
          Carrier:    d.Carrier    || null,
          TrackingNo: d.TrackingNo || null,
          Status:     d.Status,
        },
      })
    } else {
      createMut.mutate({
        PoID:       Number(d.PoID),
        SupplierID: Number(d.SupplierID),
        ShipDate:   d.ShipDate   || null,
        Carrier:    d.Carrier    || null,
        TrackingNo: d.TrackingNo || null,
        Status:     d.Status,
      })
    }
  }

  const isPending = createMut.isPending || updateMut.isPending
  const getSupplierName = (id) =>
    suppliers.find(s => s.supplierID === id)?.legalName ?? `Supplier #${id}`

  return (
    <div>
      <div className="flex items-center justify-between mb-4">
        <input className="sh-input max-w-xs"
          placeholder="Search by ID, PO, status or carrier…"
          value={search} onChange={e => setSearch(e.target.value)} />
        <button className="btn btn-primary" onClick={openCreate}>
          + New Shipment
        </button>
      </div>

      {/* Shipments table */}
      <div className="sh-card p-0 overflow-hidden mb-5">
        {isLoading ? <div className="flex justify-center py-12"><Spinner /></div>
          : rows.length === 0 ? <EmptyState message="No shipments found." />
          : (
          <table className="sh-table">
            <thead>
              <tr>
                <th>Sr. No.</th><th>Shipment ID</th><th>PO ID</th>
                <th>Supplier</th><th>Ship Date</th><th>Carrier</th>
                <th>Tracking No</th><th>Status</th><th>Actions</th>
              </tr>
            </thead>
            <tbody>
              {rows.map((s, i) => (
                <tr key={s.shipmentID}
                  className={activeShipment?.shipmentID === s.shipmentID ? 'bg-blue-50' : ''}>
                  <td className="text-gray-400 text-xs">{i + 1}</td>
                  <td className="font-medium">#{s.shipmentID}</td>
                  <td>PO-{s.poID}</td>
                  <td>{getSupplierName(s.supplierID)}</td>
                  <td className="text-xs text-gray-500">
                    {s.shipDate ? new Date(s.shipDate).toLocaleDateString() : '—'}
                  </td>
                  <td>{s.carrier ?? '—'}</td>
                  <td className="font-mono text-xs">{s.trackingNo ?? '—'}</td>
                  <td><span className={statusColor(s.status)}>{s.status}</span></td>
                  <td>
                    <div className="flex gap-1">
                      <button
                        className={`btn btn-sm ${
                          activeShipment?.shipmentID === s.shipmentID
                            ? 'btn-primary' : 'btn-secondary'
                        }`}
                        onClick={() =>
                          setActiveShipment(
                            activeShipment?.shipmentID === s.shipmentID ? null : s
                          )
                        }>
                        {activeShipment?.shipmentID === s.shipmentID ? 'Hide ASN' : 'View ASN'}
                      </button>
                      <button className="btn btn-ghost btn-sm" onClick={() => openEdit(s)}>
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

      {/* ASN Panel */}
      {activeShipment && (
        <AsnPanel shipment={activeShipment} onClose={() => setActiveShipment(null)} />
      )}

      {/* Create / Edit Modal */}
      {modal === 'form' && (
        <Modal
          title={selected ? `Edit Shipment #${selected.shipmentID}` : 'New Shipment'}
          onClose={() => setModal(null)}
          footer={
            <>
              <button className="btn btn-secondary btn-sm" onClick={() => setModal(null)}>Cancel</button>
              <button className="btn btn-primary btn-sm"
                onClick={handleSubmit(onSubmit)} disabled={isPending}>
                {isPending ? 'Saving…' : selected ? 'Update' : 'Create'}
              </button>
            </>
          }>
          <form className="space-y-3" noValidate>
            {!selected && (
              <>
                <div>
                  <label className="sh-label">Purchase Order *</label>
                  <select className="sh-select"
                    {...register('PoID', { required: 'PO is required' })}>
                    <option value="">— Select PO —</option>
                    {pos.map(p => (
                      <option key={p.poID} value={p.poID}>
                        PO-{p.poID} ({p.status})
                      </option>
                    ))}
                  </select>
                  {errors.PoID && <p className="text-red-500 text-xs mt-1">{errors.PoID.message}</p>}
                </div>
                <div>
                  <label className="sh-label">Supplier *</label>
                  <select className="sh-select"
                    {...register('SupplierID', { required: 'Supplier is required' })}>
                    <option value="">— Select supplier —</option>
                    {suppliers.map(s => (
                      <option key={s.supplierID} value={s.supplierID}>{s.legalName}</option>
                    ))}
                  </select>
                  {errors.SupplierID && <p className="text-red-500 text-xs mt-1">{errors.SupplierID.message}</p>}
                </div>
              </>
            )}
            <div>
              <label className="sh-label">Ship Date</label>
              <input type="date" className="sh-input" {...register('ShipDate')} />
            </div>
            <div>
              <label className="sh-label">Carrier</label>
              <input className="sh-input" placeholder="e.g. FedEx, DHL, BlueDart"
                {...register('Carrier')} />
            </div>
            <div>
              <label className="sh-label">Tracking Number</label>
              <input className="sh-input font-mono" placeholder="e.g. FX123456789IN"
                {...register('TrackingNo')} />
            </div>
            <div>
              <label className="sh-label">Status *</label>
              <select className="sh-select" {...register('Status', { required: true })}>
                {SHIPMENT_STATUSES.map(s => <option key={s}>{s}</option>)}
              </select>
            </div>
          </form>
        </Modal>
      )}
    </div>
  )
}

// ─────────────────────────────────────────────────────────
// ASN PANEL — shows ASN for a selected shipment
// ─────────────────────────────────────────────────────────
function AsnPanel({ shipment, onClose }) {
  const qc = useQueryClient()
  const [modal, setModal]       = useState(null)
  const [selected, setSelected] = useState(null)
  const [activeAsn, setActiveAsn] = useState(null)

  const { data, isLoading } = useQuery({
    queryKey: ['asns'],
    queryFn:  shippingApi.getAllAsns,
    select: (d) => (d?.data ?? d ?? []).filter(a => a.shipmentID === shipment.shipmentID),
  })
  const asns = data ?? []

  const { register, handleSubmit, reset } = useForm()

  const createMut = useMutation({
    mutationFn: shippingApi.createAsn,
    onSuccess: () => {
      qc.invalidateQueries(['asns'])
      setModal(null)
      toast.success('ASN created')
    },
    onError: e => toast.error(e.response?.data?.message ?? 'Failed'),
  })

  const updateMut = useMutation({
    mutationFn: ({ id, dto }) => shippingApi.updateAsn(id, dto),
    onSuccess: () => {
      qc.invalidateQueries(['asns'])
      setModal(null)
      toast.success('ASN updated')
    },
    onError: e => toast.error(e.response?.data?.message ?? 'Failed'),
  })

  const openCreate = () => {
    reset({ AsnNo: '', CreatedDate: new Date().toISOString().split('T')[0], Status: 'NEW' })
    setSelected(null)
    setModal('form')
  }

  const openEdit = (row) => {
    setSelected(row)
    reset({
      AsnNo:       row.asnNo       ?? '',
      CreatedDate: row.createdDate?.split('T')[0] ?? '',
      Status:      row.status      ?? 'NEW',
    })
    setModal('form')
  }

  const onSubmit = (d) => {
    if (selected) {
      // AsnUpdateDto — no ID in body
      updateMut.mutate({
        id: selected.asnID,
        dto: {
          AsnNo:       d.AsnNo       || null,
          CreatedDate: d.CreatedDate || null,
          Status:      d.Status,
        },
      })
    } else {
      createMut.mutate({
        ShipmentID:  shipment.shipmentID,
        AsnNo:       d.AsnNo       || null,
        CreatedDate: d.CreatedDate || null,
        Status:      d.Status,
      })
    }
  }

  const isPending = createMut.isPending || updateMut.isPending

  return (
    <div className="sh-card p-0 overflow-hidden mb-5">
      {/* Panel header */}
      <div className="flex items-center justify-between px-5 py-3 border-b border-gray-100 bg-blue-50">
        <div>
          <h3 className="font-semibold text-gray-900 text-sm">
            ASN — Shipment #{shipment.shipmentID}
          </h3>
          <p className="text-xs text-gray-500 mt-0.5">
            PO-{shipment.poID} &nbsp;·&nbsp;
            Carrier: {shipment.carrier ?? '—'} &nbsp;·&nbsp;
            Tracking: {shipment.trackingNo ?? '—'}
          </p>
        </div>
        <div className="flex gap-2 items-center">
          <button className="btn btn-primary btn-sm" onClick={openCreate}>+ Create ASN</button>
          <button className="btn btn-ghost btn-sm text-gray-400" onClick={onClose}>✕</button>
        </div>
      </div>

      {/* ASN table */}
      <div className="p-4">
        {isLoading ? <div className="flex justify-center py-6"><Spinner /></div>
          : asns.length === 0 ? (
          <div className="text-center py-6">
            <p className="text-sm text-gray-400 mb-3">No ASN created for this shipment yet.</p>
            <button className="btn btn-primary btn-sm" onClick={openCreate}>+ Create ASN</button>
          </div>
        ) : (
          <div className="space-y-3">
            <table className="sh-table">
              <thead>
                <tr>
                  <th>Sr. No.</th><th>ASN ID</th><th>ASN No.</th>
                  <th>Created Date</th><th>Status</th><th>Actions</th>
                </tr>
              </thead>
              <tbody>
                {asns.map((a, i) => (
                  <tr key={a.asnID}
                    className={activeAsn?.asnID === a.asnID ? 'bg-green-50' : ''}>
                    <td className="text-gray-400 text-xs">{i + 1}</td>
                    <td className="font-medium">#{a.asnID}</td>
                    <td className="font-mono text-xs">{a.asnNo ?? '—'}</td>
                    <td className="text-xs text-gray-500">
                      {a.createdDate ? new Date(a.createdDate).toLocaleDateString() : '—'}
                    </td>
                    <td><span className={statusColor(a.status)}>{a.status}</span></td>
                    <td>
                      <div className="flex gap-1">
                        <button
                          className={`btn btn-sm ${
                            activeAsn?.asnID === a.asnID ? 'btn-primary' : 'btn-secondary'
                          }`}
                          onClick={() =>
                            setActiveAsn(activeAsn?.asnID === a.asnID ? null : a)
                          }>
                          {activeAsn?.asnID === a.asnID ? 'Hide Items' : 'View Items'}
                        </button>
                        <button className="btn btn-ghost btn-sm" onClick={() => openEdit(a)}>
                          Edit
                        </button>
                      </div>
                    </td>
                  </tr>
                ))}
              </tbody>
            </table>

            {/* ASN Items */}
            {activeAsn && (
              <AsnItemsSection asn={activeAsn} shipment={shipment} />
            )}
          </div>
        )}
      </div>

      {/* ASN Create / Edit Modal */}
      {modal === 'form' && (
        <Modal
          title={selected ? `Edit ASN #${selected.asnID}` : 'Create ASN'}
          onClose={() => setModal(null)}
          footer={
            <>
              <button className="btn btn-secondary btn-sm" onClick={() => setModal(null)}>Cancel</button>
              <button className="btn btn-primary btn-sm"
                onClick={handleSubmit(onSubmit)} disabled={isPending}>
                {isPending ? 'Saving…' : selected ? 'Update' : 'Create ASN'}
              </button>
            </>
          }>
          <form className="space-y-3" noValidate>
            <div>
              <label className="sh-label">ASN Number</label>
              <input className="sh-input font-mono"
                placeholder="e.g. ASN-2025-001"
                {...register('AsnNo')} />
              <p className="text-xs text-gray-400 mt-1">
                Leave blank to auto-assign
              </p>
            </div>
            <div>
              <label className="sh-label">Document Date</label>
              <input type="date" className="sh-input" {...register('CreatedDate')} />
            </div>
            <div>
              <label className="sh-label">Status *</label>
              <select className="sh-select" {...register('Status', { required: true })}>
                {ASN_STATUSES.map(s => <option key={s}>{s}</option>)}
              </select>
            </div>
          </form>
        </Modal>
      )}
    </div>
  )
}

// ─────────────────────────────────────────────────────────
// ASN ITEMS SECTION
// ─────────────────────────────────────────────────────────
function AsnItemsSection({ asn, shipment }) {
  const qc = useQueryClient()
  const [modal, setModal]       = useState(null)
  const [selected, setSelected] = useState(null)

  const { data, isLoading } = useQuery({
    queryKey: ['asn-items', asn.asnID],
    queryFn:  () => shippingApi.getAsnItems(asn.asnID),
  })
  const items = data?.data ?? data ?? []

  const { register, handleSubmit, reset } = useForm()

  const addMut = useMutation({
    mutationFn: shippingApi.addAsnItem,
    onSuccess: () => {
      qc.invalidateQueries(['asn-items', asn.asnID])
      setModal(null)
      toast.success('ASN item added')
    },
    onError: e => toast.error(e.response?.data?.message ?? 'Failed'),
  })

  const updateMut = useMutation({
    mutationFn: ({ id, dto }) => shippingApi.updateAsnItem(id, dto),
    onSuccess: () => {
      qc.invalidateQueries(['asn-items', asn.asnID])
      setModal(null)
      toast.success('ASN item updated')
    },
    onError: e => toast.error(e.response?.data?.message ?? 'Failed'),
  })

  const openAdd = () => {
    reset({ PoLineID: '', ShippedQty: '', LotBatch: '', SerialJson: '', Notes: '', Status: 'SHIPPED' })
    setSelected(null)
    setModal('form')
  }

  const openEdit = (row) => {
    setSelected(row)
    reset({
      ShippedQty:  row.shippedQty  ?? '',
      LotBatch:    row.lotBatch    ?? '',
      SerialJson:  row.serialJson  ?? '',
      Notes:       row.notes       ?? '',
      Status:      row.status      ?? 'SHIPPED',
    })
    setModal('form')
  }

  const onSubmit = (d) => {
    if (selected) {
      // AsnItemUpdateDto — no ID in body
      updateMut.mutate({
        id: selected.asnItemID,
        dto: {
          ShippedQty: d.ShippedQty ? Number(d.ShippedQty) : null,
          LotBatch:   d.LotBatch   || null,
          SerialJson: d.SerialJson || null,
          Notes:      d.Notes      || null,
          Status:     d.Status,
        },
      })
    } else {
      addMut.mutate({
        AsnID:      asn.asnID,
        PoLineID:   Number(d.PoLineID),
        ShippedQty: d.ShippedQty ? Number(d.ShippedQty) : null,
        LotBatch:   d.LotBatch   || null,
        SerialJson: d.SerialJson || null,
        Notes:      d.Notes      || null,
        Status:     d.Status,
      })
    }
  }

  const isPending = addMut.isPending || updateMut.isPending

  return (
    <div className="bg-gray-50 border border-gray-200 rounded-lg p-4">
      <div className="flex items-center justify-between mb-3">
        <h5 className="text-xs font-semibold text-gray-600 uppercase tracking-wide">
          ASN Items — #{asn.asnID} ({asn.asnNo ?? 'No ASN No.'})
        </h5>
        <button className="btn btn-primary btn-sm" onClick={openAdd}>+ Add Item</button>
      </div>

      {isLoading ? <div className="flex justify-center py-4"><Spinner size={16} /></div>
        : items.length === 0 ? (
        <div className="text-center py-4">
          <p className="text-xs text-gray-400 mb-2">No items in this ASN yet.</p>
          <button className="btn btn-primary btn-sm" onClick={openAdd}>+ Add First Item</button>
        </div>
      ) : (
        <table className="sh-table">
          <thead>
            <tr>
              <th>Sr. No.</th><th>Item ID</th><th>PO Line ID</th>
              <th>Shipped Qty</th><th>Lot/Batch</th>
              <th>Notes</th><th>Status</th><th>Actions</th>
            </tr>
          </thead>
          <tbody>
            {items.map((item, i) => (
              <tr key={item.asnItemID}>
                <td className="text-gray-400 text-xs">{i + 1}</td>
                <td className="text-gray-400 text-xs">{item.asnItemID}</td>
                <td>#{item.poLineID}</td>
                <td className="font-medium">{item.shippedQty ?? '—'}</td>
                <td className="font-mono text-xs">{item.lotBatch ?? '—'}</td>
                <td className="text-xs text-gray-500 max-w-xs truncate">{item.notes ?? '—'}</td>
                <td><span className={statusColor(item.status)}>{item.status}</span></td>
                <td>
                  <button className="btn btn-ghost btn-sm" onClick={() => openEdit(item)}>
                    Edit
                  </button>
                </td>
              </tr>
            ))}
          </tbody>
        </table>
      )}

      {modal === 'form' && (
        <Modal
          title={selected ? `Edit Item #${selected.asnItemID}` : 'Add ASN Item'}
          onClose={() => setModal(null)}
          footer={
            <>
              <button className="btn btn-secondary btn-sm" onClick={() => setModal(null)}>Cancel</button>
              <button className="btn btn-primary btn-sm"
                onClick={handleSubmit(onSubmit)} disabled={isPending}>
                {isPending ? 'Saving…' : selected ? 'Update' : 'Add'}
              </button>
            </>
          }>
          <form className="space-y-3" noValidate>
            {!selected && (
              <div>
                <label className="sh-label">PO Line ID *</label>
                <input type="number" className="sh-input"
                  placeholder="Enter PO Line ID from PO-{shipment.poID}"
                  {...register('PoLineID', { required: 'PO Line ID is required' })} />
                <p className="text-xs text-gray-400 mt-1">
                  PO Line from PO-{shipment.poID}
                </p>
              </div>
            )}
            <div>
              <label className="sh-label">Shipped Quantity</label>
              <input type="number" step="0.01" min="0" className="sh-input"
                {...register('ShippedQty')} />
            </div>
            <div>
              <label className="sh-label">Lot / Batch Number</label>
              <input className="sh-input font-mono"
                placeholder="e.g. LOT-2025-A01"
                {...register('LotBatch')} />
            </div>
            <div>
              <label className="sh-label">
                Serial Numbers (JSON)
                <span className="text-gray-400 font-normal ml-1">(optional)</span>
              </label>
              <input className="sh-input font-mono text-xs"
                placeholder='["SN001","SN002","SN003"]'
                {...register('SerialJson')} />
            </div>
            <div>
              <label className="sh-label">Notes</label>
              <input className="sh-input" {...register('Notes')} />
            </div>
            <div>
              <label className="sh-label">Status *</label>
              <select className="sh-select" {...register('Status', { required: true })}>
                {ASN_ITEM_STATUSES.map(s => <option key={s}>{s}</option>)}
              </select>
            </div>
          </form>
        </Modal>
      )}
    </div>
  )
}

// ─────────────────────────────────────────────────────────
// DELIVERY SLOTS TAB
// ─────────────────────────────────────────────────────────
function DeliverySlotsTab() {
  const qc = useQueryClient()
  const [siteID, setSiteID]     = useState('')
  const [searched, setSearched] = useState(false)
  const [modal, setModal]       = useState(null)
  const [selected, setSelected] = useState(null)

  const { data, isLoading, refetch } = useQuery({
    queryKey: ['slots', siteID],
    queryFn:  () => shippingApi.getSlots(siteID),
    enabled: false,   // only fetch when user clicks Search
  })
  const slots = data?.data ?? data ?? []

  const { register, handleSubmit, reset, formState: { errors } } = useForm()

  const createMut = useMutation({
    mutationFn: shippingApi.createSlot,
    onSuccess: () => {
      qc.invalidateQueries(['slots', siteID])
      refetch()
      setModal(null)
      toast.success('Delivery slot created')
    },
    onError: e => toast.error(e.response?.data?.message ?? 'Failed'),
  })

  const updateMut = useMutation({
    mutationFn: ({ id, dto }) => shippingApi.updateSlot(id, dto),
    onSuccess: () => {
      qc.invalidateQueries(['slots', siteID])
      refetch()
      setModal(null)
      toast.success('Slot updated')
    },
    onError: e => toast.error(e.response?.data?.message ?? 'Failed'),
  })

  const openCreate = () => {
    reset({
      SiteID: siteID, Date: '', StartTime: '09:00',
      EndTime: '10:00', Capacity: '', Status: 'AVAILABLE',
    })
    setSelected(null)
    setModal('form')
  }

  const openEdit = (row) => {
    setSelected(row)
    reset({
      Date:      row.date?.split('T')[0] ?? '',
      // TimeSpan serializes as "HH:mm:ss"
      StartTime: row.startTime?.substring(0, 5) ?? '',
      EndTime:   row.endTime?.substring(0, 5)   ?? '',
      Capacity:  row.capacity ?? '',
      Status:    row.status   ?? 'AVAILABLE',
    })
    setModal('form')
  }

  const onSubmit = (d) => {
    const timeToTimeSpan = (t) => `${t}:00`  // "09:00" → "09:00:00" for TimeSpan

    if (selected) {
      // DeliverySlotUpdateDto — no ID, uses route param
      updateMut.mutate({
        id: selected.slotID,
        dto: {
          Date:      d.Date,
          StartTime: timeToTimeSpan(d.StartTime),
          EndTime:   timeToTimeSpan(d.EndTime),
          Capacity:  d.Capacity ? Number(d.Capacity) : null,
          Status:    d.Status,
        },
      })
    } else {
      createMut.mutate({
        SiteID:    Number(d.SiteID),
        Date:      d.Date,
        StartTime: timeToTimeSpan(d.StartTime),
        EndTime:   timeToTimeSpan(d.EndTime),
        Capacity:  d.Capacity ? Number(d.Capacity) : null,
        Status:    d.Status,
      })
    }
  }

  const handleSearch = () => {
    if (!siteID) { toast.error('Enter a Site ID first'); return }
    setSearched(true)
    refetch()
  }

  const isPending = createMut.isPending || updateMut.isPending

  const slotStatusColor = (s) => {
    const m = { AVAILABLE: 'pill-green', Open: 'pill-blue', Booked: 'pill-amber', Closed: 'pill-gray' }
    return `pill ${m[s] ?? 'pill-gray'}`
  }

  return (
    <div>
      {/* Site search */}
      <div className="sh-card mb-5">
        <h3 className="font-semibold text-gray-800 text-sm mb-3">Search Delivery Slots by Site</h3>
        <div className="flex gap-3 items-end">
          <div className="flex-1 max-w-xs">
            <label className="sh-label">Site ID *</label>
            <input
              type="number"
              className="sh-input"
              placeholder="Enter site ID"
              value={siteID}
              onChange={e => setSiteID(e.target.value)}
              onKeyDown={e => e.key === 'Enter' && handleSearch()}
            />
          </div>
          <button className="btn btn-primary" onClick={handleSearch}>
            Search Slots
          </button>
          <button className="btn btn-secondary" onClick={openCreate}>
            + Create Slot
          </button>
        </div>
      </div>

      {/* Slots table */}
      {searched && (
        <div className="sh-card p-0 overflow-hidden">
          <div className="px-4 py-3 border-b border-gray-100 bg-gray-50 flex items-center justify-between">
            <span className="text-sm font-medium text-gray-700">
              Delivery Slots — Site #{siteID}
            </span>
            <span className="text-xs text-gray-400">
              {slots.length} slot{slots.length !== 1 ? 's' : ''} found
            </span>
          </div>

          {isLoading ? <div className="flex justify-center py-8"><Spinner /></div>
            : slots.length === 0 ? (
            <div className="text-center py-8">
              <p className="text-sm text-gray-400 mb-3">No slots found for Site #{siteID}.</p>
              <button className="btn btn-primary btn-sm" onClick={openCreate}>
                + Create First Slot
              </button>
            </div>
          ) : (
            <table className="sh-table">
              <thead>
                <tr>
                  <th>Sr. No.</th><th>Slot ID</th><th>Date</th>
                  <th>Start Time</th><th>End Time</th>
                  <th>Capacity</th><th>Status</th><th>Actions</th>
                </tr>
              </thead>
              <tbody>
                {slots.map((slot, i) => (
                  <tr key={slot.slotID}>
                    <td className="text-gray-400 text-xs">{i + 1}</td>
                    <td className="text-gray-400 text-xs">{slot.slotID}</td>
                    <td className="font-medium">
                      {slot.date ? new Date(slot.date).toLocaleDateString() : '—'}
                    </td>
                    {/* TimeSpan comes as "HH:mm:ss" */}
                    <td className="font-mono text-sm">
                      {slot.startTime?.substring(0, 5) ?? '—'}
                    </td>
                    <td className="font-mono text-sm">
                      {slot.endTime?.substring(0, 5) ?? '—'}
                    </td>
                    <td>{slot.capacity ?? '—'}</td>
                    <td><span className={slotStatusColor(slot.status)}>{slot.status}</span></td>
                    <td>
                      <button className="btn btn-ghost btn-sm" onClick={() => openEdit(slot)}>
                        Edit
                      </button>
                    </td>
                  </tr>
                ))}
              </tbody>
            </table>
          )}
        </div>
      )}

      {/* Create / Edit Slot Modal */}
      {modal === 'form' && (
        <Modal
          title={selected ? `Edit Slot #${selected.slotID}` : 'Create Delivery Slot'}
          onClose={() => setModal(null)}
          footer={
            <>
              <button className="btn btn-secondary btn-sm" onClick={() => setModal(null)}>Cancel</button>
              <button className="btn btn-primary btn-sm"
                onClick={handleSubmit(onSubmit)} disabled={isPending}>
                {isPending ? 'Saving…' : selected ? 'Update' : 'Create Slot'}
              </button>
            </>
          }>
          <form className="space-y-3" noValidate>
            {!selected && (
              <div>
                <label className="sh-label">Site ID *</label>
                <input type="number" className="sh-input"
                  {...register('SiteID', { required: 'Site ID is required' })} />
                {errors.SiteID && <p className="text-red-500 text-xs mt-1">{errors.SiteID.message}</p>}
              </div>
            )}
            <div>
              <label className="sh-label">Date *</label>
              <input type="date" className="sh-input"
                {...register('Date', { required: 'Date is required' })} />
              {errors.Date && <p className="text-red-500 text-xs mt-1">{errors.Date.message}</p>}
            </div>
            <div className="grid grid-cols-2 gap-3">
              <div>
                <label className="sh-label">Start Time *</label>
                <input type="time" className="sh-input"
                  {...register('StartTime', { required: 'Required' })} />
              </div>
              <div>
                <label className="sh-label">End Time *</label>
                <input type="time" className="sh-input"
                  {...register('EndTime', { required: 'Required' })} />
              </div>
            </div>
            <div>
              <label className="sh-label">
                Capacity
                <span className="text-gray-400 font-normal ml-1">(max vehicles)</span>
              </label>
              <input type="number" min="1" className="sh-input"
                placeholder="e.g. 5" {...register('Capacity')} />
            </div>
            <div>
              <label className="sh-label">Status *</label>
              <select className="sh-select" {...register('Status', { required: true })}>
                {SLOT_STATUSES.map(s => <option key={s}>{s}</option>)}
              </select>
            </div>
          </form>
        </Modal>
      )}
    </div>
  )
}