import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { APP_ROUTES } from '../constants/app-routes.constants';
import { getAuthContextFromStorage } from '../utils/auth-storage';

export const authGuard: CanActivateFn = () => {
  const router = inject(Router);

  const context = getAuthContextFromStorage();
  if (context) return true;

  router.navigate([APP_ROUTES.LOGIN]);
  return false;
};
