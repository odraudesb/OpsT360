export type ItemsModalField = {
  label: string;
  value: string;
  action: 'itemsModal';
  payload: { title: string; items: unknown[] };
};
