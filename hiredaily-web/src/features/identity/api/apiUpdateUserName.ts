import { httpClient } from "../../../api/httpClient";

export async function apiUpdateUserName(
    id: string,
    name: string): Promise<void> {
    const response = await httpClient.put<void>(
        `/user/${id}/name`,
        undefined,
        { params: { name } });
    return response.data;
}
