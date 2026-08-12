import { useMemo, useState, type FormEvent } from "react";
// import { httpClient } from "../api/httpClient";
// import type { JobSkill } from "../types/JobSkill";
import "./JobDetailsComponent.css";
import type { JobSkill } from "../../../types/JobSkill";
import { httpClient } from "../../../api/httpClient";

type CreateJobComponentProps = {
  organizationId?: string;
  onCreated?: () => void | Promise<void>;
};

type CreateJobForm = {
  organizationId: string;
  hourlyRateAmount: string;
  hourlyRateCurrency: string;
  latitude: string;
  longitude: string;
  addressLine1: string;
  addressLine2: string;
  city: string;
  state: string;
  country: string;
  postalCode: string;
  title: string;
};

const emptyForm = (organizationId = ""): CreateJobForm => ({
  organizationId,
  hourlyRateAmount: "",
  hourlyRateCurrency: "",
  latitude: "",
  longitude: "",
  addressLine1: "",
  addressLine2: "",
  city: "",
  state: "",
  country: "",
  postalCode: "",
  title: ""
});

const emptySkill: JobSkill = {
  name: "",
  field: "",
  description: "",
  skillLevel: 0
};

const skillLabels = ["Beginner", "Intermediate", "Advanced", "Expert"];

export function CreateJobComponent({ organizationId, onCreated }: CreateJobComponentProps) {
  const [form, setForm] = useState(() => emptyForm(organizationId));
  const [skill, setSkill] = useState<JobSkill>(emptySkill);
  const [requiredSkills, setRequiredSkills] = useState<JobSkill[]>([]);
  const [saving, setSaving] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [success, setSuccess] = useState<string | null>(null);

  const resolvedOrganizationId = organizationId ?? form.organizationId;
  const canSubmit = useMemo(
    () => resolvedOrganizationId.trim().length > 0 && Number(form.hourlyRateAmount) >= 0,
    [form.hourlyRateAmount, resolvedOrganizationId]
  );
  const canAddSkill = skill.name.trim().length > 0
    && skill.field.trim().length > 0
    && skill.description.trim().length > 0;

  const addSkill = () => {
    if (!canAddSkill) return;
    setRequiredSkills(current => [...current, skill]);
    setSkill(emptySkill);
  };

  const removeSkill = (indexToRemove: number) => {
    setRequiredSkills(current => current.filter((_, index) => index !== indexToRemove));
  };

  const createJob = async (event: FormEvent<HTMLFormElement>) => {
    event.preventDefault();
    setSaving(true);
    setError(null);
    setSuccess(null);

    try {
      const skillsToSave = canAddSkill ? [...requiredSkills, skill] : requiredSkills;

      await httpClient.post("/job", {
        organizationId: resolvedOrganizationId,
        hourlyRateAmount: Number(form.hourlyRateAmount),
        hourlyRateCurrency: form.hourlyRateCurrency,
        latitude: form.latitude,
        longitude: form.longitude,
        addressLine1: form.addressLine1,
        addressLine2: form.addressLine2 || null,
        city: form.city,
        state: form.state,
        country: form.country,
        postalCode: form.postalCode,
        requiredSkills: skillsToSave,
        title: form.title
      });

      setForm(emptyForm(organizationId));
      setRequiredSkills([]);
      setSkill(emptySkill);
      setSuccess("Job created successfully.");
      await onCreated?.();
    } catch (err) {
      setError(err instanceof Error ? err.message : "Failed to create job");
    } finally {
      setSaving(false);
    }
  };

  return (
    <section className="job-details job-create">
      <header className="job-details-header">
        <div>
          <p className="job-details-kicker">Job</p>
          <h2 className="job-details-title">Create Job</h2>
        </div>
      </header>

      {error && <p className="job-details-error">{error}</p>}
      {success && <p className="job-details-success">{success}</p>}

      <form className="job-details-section job-form job-create-form" onSubmit={createJob}>
        {!organizationId && (
          <label className="full-width">
            Organization ID
            <input value={form.organizationId} onChange={event => setForm({ ...form, organizationId: event.target.value })} required />
          </label>
        )}
        
        <label>
          Job Title
          <input type="text"  value={form.title} onChange={event => setForm({ ...form, title: event.target.value })} required />
        </label>
        <label>
          Hourly rate amount
          <input type="number" min="0" step="0.01" value={form.hourlyRateAmount} onChange={event => setForm({ ...form, hourlyRateAmount: event.target.value })} required />
        </label>
        <label>
          Currency
          <input value={form.hourlyRateCurrency} onChange={event => setForm({ ...form, hourlyRateCurrency: event.target.value })} required />
        </label>
        <label>
          Latitude
          <input value={form.latitude} onChange={event => setForm({ ...form, latitude: event.target.value })} required />
        </label>
        <label>
          Longitude
          <input value={form.longitude} onChange={event => setForm({ ...form, longitude: event.target.value })} required />
        </label>
        <label className="full-width">
          Address line 1
          <input value={form.addressLine1} onChange={event => setForm({ ...form, addressLine1: event.target.value })} required />
        </label>
        <label className="full-width">
          Address line 2
          <input value={form.addressLine2} onChange={event => setForm({ ...form, addressLine2: event.target.value })} />
        </label>
        <label>
          City
          <input value={form.city} onChange={event => setForm({ ...form, city: event.target.value })} required />
        </label>
        <label>
          State
          <input value={form.state} onChange={event => setForm({ ...form, state: event.target.value })} required />
        </label>
        <label>
          Country
          <input value={form.country} onChange={event => setForm({ ...form, country: event.target.value })} required />
        </label>
        <label>
          Postal code
          <input value={form.postalCode} onChange={event => setForm({ ...form, postalCode: event.target.value })} required />
        </label>

        <div className="job-create-skills-editor full-width">
          <div className="job-section-heading">
            <h3>Required Skills</h3>
          </div>

          <div className="job-form job-skill-form">
            <label>
              Name
              <input value={skill.name} onChange={event => setSkill({ ...skill, name: event.target.value })} />
            </label>
            <label>
              Field
              <input value={skill.field} onChange={event => setSkill({ ...skill, field: event.target.value })} />
            </label>
            <label className="full-width">
              Description
              <textarea value={skill.description} onChange={event => setSkill({ ...skill, description: event.target.value })} />
            </label>
            <label>
              Level
              <select value={skill.skillLevel} onChange={event => setSkill({ ...skill, skillLevel: Number(event.target.value) })}>
                {skillLabels.map((label, index) => <option key={label} value={index}>{label}</option>)}
              </select>
            </label>
            <div className="job-form-actions full-width">
              <button type="button" onClick={addSkill} disabled={saving || !canAddSkill}>Add skill</button>
            </div>
          </div>

          {requiredSkills.length ? (
            <div className="job-skills job-create-skills">
              {requiredSkills.map((item, index) => (
                <article className="job-skill" key={`${item.name}-${item.field}-${index}`}>
                  <button type="button" onClick={() => removeSkill(index)} disabled={saving}>Remove</button>
                  <h4>{item.name}</h4>
                  <p className="job-skill-field">{item.field}</p>
                  <p>{item.description}</p>
                  <span>{skillLabels[item.skillLevel] ?? "Unknown"}</span>
                </article>
              ))}
            </div>
          ) : (
            <p className="job-empty">No required skills added.</p>
          )}
        </div>

        <div className="job-form-actions full-width">
          <button type="submit" disabled={saving || !canSubmit}>Create job</button>
        </div>
      </form>
    </section>
  );
}

export default CreateJobComponent;
