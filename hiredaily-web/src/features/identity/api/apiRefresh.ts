import { httpClient } from "../../../api/httpClient";
import type { LoginUserResponse } from "../types/LoginUserResponse";

let refreshRequest: Promise<LoginUserResponse> | null = null;

export async function apiRefresh(): Promise<LoginUserResponse> {
    if (!refreshRequest) {
        refreshRequest = httpClient
            .post<LoginUserResponse>("/auth/refresh")
            .then(response => response.data)
            .catch(error => {
                console.error("Failed to refresh authentication:", error);
                throw error;
            })
            .finally(() => {
                refreshRequest = null;
            });
    }

    return refreshRequest;
}
