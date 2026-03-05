// Internal app routes (Angular Router)
export const APP_ROUTES = {
  LOGIN: 'login',
  FORGOT_PASSWORD: 'forgot-password',
  HOME: 'home',
  CONTAINERS_EXPORTATION: 'containers-exportation',
  CONTAINER_DETAIL: 'container-detail',
  TRANSACTIONS: 'transactions',
  USER_MANAGEMENT: 'user-management',
  ADD_USER: 'add-user',
  PDF_PREVIEW: 'pdf-preview',
  NOTIFICATIONS: 'notifications',
  SEALS_MANAGEMENT: 'seals-management',
  RFID_SEAL_SIMULATOR: 'rfid-seal-simulator',
  RFID_EQUIPMENT: 'rfid-equipment',
  CONFIGURATIONS: 'configurations',
  SUBSCRIPTIONS: 'subscriptions',
  COMPANIES: 'companies',
  COMPANY_DATA_IMPORT: 'company-data-import',
} as const;

// API routes (used later in api.service)
export const API_ROUTES = {
  auth: {
    login: '/auth/login',
    register: '/auth/register',
  },
  users: {
    list: '/users',
    detail: (id: string) => `/users/${id}`,
  },
  containers: {
    list: '/containers',
  },
} as const;
