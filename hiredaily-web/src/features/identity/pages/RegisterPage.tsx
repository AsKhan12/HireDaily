import { Link } from "react-router-dom";
import "./IdentityPages.css";

export default function RegisterPage() {
    return (
        <div className="identity-page">
            <div className="identity-card">
                <h1 className="identity-title">
                    Register
                </h1>

                <p className="identity-subtitle">
                    Choose how you would like to use Hiredaily.
                </p>

                <div className="identity-actions">
                    <Link to="/auth/user">
                        <button className="identity-button">
                            Job Seeker
                        </button>
                    </Link>

                    <Link to="/auth/organization">
                        <button className="identity-button">
                            Organization
                        </button>
                    </Link>
                </div>
            </div>
        </div>
    );
}