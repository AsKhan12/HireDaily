import { useAuth } from "../../identity/context/AuthContext";
import OrganizationHomePage from "./OrganizationHomePage";
import UnAuthenticateHomePage from "./UnAuthenticateHomePage";
import "./HomePage.css";
import UserHomepage from "./UserHomePage";

export default function HomePage() {
  const { isAuthenticated, isLoading, user } = useAuth();

  if (isLoading) {
    return (
      <main className="home-page">
        <section className="hero">
          <p className="hero-subtitle">Loading...</p>
        </section>
      </main>
    );
  }

  if (!isAuthenticated || !user) {
    return <UnAuthenticateHomePage />;
  }

  const role = user.role.toLowerCase();
  return (role === "organization"
    ? <OrganizationHomePage />
    : <UserHomepage />);
}
