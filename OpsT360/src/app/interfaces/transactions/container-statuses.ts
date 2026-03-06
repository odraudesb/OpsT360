export type ContainerStatus =
  | 'shipped'
  | 'inPort'
  | 'inTransit'
  | 'outOfPort'
  | 'alerts'
  | 'unknown';
