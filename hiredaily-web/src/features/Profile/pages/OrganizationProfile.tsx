import { useEffect, useState } from "react";
import { apiActivateOrganization } from "../../identity/api/apiActivateOrganization";
import { apiFetchOrganization } from "../../identity/api/apiFetchOrganization";
import { apiGetCurrentLocation } from "../../identity/api/apiGetCurrentLocation";
import { apiSuspendOrganization } from "../../identity/api/apiSuspendOrganization";
import { apiUpdateOrganization } from "../../identity/api/apiUpdateOrganization";
import { OrganizationAddressSection } from "../components/OrganizationAddressSection";
import { OrganizationDescriptionSection } from "../components/OrganizationDescriptionSection";
import { OrganizationInformationSection } from "../components/OrganizationInformationSection";
import { useAuth } from "../../identity/context/AuthContext";
import type { OrganizationProfile } from "../types/OrganizationProfile";
import type { UpdateOrganizationAddress } from "../../identity/types/UpdateOrganizationRequest";
import "./Profile.css";
import { ProfileTimelineSection } from "../components/ProfileTimelineSection";
import { ProfileHeader } from "../components/ProfileHeader";

const profileSteps = [
  "Organization Information",
  "About the Organization",
  "Address & Contact",
  "Timeline"
];

interface OrganizationProfileProps {
  onComplete: () => void;
  onCompleteText?: string
}

export default function OrganizationProfile({onComplete, onCompleteText = "Next"} : OrganizationProfileProps) {
  const { user, isLoading } = useAuth();
  const [organization, setOrganization] = useState<OrganizationProfile | null>(null);
  const [loading, setLoading] = useState(true);
  const [saving, setSaving] = useState(false);
  const [locating, setLocating] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [activeStep, setActiveStep] = useState(0);
  const [editingName, setEditingName] = useState(false);
  const [editingDescription, setEditingDescription] = useState(false);
  const [editingAddress, setEditingAddress] = useState(false);
  const [name, setName] = useState("");
  const [description, setDescription] = useState("");
  const [address, setAddress] = useState<UpdateOrganizationAddress | null>(null);

  const handleNext = () => {
    var lastStep = profileSteps.length - 1;
    if (activeStep < lastStep) {
      setActiveStep(step => step + 1);
      return;
    }
    onComplete();
  }
  const applyProfile = (profile: OrganizationProfile) => {
    setOrganization(profile);
    setName(profile.organizationName);
    setDescription(profile.organizationDescription ?? "");
    setAddress({
      isInitialized: true,
      location: {
        lat: profile?.address?.location?.lat ?? "",
        long: profile?.address?.location?.long ?? ""
      },
      postalAddress: {
        addressLine1: profile?.address?.postalAddress?.addressLine1 ?? "",
        addressLine2: profile?.address?.postalAddress?.addressLine2,
        city: profile?.address?.postalAddress?.city ?? "",
        state: profile?.address?.postalAddress?.state ?? "",
        country: profile?.address?.postalAddress?.country ?? "",
        postalCode: profile?.address?.postalAddress?.postalCode ?? ""
      },
      contactDetails: {
        email: profile?.address?.contactDetails?.email ?? "",
        phone: profile?.address?.contactDetails?.phone ?? "",
        websiteUrl: profile?.address?.contactDetails?.websiteUrl ?? ""
      }
    });
  };

  const loadProfile = async (id: string) => {
    applyProfile(await apiFetchOrganization(id));
  };

  useEffect(() => {
    if (isLoading || !user) return;

    apiFetchOrganization(user.userId)
      .then(applyProfile)
      .catch(err => setError(err instanceof Error ? err.message : "Failed to load organization profile"))
      .finally(() => setLoading(false));
  }, [isLoading, user]);

  const save = async (action: () => Promise<void>): Promise<boolean> => {
    if (!user) return false;
    setSaving(true);
    setError(null);

    try {
      await action();
      await loadProfile(user.userId);
      return true;
    } catch (err) {
      setError(err instanceof Error ? err.message : "Failed to update organization profile");
      return false;
    } finally {
      setSaving(false);
    }
  };

  const updateCurrentLocation = async () => {
    setLocating(true);
    setError(null);

    try {
      const position = await apiGetCurrentLocation();
      setAddress(current => current ? {
        ...current,
        location: {
          lat: position.coords.latitude.toString(),
          long: position.coords.longitude.toString()
        }
      } : current);
    } catch (err) {
      setError(err instanceof Error ? err.message : "Unable to retrieve your current location");
    } finally {
      setLocating(false);
    }
  };

  const beginEditingAddress = () => {
    if (editingAddress) return;
    setEditingAddress(true);
    void updateCurrentLocation();
  };

  if (!isLoading && !user) {
    return <div className="user-profile-page"><div className="error">Organization is not authenticated</div></div>;
  }

  if (!isLoading && user?.role.toLowerCase() !== "organization") {
    return <div className="user-profile-page"><div className="error">This profile is only available to organizations</div></div>;
  }

  if (loading) {
    return <div className="user-profile-page"><div className="loading">Loading organization profile...</div></div>;
  }

  if (!organization || !user || !address) {
    return <div className="user-profile-page"><div className="error">{error ?? "No organization profile found"}</div></div>;
  }

  const sections = [
    <OrganizationInformationSection
      key="information"
      organization={organization}
      name={name}
      editing={editingName}
      saving={saving}
      onNameChange={setName}
      onEdit={() => setEditingName(true)}
      onCancel={() => {
        setName(organization.organizationName);
        setEditingName(false);
      }}
      onSave={async () => {
        const saved = await save(() => apiUpdateOrganization(organization.organizationId, {
          updatedName: name
        }));
        if (saved) setEditingName(false);
      }}
      onStatusChange={async () => {
        await save(() => organization.status === 1
          ? apiSuspendOrganization(organization.organizationId)
          : apiActivateOrganization(organization.organizationId));
      }}
    />,
    <OrganizationDescriptionSection
      key="description"
      currentDescription={organization.organizationDescription}
      description={description}
      editing={editingDescription}
      saving={saving}
      onDescriptionChange={setDescription}
      onEdit={() => setEditingDescription(true)}
      onCancel={() => {
        setDescription(organization.organizationDescription ?? "");
        setEditingDescription(false);
      }}
      onSave={async () => {
        const saved = await save(() => apiUpdateOrganization(organization.organizationId, {
          updatedDescription: description
        }));
        if (saved) setEditingDescription(false);
      }}
    />,
    <OrganizationAddressSection
      key="address"
      currentAddress={organization.address}
      address={address}
      editing={editingAddress}
      saving={saving}
      locating={locating}
      onAddressChange={setAddress}
      onEdit={beginEditingAddress}
      onCancel={() => {
        applyProfile(organization);
        setEditingAddress(false);
      }}
      onSave={async () => {
        const saved = await save(() => apiUpdateOrganization(organization.organizationId, {
          updatedAddress: address
        }));
        if (saved) setEditingAddress(false);
      }}
      onUseCurrentLocation={updateCurrentLocation}
    />,
    <ProfileTimelineSection
      key="timeline"
      createdAt={organization.createdAt}
      updatedAt={organization.updatedAt}
    />
  ];

  return (
    <div className="user-profile-page">
      <div className="profile-container">
        <ProfileHeader
          name={organization.organizationName}
          username={organization.username || user.username}
        />

        {error && <div className="update-error">{error}</div>}

        <div className="profile-step-status">
          Step {activeStep + 1} of {profileSteps.length}: {profileSteps[activeStep]}
        </div>

        {sections[activeStep]}

        <nav className="profile-navigation" aria-label="Organization profile sections">
          <button
            type="button"
            className="secondary"
            disabled={activeStep === 0 || saving || locating}
            onClick={() => setActiveStep(step => step - 1)}
          >
            Previous
          </button>
          <button
            type="button"
            onClick={handleNext}
          >
            {activeStep < profileSteps.length - 1 ? 'Next' : onCompleteText}
          </button>
        </nav>
      </div>
    </div>
  );
}
