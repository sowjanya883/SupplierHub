import { useState } from 'react'
import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query'
import { useForm } from 'react-hook-form'
import toast from 'react-hot-toast'
import { shippingApi } from '../../api/operations.api'
import { PageHeader, Spinner, EmptyState } from '../../components/ui/index'
import { StatusPill } from '../../components/ui/StatusPill'

// Backend exposes only POST endpoints + a slot-by-site GET (no list endpoints for shipments/ASNs).
// We hold "recent creations from this session" in this parent component so they
// persist across tab switches; they reset on a full page refresh.
export default function ShippingPage() {
  const [section, setSection] = useState('Shipment')
  const [shipments, setShipments] = useState([])
  const [asns, setAsns]           = useState([])
  const [asnItems, setAsnItems]   = useState([])

  return (
    <div>
      <PageHeader title="Shipments & ASN" subtitle="Create shipments, ASNs, ASN items and delivery slots" />

      <div className="flex gap-1 mb-4 border-b border-gray-200">
        {['Shipment','ASN','ASN Item','Delivery Slot'].map(t => (
          <button key={t} onClick={() => setSection(t)}
            className={`px-4 py-2 text-sm font-medium border-b-2 transition-colors ${
              section === t ? 'border-blue-600 text-blue-700' : 'border-transparent text-gray-500 hover:text-gray-700'}`}>
            {t}
          </button>
        ))}
      </div>

      {section === 'Shipment'      && <ShipmentSection items={shipments} onAdd={s => setShipments(prev => [s, ...prev])} />}
      {section === 'ASN'           && <AsnSection      items={asns}      onAdd={a => setAsns(prev => [a, ...prev])} />}
      {section === 'ASN Item'      && <AsnItemSection  items={asnItems}  onAdd={i => setAsnItems(prev => [i, ...prev])} />}
      {section === 'Delivery Slot' && <DeliverySlotSection />}
    </div>
  )
}

/* ─── Shipment ─── */
function ShipmentSection({ items, onAdd }) {
  const { register, handleSubmit, reset } = useForm()

  const mut = useMutation({
    mutationFn: shippingApi.createShipment,
    onSuccess: (res) => {
      const shipment = res?.data ?? res
      onAdd(shipment)
      reset()
      toast.success(`Shipment created${shipment?.shipmentID ? ` (#${shipment.shipmentID})` : ''}`)
    },
    onError: e => toast.error(extract(e) ?? 'Failed to create shipment'),
  })

  const onSubmit = (form) => mut.mutate({
    poID:       Number(form.poID),
    supplierID: Number(form.supplierID),
    shipDate:   form.shipDate ? new Date(form.shipDate).toISOString() : new Date().toISOString(),
    carrier:    form.carrier,
    trackingNo: form.trackingNo,
    status:     form.status,
  })

  return (
    <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
      <div className="sh-card">
        <h3 className="font-semibold text-gray-800 text-sm mb-3">Create Shipment</h3>
        <form className="space-y-3" onSubmit={handleSubmit(onSubmit)} noValidate>
          <Field label="PO ID *"><input type="number" className="sh-input" {...register('poID', { required: true })} /></Field>
          <Field label="Supplier ID *"><input type="number" className="sh-input" {...register('supplierID', { required: true })} /></Field>
          <Field label="Ship Date"><input type="date" className="sh-input" defaultValue={today()} {...register('shipDate')} /></Field>
          <Field label="Carrier"><input className="sh-input" placeholder="FedEx" {...register('carrier')} /></Field>
          <Field label="Tracking No"><input className="sh-input" {...register('trackingNo')} /></Field>
          <Field label="Status">
            <select className="sh-select" {...register('status')}>
              <option>Planned</option><option>Shipped</option><option>Delivered</option><option>Exception</option>
            </select>
          </Field>
          <button type="submit" className="btn btn-primary w-full justify-center" disabled={mut.isPending}>
            {mut.isPending ? 'Creating…' : 'Create Shipment'}
          </button>
        </form>
      </div>

      <RecentList title={`Recent shipments (${items.length})`} items={items}
        render={s => (
          <div className="text-sm">
            <div><span className="text-gray-500">ID:</span> #{s.shipmentID} — <span className="text-gray-500">PO:</span> {s.poID}</div>
            <div><span className="text-gray-500">Carrier:</span> {s.carrier} <span className="text-gray-400 ml-2">{s.trackingNo}</span></div>
            <div><StatusPill status={s.status} /></div>
          </div>
        )} />
    </div>
  )
}

/* ─── ASN ─── */
function AsnSection({ items, onAdd }) {
  const { register, handleSubmit, reset } = useForm()

  const mut = useMutation({
    mutationFn: shippingApi.createAsn,
    onSuccess: (res) => {
      const asn = res?.data ?? res
      onAdd(asn)
      reset()
      toast.success(`ASN created${asn?.asnid ? ` (#${asn.asnid})` : ''}`)
    },
    onError: e => toast.error(extract(e) ?? 'Failed to create ASN'),
  })

  const onSubmit = (form) => mut.mutate({
    shipmentID:  Number(form.shipmentID),
    asnNo:       form.asnNo,
    createdDate: form.createdDate ? new Date(form.createdDate).toISOString() : new Date().toISOString(),
    status:      form.status,
  })

  return (
    <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
      <div className="sh-card">
        <h3 className="font-semibold text-gray-800 text-sm mb-3">Create ASN</h3>
        <form className="space-y-3" onSubmit={handleSubmit(onSubmit)} noValidate>
          <Field label="Shipment ID *"><input type="number" className="sh-input" {...register('shipmentID', { required: true })} /></Field>
          <Field label="ASN No *"><input className="sh-input" placeholder="ASN-2026-001" {...register('asnNo', { required: true })} /></Field>
          <Field label="Created Date"><input type="date" className="sh-input" defaultValue={today()} {...register('createdDate')} /></Field>
          <Field label="Status">
            <select className="sh-select" {...register('status')}>
              <option>Open</option><option>Posted</option>
            </select>
          </Field>
          <button type="submit" className="btn btn-primary w-full justify-center" disabled={mut.isPending}>
            {mut.isPending ? 'Creating…' : 'Create ASN'}
          </button>
        </form>
      </div>

      <RecentList title={`Recent ASNs (${items.length})`} items={items}
        render={a => (
          <div className="text-sm">
            <div><span className="text-gray-500">ID:</span> #{a.asnid ?? a.asnID} — {a.asnNo}</div>
            <div><span className="text-gray-500">Shipment:</span> {a.shipmentID}</div>
            <div><StatusPill status={a.status} /></div>
          </div>
        )} />
    </div>
  )
}

/* ─── ASN Item ─── */
function AsnItemSection({ items, onAdd }) {
  const { register, handleSubmit, reset } = useForm()

  const mut = useMutation({
    mutationFn: shippingApi.addAsnItem,
    onSuccess: (res) => {
      const item = res?.data ?? res
      onAdd(item)
      reset()
      toast.success('ASN item added')
    },
    onError: e => toast.error(extract(e) ?? 'Failed to add ASN item'),
  })

  const onSubmit = (form) => mut.mutate({
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
        <form className="space-y-3" onSubmit={handleSubmit(onSubmit)} noValidate>
          <Field label="ASN ID *"><input type="number" className="sh-input" {...register('asnid', { required: true })} /></Field>
          <Field label="PO Line ID *"><input type="number" className="sh-input" {...register('poLineID', { required: true })} /></Field>
          <Field label="Shipped Qty *"><input type="number" step="0.01" className="sh-input" {...register('shippedQty', { required: true })} /></Field>
          <Field label="Lot / Batch"><input className="sh-input" {...register('lotBatch')} /></Field>
          <Field label="Serial (JSON)"><input className="sh-input font-mono" {...register('serialJSON')} /></Field>
          <Field label="Notes"><textarea rows={2} className="sh-input" {...register('notes')} /></Field>
          <button type="submit" className="btn btn-primary w-full justify-center" disabled={mut.isPending}>
            {mut.isPending ? 'Saving…' : 'Add ASN Item'}
          </button>
        </form>
      </div>

      <RecentList title={`Recent ASN items (${items.length})`} items={items}
        render={i => (
          <div className="text-sm">
            <div><span className="text-gray-500">ASN:</span> {i.asnid ?? i.asnID} — <span className="text-gray-500">PO Line:</span> {i.poLineID}</div>
            <div><span className="text-gray-500">Qty:</span> {i.shippedQty} {i.lotBatch && <span className="text-gray-400 ml-2">{i.lotBatch}</span>}</div>
          </div>
        )} />
    </div>
  )
}

/* ─── Delivery Slot ─── */
function DeliverySlotSection() {
  const qc = useQueryClient()
  const [siteId, setSiteId] = useState('')
  const [activeSite, setActiveSite] = useState(null)
  const { register, handleSubmit, reset } = useForm()

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
          <strong>Site ID</strong> is an external identifier for the delivery dock /
          warehouse. There is no Sites table in this build — use any positive
          integer (e.g. <code className="font-mono">1</code> for your default site)
          and reuse it consistently.
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
            <p className="text-[11px] text-gray-500 mt-1">Only <code>AVAILABLE</code> slots show up in the lookup.</p>
          </Field>
          <button type="submit" className="btn btn-primary w-full justify-center" disabled={mut.isPending}>
            {mut.isPending ? 'Creating…' : 'Create Slot'}
          </button>
        </form>
      </div>

      <div className="sh-card">
        <h3 className="font-semibold text-gray-800 text-sm mb-3">Lookup AVAILABLE Slots by Site</h3>
        <div className="flex gap-2 mb-3">
          <input type="number" className="sh-input flex-1" placeholder="Site ID" value={siteId} onChange={e => setSiteId(e.target.value)} />
          <button className="btn btn-secondary btn-sm" onClick={() => setActiveSite(siteId ? Number(siteId) : null)} disabled={!siteId}>Load</button>
        </div>
        {!activeSite ? <p className="text-sm text-gray-500">Enter a site ID and click Load.</p>
          : isLoading ? <Spinner />
          : slots.length === 0 ? <EmptyState message="No AVAILABLE slots for this site." />
          : (
            <table className="sh-table">
              <thead><tr><th>Slot</th><th>Date</th><th>Start</th><th>End</th><th>Capacity</th><th>Status</th></tr></thead>
              <tbody>{slots.map(s => (
                <tr key={s.slotID}>
                  <td className="text-gray-400 text-xs">{s.slotID}</td>
                  <td className="text-xs text-gray-500">{s.date ? new Date(s.date).toLocaleDateString() : '—'}</td>
                  <td>{formatTime(s.startTime)}</td><td>{formatTime(s.endTime)}</td><td>{s.capacity}</td>
                  <td><StatusPill status={s.status} /></td>
                </tr>
              ))}</tbody>
            </table>
          )}
      </div>
    </div>
  )
}

/* ─── Reusable bits ─── */
function Field({ label, children }) {
  return <div><label className="sh-label">{label}</label>{children}</div>
}

function RecentList({ title, items, render }) {
  return (
    <div className="sh-card">
      <h3 className="font-semibold text-gray-800 text-sm mb-3">{title}</h3>
      {items.length === 0
        ? <EmptyState message="None created yet." />
        : <div className="divide-y">{items.map((it, i) => <div key={i} className="py-2">{render(it)}</div>)}</div>}
    </div>
  )
}

const today = () => new Date().toISOString().slice(0, 10)
// "09:00" -> "09:00:00" so .NET TimeSpan parses cleanly
const toTimeSpan = (t) => t ? (t.length === 5 ? `${t}:00` : t) : '00:00:00'
const formatTime = (t) => typeof t === 'string' ? t.slice(0, 5) : ''

function extract(err) {
  const data = err?.response?.data
  if (!data) return null
  const firstModelError = data.errors ? Object.values(data.errors).flat().find(Boolean) : null
  return firstModelError ?? data.message ?? (typeof data === 'string' ? data : null)
}
