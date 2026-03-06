import { Injectable } from '@angular/core';
import type { Observable } from 'rxjs';

import { ApiService } from '../api/api.service';
import type { ApiResponse } from '../../interfaces/api/api-response.interface';
import type { Company } from '../../interfaces/company/company.interface';

@Injectable({ providedIn: 'root' })
export class CompaniesService {
  constructor(private readonly api: ApiService) {}

  getCompanies(): Observable<ApiResponse<Company[] | Company>> {
    return this.api.get<ApiResponse<Company[] | Company>>('/companies');
  }
}
