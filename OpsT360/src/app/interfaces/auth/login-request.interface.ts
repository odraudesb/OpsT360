export interface LoginRequest {
    username: string;
    password: string;
    ip: string;
    device: 'web' | string;
}
