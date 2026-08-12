import { httpClient } from "../../../api/httpClient";
import type { JobSkill } from "../../../types/JobSkill";

export async function apiRemoveUserSkill(
    id: string,
    skill: JobSkill): Promise<void> {
    const response = await httpClient.post<void>(
        `/user/${id}/skills/remove`,
        skill);
    return response.data;
}
