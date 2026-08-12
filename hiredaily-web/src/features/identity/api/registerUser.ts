import { type AxiosResponse } from "axios";
import type { RegisterUserRequest } from "../types/RegisterUserRequest";
import { httpClient } from "../../../api/httpClient";
export async function registerUser(
    request: RegisterUserRequest): Promise<AxiosResponse<any, any, {}>> {
    return httpClient.post(
        "/users",
        request);
}
