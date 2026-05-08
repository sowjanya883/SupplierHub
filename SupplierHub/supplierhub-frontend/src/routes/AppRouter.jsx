import { Routes, Route, Navigate } from 'react-router-dom'
import PrivateRoute from './PrivateRoute'
import AppLayout from '../layouts/AppLayout'
import AuthLayout from '../layouts/AuthLayout'

import SignUpPage from '../pages/Login/SignupPage'
import LoginPage from '../pages/Login/LoginPage'
import DashboardPage from '../pages/Dashboard/DashboardPage'
import SuppliersPage from '../pages/Suppliers/SuppliersPage'
import SupplierDetailPage from '../pages/Suppliers/SupplierDetailPage'
import OrganizationsPage from '../pages/Organizations/OrganizationsPage'
import CategoriesPage from '../pages/Categories/CategoriesPage'
import ItemsPage from '../pages/Items/ItemsPage'
import CatalogsPage from '../pages/Catalogs/CatalogsPage'
import ContractsPage from '../pages/Contracts/ContractsPage'
import ComplianceDocsPage from '../pages/ComplianceDocs/ComplianceDocsPage'
import RFxPage from '../pages/RFx/RFxPage'
import RFxDetailPage from '../pages/RFx/RFxDetailPage'
import RequisitionsPage from '../pages/Requisitions/RequisitionsPage'
import RequisitionDetailPage from '../pages/Requisitions/RequisitionDetailPage'
import PurchaseOrdersPage from '../pages/PurchaseOrders/PurchaseOrdersPage'
import PurchaseOrderDetailPage from '../pages/PurchaseOrders/PurchaseOrderDetailPage'
import ShippingPage from '../pages/Shipping/ShippingPage'
import GRNPage from '../pages/GRN/GRNPage'
import GRNDetailPage from '../pages/GRN/GRNDetailPage'
import InvoicesPage from '../pages/Invoices/InvoicesPage'
import InvoiceDetailPage from '../pages/Invoices/InvoiceDetailPage'
import NCRPage from '../pages/NCR/NCRPage'
import PerformancePage from '../pages/Performance/PerformancePage'
import NotificationsPage from '../pages/Notifications/NotificationsPage'
import AuditLogsPage from '../pages/AuditLogs/AuditLogsPage'
import AdminPage from '../pages/Admin/AdminPage'
import UsersPage from '../pages/Users/UsersPage'
import ErpExportRefsPage from '../pages/ErpExportRefs/ErpExportRefsPage'

// Role rules mirror Sidebar.jsx — keep them in sync.
const ROLES = {
  organizations:   ['Admin'],
  suppliers:       ['Admin','CategoryManager','ComplianceOfficer'],
  categories:      ['Admin','CategoryManager','Buyer'],
  items:           ['Admin','CategoryManager','Buyer','WarehouseManager'],
  catalogs:        ['Admin','CategoryManager','Buyer','SupplierUser'],
  contracts:       ['Admin','CategoryManager'],
  rfx:             ['Admin','CategoryManager','SupplierUser'],
  requisitions:    ['Admin','Buyer','CategoryManager'],
  purchaseOrders:  ['Admin','Buyer','SupplierUser'],
  shipping:        ['Admin','SupplierUser','WarehouseManager','ReceivingUser'],
  grn:             ['Admin','ReceivingUser','WarehouseManager'],
  invoices:        ['Admin','AccountsPayable','SupplierUser'],
  ncr:             ['Admin','ReceivingUser','ComplianceOfficer'],
  complianceDocs:  ['Admin','ComplianceOfficer','SupplierUser'],
  performance:     ['Admin','CategoryManager'],
  users:           ['Admin'],
  admin:           ['Admin'],
  auditLogs:       ['Admin'],
  erpExports:      ['Admin'],
}

export default function AppRouter() {
  return (
    <Routes>
      {/* Public */}
      <Route element={<AuthLayout />}>
        <Route path="/login" element={<LoginPage />} />
        <Route path="/signup" element={<SignUpPage />} />
      </Route>

      {/* Authenticated — accessible to anyone signed in */}
      <Route element={<PrivateRoute />}>
        <Route element={<AppLayout />}>
          <Route path="/" element={<Navigate to="/dashboard" replace />} />
          <Route path="/dashboard"     element={<DashboardPage />} />
          <Route path="/notifications" element={<NotificationsPage />} />
        </Route>
      </Route>

      {/* Catalog */}
      <Route element={<PrivateRoute roles={ROLES.categories} />}>
        <Route element={<AppLayout />}>
          <Route path="/categories" element={<CategoriesPage />} />
        </Route>
      </Route>
      <Route element={<PrivateRoute roles={ROLES.items} />}>
        <Route element={<AppLayout />}>
          <Route path="/items" element={<ItemsPage />} />
        </Route>
      </Route>
      <Route element={<PrivateRoute roles={ROLES.catalogs} />}>
        <Route element={<AppLayout />}>
          <Route path="/catalogs" element={<CatalogsPage />} />
        </Route>
      </Route>
      <Route element={<PrivateRoute roles={ROLES.contracts} />}>
        <Route element={<AppLayout />}>
          <Route path="/contracts" element={<ContractsPage />} />
        </Route>
      </Route>

      {/* Procurement */}
      <Route element={<PrivateRoute roles={ROLES.rfx} />}>
        <Route element={<AppLayout />}>
          <Route path="/rfx"      element={<RFxPage />} />
          <Route path="/rfx/:id"  element={<RFxDetailPage />} />
        </Route>
      </Route>
      <Route element={<PrivateRoute roles={ROLES.requisitions} />}>
        <Route element={<AppLayout />}>
          <Route path="/requisitions"      element={<RequisitionsPage />} />
          <Route path="/requisitions/:id"  element={<RequisitionDetailPage />} />
        </Route>
      </Route>
      <Route element={<PrivateRoute roles={ROLES.purchaseOrders} />}>
        <Route element={<AppLayout />}>
          <Route path="/purchase-orders"      element={<PurchaseOrdersPage />} />
          <Route path="/purchase-orders/:id"  element={<PurchaseOrderDetailPage />} />
        </Route>
      </Route>

      {/* Operations */}
      <Route element={<PrivateRoute roles={ROLES.shipping} />}>
        <Route element={<AppLayout />}>
          <Route path="/shipping" element={<ShippingPage />} />
        </Route>
      </Route>
      <Route element={<PrivateRoute roles={ROLES.grn} />}>
        <Route element={<AppLayout />}>
          <Route path="/grn"      element={<GRNPage />} />
          <Route path="/grn/:id"  element={<GRNDetailPage />} />
        </Route>
      </Route>
      <Route element={<PrivateRoute roles={ROLES.invoices} />}>
        <Route element={<AppLayout />}>
          <Route path="/invoices"      element={<InvoicesPage />} />
          <Route path="/invoices/:id"  element={<InvoiceDetailPage />} />
        </Route>
      </Route>
      <Route element={<PrivateRoute roles={ROLES.ncr} />}>
        <Route element={<AppLayout />}>
          <Route path="/ncr" element={<NCRPage />} />
        </Route>
      </Route>

      {/* Compliance */}
      <Route element={<PrivateRoute roles={ROLES.complianceDocs} />}>
        <Route element={<AppLayout />}>
          <Route path="/compliance-docs" element={<ComplianceDocsPage />} />
        </Route>
      </Route>
      <Route element={<PrivateRoute roles={ROLES.performance} />}>
        <Route element={<AppLayout />}>
          <Route path="/performance" element={<PerformancePage />} />
        </Route>
      </Route>

      {/* Admin-only */}
      <Route element={<PrivateRoute roles={ROLES.organizations} />}>
        <Route element={<AppLayout />}>
          <Route path="/organizations" element={<OrganizationsPage />} />
        </Route>
      </Route>
      <Route element={<PrivateRoute roles={ROLES.suppliers} />}>
        <Route element={<AppLayout />}>
          <Route path="/suppliers"      element={<SuppliersPage />} />
          <Route path="/suppliers/:id"  element={<SupplierDetailPage />} />
        </Route>
      </Route>
      <Route element={<PrivateRoute roles={ROLES.users} />}>
        <Route element={<AppLayout />}>
          <Route path="/users" element={<UsersPage />} />
        </Route>
      </Route>
      <Route element={<PrivateRoute roles={ROLES.admin} />}>
        <Route element={<AppLayout />}>
          <Route path="/admin" element={<AdminPage />} />
        </Route>
      </Route>
      <Route element={<PrivateRoute roles={ROLES.auditLogs} />}>
        <Route element={<AppLayout />}>
          <Route path="/audit-logs" element={<AuditLogsPage />} />
        </Route>
      </Route>
      <Route element={<PrivateRoute roles={ROLES.erpExports} />}>
        <Route element={<AppLayout />}>
          <Route path="/erp-exports" element={<ErpExportRefsPage />} />
        </Route>
      </Route>

      <Route path="*" element={<Navigate to="/dashboard" replace />} />
    </Routes>
  )
}
