import {
    useContext
} from "react";

import AuthContext 
    from "../context/AuthContext";

export function useAuth()
{
    const context =
        useContext(AuthContext);

    if (!context)
    {
        throw new Error(
            "AuthContext not found");
    }

    return context;
}