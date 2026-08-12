import { httpClient } from "../../../api/httpClient";
import type { OrganizationProfile } from "../../Profile/types/OrganizationProfile";

type OrganizationProfileResponse = Omit<OrganizationProfile, "organizationId"> & {
  organizationId: string | { value: string };
};

export async function apiFetchOrganization(id: string): Promise<OrganizationProfile> {
  const response = await httpClient.get<OrganizationProfileResponse>(`/organization/${id}`);
  const organization = response.data;

  return {
    ...organization,
    organizationId: typeof organization.organizationId === "string"
      ? organization.organizationId
      : organization.organizationId.value
  };
}
