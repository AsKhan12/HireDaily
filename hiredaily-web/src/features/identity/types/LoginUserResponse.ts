import type { AuthenticatedUser } from "./AuthenticatedUser";

export interface LoginUserResponse {
    token: string;
    refreshTokenExpiresAt: string;
    user: AuthenticatedUser;
}
