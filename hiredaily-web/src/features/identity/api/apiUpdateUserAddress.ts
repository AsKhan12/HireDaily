import { httpClient } from "../../../api/httpClient";
import type { UpdateUserAddressRequest } from "../types/UpdateUserAddressRequest";

export async function apiUpdateUserAddress(
    id: string,
    request: UpdateUserAddressRequest): Promise<void> {
    const response = await httpClient.put<void>(
        `/user/${id}/address`,
        request);
    return response.data;
}
