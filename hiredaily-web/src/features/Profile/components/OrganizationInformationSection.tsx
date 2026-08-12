import type { FormEvent } from "react";
import type { OrganizationProfile } from "../types/OrganizationProfile";
import { ProfileDetail } from "./ProfileDetail";

type OrganizationInformationSectionProps = {
  organization: OrganizationProfile;
  name: string;
  editing: boolean;
  saving: boolean;
  onNameChange: (name: string) => void;
  onEdit: () => void;
  onCancel: () => void;
  onSave: () => Promise<void>;
  onStatusChange: () => Promise<void>;
};

export function OrganizationInformationSection({
  organization,
  name,
  editing,
  saving,
  onNameChange,
  onEdit,
  onCancel,
  onSave}: OrganizationInformationSectionProps) {
  const statusLabels: Record<OrganizationProfile["status"], string> = {
    1: "Active",
    2: "Suspended",
    3: "Archived"
  };
  const handleSubmit = async (event: FormEvent<HTMLFormElement>) => {
    event.preventDefault();
    await onSave();
  };

  return (
    <section className="profile-section editable-section" onClick={onEdit}>
      <div className="section-heading">
        <h2 className="section-title">Organization Information</h2>
        <span className="edit-hint">Click to edit name</span>
      </div>
      {editing ? (
        <form className="edit-form" onClick={event => event.stopPropagation()} onSubmit={handleSubmit}>
          <label>
            Organization name
            <input value={name} onChange={event => onNameChange(event.target.value)} required />
          </label>
          <div className="form-actions">
            <button disabled={saving} type="submit">Save</button>
            <button type="button" className="secondary" onClick={onCancel}>Cancel</button>
          </div>
        </form>
      ) : (
        <div className="profile-details">
          <ProfileDetail label="ID" value={organization.organizationId} />
          <ProfileDetail label="Name" value={organization.organizationName} />
          <ProfileDetail label="Username" value={organization.username} />
          <ProfileDetail label="Status" value={statusLabels[organization.status]} />
        </div>
      )}
    </section>
  );
}
