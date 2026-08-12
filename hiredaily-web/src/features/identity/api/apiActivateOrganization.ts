import { httpClient } from "../../../api/httpClient";

export async function apiActivateOrganization(id: string): Promise<void> {
  await httpClient.post("/organization/activate", {
    requestId: crypto.randomUUID(),
    requestedAt: new Date().toISOString(),
    requestedBy: "Organization",
    organizationId: { value: id }
  });
}
