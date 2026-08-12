import React, { createContext, useContext, useEffect, useRef, useState } from "react";

import type { AuthenticatedUser } from "../types/AuthenticatedUser";
import type { LoginUserRequest } from "../types/LoginUserRequest";
import { apiRefresh } from "../api/apiRefresh";
import { apiLogin } from "../api/apiLogin";
import type { LoginUserResponse } from "../types/LoginUserResponse";

type AuthContextType = {
    user: AuthenticatedUser | null;
    accessToken: string | null;
    isAuthenticated: boolean;
    isLoading: boolean;

    login: (request: LoginUserRequest) => Promise<void>;
    logout: () => void;
};

const AuthContext = createContext<AuthContextType | undefined>(undefined);

export function useAuth() {
    const ctx = useContext(AuthContext);

    if (!ctx) {
        throw new Error("useAuth must be used within AuthProvider");
    }

    return ctx;
}

export function AuthProvider({
    children,
}: {
    children: React.ReactNode;
}) {
    const [accessToken, setAccessToken] = useState<string | null>(null);
    const [user, setUser] = useState<AuthenticatedUser | null>(null);
    const [isAuthenticated, setIsAuthenticated] = useState(false);
    const [isLoading, setIsLoading] = useState(true);
    const isMountedRef = useRef(false);

    function setSession(res: LoginUserResponse) {
        const token = res.token;

        const authenticatedUser = res.user;

        if (!token) {
            throw new Error("No access token returned from server.");
        }

        setAccessToken(token);
        setUser(authenticatedUser);
        setIsAuthenticated(true);
    }

    function clearSession() {
        setAccessToken(null);
        setUser(null);
        setIsAuthenticated(false);
    }

    useEffect(() => {
        isMountedRef.current = true;

        async function initialize() {
            try {
                const response = await apiRefresh();

                if (!isMountedRef.current) {
                    return;
                }

                setSession(response);
            } catch {
                if (!isMountedRef.current) {
                    return;
                }

                clearSession();
            } finally {
                if (isMountedRef.current) {
                    setIsLoading(false);
                }
            }
        }

        initialize();

        return () => {
            isMountedRef.current = false;
        };
    }, []);

    async function login(request: LoginUserRequest): Promise<void> {
        const response = await apiLogin(request);

        setSession(response);
    }

    function logout(): void {
        clearSession();

        // Optional:
        // await logoutApi();
        // navigate("/home");
    }

    return (
        <AuthContext.Provider
            value={{
                user,
                accessToken,
                isAuthenticated,
                isLoading,
                login,
                logout,
            }}
        >
            {children}
        </AuthContext.Provider>
    );
}

export default AuthContext;
