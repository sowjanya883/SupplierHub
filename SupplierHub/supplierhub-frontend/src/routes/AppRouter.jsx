import { Routes, Route, Navigate } from 'react-router-dom'
import PrivateRoute from './PrivateRoute'
import AppLayout from '../layouts/AppLayout'
import AuthLayout from '../layouts/AuthLayout'

import LoginPage from '../pages/Login/LoginPage'
import SignupPage from '../pages/Login/SignupPage'
import DashboardPage from '../pages/Dashboard/DashboardPage'
import NotificationsPage from '../pages/Notifications/NotificationsPage'
import PerformancePage from '../pages/Performance/PerformancePage'

import OrganizationsPage from '../pages/Organizations/OrganizationsPage'
import SuppliersPage from '../pages/Suppliers/SuppliersPage'
import SupplierDetailPage from '../pages/Suppliers/SupplierDetailPage'

import CategoriesPage from '../pages/Categories/CategoriesPage'
import ItemsPage from '../pages/Items/ItemsPage'
import CatalogsPage from '../pages/Catalogs/CatalogsPage'
import ContractsPage from '../pages/Contracts/ContractsPage'

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
import ComplianceDocsPage from '../pages/ComplianceDocs/ComplianceDocsPage'

import UsersPage from '../pages/Users/UsersPage'
import AdminPage from '../pages/Admin/AdminPage'
import AuditLogsPage from '../pages/AuditLogs/AuditLogsPage'

// ── Role constants ─────────────────────────────────────
const ALL_ROLES = [
    'Admin', 'Buyer', 'SupplierUser', 'CategoryManager',
    'AccountsPayable', 'ComplianceOfficer', 'ReceivingUser',
]

export default function AppRouter() {
    return (
        <Routes>

            {/* ── Public ──────────────────────────────────── */}
            <Route element={<AuthLayout />}>
                <Route path="/login" element={<LoginPage />} />
                <Route path="/signup" element={<SignupPage />} />
            </Route>

            {/* ── Any logged-in user ──────────────────────── */}
            <Route element={<PrivateRoute roles={ALL_ROLES} />}>
                <Route element={<AppLayout />}>
                    <Route path="/" element={<Navigate to="/dashboard" replace />} />
                    <Route path="/dashboard" element={<DashboardPage />} />
                    <Route path="/notifications" element={<NotificationsPage />} />
                    <Route path="/performance" element={<PerformancePage />} />
                </Route>
            </Route>

            {/* ── Suppliers — Admin, CategoryManager, ComplianceOfficer, Buyer, SupplierUser ── */}
            <Route element={<PrivateRoute roles={['Admin', 'CategoryManager', 'ComplianceOfficer', 'Buyer', 'SupplierUser']} />}>
                <Route element={<AppLayout />}>
                    <Route path="/suppliers" element={<SuppliersPage />} />
                    <Route path="/suppliers/:id" element={<SupplierDetailPage />} />
                </Route>
            </Route>

            {/* ── Catalog — Admin, CategoryManager, Buyer, SupplierUser ── */}
            <Route element={<PrivateRoute roles={['Admin', 'CategoryManager', 'Buyer', 'SupplierUser']} />}>
                <Route element={<AppLayout />}>
                    <Route path="/categories" element={<CategoriesPage />} />
                    <Route path="/items" element={<ItemsPage />} />
                    <Route path="/catalogs" element={<CatalogsPage />} />
                </Route>
            </Route>

            {/* ── Contracts — Admin, CategoryManager, SupplierUser ── */}
            <Route element={<PrivateRoute roles={['Admin', 'CategoryManager', 'SupplierUser']} />}>
                <Route element={<AppLayout />}>
                    <Route path="/contracts" element={<ContractsPage />} />
                </Route>
            </Route>

            {/* ── RFx — Admin, Buyer, CategoryManager, SupplierUser ── */}
            <Route element={<PrivateRoute roles={['Admin', 'Buyer', 'CategoryManager', 'SupplierUser']} />}>
                <Route element={<AppLayout />}>
                    <Route path="/rfx" element={<RFxPage />} />
                    <Route path="/rfx/:id" element={<RFxDetailPage />} />
                </Route>
            </Route>

            {/* ── Requisitions — Admin, Buyer, CategoryManager, SupplierUser (read + respond) ── */}
            <Route element={<PrivateRoute roles={['Admin', 'Buyer', 'CategoryManager', 'SupplierUser']} />}>
                <Route element={<AppLayout />}>
                    <Route path="/requisitions" element={<RequisitionsPage />} />
                    <Route path="/requisitions/:id" element={<RequisitionDetailPage />} />
                </Route>
            </Route>

            {/* ── Purchase Orders — Admin, Buyer, CategoryManager, SupplierUser ── */}
            <Route element={<PrivateRoute roles={['Admin', 'Buyer', 'CategoryManager', 'SupplierUser']} />}>
                <Route element={<AppLayout />}>
                    <Route path="/purchase-orders" element={<PurchaseOrdersPage />} />
                    <Route path="/purchase-orders/:id" element={<PurchaseOrderDetailPage />} />
                </Route>
            </Route>

            {/* ── Shipping — Admin, SupplierUser, Buyer, ReceivingUser ── */}
            <Route element={<PrivateRoute roles={['Admin', 'SupplierUser', 'Buyer', 'ReceivingUser']} />}>
                <Route element={<AppLayout />}>
                    <Route path="/shipping" element={<ShippingPage />} />
                </Route>
            </Route>

            {/* ── GRN — Admin, SupplierUser, ReceivingUser ── */}
            <Route element={<PrivateRoute roles={['Admin', 'SupplierUser', 'ReceivingUser']} />}>
                <Route element={<AppLayout />}>
                    <Route path="/grn" element={<GRNPage />} />
                    <Route path="/grn/:id" element={<GRNDetailPage />} />
                </Route>
            </Route>

            {/* ── Invoices — Admin, SupplierUser, AccountsPayable, Buyer ── */}
            <Route element={<PrivateRoute roles={['Admin', 'SupplierUser', 'AccountsPayable', 'Buyer']} />}>
                <Route element={<AppLayout />}>
                    <Route path="/invoices" element={<InvoicesPage />} />
                    <Route path="/invoices/:id" element={<InvoiceDetailPage />} />
                </Route>
            </Route>

            {/* ── NCR — Admin, SupplierUser, ReceivingUser ── */}
            <Route element={<PrivateRoute roles={['Admin', 'SupplierUser', 'ReceivingUser']} />}>
                <Route element={<AppLayout />}>
                    <Route path="/ncr" element={<NCRPage />} />
                </Route>
            </Route>

            {/* ── Compliance Docs — Admin, ComplianceOfficer, SupplierUser ── */}
            <Route element={<PrivateRoute roles={['Admin', 'ComplianceOfficer', 'SupplierUser']} />}>
                <Route element={<AppLayout />}>
                    <Route path="/compliance-docs" element={<ComplianceDocsPage />} />
                </Route>
            </Route>

            {/* ── Admin only ──────────────────────────────── */}
            <Route element={<PrivateRoute roles={['Admin']} />}>
                <Route element={<AppLayout />}>
                    <Route path="/organizations" element={<OrganizationsPage />} />
                    <Route path="/users" element={<UsersPage />} />
                    <Route path="/admin" element={<AdminPage />} />
                    <Route path="/audit-logs" element={<AuditLogsPage />} />
                </Route>
            </Route>

            <Route path="*" element={<Navigate to="/dashboard" replace />} />
        </Routes>
    )
}