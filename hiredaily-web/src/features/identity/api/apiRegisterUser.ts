import type { RegisterUserRequest } from "../types/RegisterUserRequest";
import { httpClient } from "../../../api/httpClient";

export async function apiRegisterUser(
    request: RegisterUserRequest): Promise<void> {
    try {
        const response = await httpClient.post<void>(
            "/user",
            request);
        return response.data;
    } catch (error) {
        console.error("Failed to register user:", error);
        throw error;
    }
}
