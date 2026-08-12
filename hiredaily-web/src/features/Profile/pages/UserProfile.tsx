import { useEffect, useState } from "react";
import { apiAddUserSkill } from "../../identity/api/apiAddUserSkill";
import { apiFetchUser } from "../../identity/api/apiFetchUser";
import { apiGetCurrentLocation } from "../../identity/api/apiGetCurrentLocation";
import { apiRemoveUserSkill } from "../../identity/api/apiRemoveUserSkill";
import { apiUpdateUserAddress } from "../../identity/api/apiUpdateUserAddress";
import { apiUpdateUserName } from "../../identity/api/apiUpdateUserName";
import { AddressContactSection } from "../components/AddressContactSection";
import { PersonalInformationSection } from "../components/PersonalInformationSection";
import { ProfileHeader } from "../components/ProfileHeader";
import { ProfileTimelineSection } from "../components/ProfileTimelineSection";
import { SkillsSection } from "../../../components/SkillsSection";
import { useAuth } from "../../identity/context/AuthContext";
import type { UpdateUserAddressRequest } from "../../identity/types/UpdateUserAddressRequest";
import type { UserProfile } from "../types/UserProfile";
import "./Profile.css";
import type { JobSkill } from "../../../types/JobSkill";

const emptySkill: JobSkill = {
  name: "",
  field: "",
  description: "",
  skillLevel: 0
};

const profileSteps = [
  "Personal Information",
  "Address & Contact",
  "Skills",
  "Timeline"
];

export default function UserProfile() {
  const { user, isLoading } = useAuth();
  const [userProfile, setUserProfile] = useState<UserProfile | null>(null);
  const [loading, setLoading] = useState(true);
  const [saving, setSaving] = useState(false);
  const [locating, setLocating] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [activeStep, setActiveStep] = useState(0);
  const [editingName, setEditingName] = useState(false);
  const [editingAddress, setEditingAddress] = useState(false);
  const [addingSkill, setAddingSkill] = useState(false);
  const [name, setName] = useState("");
  const [address, setAddress] = useState<UpdateUserAddressRequest | null>(null);
  const [skill, setSkill] = useState<JobSkill>(emptySkill);

  const applyProfile = (data: UserProfile) => {
    setUserProfile(data);
    setName(data.name);
    setAddress({
      email: data.address.contactDetails.email,
      phone: data.address.contactDetails.phone,
      addressLine1: data.address.postalAddress.addressLine1,
      addressLine2: data.address.postalAddress.addressLine2,
      city: data.address.postalAddress.city,
      state: data.address.postalAddress.state,
      country: data.address.postalAddress.country,
      postalCode: data.address.postalAddress.postalCode,
      latitude: data.address.locatoin.lat,
      longitude: data.address.locatoin.long
    });
  };

  const loadProfile = async (id: string) => {
    const data = await apiFetchUser(id);
    applyProfile(data);
  };

  useEffect(() => {
    if (isLoading || !user) return;

    apiFetchUser(user.userId)
      .then(applyProfile)
      .catch(err => setError(err instanceof Error ? err.message : "Failed to load profile"))
      .finally(() => setLoading(false));
  }, [isLoading, user]);

  const save = async (action: () => Promise<void>) => {
    if (!user) return;
    setSaving(true);
    setError(null);

    try {
      await action();
      await loadProfile(user.userId);
    } catch (err) {
      setError(err instanceof Error ? err.message : "Failed to update profile");
    } finally {
      setSaving(false);
    }
  };

  const updateCurrentLocation = async () => {
    setLocating(true);
    setError(null);

    try {
      const position = await apiGetCurrentLocation();
      const latitude = position.coords.latitude.toString();
      const longitude = position.coords.longitude.toString();

      setAddress(current => current
        ? { ...current, latitude, longitude }
        : current);
    } catch (err) {
      let message = "Unable to retrieve your current location";

      if (err instanceof Error) {
        message = err.message;
      } else if (
        typeof err === "object" &&
        err !== null &&
        "message" in err &&
        typeof err.message === "string"
      ) {
        message = err.message;
      }

      setError(message);
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
    return <div className="user-profile-page"><div className="error">User is not authenticated</div></div>;
  }

  if (loading) {
    return <div className="user-profile-page"><div className="loading">Loading profile...</div></div>;
  }

  if (!userProfile || !address) {
    return <div className="user-profile-page"><div className="error">{error ?? "No profile data found"}</div></div>;
  }

  const sections = [
    <PersonalInformationSection
      key="personal"
      profile={userProfile}
      name={name}
      editing={editingName}
      saving={saving}
      onNameChange={setName}
      onEdit={() => setEditingName(true)}
      onCancel={() => {
        setName(userProfile.name);
        setEditingName(false);
      }}
      onSave={async () => {
        await save(() => apiUpdateUserName(userProfile.userId, name));
        setEditingName(false);
      }}
    />,
    <AddressContactSection
      key="address"
      userAddress={userProfile.address}
      address={address}
      editing={editingAddress}
      saving={saving}
      locating={locating}
      onAddressChange={setAddress}
      onEdit={beginEditingAddress}
      onCancel={() => setEditingAddress(false)}
      onSave={async () => {
        await save(() => apiUpdateUserAddress(userProfile.userId, address));
        setEditingAddress(false);
      }}
      onUseCurrentLocation={updateCurrentLocation}
    />,
    <SkillsSection
      key="skills"
      skills={userProfile.skills}
      skill={skill}
      adding={addingSkill}
      saving={saving}
      onSkillChange={setSkill}
      onAdd={() => setAddingSkill(true)}
      onCancel={() => setAddingSkill(false)}
      onSave={async () => {
        await save(() => apiAddUserSkill(userProfile.userId, skill));
        setSkill(emptySkill);
        setAddingSkill(false);
      }}
      onRemove={item => save(() => apiRemoveUserSkill(userProfile.userId, item))}
    />,
    <ProfileTimelineSection
      key="timeline"
      createdAt={userProfile.createdAt}
      updatedAt={userProfile.updatedAt}
    />
  ];

  return (
    <div className="user-profile-page">
      <div className="profile-container">
        <ProfileHeader name={userProfile.name} username={userProfile.username} />

        {error && <div className="update-error">{error}</div>}

        <div className="profile-step-status">
          Step {activeStep + 1} of {profileSteps.length}: {profileSteps[activeStep]}
        </div>

        {sections[activeStep]}

        <nav className="profile-navigation" aria-label="Profile sections">
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
            disabled={activeStep === profileSteps.length - 1 || saving || locating}
            onClick={() => setActiveStep(step => step + 1)}
          >
            Next
          </button>
        </nav>
      </div>
    </div>
  );
}
