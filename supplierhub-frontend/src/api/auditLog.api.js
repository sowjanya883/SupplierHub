import api from './axiosClient'
export const auditLogApi = {
  getAll: (includeDeleted = false) => api.get('/api/auditlog', { params: { includeDeleted } }).then(r => r.data),
  getById: (id) => api.get(`/api/auditlog/${id}`).then(r => r.data),
  create: (dto) => api.post('/api/auditlog', dto).then(r => r.data),
}
