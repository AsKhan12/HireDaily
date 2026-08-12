import type { FormEvent } from "react";
import { ProfileDetail } from "./ProfileDetail";

type OrganizationDescriptionSectionProps = {
  currentDescription: string | null;
  description: string;
  editing: boolean;
  saving: boolean;
  onDescriptionChange: (description: string) => void;
  onEdit: () => void;
  onCancel: () => void;
  onSave: () => Promise<void>;
};

export function OrganizationDescriptionSection({
  currentDescription,
  description,
  editing,
  saving,
  onDescriptionChange,
  onEdit,
  onCancel,
  onSave
}: OrganizationDescriptionSectionProps) {
  const handleSubmit = async (event: FormEvent<HTMLFormElement>) => {
    event.preventDefault();
    await onSave();
  };

  return (
    <section className="profile-section editable-section" onClick={onEdit}>
      <div className="section-heading">
        <h2 className="section-title">About the Organization</h2>
        <span className="edit-hint">Click to edit description</span>
      </div>
      {editing ? (
        <form className="edit-form" onClick={event => event.stopPropagation()} onSubmit={handleSubmit}>
          <label>
            Description
            <textarea value={description} onChange={event => onDescriptionChange(event.target.value)} required />
          </label>
          <div className="form-actions">
            <button disabled={saving} type="submit">Save</button>
            <button type="button" className="secondary" onClick={onCancel}>Cancel</button>
          </div>
        </form>
      ) : (
        <div className="profile-details">
          <ProfileDetail
            label="Description"
            value={currentDescription?.trim() || "No organization description provided"}
          />
        </div>
      )}
    </section>
  );
}
