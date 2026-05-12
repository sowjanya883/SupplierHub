import { useState } from 'react'
import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query'
import { useForm } from 'react-hook-form'
import toast from 'react-hot-toast'
import { shippingApi } from '../../api/operations.api'
import { PageHeader, Spinner, EmptyState } from '../../components/ui/index'
import { StatusPill } from '../../components/ui/StatusPill'
import Modal from '../../components/ui/Modal'

// Shipments and ASNs are now persisted server-side, so the list survives
// tab switches and navigation. Each row has an Edit Status control.
export default function ShippingPage() {
  const [section, setSection] = useState('Shipment')

  return (
    <div>
      <PageHeader title="Shipments & ASN" subtitle="Create and update shipments, ASNs, ASN items and delivery slots" />

      <div className="flex gap-1 mb-4 border-b border-gray-200">
        {['Shipment','ASN','ASN Item','Delivery Slot'].map(t => (
          <button key={t} onClick={() => setSection(t)}
            className={`px-4 py-2 text-sm font-medium border-b-2 transition-colors ${
              section === t ? 'border-blue-600 text-blue-700' : 'border-transparent text-gray-500 hover:text-gray-700'}`}>
            {t}
          </button>
        ))}
      </div>

      {section === 'Shipment'      && <ShipmentSection />}
      {section === 'ASN'           && <AsnSection />}
      {section === 'ASN Item'      && <AsnItemSection />}
      {section === 'Delivery Slot' && <DeliverySlotSection />}
    </div>
  )
}

/* ─── Shipment ─── */
function ShipmentSection() {
  const qc = useQueryClient()
  const [open, setOpen] = useState(false)
  const [editing, setEditing] = useState(null)
  const create = useForm()
  const edit   = useForm()

  const { data, isLoading } = useQuery({
    queryKey: ['shipments'],
    queryFn: shippingApi.getAllShipments,
  })
  const rows = (data?.data ?? data ?? [])

  const createMut = useMutation({
    mutationFn: shippingApi.createShipment,
    onSuccess: () => {
      qc.invalidateQueries({ queryKey: ['shipments'] })
      setOpen(false); create.reset()
      toast.success('Shipment created')
    },
    onError: e => toast.error(extract(e) ?? 'Failed to create shipment'),
  })

  const updateMut = useMutation({
    mutationFn: ({ id, dto }) => shippingApi.updateShipment(id, dto),
    onSuccess: () => {
      qc.invalidateQueries({ queryKey: ['shipments'] })
      setEditing(null); edit.reset()
      toast.success('Shipment updated')
    },
    onError: e => toast.error(extract(e) ?? 'Failed to update shipment'),
  })

  const onCreate = (form) => createMut.mutate({
    poID:       Number(form.poID),
    supplierID: Number(form.supplierID),
    shipDate:   form.shipDate ? new Date(form.shipDate).toISOString() : new Date().toISOString(),
    carrier:    form.carrier,
    trackingNo: form.trackingNo,
    status:     form.status,
  })

  const onEdit = (form) => updateMut.mutate({
    id: editing.shipmentID,
    dto: {
      shipDate:   form.shipDate ? new Date(form.shipDate).toISOString() : null,
      carrier:    form.carrier,
      trackingNo: form.trackingNo,
      status:     form.status,
    },
  })

  return (
    <div>
      <div className="flex items-center justify-between mb-3">
        <div className="text-sm text-gray-600">All shipments</div>
        <button className="btn btn-primary btn-sm" onClick={() => {
          create.reset({ poID: '', supplierID: '', shipDate: today(), carrier: '', trackingNo: '', status: 'Planned' })
          setOpen(true)
        }}>+ Create Shipment</button>
      </div>

      <div className="sh-card p-0 overflow-hidden">
        {isLoading ? <div className="py-8 flex justify-center"><Spinner /></div>
          : rows.length === 0 ? <EmptyState message="No shipments yet." />
          : (
            <table className="sh-table">
              <thead><tr>
                <th>ID</th><th>PO ID</th><th>Supplier</th><th>Ship Date</th>
                <th>Carrier</th><th>Tracking</th><th>Status</th><th>Actions</th>
              </tr></thead>
              <tbody>
                {rows.map(s => (
                  <tr key={s.shipmentID}>
                    <td className="text-gray-400 text-xs">{s.shipmentID}</td>
                    <td>{s.poID}</td>
                    <td>{s.supplierID}</td>
                    <td className="text-xs text-gray-500">{s.shipDate ? new Date(s.shipDate).toLocaleDateString() : '—'}</td>
                    <td>{s.carrier ?? '—'}</td>
                    <td className="font-mono text-xs">{s.trackingNo ?? '—'}</td>
                    <td><StatusPill status={s.status} /></td>
                    <td>
                      <button className="btn btn-ghost btn-sm" onClick={() => {
                        edit.reset({
                          shipDate:   s.shipDate ? s.shipDate.slice(0, 10) : today(),
                          carrier:    s.carrier ?? '',
                          trackingNo: s.trackingNo ?? '',
                          status:     s.status ?? 'Planned',
                        })
                        setEditing(s)
                      }}>Edit Status</button>
                    </td>
                  </tr>
                ))}
              </tbody>
            </table>
          )}
      </div>

      {open && (
        <Modal title="Create Shipment" onClose={() => setOpen(false)}
          footer={<>
            <button className="btn btn-secondary btn-sm" onClick={() => setOpen(false)}>Cancel</button>
            <button className="btn btn-primary btn-sm" onClick={create.handleSubmit(onCreate)} disabled={createMut.isPending}>
              {createMut.isPending ? 'Creating…' : 'Create'}
            </button>
          </>}>
          <form className="space-y-3" noValidate>
            <Field label="PO ID *"><input type="number" className="sh-input" {...create.register('poID', { required: true })} /></Field>
            <Field label="Supplier ID *"><input type="number" className="sh-input" {...create.register('supplierID', { required: true })} /></Field>
            <Field label="Ship Date"><input type="date" className="sh-input" {...create.register('shipDate')} /></Field>
            <Field label="Carrier"><input className="sh-input" placeholder="FedEx" {...create.register('carrier')} /></Field>
            <Field label="Tracking No"><input className="sh-input" {...create.register('trackingNo')} /></Field>
            <Field label="Status">
              <select className="sh-select" {...create.register('status')}>
                <option>Planned</option><option>Shipped</option><option>Delivered</option><option>Exception</option>
              </select>
            </Field>
          </form>
        </Modal>
      )}

      {editing && (
        <Modal title={`Edit Shipment #${editing.shipmentID}`} onClose={() => setEditing(null)}
          footer={<>
            <button className="btn btn-secondary btn-sm" onClick={() => setEditing(null)}>Cancel</button>
            <button className="btn btn-primary btn-sm" onClick={edit.handleSubmit(onEdit)} disabled={updateMut.isPending}>
              {updateMut.isPending ? 'Saving…' : 'Save'}
            </button>
          </>}>
          <form className="space-y-3" noValidate>
            <Field label="Status *">
              <select className="sh-select" {...edit.register('status', { required: true })}>
                <option>Planned</option><option>Shipped</option><option>Delivered</option><option>Exception</option>
              </select>
            </Field>
            <Field label="Ship Date"><input type="date" className="sh-input" {...edit.register('shipDate')} /></Field>
            <Field label="Carrier"><input className="sh-input" {...edit.register('carrier')} /></Field>
            <Field label="Tracking No"><input className="sh-input" {...edit.register('trackingNo')} /></Field>
            <p className="text-xs text-gray-500">Admin, Buyer and ReceivingUser will be notified of the status change.</p>
          </form>
        </Modal>
      )}
    </div>
  )
}

/* ─── ASN ─── */
function AsnSection() {
  const qc = useQueryClient()
  const [open, setOpen] = useState(false)
  const [editing, setEditing] = useState(null)
  const create = useForm()
  const edit   = useForm()

  const { data, isLoading } = useQuery({
    queryKey: ['asns'],
    queryFn: shippingApi.getAllAsns,
  })
  const rows = (data?.data ?? data ?? [])

  const createMut = useMutation({
    mutationFn: shippingApi.createAsn,
    onSuccess: () => {
      qc.invalidateQueries({ queryKey: ['asns'] })
      setOpen(false); create.reset()
      toast.success('ASN created')
    },
    onError: e => toast.error(extract(e) ?? 'Failed to create ASN'),
  })

  const updateMut = useMutation({
    mutationFn: ({ id, dto }) => shippingApi.updateAsn(id, dto),
    onSuccess: () => {
      qc.invalidateQueries({ queryKey: ['asns'] })
      setEditing(null); edit.reset()
      toast.success('ASN updated')
    },
    onError: e => toast.error(extract(e) ?? 'Failed to update ASN'),
  })

  const onCreate = (form) => createMut.mutate({
    shipmentID:  Number(form.shipmentID),
    asnNo:       form.asnNo,
    createdDate: form.createdDate ? new Date(form.createdDate).toISOString() : new Date().toISOString(),
    status:      form.status,
  })

  const onEdit = (form) => updateMut.mutate({
    id: editing.asnID,
    dto: {
      asnNo:  form.asnNo,
      status: form.status,
    },
  })

  return (
    <div>
      <div className="flex items-center justify-between mb-3">
        <div className="text-sm text-gray-600">All Advance Shipping Notices</div>
        <button className="btn btn-primary btn-sm" onClick={() => {
          create.reset({ shipmentID: '', asnNo: '', createdDate: today(), status: 'Open' })
          setOpen(true)
        }}>+ Create ASN</button>
      </div>

      <div className="sh-card p-0 overflow-hidden">
        {isLoading ? <div className="py-8 flex justify-center"><Spinner /></div>
          : rows.length === 0 ? <EmptyState message="No ASNs yet." />
          : (
            <table className="sh-table">
              <thead><tr>
                <th>ID</th><th>ASN No</th><th>Shipment</th><th>Created</th><th>Status</th><th>Actions</th>
              </tr></thead>
              <tbody>
                {rows.map(a => (
                  <tr key={a.asnID}>
                    <td className="text-gray-400 text-xs">{a.asnID}</td>
                    <td className="font-mono text-xs">{a.asnNo}</td>
                    <td>{a.shipmentID}</td>
                    <td className="text-xs text-gray-500">{a.createdDate ? new Date(a.createdDate).toLocaleDateString() : '—'}</td>
                    <td><StatusPill status={a.status} /></td>
                    <td>
                      <button className="btn btn-ghost btn-sm" onClick={() => {
                        edit.reset({ asnNo: a.asnNo ?? '', status: a.status ?? 'Open' })
                        setEditing(a)
                      }}>Edit Status</button>
                    </td>
                  </tr>
                ))}
              </tbody>
            </table>
          )}
      </div>

      {open && (
        <Modal title="Create ASN" onClose={() => setOpen(false)}
          footer={<>
            <button className="btn btn-secondary btn-sm" onClick={() => setOpen(false)}>Cancel</button>
            <button className="btn btn-primary btn-sm" onClick={create.handleSubmit(onCreate)} disabled={createMut.isPending}>
              {createMut.isPending ? 'Creating…' : 'Create'}
            </button>
          </>}>
          <form className="space-y-3" noValidate>
            <Field label="Shipment ID *"><input type="number" className="sh-input" {...create.register('shipmentID', { required: true })} /></Field>
            <Field label="ASN No *"><input className="sh-input" placeholder="ASN-2026-001" {...create.register('asnNo', { required: true })} /></Field>
            <Field label="Created Date"><input type="date" className="sh-input" {...create.register('createdDate')} /></Field>
            <Field label="Status">
              <select className="sh-select" {...create.register('status')}>
                <option>Open</option><option>Posted</option>
              </select>
            </Field>
          </form>
        </Modal>
      )}

      {editing && (
        <Modal title={`Edit ASN #${editing.asnID}`} onClose={() => setEditing(null)}
          footer={<>
            <button className="btn btn-secondary btn-sm" onClick={() => setEditing(null)}>Cancel</button>
            <button className="btn btn-primary btn-sm" onClick={edit.handleSubmit(onEdit)} disabled={updateMut.isPending}>
              {updateMut.isPending ? 'Saving…' : 'Save'}
            </button>
          </>}>
          <form className="space-y-3" noValidate>
            <Field label="ASN No"><input className="sh-input" {...edit.register('asnNo')} /></Field>
            <Field label="Status *">
              <select className="sh-select" {...edit.register('status', { required: true })}>
                <option>Open</option><option>Posted</option>
              </select>
            </Field>
          </form>
        </Modal>
      )}
    </div>
  )
}

/* ─── ASN Item ─── */
function AsnItemSection() {
  const qc = useQueryClient()
  const [asnLookup, setAsnLookup] = useState('')
  const [activeAsn, setActiveAsn] = useState(null)
  const create = useForm()

  const { data: itemsData, isLoading } = useQuery({
    queryKey: ['asn-items', activeAsn],
    queryFn: () => shippingApi.getAsnItems(activeAsn),
    enabled: !!activeAsn,
  })
  const items = (itemsData?.data ?? itemsData ?? [])

  const createMut = useMutation({
    mutationFn: shippingApi.addAsnItem,
    onSuccess: () => {
      if (activeAsn) qc.invalidateQueries({ queryKey: ['asn-items', activeAsn] })
      create.reset({ asnid: activeAsn ?? '', poLineID: '', shippedQty: '', lotBatch: '', serialJSON: '', notes: '' })
      toast.success('ASN item added')
    },
    onError: e => toast.error(extract(e) ?? 'Failed to add ASN item'),
  })

  const onCreate = (form) => createMut.mutate({
    asnid:       Number(form.asnid),
    poLineID:    Number(form.poLineID),
    shippedQty:  Number(form.shippedQty),
    lotBatch:    form.lotBatch ?? '',
    serialJSON:  form.serialJSON ?? '',
    notes:       form.notes ?? '',
  })

  return (
    <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
      <div className="sh-card">
        <h3 className="font-semibold text-gray-800 text-sm mb-3">Add ASN Item</h3>
        <form className="space-y-3" onSubmit={create.handleSubmit(onCreate)} noValidate>
          <Field label="ASN ID *"><input type="number" className="sh-input" {...create.register('asnid', { required: true })} /></Field>
          <Field label="PO Line ID *"><input type="number" className="sh-input" {...create.register('poLineID', { required: true })} /></Field>
          <Field label="Shipped Qty *"><input type="number" step="0.01" className="sh-input" {...create.register('shippedQty', { required: true })} /></Field>
          <Field label="Lot / Batch"><input className="sh-input" {...create.register('lotBatch')} /></Field>
          <Field label="Serial (JSON)"><input className="sh-input font-mono" {...create.register('serialJSON')} /></Field>
          <Field label="Notes"><textarea rows={2} className="sh-input" {...create.register('notes')} /></Field>
          <button type="submit" className="btn btn-primary w-full justify-center" disabled={createMut.isPending}>
            {createMut.isPending ? 'Saving…' : 'Add ASN Item'}
          </button>
        </form>
      </div>

      <div className="sh-card">
        <h3 className="font-semibold text-gray-800 text-sm mb-3">View Items for an ASN</h3>
        <div className="flex gap-2 mb-3">
          <input type="number" className="sh-input flex-1" placeholder="ASN ID" value={asnLookup} onChange={e => setAsnLookup(e.target.value)} />
          <button className="btn btn-secondary btn-sm" onClick={() => setActiveAsn(asnLookup ? Number(asnLookup) : null)} disabled={!asnLookup}>Load</button>
        </div>
        {!activeAsn ? <p className="text-sm text-gray-500">Enter an ASN ID and click Load.</p>
          : isLoading ? <Spinner />
          : items.length === 0 ? <EmptyState message="No items for this ASN." />
          : (
            <table className="sh-table">
              <thead><tr><th>Item</th><th>PO Line</th><th>Qty</th><th>Lot</th></tr></thead>
              <tbody>{items.map(i => (
                <tr key={i.asnItemID}>
                  <td className="text-gray-400 text-xs">{i.asnItemID}</td>
                  <td>{i.poLineID}</td>
                  <td>{i.shippedQty}</td>
                  <td>{i.lotBatch ?? '—'}</td>
                </tr>
              ))}</tbody>
            </table>
          )}
      </div>
    </div>
  )
}

/* ─── Delivery Slot ─── */
function DeliverySlotSection() {
  const qc = useQueryClient()
  const [siteId, setSiteId] = useState('')
  const [activeSite, setActiveSite] = useState(null)
  const [editingSlot, setEditingSlot] = useState(null)
  const { register, handleSubmit, reset } = useForm()
  const statusForm = useForm()

  const { data, isLoading } = useQuery({
    queryKey: ['slots', activeSite],
    queryFn: () => shippingApi.getSlots(activeSite),
    enabled: !!activeSite,
  })
  const slots = (data?.data ?? data ?? [])

  const mut = useMutation({
    mutationFn: shippingApi.createSlot,
    onSuccess: () => {
      if (activeSite) qc.invalidateQueries({ queryKey: ['slots', activeSite] })
      reset()
      toast.success('Slot created')
    },
    onError: e => toast.error(extract(e) ?? 'Failed to create slot'),
  })

  const statusMut = useMutation({
    mutationFn: ({ slotId, status }) => shippingApi.updateSlotStatus(slotId, status),
    onSuccess: () => {
      if (activeSite) qc.invalidateQueries({ queryKey: ['slots', activeSite] })
      setEditingSlot(null)
      statusForm.reset()
      toast.success('Slot status updated')
    },
    onError: e => toast.error(extract(e) ?? 'Failed to update slot status'),
  })

  const onStatusSubmit = (form) => statusMut.mutate({
    slotId: editingSlot.slotID,
    status: form.status,
  })

  const onSubmit = (form) => mut.mutate({
    siteID:    Number(form.siteID),
    date:      form.date ? new Date(form.date).toISOString() : null,
    startTime: toTimeSpan(form.startTime),
    endTime:   toTimeSpan(form.endTime),
    capacity:  Number(form.capacity),
    status:    form.status,
  })

  return (
    <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
      <div className="sh-card">
        <h3 className="font-semibold text-gray-800 text-sm mb-3">Create Delivery Slot</h3>
        <p className="text-xs text-gray-500 mb-3">
          <strong>Site ID</strong> is an external identifier (no Sites table in this build). Use any positive integer (e.g. <code className="font-mono">1</code>) consistently.
        </p>
        <form className="space-y-3" onSubmit={handleSubmit(onSubmit)} noValidate>
          <Field label="Site ID *"><input type="number" className="sh-input" defaultValue={1} {...register('siteID', { required: true })} /></Field>
          <Field label="Date *"><input type="date" className="sh-input" defaultValue={today()} {...register('date', { required: true })} /></Field>
          <div className="grid grid-cols-2 gap-3">
            <Field label="Start Time"><input type="time" className="sh-input" defaultValue="09:00" {...register('startTime')} /></Field>
            <Field label="End Time"><input type="time" className="sh-input" defaultValue="12:00" {...register('endTime')} /></Field>
          </div>
          <Field label="Capacity *"><input type="number" className="sh-input" defaultValue={5} {...register('capacity', { required: true })} /></Field>
          <Field label="Status">
            <select className="sh-select" defaultValue="AVAILABLE" {...register('status')}>
              <option value="AVAILABLE">AVAILABLE</option>
              <option value="BOOKED">BOOKED</option>
              <option value="CLOSED">CLOSED</option>
            </select>
          </Field>
          <button type="submit" className="btn btn-primary w-full justify-center" disabled={mut.isPending}>
            {mut.isPending ? 'Creating…' : 'Create Slot'}
          </button>
        </form>
      </div>

      <div className="sh-card">
        <h3 className="font-semibold text-gray-800 text-sm mb-3">Lookup Slots by Site</h3>
        <p className="text-xs text-gray-500 mb-3">Shows all slots (Available / Booked / Closed) for the given site.</p>
        <div className="flex gap-2 mb-3">
          <input type="number" className="sh-input flex-1" placeholder="Site ID" value={siteId} onChange={e => setSiteId(e.target.value)} />
          <button className="btn btn-secondary btn-sm" onClick={() => setActiveSite(siteId ? Number(siteId) : null)} disabled={!siteId}>Load</button>
        </div>
        {!activeSite ? <p className="text-sm text-gray-500">Enter a site ID and click Load.</p>
          : isLoading ? <Spinner />
          : slots.length === 0 ? <EmptyState message="No slots for this site." />
          : (
            <table className="sh-table">
              <thead><tr>
                <th>Slot</th><th>Date</th><th>Start</th><th>End</th><th>Capacity</th><th>Status</th><th>Actions</th>
              </tr></thead>
              <tbody>{slots.map(s => (
                <tr key={s.slotID}>
                  <td className="text-gray-400 text-xs">{s.slotID}</td>
                  <td className="text-xs text-gray-500">{formatDateOnly(s.date)}</td>
                  <td>{formatTime(s.startTime)}</td><td>{formatTime(s.endTime)}</td><td>{s.capacity}</td>
                  <td><StatusPill status={s.status} /></td>
                  <td>
                    <button
                      className="btn btn-ghost btn-sm"
                      onClick={() => {
                        statusForm.reset({ status: s.status ?? 'AVAILABLE' })
                        setEditingSlot(s)
                      }}
                    >
                      Edit Status
                    </button>
                  </td>
                </tr>
              ))}</tbody>
            </table>
          )}
      </div>

      {editingSlot && (
        <Modal
          title={`Slot #${editingSlot.slotID} — change status`}
          onClose={() => setEditingSlot(null)}
          footer={
            <>
              <button className="btn btn-secondary btn-sm" onClick={() => setEditingSlot(null)}>Cancel</button>
              <button
                className="btn btn-primary btn-sm"
                onClick={statusForm.handleSubmit(onStatusSubmit)}
                disabled={statusMut.isPending}
              >
                {statusMut.isPending ? 'Saving…' : 'Save'}
              </button>
            </>
          }
        >
          <form className="space-y-3" noValidate>
            <div className="text-sm text-gray-600">
              <strong>Site:</strong> {editingSlot.siteID} &nbsp;·&nbsp;
              <strong>Date:</strong> {formatDateOnly(editingSlot.date)} &nbsp;·&nbsp;
              <strong>Window:</strong> {formatTime(editingSlot.startTime)}–{formatTime(editingSlot.endTime)}
            </div>
            <Field label="New Status *">
              <select className="sh-select" {...statusForm.register('status', { required: true })}>
                <option value="AVAILABLE">AVAILABLE</option>
                <option value="BOOKED">BOOKED</option>
                <option value="CLOSED">CLOSED</option>
              </select>
            </Field>
            <p className="text-xs text-gray-500">Admin, ReceivingUser and WarehouseManager will be notified.</p>
          </form>
        </Modal>
      )}
    </div>
  )
}

/* ─── shared bits ─── */
function Field({ label, children }) {
  return <div><label className="sh-label">{label}</label>{children}</div>
}
const today = () => new Date().toISOString().slice(0, 10)
const toTimeSpan = (t) => t ? (t.length === 5 ? `${t}:00` : t) : '00:00:00'

// .NET TimeSpan serializes as "HH:mm:ss" / "HH:mm:ss.fffffff" depending on .NET version.
// Some versions also emit { ticks, hours, minutes, ... }. Handle both safely.
const formatTime = (t) => {
  if (t == null || t === '') return '—'
  if (typeof t === 'string') {
    const m = t.match(/(\d{1,2}):(\d{2})/)
    return m ? `${m[1].padStart(2, '0')}:${m[2]}` : t
  }
  if (typeof t === 'object') {
    const h = t.hours ?? 0
    const mi = t.minutes ?? 0
    return `${String(h).padStart(2, '0')}:${String(mi).padStart(2, '0')}`
  }
  return String(t)
}

// Slots are date-only — strip any time/tz portion so display doesn't shift by a day.
const formatDateOnly = (d) => {
  if (!d) return '—'
  const datePart = typeof d === 'string' ? d.split('T')[0] : d
  const dt = new Date(datePart)
  return isNaN(dt.getTime()) ? String(d) : dt.toLocaleDateString()
}

function extract(err) {
  const data = err?.response?.data
  if (!data) return null
  const firstModelError = data.errors ? Object.values(data.errors).flat().find(Boolean) : null
  return firstModelError ?? data.message ?? data.detail ?? (typeof data === 'string' ? data : null)
}
