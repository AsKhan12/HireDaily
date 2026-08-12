export interface UpdateUserAddressRequest {
    email: string;
    phone: string;
    addressLine1: string;
    addressLine2: string | null;
    city: string;
    state: string;
    country: string;
    postalCode: string;
    latitude: string;
    longitude: string;
}
