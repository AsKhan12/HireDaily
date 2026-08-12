import { httpClient } from "../../../api/httpClient";
import type { UpdateOrganizationRequest } from "../types/UpdateOrganizationRequest";

const createCommandMetadata = (id: string) => ({
  requestId: crypto.randomUUID(),
  requestedAt: new Date().toISOString(),
  requestedBy: "Organization",
  organizationId: { value: id }
});

export async function apiUpdateOrganization(
  id: string,
  request: UpdateOrganizationRequest
): Promise<void> {
  await httpClient.put("/organization", {
    ...createCommandMetadata(id),
    updatedName: request.updatedName ?? null,
    updatedDescription: request.updatedDescription ?? null,
    updatedAddress: request.updatedAddress ?? null
  });
}
