type ProfileDetailProps = {
  label: string;
  value: string;
};

export function ProfileDetail({ label, value }: ProfileDetailProps) {
  return (
    <div className="detail-row">
      <span className="detail-label">{label}:</span>
      <span className="detail-value">{value}</span>
    </div>
  );
}
