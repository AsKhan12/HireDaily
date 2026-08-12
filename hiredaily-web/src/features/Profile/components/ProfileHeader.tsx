type ProfileHeaderProps = {
  name: string;
  username: string;
};

export function ProfileHeader({ name, username }: ProfileHeaderProps) {
  return (
    <div className="profile-header">
      <h1 className="profile-title">{name}</h1>
      <p className="profile-username">@{username}</p>
    </div>
  );
}
