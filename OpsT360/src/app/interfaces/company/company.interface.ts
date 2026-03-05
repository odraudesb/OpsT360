export type Company = {
    companyId: number;
    name: string;
    address?: string | null;
    contact?: string | null;
    email?: string | null;
    phone?: string | null;
    legalAgent?: string | null;
    webUrl?: string | null;
    logoUrl?: string | null;
    countryId?: number | null;
    status?: boolean | null;
};
