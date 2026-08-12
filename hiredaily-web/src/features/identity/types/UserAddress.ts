import type { GeoLocation } from "../../../types/GeoLocation";
import type { PostalAddress } from "../../../types/PostalAddress";

export interface UserAddress {
  locatoin: GeoLocation;
  postalAddress: PostalAddress;
  contactDetails: {
    phone: string;
    email: string;
  };
}
