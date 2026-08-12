import type { GeoLocation } from "./GeoLocation";
import type { PostalAddress } from "./PostalAddress";


export interface JobSite {
    location: GeoLocation;
    address: PostalAddress;
}
