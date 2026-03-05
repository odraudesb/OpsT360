export interface ContainerShipmentDetails {
  containerId: string;
  size: string;
  shippingLine: string;
  pod: string;
  goods: string;
  booking: string;
  weight: string;

  gateIn: string | null;
  preAdvice: string | null;

  transportationCompany: string;
  truckPlate: string;
  driver: string;
  seals: string;
}
