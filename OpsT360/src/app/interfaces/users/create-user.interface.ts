export interface CreateUserRequest {
  email: string;
  password: string;
  name: string;
  lastName: string;
  companyId: number;
}

export interface CreateUserResponse {
  message: string;
  data?: {
    id?: string | number;
    email?: string;
  };
  errors?: unknown;
}
