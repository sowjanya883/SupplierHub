import api from './axiosClient'

export const authApi = {
  login: (dto) => api.post('/api/auth/login', dto).then(r => r.data),
}
