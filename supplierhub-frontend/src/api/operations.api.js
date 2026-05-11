import api from './axiosClient'

/* ── Shipping / ASN ─────────────────────────────────────── */
export const shippingApi = {
  // Shipments
  getAllShipments:   ()        => api.get('/api/shipping/shipments').then(r => r.data),
  getShipmentById:  (id)      => api.get(`/api/shipping/shipments/${id}`).then(r => r.data),
  createShipment:   (dto)     => api.post('/api/shipping/shipments', dto).then(r => r.data),
  updateShipment:   (id, dto) => api.put(`/api/shipping/shipments/${id}`, dto).then(r => r.data),

  // ASN
  getAllAsns:        ()        => api.get('/api/shipping/asn').then(r => r.data),
  getAsnById:       (id)      => api.get(`/api/shipping/asn/${id}`).then(r => r.data),
  createAsn:        (dto)     => api.post('/api/shipping/asn', dto).then(r => r.data),
  updateAsn:        (id, dto) => api.put(`/api/shipping/asn/${id}`, dto).then(r => r.data),

  // ASN Items
  getAsnItems:      (asnId)   => api.get(`/api/shipping/asn/${asnId}/items`).then(r => r.data),
  addAsnItem:       (dto)     => api.post('/api/shipping/asn/items', dto).then(r => r.data),
  updateAsnItem:    (id, dto) => api.put(`/api/shipping/asn/items/${id}`, dto).then(r => r.data),

  // Delivery Slots
  getSlots:         (siteId)  => api.get(`/api/shipping/slots/${siteId}`).then(r => r.data),
  createSlot:       (dto)     => api.post('/api/shipping/slots', dto).then(r => r.data),
  updateSlot:       (id, dto) => api.put(`/api/shipping/slots/${id}`, dto).then(r => r.data),
}

/* ── GRN ────────────────────────────────────────────────── */
export const grnApi = {
  getAll:  ()        => api.get('/api/grn').then(r => r.data),
  getById: (id)      => api.get(`/api/grn/${id}`).then(r => r.data),
  create:  (dto)     => api.post('/api/grn', dto).then(r => r.data),
  update:  (id, dto) => api.put(`/api/grn/${id}`, dto).then(r => r.data),
  delete:  (id)      => api.delete(`/api/grn/${id}`).then(r => r.data),
}

export const grnItemApi = {
  getAll:       ()      => api.get('/api/grnitem').then(r => r.data),
  getById:      (id)    => api.get(`/api/grnitem/${id}`).then(r => r.data),
  getByGrnId:   (grnId) => api.get(`/api/grnitem/grn/${grnId}`).then(r => r.data),
  create:       (dto)   => api.post('/api/grnitem', dto).then(r => r.data),
  update:       (id, dto) => api.put(`/api/grnitem/${id}`, dto).then(r => r.data),
  delete:       (id)    => api.delete(`/api/grnitem/${id}`).then(r => r.data),
}

export const inspectionApi = {
  getAll:       ()          => api.get('/api/inspection').then(r => r.data),
  getById:      (id)        => api.get(`/api/inspection/${id}`).then(r => r.data),
  getByItemId:  (grnItemId) => api.get(`/api/inspection/item/${grnItemId}`).then(r => r.data),
  create:       (dto)       => api.post('/api/inspection', dto).then(r => r.data),
  update:       (id, dto)   => api.put(`/api/inspection/${id}`, dto).then(r => r.data),
  delete:       (id)        => api.delete(`/api/inspection/${id}`).then(r => r.data),
}

export const ncrApi = {
  getAll:      ()          => api.get('/api/ncr').then(r => r.data),
  getById:     (id)        => api.get(`/api/ncr/${id}`).then(r => r.data),
  getByItemId: (grnItemId) => api.get(`/api/ncr/item/${grnItemId}`).then(r => r.data),
  create:      (dto)       => api.post('/api/ncr', dto).then(r => r.data),
  update:      (id, dto)   => api.put(`/api/ncr/${id}`, dto).then(r => r.data),
  delete:      (id)        => api.delete(`/api/ncr/${id}`).then(r => r.data),
}

/* ── Invoices ───────────────────────────────────────────── */
export const invoicesApi = {
  getAll:    ()      => api.get('/api/invoices').then(r => r.data),
  getById:   (id)    => api.get(`/api/invoices/${id}`).then(r => r.data),
  getByPoId: (poId)  => api.get(`/api/invoices/po/${poId}`).then(r => r.data),
  create:    (dto)   => api.post('/api/invoices', dto).then(r => r.data),
  update:    (id, dto) => api.put(`/api/invoices/${id}`, dto).then(r => r.data),
}

export const invoiceLinesApi = {
  getByInvoiceId: (invId) => api.get(`/api/invoicelines/invoice/${invId}`).then(r => r.data),
  getById:        (id)    => api.get(`/api/invoicelines/${id}`).then(r => r.data),
  create:         (dto)   => api.post('/api/invoicelines', dto).then(r => r.data),
  update:         (id, dto) => api.put(`/api/invoicelines/${id}`, dto).then(r => r.data),
}

export const matchRefsApi = {
  getByInvoiceId: (invId) => api.get(`/api/matchrefs/invoice/${invId}`).then(r => r.data),
  getById:        (id)    => api.get(`/api/matchrefs/${id}`).then(r => r.data),
  create:         (dto)   => api.post('/api/matchrefs', dto).then(r => r.data),
  update:         (id, dto) => api.put(`/api/matchrefs/${id}`, dto).then(r => r.data),
}

/* ── Notifications ──────────────────────────────────────── */
export const notificationsApi = {
  getAll:       ()      => api.get('/api/notification').then(r => r.data),
  getById:      (id)    => api.get(`/api/notification/${id}`).then(r => r.data),
  create:       (dto)   => api.post('/api/notification', dto).then(r => r.data),
  updateStatus: (id, dto) => api.patch(`/api/notification/${id}`, dto).then(r => r.data),
  restore:      (id)    => api.post(`/api/notification/${id}/restore`).then(r => r.data),
}

/* ── Users ──────────────────────────────────────────────── */
export const usersApi = {
  getAll:  (includeDeleted = false) => api.get('/api/user', { params: { includeDeleted } }).then(r => r.data),
  getById: (id)      => api.get(`/api/user/${id}`).then(r => r.data),
  create:  (dto)     => api.post('/api/user', dto).then(r => r.data),
  update:  (id, dto) => api.put(`/api/user/${id}`, dto).then(r => r.data),
  delete:  (id)      => api.delete(`/api/user/${id}`).then(r => r.data),
}

export const rolesApi = {
  getAll:  (includeDeleted = false) => api.get('/api/role', { params: { includeDeleted } }).then(r => r.data),
  getById: (id)      => api.get(`/api/role/${id}`).then(r => r.data),
  create:  (dto)     => api.post('/api/role', dto).then(r => r.data),
  update:  (id, dto) => api.put(`/api/role/${id}`, dto).then(r => r.data),
  delete:  (id)      => api.delete(`/api/role/${id}`).then(r => r.data),
}

export const permissionsApi = {
  getAll:  (includeDeleted = false) => api.get('/api/permission', { params: { includeDeleted } }).then(r => r.data),
  getById: (id)      => api.get(`/api/permission/${id}`).then(r => r.data),
  create:  (dto)     => api.post('/api/permission', dto).then(r => r.data),
  update:  (id, dto) => api.put(`/api/permission/${id}`, dto).then(r => r.data),
  delete:  (id)      => api.delete(`/api/permission/${id}`).then(r => r.data),
}

export const rolePermissionsApi = {
  getAll:        (includeDeleted = false) => api.get('/api/rolepermission', { params: { includeDeleted } }).then(r => r.data),
  getByRole:     (roleId)                  => api.get(`/api/rolepermission/role/${roleId}`).then(r => r.data),
  getByIds:      (roleId, permId)          => api.get(`/api/rolepermission/${roleId}/${permId}`).then(r => r.data),
  create:        (dto)                     => api.post('/api/rolepermission', dto).then(r => r.data),
  update:        (roleId, permId, dto)     => api.put(`/api/rolepermission/${roleId}/${permId}`, dto).then(r => r.data),
  delete:        (roleId, permId)          => api.delete(`/api/rolepermission/${roleId}/${permId}`).then(r => r.data),
}

export const userRolesApi = {
  getAll:    (includeDeleted = false) => api.get('/api/userrole', { params: { includeDeleted } }).then(r => r.data),
  getByUser: (userId)                  => api.get(`/api/userrole/user/${userId}`).then(r => r.data),
  getByIds:  (userId, roleId)          => api.get(`/api/userrole/${userId}/${roleId}`).then(r => r.data),
  create:    (dto)                     => api.post('/api/userrole', dto).then(r => r.data),
  update:    (userId, roleId, dto)     => api.put(`/api/userrole/${userId}/${roleId}`, dto).then(r => r.data),
  delete:    (userId, roleId)          => api.delete(`/api/userrole/${userId}/${roleId}`).then(r => r.data),
}
