export interface ApiResponse<T> {
    data: T;
    message?: string;
    errors?: unknown;
}
