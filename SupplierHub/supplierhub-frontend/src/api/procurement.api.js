import api from './axiosClient'

/* ── Compliance ─────────────────────────────────────────── */
export const complianceDocsApi = {
  getAll:  ()        => api.get('/api/compliancedocs').then(r => r.data),
  getById: (id)      => api.get(`/api/compliancedocs/${id}`).then(r => r.data),
  create:  (dto)     => api.post('/api/compliancedocs', dto).then(r => r.data),
  update:  (id, dto) => api.put(`/api/compliancedocs/${id}`, dto).then(r => r.data),
  delete:  (dto)     => api.delete('/api/compliancedocs', { data: dto }).then(r => r.data),
}

/* ── RFx (events, lines, invites, bids, bid-lines, awards) ─ */
export const rfxApi = {
  // Events
  getAllRfx:        ()      => api.get('/api/rfx/rfx').then(r => r.data),
  getRfxById:       (id)    => api.get(`/api/rfx/rfx/${id}`).then(r => r.data),
  createRfx:        (dto)   => api.post('/api/rfx/rfx', dto).then(r => r.data),
  updateRfx:        (dto)   => api.put('/api/rfx/rfx', dto).then(r => r.data),
  // Lines
  getRfxLines:      (rfxId) => api.get(`/api/rfx/rfx/${rfxId}/lines`).then(r => r.data),
  addRfxLine:       (dto)   => api.post('/api/rfx/rfx-lines', dto).then(r => r.data),
  updateRfxLine:    (dto)   => api.put('/api/rfx/rfx-lines', dto).then(r => r.data),
  // Invites
  getInvites:       (rfxId) => api.get(`/api/rfx/rfx/${rfxId}/invites`).then(r => r.data),
  addInvite:        (dto)   => api.post('/api/rfx/invites', dto).then(r => r.data),
  updateInvite:     (dto)   => api.put('/api/rfx/invites', dto).then(r => r.data),
  // Bids
  getBids:          (rfxId) => api.get(`/api/rfx/rfx/${rfxId}/bids`).then(r => r.data),
  getBidById:       (bidId) => api.get(`/api/rfx/bids/${bidId}`).then(r => r.data),
  addBid:           (dto)   => api.post('/api/rfx/bids', dto).then(r => r.data),
  updateBid:        (dto)   => api.put('/api/rfx/bids', dto).then(r => r.data),
  // Bid Lines
  getBidLines:      (bidId) => api.get(`/api/rfx/bids/${bidId}/lines`).then(r => r.data),
  addBidLine:       (dto)   => api.post('/api/rfx/bid-lines', dto).then(r => r.data),
  updateBidLine:    (dto)   => api.put('/api/rfx/bid-lines', dto).then(r => r.data),
  // Awards
  getAwards:        (rfxId) => api.get(`/api/rfx/rfx/${rfxId}/awards`).then(r => r.data),
  addAward:         (dto)   => api.post('/api/rfx/awards', dto).then(r => r.data),
  updateAward:      (dto)   => api.put('/api/rfx/awards', dto).then(r => r.data),
}

/* ── Requisitions ───────────────────────────────────────── */
export const requisitionsApi = {
  getAll:              ()             => api.get('/api/requisitions').then(r => r.data),
  create:              (dto)          => api.post('/api/requisitions', dto).then(r => r.data),
  getById:             (id)           => api.get(`/api/requisitions/${id}`).then(r => r.data),
  getLines:            (prId)         => api.get(`/api/requisitions/${prId}/lines`).then(r => r.data),
  addLine:             (dto)          => api.post('/api/requisitions/lines', dto).then(r => r.data),
  getApprovals:        (prId)         => api.get(`/api/requisitions/${prId}/approvals`).then(r => r.data),
  createApprovalStep:  (dto)          => api.post('/api/requisitions/approvals', dto).then(r => r.data),
  updateApproval:      (stepId, dto)  => api.put(`/api/requisitions/approvals/${stepId}`, dto).then(r => r.data),
  // Supplier-facing accept / decline of the PR overall
  updateStatus:        (prId, status) => api.put(`/api/requisitions/${prId}/status`, { status }).then(r => r.data),
}

/* ── Purchase Orders ────────────────────────────────────── */
export const purchaseOrdersApi = {
  getAll:  ()        => api.get('/api/purchaseorders').then(r => r.data),
  getById: (id)      => api.get(`/api/purchaseorders/${id}`).then(r => r.data),
  create:  (dto)     => api.post('/api/purchaseorders', dto).then(r => r.data),
  update:  (id, dto) => api.put(`/api/purchaseorders/${id}`, dto).then(r => r.data),
  delete:  (id)      => api.delete(`/api/purchaseorders/${id}`).then(r => r.data),
}

export const poLinesApi = {
  getByPoId: (poId)    => api.get(`/api/polines/po/${poId}`).then(r => r.data),
  getById:   (id)      => api.get(`/api/polines/${id}`).then(r => r.data),
  create:    (dto)     => api.post('/api/polines', dto).then(r => r.data),
  update:    (id, dto) => api.put(`/api/polines/${id}`, dto).then(r => r.data),
  delete:    (id)      => api.delete(`/api/polines/${id}`).then(r => r.data),
}

export const poAcksApi = {
  getAll:  ()        => api.get('/api/poacks').then(r => r.data),
  getById: (id)      => api.get(`/api/poacks/${id}`).then(r => r.data),
  create:  (dto)     => api.post('/api/poacks', dto).then(r => r.data),
  update:  (id, dto) => api.put(`/api/poacks/${id}`, dto).then(r => r.data),
}

export const poRevisionsApi = {
  getByPoId: (poId) => api.get(`/api/porevisions/po/${poId}`).then(r => r.data),
  getById:   (id)   => api.get(`/api/porevisions/${id}`).then(r => r.data),
  create:    (dto)  => api.post('/api/porevisions', dto).then(r => r.data),
}

export const erpExportRefsApi = {
  getAll:  ()        => api.get('/api/erpexportrefs').then(r => r.data),
  getById: (id)      => api.get(`/api/erpexportrefs/${id}`).then(r => r.data),
  create:  (dto)     => api.post('/api/erpexportrefs', dto).then(r => r.data),
  update:  (id, dto) => api.put(`/api/erpexportrefs/${id}`, dto).then(r => r.data),
  delete:  (id)      => api.delete(`/api/erpexportrefs/${id}`).then(r => r.data),
}
