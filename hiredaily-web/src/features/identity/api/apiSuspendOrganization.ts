import { httpClient } from "../../../api/httpClient";

export async function apiSuspendOrganization(id: string): Promise<void> {
  await httpClient.put("/organization/suspend", {
    requestId: crypto.randomUUID(),
    requestedAt: new Date().toISOString(),
    requestedBy: "Organization",
    organizationId: { value: id }
  });
}
