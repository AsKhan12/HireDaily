import { useState } from "react";
import { useNavigate } from "react-router-dom";
import "./IdentityPages.css";
import { apiRegisterOrganization } from "../api/apiRegisterOrganization";
import type { RegisterOrganizationRequest } from "../types/RegisterOrganizationRequest";
import type { LoginUserRequest } from "../types/LoginUserRequest";
import { useAuth } from "../context/AuthContext";

export default function RegisterOrganizationPage() {
    const { login } = useAuth();
    const [name, setName] = useState("");
    const [email, setEmail] = useState("");
    const [password, setPassword] = useState("");

    const navigate = useNavigate();

    const handleSubmit = async (
        e: React.SubmitEvent
    ) => {
        e.preventDefault();
        const request: RegisterOrganizationRequest = {
            name: name,
            email: email,
            password: password
        };

        try {
            await apiRegisterOrganization(request);
            const loginRequest: LoginUserRequest = {
                email: request.email,
                password: request.password
            }
            await login(loginRequest);
            navigate("/auth/profile");
        } catch (err) {
            console.error("Organization registration failed", err);
            alert("Registration failed. Please try again.");
        }
    };

    return (
        <div className="identity-page">
            <div className="identity-card">
                <h1 className="identity-title">
                    Register Organization
                </h1>

                <form
                    className="identity-form"
                    onSubmit={handleSubmit}
                >
                    <input
                        className="identity-input"
                        value={name}
                        onChange={e => setName(e.target.value)}
                        placeholder="Organization Name"
                    />

                    <input
                        className="identity-input"
                        value={email}
                        onChange={e => setEmail(e.target.value)}
                        placeholder="Email"
                    />

                    <input
                        className="identity-input"
                        type="password"
                        value={password}
                        onChange={e => setPassword(e.target.value)}
                        placeholder="Password"
                    />

                    <button
                        className="identity-button"
                        type="submit"
                    >
                        Register
                    </button>
                </form>
            </div>
        </div>
    );
}
