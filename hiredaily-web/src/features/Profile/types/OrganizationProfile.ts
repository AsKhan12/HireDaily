import type { GeoLocation } from "../../../types/GeoLocation";
import type { PostalAddress } from "../../../types/PostalAddress";

export interface OrganizationProfile {
  organizationId: string;
  organizationName: string;
  username: string;
  organizationDescription: string | null;
  address: {
    isInitialized: boolean;
    location: GeoLocation;
    postalAddress: PostalAddress;
    contactDetails: {
      email: string | null;
      phone: string | null;
      websiteUrl: string | null;
    };
  };
  status: 1 | 2 | 3;
  createdAt: string;
  updatedAt: string | null;
}
