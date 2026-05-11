import { useParams, useNavigate } from 'react-router-dom'
import { useQuery } from '@tanstack/react-query'
import { suppliersApi } from '../../api/suppliers.api'
import { supplierContactsApi, supplierRisksApi } from '../../api/supplier.extras.api'
import { StatusPill } from '../../components/ui/StatusPill'
import { Spinner } from '../../components/ui/index'

function Section({ title, children }) {
  return (
    <div className="sh-card mb-4">
      <h3 className="font-semibold text-gray-800 text-sm mb-3">{title}</h3>
      {children}
    </div>
  )
}

export default function SupplierDetailPage() {
  const { id } = useParams()
  const navigate = useNavigate()

  const { data, isLoading } = useQuery({
    queryKey: ['supplier', id],
    queryFn: () => suppliersApi.getById(id),
  })
  const { data: contactsData } = useQuery({
    queryKey: ['supplier-contacts', id],
    queryFn: () => suppliersApi.getById(id), // replace when contacts-by-supplier endpoint available
  })

  if (isLoading) return <div className="flex justify-center py-16"><Spinner /></div>

  const s = data?.data ?? data
  if (!s) return <p className="text-gray-500">Supplier not found.</p>

  return (
    <div>
      <div className="page-header">
        <div>
          <button onClick={() => navigate(-1)} className="text-sm text-blue-600 hover:underline mb-1 block">
            ← Back
          </button>
          <h1 className="page-title">{s.legalName}</h1>
        </div>
        <StatusPill status={s.status} />
      </div>

      <Section title="Supplier Details">
        <div className="grid grid-cols-2 gap-4 text-sm">
          <div><span className="text-gray-500">Supplier ID</span><p className="font-medium mt-0.5">#{s.supplierID}</p></div>
          <div><span className="text-gray-500">Tax ID</span><p className="font-medium mt-0.5">{s.taxID || '—'}</p></div>
          <div><span className="text-gray-500">DUNS / Reg No</span><p className="font-medium mt-0.5">{s.dunsOrRegNo || '—'}</p></div>
          <div><span className="text-gray-500">Status</span><p className="mt-0.5"><StatusPill status={s.status} /></p></div>
        </div>
      </Section>

      <Section title="Bank & Tax Info">
        <pre className="text-xs bg-gray-50 rounded-lg p-3 text-gray-700 overflow-auto">
          {s.bankInfoJSON ? JSON.stringify(JSON.parse(s.bankInfoJSON), null, 2) : 'No bank info'}
        </pre>
      </Section>
    </div>
  )
}
