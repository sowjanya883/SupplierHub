import api from './axiosClient'

const SC = '/api/suppliercontacts'
export const supplierContactsApi = {
  getAll:  ()        => api.get(SC).then(r => r.data),
  getById: (id)      => api.get(`${SC}/${id}`).then(r => r.data),
  create:  (dto)     => api.post(SC, dto).then(r => r.data),
  update:  (id, dto) => api.put(`${SC}/${id}`, dto).then(r => r.data),
  delete:  (dto)     => api.delete(SC, { data: dto }).then(r => r.data),
}

const SR = '/api/supplierrisks'
export const supplierRisksApi = {
  getAll:  ()        => api.get(SR).then(r => r.data),
  getById: (id)      => api.get(`${SR}/${id}`).then(r => r.data),
  create:  (dto)     => api.post(SR, dto).then(r => r.data),
  update:  (id, dto) => api.put(`${SR}/${id}`, dto).then(r => r.data),
  delete:  (dto)     => api.delete(SR, { data: dto }).then(r => r.data),
}

const SK = '/api/supplierkpi'
export const supplierKpiApi = {
  getAll:          ()    => api.get(SK).then(r => r.data),
  getById:         (id)  => api.get(`${SK}/${id}`).then(r => r.data),
  getBySupplier:   (sid) => api.get(`${SK}/supplier/${sid}`).then(r => r.data),
  create:          (dto) => api.post(SK, dto).then(r => r.data),
  update:          (id, dto) => api.put(`${SK}/${id}`, dto).then(r => r.data),
  delete:          (id)  => api.delete(`${SK}/${id}`).then(r => r.data),
}

const SCD = '/api/scorecard'
export const scorecardsApi = {
  getAll:         ()    => api.get(SCD).then(r => r.data),
  getById:        (id)  => api.get(`${SCD}/${id}`).then(r => r.data),
  getBySupplier:  (sid) => api.get(`${SCD}/supplier/${sid}`).then(r => r.data),
  create:         (dto) => api.post(SCD, dto).then(r => r.data),
  update:         (id, dto) => api.put(`${SCD}/${id}`, dto).then(r => r.data),
  delete:         (id)  => api.delete(`${SCD}/${id}`).then(r => r.data),
}
