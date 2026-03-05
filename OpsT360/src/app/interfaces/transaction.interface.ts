export interface Transaction {
    id: string;
    containerId: string;
    type: 'inbound' | 'outbound';
    date: string;
    status: 'completed' | 'pending' | 'cancelled';
  }
  