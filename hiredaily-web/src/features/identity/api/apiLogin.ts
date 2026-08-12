import type { LoginUserRequest } from "../types/LoginUserRequest";
import type { LoginUserResponse } from "../types/LoginUserResponse";
import { httpClient } from "../../../api/httpClient";

export async function apiLogin(
    request: LoginUserRequest): Promise<LoginUserResponse> {
    try {
        const response = await httpClient.post<LoginUserResponse>(
            "/auth/login",
            request);
        return response.data;
    } catch (error) {
        console.error("Failed to log in:", error);
        throw error;
    }
}
