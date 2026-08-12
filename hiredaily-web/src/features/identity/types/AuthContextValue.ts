import type { AuthenticatedUser } from "./AuthenticatedUser";

export interface AuthContextValue {
    user: AuthenticatedUser | null;
    accessToken: string | null;
    isAuthenticated: boolean;

    signIn: (
        accessToken: string,
        user: AuthenticatedUser
    ) => void;

    signOut: () => void;
}
