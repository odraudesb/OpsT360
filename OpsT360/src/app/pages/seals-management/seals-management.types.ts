export type SealTab =
  | 'purchase-request'
  | 'seal-purchase'
  | 'inventory'
  | 'operational'
  | 'reconciliation'
  | 'actions';

export type RequestRow = {
  id: string;
  description: string;
  quantity: string;
  requester: string;
  date: string;
  sealType: string;
  status: string;
  timeSince: string;
};

export type PurchaseRequestFilter = 'id' | 'requester' | 'status' | 'all';

export type InventoryFilter =
  | 'id'
  | 'requestOrderId'
  | 'description'
  | 'requester'
  | 'uploadDate'
  | 'sealType'
  | 'totalSeals'
  | 'receivedBy'
  | 'all';

export type PurchaseMgmtRow = {
  id: string;
  description: string;
  requester: string;
  date: string;
  sealType: string;
  total: string;
  status: string;
};

export type InventoryRow = {
  id: string;
  requestOrderId: string;
  description: string;
  requester: string;
  uploadDate: string;
  sealType: string;
  totalSeals: string;
  receivedBy: string;
};

export type OperationalRow = {
  id: string;
  vessel: string;
  category: string;
  containers: number;
  sealsRequested: number | string;
  sealsDelivered: number | string;
  sealsReturned: number | string;
  status: string;
  requester: string;
  delivery: string;
};

export type ReconciliationRow = {
  id: string;
  date: string;
  vessel: string;
  operationType: string;
  systemSeals: number;
  physicalSeals: number | string;
  difference: number | string;
  status: string;
  user: string;
};

export type UploadField = {
  id: string;
  label: string;
  type: 'string' | 'number' | 'date';
  required?: boolean;
};

export type UploadError = {
  line: number;
  column?: string;
  value?: string;
  message: string;
};

export type UploadSummary = {
  total: number;
  success: number;
  errors: number;
};
