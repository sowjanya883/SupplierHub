import { useState } from 'react'
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query'
import { useForm } from 'react-hook-form'
import toast from 'react-hot-toast'
import { catalogsApi, catalogItemsApi } from '../../api/catalog.api'
import { suppliersApi } from '../../api/suppliers.api'
import { itemsApi } from '../../api/catalog.api'
import { StatusPill } from '../../components/ui/StatusPill'
import { PageHeader, Spinner, EmptyState, ConfirmDialog } from '../../components/ui/index'
import Modal from '../../components/ui/Modal'
import useAuthStore from '../../store/auth.store'

export default function CatalogsPage() {
  const qc = useQueryClient()
  const { user } = useAuthStore()

  const isAdmin           = user?.roles?.includes('Admin')
  const isCategoryManager = user?.roles?.includes('CategoryManager')
  const canCreateEdit     = isAdmin || isCategoryManager
  const canDelete         = isAdmin

  // ── Modal & selection state ────────────────────────────
  const [catalogModal, setCatalogModal]   = useState(null) // 'form' | 'delete'
  const [itemModal, setItemModal]         = useState(null) // 'form' | 'delete'
  const [selectedCatalog, setSelectedCatalog] = useState(null)
  const [selectedItem, setSelectedItem]   = useState(null)
  const [activeCatalog, setActiveCatalog] = useState(null) // catalog whose items are shown
  const [search, setSearch]               = useState('')
  const [loadingEdit, setLoadingEdit]     = useState(false)

  // ── Fetch catalogs ─────────────────────────────────────
  const { data: catData, isLoading: catLoading } = useQuery({
    queryKey: ['catalogs'],
    queryFn: catalogsApi.getAll,
  })
  const catalogs = (catData?.data ?? catData ?? []).filter(c =>
    (c.catalogName ?? '').toLowerCase().includes(search.toLowerCase())
  )

  // ── Fetch all catalog items (filter client-side) ───────
  const { data: itemsData, isLoading: itemsLoading } = useQuery({
    queryKey: ['catalog-items'],
    queryFn: catalogItemsApi.getAll,
    enabled: !!activeCatalog,
  })
  const allCatalogItems = itemsData?.data ?? itemsData ?? []
  // Filter by active catalog — works if backend returns catalogID field
  const catalogItems = activeCatalog
    ? allCatalogItems.filter(i =>
        i.catalogID === activeCatalog.catalogID ||
        i.CatalogID === activeCatalog.catalogID
      )
    : []

  // ── Fetch suppliers & items for dropdowns ──────────────
  const { data: suppData } = useQuery({
    queryKey: ['suppliers'],
    queryFn: suppliersApi.getAll,
  })
  const suppliers = suppData?.data ?? suppData ?? []

  const { data: masterItemsData } = useQuery({
    queryKey: ['items'],
    queryFn: itemsApi.getAll,
  })
  const masterItems = masterItemsData?.data ?? masterItemsData ?? []

  // ── Catalog form ───────────────────────────────────────
  const {
    register: regCat,
    handleSubmit: hsCat,
    reset: resetCat,
    formState: { errors: errCat },
  } = useForm()

  // ── CatalogItem form ───────────────────────────────────
  const {
    register: regItem,
    handleSubmit: hsItem,
    reset: resetItem,
    formState: { errors: errItem },
  } = useForm()

  // ── Catalog mutations ──────────────────────────────────
  const createCatMut = useMutation({
    mutationFn: catalogsApi.create,
    onSuccess: () => {
      qc.invalidateQueries(['catalogs'])
      setCatalogModal(null)
      toast.success('Catalog created successfully')
    },
    onError: e => toast.error(e.response?.data?.message ?? 'Failed to create catalog'),
  })
  const updateCatMut = useMutation({
    mutationFn: ({ id, dto }) => catalogsApi.update(id, dto),
    onSuccess: () => {
      qc.invalidateQueries(['catalogs'])
      setCatalogModal(null)
      toast.success('Catalog updated successfully')
    },
    onError: e => toast.error(e.response?.data?.message ?? 'Failed to update catalog'),
  })
  const deleteCatMut = useMutation({
    mutationFn: (dto) => catalogsApi.delete(dto),
    onSuccess: () => {
      qc.invalidateQueries(['catalogs'])
      setCatalogModal(null)
      if (activeCatalog?.catalogID === selectedCatalog?.catalogID)
        setActiveCatalog(null)
      toast.success('Catalog deleted successfully')
    },
    onError: e => toast.error(e.response?.data?.message ?? 'Failed to delete catalog'),
  })

  // ── CatalogItem mutations ──────────────────────────────
  const createItemMut = useMutation({
    mutationFn: catalogItemsApi.create,
    onSuccess: () => {
      qc.invalidateQueries(['catalog-items'])
      setItemModal(null)
      toast.success('Catalog item added successfully')
    },
    onError: e => toast.error(e.response?.data?.message ?? 'Failed to add item'),
  })
  const updateItemMut = useMutation({
    mutationFn: ({ id, dto }) => catalogItemsApi.update(id, dto),
    onSuccess: () => {
      qc.invalidateQueries(['catalog-items'])
      setItemModal(null)
      toast.success('Catalog item updated successfully')
    },
    onError: e => toast.error(e.response?.data?.message ?? 'Failed to update item'),
  })
  const deleteItemMut = useMutation({
    mutationFn: (dto) => catalogItemsApi.delete(dto),
    onSuccess: () => {
      qc.invalidateQueries(['catalog-items'])
      setItemModal(null)
      toast.success('Catalog item deleted successfully')
    },
    onError: e => toast.error(e.response?.data?.message ?? 'Failed to delete item'),
  })

  // ── Catalog handlers ───────────────────────────────────
  const openCreateCatalog = () => {
    resetCat({
      SupplierID:  '',
      CatalogName: '',
      ValidFrom:   '',
      ValidTo:     '',
      Status:      'Active',
    })
    setSelectedCatalog(null)
    setCatalogModal('form')
  }

  const openEditCatalog = async (row) => {
    setLoadingEdit(true)
    try {
      const res  = await catalogsApi.getById(row.catalogID)
      const full = res?.data ?? res
      setSelectedCatalog(full)
      resetCat({
        SupplierID:  full.supplierID  ?? '',
        CatalogName: full.catalogName ?? '',
        ValidFrom:   full.validFrom?.split('T')[0] ?? '',
        ValidTo:     full.validTo?.split('T')[0]   ?? '',
        Status:      full.status      ?? 'Active',
      })
      setCatalogModal('form')
    } catch {
      toast.error('Failed to load catalog details')
    } finally {
      setLoadingEdit(false)
    }
  }

  const onSubmitCatalog = (d) => {
    const payload = {
      SupplierID:  Number(d.SupplierID),
      CatalogName: d.CatalogName,
      ValidFrom:   d.ValidFrom || null,
      ValidTo:     d.ValidTo   || null,
      Status:      d.Status,
    }
    if (selectedCatalog) {
      updateCatMut.mutate({
        id:  selectedCatalog.catalogID,
        dto: { ...payload, CatalogID: selectedCatalog.catalogID },
      })
    } else {
      createCatMut.mutate(payload)
    }
  }

  // ── CatalogItem handlers ───────────────────────────────
  const openAddItem = () => {
    resetItem({
      ItemID:         '',
      Price:          '',
      Currency:       'INR',
      MinOrderQty:    '',
      PriceBreaksJson:'',
      Status:         'Active',
    })
    setSelectedItem(null)
    setItemModal('form')
  }

  const openEditItem = async (row) => {
    setLoadingEdit(true)
    try {
      const res  = await catalogItemsApi.getById(row.catItemID)
      const full = res?.data ?? res
      setSelectedItem(full)
      resetItem({
        ItemID:          full.itemID          ?? '',
        Price:           full.price           ?? '',
        Currency:        full.currency        ?? 'INR',
        MinOrderQty:     full.minOrderQty     ?? '',
        PriceBreaksJson: full.priceBreaksJson ?? '',
        Status:          full.status          ?? 'Active',
      })
      setItemModal('form')
    } catch {
      toast.error('Failed to load catalog item details')
    } finally {
      setLoadingEdit(false)
    }
  }

  const onSubmitItem = (d) => {
    const payload = {
      CatalogID:       activeCatalog.catalogID,
      ItemID:          Number(d.ItemID),
      Price:           Number(d.Price),
      Currency:        d.Currency,
      MinOrderQty:     d.MinOrderQty ? Number(d.MinOrderQty) : null,
      PriceBreaksJson: d.PriceBreaksJson || null,
      Status:          d.Status,
    }
    if (selectedItem) {
      updateItemMut.mutate({
        id:  selectedItem.catItemID,
        dto: { ...payload, CatItemID: selectedItem.catItemID },
      })
    } else {
      createItemMut.mutate(payload)
    }
  }

  const isCatPending  = createCatMut.isPending  || updateCatMut.isPending
  const isItemPending = createItemMut.isPending  || updateItemMut.isPending

  // ── Render ─────────────────────────────────────────────
  return (
    <div>
      <PageHeader
        title="Catalogs"
        subtitle="Supplier price catalogs and item pricing"
        action={
          canCreateEdit && (
            <button className="btn btn-primary" onClick={openCreateCatalog}>
              + New Catalog
            </button>
          )
        }
      />

      {/* Search */}
      <div className="mb-4">
        <input
          className="sh-input max-w-xs"
          placeholder="Search by catalog name…"
          value={search}
          onChange={e => setSearch(e.target.value)}
        />
      </div>

      {/* ── Catalogs Table ──────────────────────────────── */}
      <div className="sh-card p-0 overflow-hidden mb-5">
        {catLoading ? (
          <div className="flex justify-center py-12"><Spinner /></div>
        ) : catalogs.length === 0 ? (
          <EmptyState message="No catalogs found." />
        ) : (
          <table className="sh-table">
            <thead>
              <tr>
                <th>Sr. No.</th>
                <th>ID</th>
                <th>Catalog Name</th>
                <th>Status</th>
                <th>Actions</th>
              </tr>
            </thead>
            <tbody>
              {catalogs.map((c, index) => (
                <tr
                  key={c.catalogID}
                  className={activeCatalog?.catalogID === c.catalogID ? 'bg-blue-50' : ''}
                >
                  <td className="text-gray-400 text-xs">{index + 1}</td>
                  <td className="text-gray-400 text-xs">{c.catalogID}</td>
                  <td className="font-medium">{c.catalogName}</td>
                  <td><StatusPill status={c.status} /></td>
                  <td>
                    <div className="flex gap-1">
                      {/* View Items — all allowed roles */}
                      <button
                        className={`btn btn-sm ${
                          activeCatalog?.catalogID === c.catalogID
                            ? 'btn-primary'
                            : 'btn-secondary'
                        }`}
                        onClick={() =>
                          setActiveCatalog(
                            activeCatalog?.catalogID === c.catalogID ? null : c
                          )
                        }
                      >
                        {activeCatalog?.catalogID === c.catalogID
                          ? 'Hide Items'
                          : 'View Items'}
                      </button>

                      {/* Edit — Admin + CategoryManager */}
                      {canCreateEdit && (
                        <button
                          className="btn btn-ghost btn-sm"
                          onClick={() => openEditCatalog(c)}
                          disabled={loadingEdit}
                        >
                          Edit
                        </button>
                      )}

                      {/* Delete — Admin only */}
                      {canDelete && (
                        <button
                          className="btn btn-danger btn-sm"
                          onClick={() => {
                            setSelectedCatalog(c)
                            setCatalogModal('delete')
                          }}
                        >
                          Delete
                        </button>
                      )}
                    </div>
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
        )}
      </div>

      {/* ── Catalog Items Section ────────────────────────── */}
      {activeCatalog && (
        <div className="sh-card p-0 overflow-hidden">
          {/* Items header */}
          <div className="flex items-center justify-between px-4 py-3 border-b border-gray-100">
            <div>
              <h3 className="font-semibold text-gray-800 text-sm">
                Items in — {activeCatalog.catalogName}
              </h3>
              <p className="text-xs text-gray-400 mt-0.5">
                Catalog ID: {activeCatalog.catalogID}
              </p>
            </div>
            {canCreateEdit && (
              <button className="btn btn-primary btn-sm" onClick={openAddItem}>
                + Add Item
              </button>
            )}
          </div>

          {itemsLoading ? (
            <div className="flex justify-center py-8"><Spinner /></div>
          ) : catalogItems.length === 0 ? (
            <div className="py-8 text-center">
              <p className="text-sm text-gray-400">No items in this catalog.</p>
              {canCreateEdit && (
                <button
                  className="btn btn-primary btn-sm mt-3"
                  onClick={openAddItem}
                >
                  + Add First Item
                </button>
              )}
            </div>
          ) : (
            <table className="sh-table">
              <thead>
                <tr>
                  <th>Sr. No.</th>
                  <th>Cat Item ID</th>
                  <th>Item ID</th>
                  <th>Price</th>
                  <th>Currency</th>
                  <th>Min Order Qty</th>
                  <th>Status</th>
                  {canCreateEdit && <th>Actions</th>}
                </tr>
              </thead>
              <tbody>
                {catalogItems.map((ci, index) => (
                  <tr key={ci.catItemID}>
                    <td className="text-gray-400 text-xs">{index + 1}</td>
                    <td className="text-gray-400 text-xs">{ci.catItemID}</td>
                    <td className="font-medium">{ci.itemID}</td>
                    <td className="font-semibold text-green-700">
                      {ci.price?.toLocaleString()}
                    </td>
                    <td>{ci.currency}</td>
                    <td>{ci.minOrderQty ?? '—'}</td>
                    <td><StatusPill status={ci.status} /></td>
                    {canCreateEdit && (
                      <td>
                        <div className="flex gap-1">
                          <button
                            className="btn btn-ghost btn-sm"
                            onClick={() => openEditItem(ci)}
                            disabled={loadingEdit}
                          >
                            Edit
                          </button>
                          <button
                            className="btn btn-danger btn-sm"
                            onClick={() => {
                              setSelectedItem(ci)
                              setItemModal('delete')
                            }}
                          >
                            Delete
                          </button>
                        </div>
                      </td>
                    )}
                  </tr>
                ))}
              </tbody>
            </table>
          )}

          {/* Backend fix note */}
          <div className="px-4 py-2 bg-amber-50 border-t border-amber-100">
            <p className="text-xs text-amber-700">
              ⚠ Items may not filter correctly until you add <code className="bg-amber-100 px-1 rounded">CatalogID</code> to
              <code className="bg-amber-100 px-1 rounded ml-1">CatalogItemGetAllDto.cs</code> in the backend.
            </p>
          </div>
        </div>
      )}

      {/* ── Catalog Create / Edit Modal ──────────────────── */}
      {catalogModal === 'form' && (
        <Modal
          title={selectedCatalog ? `Edit — ${selectedCatalog.catalogName}` : 'New Catalog'}
          onClose={() => setCatalogModal(null)}
          footer={
            <>
              <button className="btn btn-secondary btn-sm" onClick={() => setCatalogModal(null)}>
                Cancel
              </button>
              <button
                className="btn btn-primary btn-sm"
                onClick={hsCat(onSubmitCatalog)}
                disabled={isCatPending}
              >
                {isCatPending ? 'Saving…' : selectedCatalog ? 'Update' : 'Create'}
              </button>
            </>
          }
        >
          <form className="space-y-3" noValidate>
            <div>
              <label className="sh-label">Supplier *</label>
              <select
                className="sh-select"
                {...regCat('SupplierID', { required: 'Supplier is required' })}
              >
                <option value="">— Select a supplier —</option>
                {suppliers.map(s => (
                  <option key={s.supplierID} value={s.supplierID}>
                    {s.legalName}
                  </option>
                ))}
              </select>
              {errCat.SupplierID && (
                <p className="text-red-500 text-xs mt-1">{errCat.SupplierID.message}</p>
              )}
            </div>

            <div>
              <label className="sh-label">Catalog Name *</label>
              <input
                className="sh-input"
                placeholder="e.g. Q1 2025 Steel Pricing"
                {...regCat('CatalogName', { required: 'Catalog name is required' })}
              />
              {errCat.CatalogName && (
                <p className="text-red-500 text-xs mt-1">{errCat.CatalogName.message}</p>
              )}
            </div>

            <div className="grid grid-cols-2 gap-3">
              <div>
                <label className="sh-label">Valid From</label>
                <input type="date" className="sh-input" {...regCat('ValidFrom')} />
              </div>
              <div>
                <label className="sh-label">Valid To</label>
                <input type="date" className="sh-input" {...regCat('ValidTo')} />
              </div>
            </div>

            <div>
              <label className="sh-label">Status *</label>
              <select className="sh-select" {...regCat('Status', { required: true })}>
                <option>Active</option>
                <option>Inactive</option>
              </select>
            </div>
          </form>
        </Modal>
      )}

      {/* ── Catalog Delete Confirm ───────────────────────── */}
      {catalogModal === 'delete' && (
        <ConfirmDialog
          message={`Delete catalog "${selectedCatalog?.catalogName}"? All items in this catalog will also be removed.`}
          onConfirm={() =>
            deleteCatMut.mutate({ CatalogID: selectedCatalog.catalogID })
          }
          onCancel={() => setCatalogModal(null)}
          loading={deleteCatMut.isPending}
        />
      )}

      {/* ── Catalog Item Create / Edit Modal ─────────────── */}
      {itemModal === 'form' && (
        <Modal
          title={
            selectedItem
              ? `Edit Item in ${activeCatalog?.catalogName}`
              : `Add Item to ${activeCatalog?.catalogName}`
          }
          onClose={() => setItemModal(null)}
          footer={
            <>
              <button className="btn btn-secondary btn-sm" onClick={() => setItemModal(null)}>
                Cancel
              </button>
              <button
                className="btn btn-primary btn-sm"
                onClick={hsItem(onSubmitItem)}
                disabled={isItemPending}
              >
                {isItemPending ? 'Saving…' : selectedItem ? 'Update' : 'Add'}
              </button>
            </>
          }
        >
          <form className="space-y-3" noValidate>
            <div>
              <label className="sh-label">Item *</label>
              <select
                className="sh-select"
                {...regItem('ItemID', { required: 'Item is required' })}
              >
                <option value="">— Select an item —</option>
                {masterItems.map(i => (
                  <option key={i.itemID} value={i.itemID}>
                    {i.sku}
                  </option>
                ))}
              </select>
              {errItem.ItemID && (
                <p className="text-red-500 text-xs mt-1">{errItem.ItemID.message}</p>
              )}
            </div>

            <div className="grid grid-cols-2 gap-3">
              <div>
                <label className="sh-label">Price *</label>
                <input
                  type="number"
                  step="0.01"
                  min="0"
                  className="sh-input"
                  placeholder="e.g. 1500.00"
                  {...regItem('Price', { required: 'Price is required' })}
                />
                {errItem.Price && (
                  <p className="text-red-500 text-xs mt-1">{errItem.Price.message}</p>
                )}
              </div>
              <div>
                <label className="sh-label">Currency *</label>
                <select
                  className="sh-select"
                  {...regItem('Currency', { required: true })}
                >
                  <option>INR</option>
                  <option>USD</option>
                  <option>EUR</option>
                  <option>GBP</option>
                </select>
              </div>
            </div>

            <div>
              <label className="sh-label">Min Order Qty</label>
              <input
                type="number"
                min="0"
                step="0.01"
                className="sh-input"
                placeholder="e.g. 10"
                {...regItem('MinOrderQty')}
              />
            </div>

            <div>
              <label className="sh-label">
                Price Breaks (JSON)
                <span className="text-gray-400 font-normal ml-1">(optional)</span>
              </label>
              <input
                className="sh-input font-mono text-xs"
                placeholder='[{"qty":100,"price":1400},{"qty":500,"price":1300}]'
                {...regItem('PriceBreaksJson')}
              />
              <p className="text-gray-400 text-xs mt-1">
                Bulk pricing tiers — quantity thresholds with discounted prices
              </p>
            </div>

            <div>
              <label className="sh-label">Status *</label>
              <select className="sh-select" {...regItem('Status', { required: true })}>
                <option>Active</option>
                <option>Inactive</option>
              </select>
            </div>
          </form>
        </Modal>
      )}

      {/* ── Catalog Item Delete Confirm ──────────────────── */}
      {itemModal === 'delete' && (
        <ConfirmDialog
          message={`Remove item from catalog? This cannot be undone.`}
          onConfirm={() =>
            deleteItemMut.mutate({ CatItemID: selectedItem.catItemID })
          }
          onCancel={() => setItemModal(null)}
          loading={deleteItemMut.isPending}
        />
      )}
    </div>
  )
}