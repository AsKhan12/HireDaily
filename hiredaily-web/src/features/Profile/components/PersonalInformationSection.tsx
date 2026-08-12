import type { FormEvent } from "react";
import type { UserProfile } from "../types/UserProfile";
import { ProfileDetail } from "./ProfileDetail";

type PersonalInformationSectionProps = {
  profile: UserProfile;
  name: string;
  editing: boolean;
  saving: boolean;
  onNameChange: (name: string) => void;
  onEdit: () => void;
  onCancel: () => void;
  onSave: () => Promise<void>;
};

export function PersonalInformationSection({
  profile,
  name,
  editing,
  saving,
  onNameChange,
  onEdit,
  onCancel,
  onSave
}: PersonalInformationSectionProps) {
  const handleSubmit = async (event: FormEvent<HTMLFormElement>) => {
    event.preventDefault();
    await onSave();
  };

  return (
    <section className="profile-section editable-section" onClick={onEdit}>
      <div className="section-heading">
        <h2 className="section-title">Personal Information</h2>
        <span className="edit-hint">Click to edit name</span>
      </div>
      {editing ? (
        <form className="edit-form" onClick={event => event.stopPropagation()} onSubmit={handleSubmit}>
          <label>
            Name
            <input value={name} onChange={event => onNameChange(event.target.value)} required />
          </label>
          <div className="form-actions">
            <button disabled={saving} type="submit">Save</button>
            <button type="button" className="secondary" onClick={onCancel}>Cancel</button>
          </div>
        </form>
      ) : (
        <div className="profile-details">
          <ProfileDetail label="ID" value={profile.userId} />
          <ProfileDetail label="Name" value={profile.name} />
          <ProfileDetail label="Username" value={profile.username} />
        </div>
      )}
    </section>
  );
}
