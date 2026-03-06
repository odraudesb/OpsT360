import { Routes } from '@angular/router';
import { APP_ROUTES } from './core/constants/app-routes.constants';
import { authGuard } from './core/guards/auth-guard';

export const routes: Routes = [
  {
    path: '',
    redirectTo: APP_ROUTES.LOGIN,
    pathMatch: 'full',
  },

  // LOGIN (public)
  {
    path: APP_ROUTES.LOGIN,
    loadComponent: () => import('./pages/login/login').then((m) => m.LoginComponent),
  },

  // FORGOT PASSWORD (public)
  {
    path: APP_ROUTES.FORGOT_PASSWORD,
    loadComponent: () =>
      import('./pages/forgot-password/forgot-password').then((m) => m.ForgotPasswordComponent),
  },

  // HOME (private)
  {
    path: APP_ROUTES.HOME,
    canActivate: [authGuard],
    loadComponent: () => import('./pages/home/home').then((m) => m.HomeComponent),
  },

  // CONTAINERS EXPORTATION (private)
  {
    path: APP_ROUTES.CONTAINERS_EXPORTATION,
    canActivate: [authGuard],
    loadComponent: () =>
      import('./pages/containers-exportacion/containers-exportacion').then(
        (m) => m.ContainersExportationComponent,
      ),
  },

  // CONTAINER DETAIL (private)
  {
    path: `${APP_ROUTES.CONTAINER_DETAIL}/:containerId`,
    canActivate: [authGuard],
    loadComponent: () =>
      import('./pages/container-detail/container-detail').then((m) => m.ContainerDetailComponent),
  },

  // ✅ PDF PREVIEW (private)
  {
    path: APP_ROUTES.PDF_PREVIEW,
    canActivate: [authGuard],
    loadComponent: () =>
      import('./pages/pdf-preview/pdf-preview').then((m) => m.PdfPreviewComponent),
  },

  // TRANSACTIONS (private)
  {
    path: APP_ROUTES.TRANSACTIONS,
    canActivate: [authGuard],
    loadComponent: () =>
      import('./pages/transactions/transactions').then((m) => m.TransactionsComponent),
  },

  // ADD USER
  {
    path: APP_ROUTES.ADD_USER,
    loadComponent: () => import('./pages/add-user/add-user').then((m) => m.AddUserComponent),
  },

  // USER MANAGEMENT (private)
  {
    path: APP_ROUTES.USER_MANAGEMENT,
    canActivate: [authGuard],
    loadComponent: () =>
      import('./pages/user-management/user-management').then((m) => m.UserManagementComponent),
  },

  // NOTIFICATIONS (private)
  {
    path: APP_ROUTES.NOTIFICATIONS,
    canActivate: [authGuard],
    loadComponent: () =>
      import('./pages/notifications/notifications').then((m) => m.NotificationsComponent),
  },

  // SEALS MANAGEMENT (private)
  {
    path: APP_ROUTES.SEALS_MANAGEMENT,
    canActivate: [authGuard],
    loadComponent: () =>
      import('./pages/seals-management/seals-management').then((m) => m.SealsManagementComponent),
  },

  // RFID SEAL SIMULATOR (private)
  {
    path: APP_ROUTES.RFID_SEAL_SIMULATOR,
    canActivate: [authGuard],
    loadComponent: () =>
      import('./pages/rfid-seal-simulator/rfid-seal-simulator').then(
        (m) => m.RfidSealSimulatorComponent,
      ),
  },

  // RFID EQUIPMENT (private)
  {
    path: APP_ROUTES.RFID_EQUIPMENT,
    canActivate: [authGuard],
    loadComponent: () =>
      import('./pages/rfid-equipment/rfid-equipment').then((m) => m.RfidEquipmentComponent),
  },

  // CONFIGURATIONS (private)
  {
    path: APP_ROUTES.CONFIGURATIONS,
    canActivate: [authGuard],
    loadComponent: () =>
      import('./pages/configurations/configurations').then(
        (m) => m.ConfigurationsComponent,
      ),
  },


  // COMPANY DATA IMPORT (private)
  {
    path: APP_ROUTES.COMPANY_DATA_IMPORT,
    canActivate: [authGuard],
    loadComponent: () =>
      import('./pages/company-data-import/company-data-import').then(
        (m) => m.CompanyDataImportComponent,
      ),
  },

  // COMPANIES (private)
  {
    path: APP_ROUTES.COMPANIES,
    canActivate: [authGuard],
    loadComponent: () => import('./pages/companies/companies').then((m) => m.CompaniesComponent),
  },

  {
    path: APP_ROUTES.SUBSCRIPTIONS,
    canActivate: [authGuard],
    loadComponent: () =>
      import('./pages/subscriptions/subscriptions').then(
        (m) => m.SubscriptionsComponent,
      ),
  },

  // ANY OTHER ROUTE -> LOGIN
  {
    path: '**',
    redirectTo: APP_ROUTES.LOGIN,
  },
];
