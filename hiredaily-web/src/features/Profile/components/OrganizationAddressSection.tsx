import type { FormEvent } from "react";
import type { OrganizationProfile } from "../types/OrganizationProfile";
import type { UpdateOrganizationAddress } from "../../identity/types/UpdateOrganizationRequest";
import { ProfileDetail } from "./ProfileDetail";

type OrganizationAddressSectionProps = {
  currentAddress: OrganizationProfile["address"];
  address: UpdateOrganizationAddress;
  editing: boolean;
  saving: boolean;
  locating: boolean;
  onAddressChange: (address: UpdateOrganizationAddress) => void;
  onEdit: () => void;
  onCancel: () => void;
  onSave: () => Promise<void>;
  onUseCurrentLocation: () => Promise<void>;
};

const displayValue = (value: string | null) => value?.trim() || "Not provided";

export function OrganizationAddressSection({
  currentAddress,
  address,
  editing,
  saving,
  locating,
  onAddressChange,
  onEdit,
  onCancel,
  onSave,
  onUseCurrentLocation
}: OrganizationAddressSectionProps) {
  // const {  postalAddress: postal, contactDetails: contact, location } = currentAddress;
  const postal = currentAddress?.postalAddress;
  const contact = currentAddress?.contactDetails;
  const location = currentAddress?.location;
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
          <label>Email<input value={address?.contactDetails?.email} onChange={event => onAddressChange({ ...address, contactDetails: { ...address?.contactDetails, email: event.target.value } })} required /></label>
          <label>Phone<input value={address?.contactDetails?.phone} onChange={event => onAddressChange({ ...address, contactDetails: { ...address?.contactDetails, phone: event.target.value } })} required /></label>
          <label>Website<input value={address?.contactDetails?.websiteUrl} onChange={event => onAddressChange({ ...address, contactDetails: { ...address?.contactDetails, websiteUrl: event.target.value } })} required /></label>
          <label>Address line 1<input value={address?.postalAddress?.addressLine1} onChange={event => onAddressChange({ ...address, postalAddress: { ...address?.postalAddress, addressLine1: event.target.value } })} required /></label>
          <label>Address line 2<input value={address?.postalAddress?.addressLine2 ?? ""} onChange={event => onAddressChange({ ...address, postalAddress: { ...address?.postalAddress, addressLine2: event.target.value || null } })} /></label>
          <label>City<input value={address?.postalAddress?.city} onChange={event => onAddressChange({ ...address, postalAddress: { ...address?.postalAddress, city: event.target.value } })} required /></label>
          <label>State<input value={address?.postalAddress?.state} onChange={event => onAddressChange({ ...address, postalAddress: { ...address?.postalAddress, state: event.target.value } })} required /></label>
          <label>Country<input value={address?.postalAddress?.country} onChange={event => onAddressChange({ ...address, postalAddress: { ...address?.postalAddress, country: event.target.value } })} required /></label>
          <label>Postal code<input value={address?.postalAddress?.postalCode} onChange={event => onAddressChange({ ...address, postalAddress: { ...address?.postalAddress, postalCode: event.target.value } })} required /></label>
          <label>Latitude<input value={address?.location?.lat} readOnly /></label>
          <label>Longitude<input value={address?.location?.long} readOnly /></label>
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
          <ProfileDetail label="Email" value={displayValue(contact?.email)} />
          <ProfileDetail label="Phone" value={displayValue(contact?.phone)} />
          <ProfileDetail label="Website" value={displayValue(contact?.websiteUrl)} />
          <ProfileDetail label="Address" value={displayValue([postal?.addressLine1, postal?.addressLine2].filter(Boolean).join(", "))} />
          <ProfileDetail label="City" value={displayValue(postal?.city)} />
          <ProfileDetail label="State" value={displayValue(postal?.state)} />
          <ProfileDetail label="Postal Code" value={displayValue(postal?.postalCode)} />
          <ProfileDetail label="Country" value={displayValue(postal?.country)} />
          <ProfileDetail label="Coordinates" value={location?.lat && location?.long ? `${location?.lat}, ${location?.long}` : "Not provided"} />
        </div>
      )}
    </section>
  );
}
