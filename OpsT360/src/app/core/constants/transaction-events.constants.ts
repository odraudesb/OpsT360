// Transaction event catalog.
export const TRANSACTION_EVENTS = {
  RFID_GATE_IN: {
    id: 1,
    name: 'Ingreso Camión Terminal',
  },
  RFID_PRE_GATE_VALIDATION: {
    id: 8,
    name: 'Colocación de Sello RFID previo Ingreso',
  },
  RFID_LOADING_EMBARQUE: {
    id: 9,
    name: 'Embarque',
  },
  RFID_LABEL_NOT_PLACED: {
    id: 30,
    name: 'Etiqueta RFID no colocada',
  },
  RFID_LABEL_PLACED_OUTSIDE_LID: {
    id: 31,
    name: 'Etiqueta RFID colocada fuera de la tapa',
  },
  RFID_LABEL_PLACED_FOLLOWING_PROCEDURE: {
    id: 32,
    name: 'Fallo en colocación de Etiqueta RFID',
  },
  RFID_LABEL_NOT_READ: {
    id: 33,
    name: 'Etiqueta RFID no leída',
  },
} as const;
