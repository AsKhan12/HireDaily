import { httpClient } from "../../../api/httpClient";
import type { JobSkill } from "../../../types/JobSkill";

export async function apiAddUserSkill(
    id: string,
    skill: JobSkill): Promise<void> {
    const response = await httpClient.put<void>(
        `/user/${id}/skills`,
        skill);
    return response.data;
}
