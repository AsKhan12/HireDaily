import type { FormEvent } from "react";
import type { UpdateUserAddressRequest } from "../../identity/types/UpdateUserAddressRequest";
import type { UserAddress } from "../../identity/types/UserAddress";
import { ProfileDetail } from "./ProfileDetail";

type AddressContactSectionProps = {
  userAddress: UserAddress;
  address: UpdateUserAddressRequest;
  editing: boolean;
  saving: boolean;
  locating: boolean;
  onAddressChange: (address: UpdateUserAddressRequest) => void;
  onEdit: () => void;
  onCancel: () => void;
  onSave: () => Promise<void>;
  onUseCurrentLocation: () => Promise<void>;
};

const addressFields = [
  ["email", "Email"],
  ["phone", "Phone"],
  ["addressLine1", "Address line 1"],
  ["addressLine2", "Address line 2"],
  ["city", "City"],
  ["state", "State"],
  ["country", "Country"],
  ["postalCode", "Postal code"]
] as const;

export function AddressContactSection({
  userAddress,
  address,
  editing,
  saving,
  locating,
  onAddressChange,
  onEdit,
  onCancel,
  onSave,
  onUseCurrentLocation
}: AddressContactSectionProps) {
  const { postalAddress: postal, contactDetails: contact, locatoin: location } = userAddress;

  const handleSubmit = async (event: FormEvent<HTMLFormElement>) => {
    event.preventDefault();
    await onSave();
  };

  return (
    <section className="profile-section editable-section" onClick={onEdit}>
      <div className="section-heading">
        <h2 className="section-title">Address &amp; Contact</h2>
        <span className="edit-hint">Click to edit</span>
      </div>
      {editing ? (
        <form className="edit-form form-grid" onClick={event => event.stopPropagation()} onSubmit={handleSubmit}>
          {addressFields.map(([key, label]) => (
            <label key={key}>
              {label}
              <input
                value={address[key] ?? ""}
                required={key !== "addressLine2"}
                onChange={event => onAddressChange({
                  ...address,
                  [key]: event.target.value || (key === "addressLine2" ? null : "")
                })}
              />
            </label>
          ))}
          <label>Latitude<input value={address.latitude} readOnly /></label>
          <label>Longitude<input value={address.longitude} readOnly /></label>
          <div className="form-actions full-width">
            <button disabled={locating} type="button" className="secondary" onClick={onUseCurrentLocation}>
              {locating ? "Getting location..." : "Use current location"}
            </button>
          </div>
          <div className="form-actions full-width">
            <button disabled={saving || locating} type="submit">Save</button>
            <button type="button" className="secondary" onClick={onCancel}>Cancel</button>
          </div>
        </form>
      ) : (
        <div className="profile-details">
          <ProfileDetail label="Email" value={contact.email} />
          <ProfileDetail label="Phone" value={contact.phone} />
          <ProfileDetail label="Address" value={[postal.addressLine1, postal.addressLine2].filter(Boolean).join(", ")} />
          <ProfileDetail label="City" value={postal.city} />
          <ProfileDetail label="State" value={postal.state} />
          <ProfileDetail label="Postal Code" value={postal.postalCode} />
          <ProfileDetail label="Country" value={postal.country} />
          <ProfileDetail label="Coordinates" value={`${location.lat}, ${location.long}`} />
        </div>
      )}
    </section>
  );
}
