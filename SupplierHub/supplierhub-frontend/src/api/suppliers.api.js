import api from './axiosClient'
const B = '/api/supplier'
export const suppliersApi = {
  getAll:  ()       => api.get(B).then(r => r.data),
  getById: (id)     => api.get(`${B}/${id}`).then(r => r.data),
  create:  (dto)    => api.post(B, dto).then(r => r.data),
  update:  (id, dto)=> api.put(`${B}/${id}`, dto).then(r => r.data),
  delete:  (dto)    => api.delete(B, { data: dto }).then(r => r.data),
}
