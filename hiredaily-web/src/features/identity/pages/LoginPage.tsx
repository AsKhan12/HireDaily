import { Link, useNavigate } from "react-router-dom";
import { useState } from "react";
import { useAuth } from "../context/AuthContext";
import "./IdentityPages.css";
import type { LoginUserRequest } from '../types/LoginUserRequest'

export default function LoginPage() {
    const [email, setEmail] = useState("");
    const [password, setPassword] = useState("");
    const { login } = useAuth();
    const navigate = useNavigate();
    const handleSubmit = async (
        e: React.SubmitEvent
    ) => {
        e.preventDefault();

        const request: LoginUserRequest = {
            email: email,
            password: password
        };
        try {
            await login(request);
            navigate("/");
        } catch (err) {
            console.error(err);
            alert("Login failed");
        }
    };

    return (
        <div className="identity-page">
            <div className="identity-card">
                <h1 className="identity-title">
                    Sign In
                </h1>

                <p className="identity-subtitle">
                    Welcome back to Hiredaily.
                </p>

                <form
                    className="identity-form"
                    onSubmit={handleSubmit}
                >
                    <input
                        className="identity-input"
                        type="email"
                        placeholder="Email"
                        value={email}
                        onChange={e =>
                            setEmail(e.target.value)}
                    />

                    <input
                        className="identity-input"
                        type="password"
                        placeholder="Password"
                        value={password}
                        onChange={e =>
                            setPassword(e.target.value)}
                    />

                    <button
                        className="identity-button"
                        type="submit"
                    >
                        Sign In
                    </button>
                </form>

                <div className="identity-footer">
                    <span>
                        Don't have an account?
                    </span>

                    <Link
                        className="identity-link"
                        to="/auth"
                    >
                        Register
                    </Link>
                </div>
            </div>
        </div>
    );
}
