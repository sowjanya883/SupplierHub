import api from './axiosClient'

const BASE = '/api/admin'

export const adminApi = {
  // System Configs
  getAllSystemConfigs: ()      => api.get(`${BASE}/system-configs`).then(r => r.data),
  getSystemConfigById: (id)   => api.get(`${BASE}/system-configs/${id}`).then(r => r.data),
  getSystemConfigByKey: (key) => api.get(`${BASE}/system-configs/key/${key}`).then(r => r.data),
  createSystemConfig: (dto)   => api.post(`${BASE}/system-configs`, dto).then(r => r.data),
  updateSystemConfig: (dto)   => api.put(`${BASE}/system-configs`, dto).then(r => r.data),

  // Approval Rules
  getAllApprovalRules: ()        => api.get(`${BASE}/approval-rules`).then(r => r.data),
  getApprovalRuleById: (id)     => api.get(`${BASE}/approval-rules/${id}`).then(r => r.data),
  getApprovalRulesByScope: (s)  => api.get(`${BASE}/approval-rules/scope/${s}`).then(r => r.data),
  createApprovalRule: (dto)     => api.post(`${BASE}/approval-rules`, dto).then(r => r.data),
  updateApprovalRule: (dto)     => api.put(`${BASE}/approval-rules`, dto).then(r => r.data),

  // Roles
  assignRole: (dto)  => api.post(`${BASE}/assign-role`, dto).then(r => r.data),
  deleteRole: (dto)  => api.delete(`${BASE}/delete-role`, { data: dto }).then(r => r.data),
}
