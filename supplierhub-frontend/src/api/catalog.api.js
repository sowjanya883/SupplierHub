import api from './axiosClient'

export const categoriesApi = {
  getAll:  ()        => api.get('/api/categories').then(r => r.data),
  getById: (id)      => api.get(`/api/categories/${id}`).then(r => r.data),
  create:  (dto)     => api.post('/api/categories', dto).then(r => r.data),
  update:  (id, dto) => api.put(`/api/categories/${id}`, dto).then(r => r.data),
  delete:  (dto)     => api.delete('/api/categories', { data: dto }).then(r => r.data),
}

export const itemsApi = {
  getAll:  ()        => api.get('/api/items').then(r => r.data),
  getById: (id)      => api.get(`/api/items/${id}`).then(r => r.data),
  create:  (dto)     => api.post('/api/items', dto).then(r => r.data),
  update:  (id, dto) => api.put(`/api/items/${id}`, dto).then(r => r.data),
  delete:  (dto)     => api.delete('/api/items', { data: dto }).then(r => r.data),
}

export const catalogsApi = {
  getAll:  ()        => api.get('/api/catalog').then(r => r.data),
  getById: (id)      => api.get(`/api/catalog/${id}`).then(r => r.data),
  create:  (dto)     => api.post('/api/catalog', dto).then(r => r.data),
  update:  (id, dto) => api.put(`/api/catalog/${id}`, dto).then(r => r.data),
  delete:  (dto)     => api.delete('/api/catalog', { data: dto }).then(r => r.data),
}

export const catalogItemsApi = {
  getAll:  ()        => api.get('/api/catalogitems').then(r => r.data),
  getById: (id)      => api.get(`/api/catalogitems/${id}`).then(r => r.data),
  create:  (dto)     => api.post('/api/catalogitems', dto).then(r => r.data),
  update:  (id, dto) => api.put(`/api/catalogitems/${id}`, dto).then(r => r.data),
  delete:  (dto)     => api.delete('/api/catalogitems', { data: dto }).then(r => r.data),
}

export const contractsApi = {
  getAll:  ()        => api.get('/api/contracts').then(r => r.data),
  getById: (id)      => api.get(`/api/contracts/${id}`).then(r => r.data),
  create:  (dto)     => api.post('/api/contracts', dto).then(r => r.data),
  update:  (id, dto) => api.put(`/api/contracts/${id}`, dto).then(r => r.data),
  delete:  (dto)     => api.delete('/api/contracts', { data: dto }).then(r => r.data),
}
