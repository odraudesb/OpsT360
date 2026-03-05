import { HttpInterceptorFn } from '@angular/common/http';

export const authInterceptor: HttpInterceptorFn = (req, next) => {
    if (!req.url.startsWith('/api')) return next(req);

    const token = localStorage.getItem('token'); // cambia la key si usas STORAGE_KEYS
    if (!token) return next(req);

    return next(req.clone({
        setHeaders: { Authorization: `Bearer ${token}` },
    }));
};
