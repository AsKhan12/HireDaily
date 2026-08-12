export interface UpdateOrganizationAddress {
  isInitialized: boolean;
  location: {
    lat: string;
    long: string;
  };
  postalAddress: {
    addressLine1: string;
    addressLine2: string | null;
    city: string;
    state: string;
    country: string;
    postalCode: string;
  };
  contactDetails: {
    email: string;
    phone: string;
    websiteUrl: string;
  };
}

export interface UpdateOrganizationRequest {
  updatedName?: string;
  updatedDescription?: string;
  updatedAddress?: UpdateOrganizationAddress;
}
