import type { Field } from './field.type';
import type { ItemsModalField } from './items-modal-field.type';

export const isItemsModalField = (f: Field): f is ItemsModalField =>
  (f as ItemsModalField)?.action === 'itemsModal' && !!(f as ItemsModalField)?.payload;
