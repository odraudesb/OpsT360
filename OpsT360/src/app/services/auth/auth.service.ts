import { Injectable } from '@angular/core';
import type { Observable } from 'rxjs';

import { ApiService } from '../api/api.service';
import type { LoginRequest } from '../../interfaces/auth/login-request.interface';
import type { LoginResponse } from '../../interfaces/auth/login-response.interface';

@Injectable({ providedIn: 'root' })
export class AuthService {
  constructor(private readonly api: ApiService) { }

  login(payload: LoginRequest): Observable<LoginResponse> {
    return this.api.post<LoginRequest, LoginResponse>('auth/login', payload);
  }
}
