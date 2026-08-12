import type { LoginUserRequest } from "../types/LoginUserRequest";
import { httpClient } from "../../../api/httpClient";
import type { LoginUserResponse } from "../types/LoginUserResponse";
export async function loginApi(request: LoginUserRequest): Promise<LoginUserResponse> {

    try {
        const response = await httpClient.post<LoginUserResponse>("/auth/login", request);
        return response.data;
    } catch (error) {
        console.error("Failed to fetch user profile:", error);
        throw error;
    }
}