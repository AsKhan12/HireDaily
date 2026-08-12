import { ProfileDetail } from "./ProfileDetail";

type ProfileTimelineSectionProps = {
  createdAt: string;
  updatedAt: string | null;
};

const formatDate = (date: string) => new Date(date).toLocaleDateString("en-US", {
  year: "numeric",
  month: "long",
  day: "numeric",
  hour: "2-digit",
  minute: "2-digit"
});

export function ProfileTimelineSection({ createdAt, updatedAt }: ProfileTimelineSectionProps) {
  return (
    <section className="profile-section">
      <h2 className="section-title">Timeline</h2>
      <div className="profile-details">
        <ProfileDetail label="Created At" value={formatDate(createdAt)} />
        {updatedAt && <ProfileDetail label="Updated At" value={formatDate(updatedAt)} />}
      </div>
    </section>
  );
}
