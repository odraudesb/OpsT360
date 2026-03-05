import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { ApiService } from '../../services/api/api.service';
import { API_ROUTES } from '../../core/constants/app-routes.constants';
import type { CreateUserRequest, CreateUserResponse } from '../../interfaces/users/create-user.interface';
import type { User } from '../../interfaces/users/user.interface';

@Injectable({
  providedIn: 'root',
})
export class UsersService {
  constructor(private readonly api: ApiService) {}

  createUser(payload: CreateUserRequest): Observable<CreateUserResponse> {
    return this.api.post<CreateUserRequest, CreateUserResponse>(
      API_ROUTES.auth.register, // o API_ROUTES.users.create
      payload,
    );
  }

  getAllUsers(): Observable<User[]> {
    return this.api.get<User[]>(API_ROUTES.users.list);
  }
}
