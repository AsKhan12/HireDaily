import { useState } from "react";
import { useNavigate } from "react-router-dom";
import type { RegisterUserRequest } from "../types/RegisterUserRequest";
import "./IdentityPages.css";
import { apiRegisterUser } from "../api/apiRegisterUser";
import { useAuth } from "../context/AuthContext";
import type { LoginUserRequest } from "../types/LoginUserRequest";


export default function RegisterUserPage() {
    const { login } = useAuth();
    const [name, setName] = useState("");
    const [email, setEmail] = useState("");
    const [password, setPassword] = useState("");


    const navigate = useNavigate();
    const handleSubmit = async (
        e: React.SubmitEvent
    ) => {
        e.preventDefault();
        const request: RegisterUserRequest = {
            email: email,
            name: name,
            password: password
        };
        try {
            await apiRegisterUser(request);
            alert("Registration successful. Please sign in.");
            const loginRequest: LoginUserRequest = {
                email: request.email,
                password: request.password
            }
            await login(loginRequest);
            navigate("/auth/profile");
        } catch (err) {
            console.error("Registration failed", err);
            alert("Registration failed. Please try again.");
        }
    };
    return (
        <div className="identity-page">
            <div className="identity-card">
                <h1 className="identity-title">
                    Create Account
                </h1>

                <form
                    className="identity-form"
                    onSubmit={handleSubmit}
                >
                    <input
                        className="identity-input"
                        value={name}
                        onChange={e =>
                            setName(e.target.value)}
                        placeholder="Name"
                    />

                    <input
                        className="identity-input"
                        value={email}
                        onChange={e =>
                            setEmail(e.target.value)}
                        placeholder="Email"
                    />

                    <input
                        className="identity-input"
                        type="password"
                        value={password}
                        onChange={e =>
                            setPassword(e.target.value)}
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
