import { Routes, Route } from "react-router-dom";

import RegisterPage
    from "../features/identity/pages/RegisterPage";

import RegisterUserPage
    from "../features/identity/pages/RegisterUserPage";

import RegisterOrganizationPage
    from "../features/identity/pages/RegisterOrganizationPage";
import LoginPage from "../features/identity/pages/LoginPage";
import ProfilePage from "../features/Profile/pages/ProfilePage";

export function IdentityRoutes() {
    return (
        <Routes>
            <Route index element={<RegisterPage />} />
            <Route path="user" element={<RegisterUserPage />} />
            <Route path="organization" element={<RegisterOrganizationPage />} />
            <Route path="login" element={<LoginPage />} />
            <Route path="profile" element={<ProfilePage />} />
        </Routes>
    );
}
