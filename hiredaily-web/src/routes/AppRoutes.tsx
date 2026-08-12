import { Routes, Route } from "react-router-dom";

import HomePage from
    "../features/home/pages/HomePage";
import { IdentityRoutes } from "./IdentityRoutes";

// function AuthLayout() {
//     return (
//         <AuthProvider>
//             <Outlet />
//         </AuthProvider>
//     );
// }

export default function AppRoutes() {
    return (
        <Routes>
            <Route path="/" element={<HomePage />} />
            <Route
                path="/auth/*"
                element={<IdentityRoutes />}
            />

            {/* <Route element={<AuthLayout />}>

            </Route> */}
        </Routes>
    );
}