import { Link } from "react-router-dom";
import "./HomePage.css";

export default function UnAuthenticateHomePage() {
    return (
        <div className="home-page">
            <section className="hero">
                <h1 className="hero-title">
                    Hiredaily
                </h1>

                <p className="hero-subtitle">
                    Connect with skilled workers
                    and discover opportunities
                    near you.
                </p>

                <div className="hero-actions">
                    <Link to="/auth">
                        <button className="primary-button">
                            Get Started
                        </button>
                    </Link>

                    <Link to="/auth/login">
                        <button className="secondary-button">
                            Sign In
                        </button>
                    </Link>
                </div>
            </section>
        </div>
    );
}
