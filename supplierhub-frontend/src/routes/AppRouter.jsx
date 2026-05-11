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
import RequisitionsPage from '../pages/Requisitions/RequisitionsPage'
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

export default function AppRouter() {
  return (
    <Routes>
      {/* Public */}
      <Route element={<AuthLayout />}>
        <Route path="/login" element={<LoginPage />} />
        <Route path="/signup" element={<SignUpPage />} />
      </Route>

      {/* Protected – any authenticated user */}
      <Route element={<PrivateRoute />}>
        <Route element={<AppLayout />}>
          <Route path="/" element={<Navigate to="/dashboard" replace />} />
          <Route path="/dashboard"        element={<DashboardPage />} />
          <Route path="/notifications"    element={<NotificationsPage />} />
          <Route path="/performance"      element={<PerformancePage />} />

          {/* Catalog */}
          <Route path="/categories"       element={<CategoriesPage />} />
          <Route path="/items"            element={<ItemsPage />} />
          <Route path="/catalogs"         element={<CatalogsPage />} />
          <Route path="/contracts"        element={<ContractsPage />} />

          {/* Procurement */}
          <Route path="/rfx"              element={<RFxPage />} />
          <Route path="/requisitions"     element={<RequisitionsPage />} />
          <Route path="/purchase-orders"  element={<PurchaseOrdersPage />} />
          <Route path="/purchase-orders/:id" element={<PurchaseOrderDetailPage />} />

          {/* Operations */}
          <Route path="/shipping"         element={<ShippingPage />} />
          <Route path="/grn"              element={<GRNPage />} />
          <Route path="/grn/:id"          element={<GRNDetailPage />} />
          <Route path="/invoices"         element={<InvoicesPage />} />
          <Route path="/invoices/:id"     element={<InvoiceDetailPage />} />
          <Route path="/ncr"              element={<NCRPage />} />
          <Route path="/compliance-docs"  element={<ComplianceDocsPage />} />
        </Route>
      </Route>

      {/* Admin-only */}
      <Route element={<PrivateRoute roles={['Admin']} />}>
        <Route element={<AppLayout />}>
          <Route path="/organizations"    element={<OrganizationsPage />} />
          <Route path="/suppliers"        element={<SuppliersPage />} />
          <Route path="/suppliers/:id"    element={<SupplierDetailPage />} />
          <Route path="/users"            element={<UsersPage />} />
          <Route path="/admin"            element={<AdminPage />} />
          <Route path="/audit-logs"       element={<AuditLogsPage />} />
        </Route>
      </Route>

      <Route path="*" element={<Navigate to="/dashboard" replace />} />
    </Routes>
  )
}
