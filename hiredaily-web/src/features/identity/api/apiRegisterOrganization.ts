import type { RegisterOrganizationRequest } from "../types/RegisterOrganizationRequest";
import { httpClient } from "../../../api/httpClient";

export async function apiRegisterOrganization(
    request: RegisterOrganizationRequest): Promise<void> {
    try {
        const response = await httpClient.post<void>(
            "/organization",
            request);
        return response.data;
    } catch (error) {
        console.error("Failed to register organization:", error);
        throw error;
    }
}
