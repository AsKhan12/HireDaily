import { useAuth } from "../../identity/context/AuthContext";
import OrganizationProfile from "./OrganizationProfile";
import UserProfile from "./UserProfile";
import "./Profile.css";import { useNavigate } from "react-router-dom";

export default function ProfilePage() {
  const { user, isLoading } = useAuth();
  const navigate = useNavigate();

  if (isLoading) {
    return <div className="user-profile-page"><div className="loading">Loading profile...</div></div>;
  }

  if (!user) {
    return <div className="user-profile-page"><div className="error">User is not authenticated</div></div>;
  }

  const role = user.role.toLowerCase();

  if (role === "user") {
    return <UserProfile />;
  }

  if (role === "organization") {
    return <OrganizationProfile onComplete={() => navigate('/')} onCompleteText="Go To Home"/>;
  }

  return (
    <div className="user-profile-page">
      <div className="error">Unsupported authenticated user role: {user.role}</div>
    </div>
  );
}
