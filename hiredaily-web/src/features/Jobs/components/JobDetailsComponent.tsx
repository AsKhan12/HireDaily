import { useEffect, useMemo, useState, type FormEvent } from "react";
import "./JobDetailsComponent.css";
import type { Job } from "../../../types/Job";
import type { JobSkill } from "../../../types/JobSkill";
import { httpClient } from "../../../api/httpClient";

type JobDetailsComponentProps = {
  job: Job;
  onJobChange?: (job: Job) => void;
  readOnly?: boolean;
};

type JobSiteForm = {
  latitude: string;
  longitude: string;
  addressLine1: string;
  addressLine2: string;
  city: string;
  state: string;
  country: string;
  postalCode: string;
};

type HourlyRateForm = {
  amount: string;
  currency: string;
};

type JobTitleForm = {
  title: string | null;
}

const emptySkill: JobSkill = {
  name: "",
  field: "",
  description: "",
  skillLevel: 0
};

const skillLabels = ["Beginner", "Intermediate", "Advanced", "Expert"];

const formatDate = (value: string | null) => {
  if (!value) return "Not updated";

  const date = new Date(value);
  return Number.isNaN(date.getTime()) ? value : date.toLocaleString();
};

const toHourlyRateForm = (job: Job): HourlyRateForm => ({
  amount: String(job.hourlyRate.amount),
  currency: job.hourlyRate.currency
});

const toJobSiteForm = (job: Job): JobSiteForm => ({
  latitude: job.jobSite.location.lat,
  longitude: job.jobSite.location.long,
  addressLine1: job.jobSite.address.addressLine1,
  addressLine2: job.jobSite.address.addressLine2 ?? "",
  city: job.jobSite.address.city,
  state: job.jobSite.address.state,
  country: job.jobSite.address.country,
  postalCode: job.jobSite.address.postalCode
});

const toTitleForm = (job: Job): JobTitleForm => ({
  title: job.title
});

export function JobDetailsComponent({ job, onJobChange, readOnly = false }: JobDetailsComponentProps) {
  const [currentJob, setCurrentJob] = useState(job);
  const [hourlyRate, setHourlyRate] = useState(() => toHourlyRateForm(job));
  const [jobSite, setJobSite] = useState(() => toJobSiteForm(job));
  const [skill, setSkill] = useState<JobSkill>(emptySkill);
  const [editingRate, setEditingRate] = useState(false);
  const [editingSite, setEditingSite] = useState(false);
  const [addingSkill, setAddingSkill] = useState(false);
  const [editingSkillIndex, setEditingSkillIndex] = useState<number | null>(null);
  const [saving, setSaving] = useState(false);
  const [title, setTitle] = useState(() => toTitleForm(job));
  const [editingTitle, setEditingTitle] = useState(false);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    setCurrentJob(job);
    setHourlyRate(toHourlyRateForm(job));
    setJobSite(toJobSiteForm(job));
    setSkill(emptySkill);
    setEditingRate(false);
    setEditingSite(false);
    setAddingSkill(false);
    setEditingSkillIndex(null);
    setError(null);
    setTitle(toTitleForm(job));
  }, [job]);

  const jobId = currentJob.jobId.value;

  const addressSummary = useMemo(
    () => [
      currentJob.jobSite.address.addressLine1,
      currentJob.jobSite.address.addressLine2,
      currentJob.jobSite.address.city,
      currentJob.jobSite.address.state,
      currentJob.jobSite.address.country,
      currentJob.jobSite.address.postalCode
    ].filter(Boolean).join(", "),
    [currentJob]
  );

  const applyJob = (updatedJob: Job) => {
    setCurrentJob(updatedJob);
    onJobChange?.(updatedJob);
  };

  const save = async (action: () => Promise<Job>) => {
    setSaving(true);
    setError(null);

    try {
      applyJob(await action());
      return true;
    } catch (err) {
      setError(err instanceof Error ? err.message : "Failed to update job");
      return false;
    } finally {
      setSaving(false);
    }
  };

  const saveHourlyRate = async (event: FormEvent<HTMLFormElement>) => {
    event.preventDefault();

    const amount = Number(hourlyRate.amount);
    const saved = await save(async () => {
      await httpClient.put(`/job/${jobId}/hourly-rate`, {
        amount,
        currency: hourlyRate.currency
      });

      return {
        ...currentJob,
        hourlyRate: {
          amount,
          currency: hourlyRate.currency.toUpperCase()
        },
        lastUpdateAt: new Date().toISOString()
      };
    });

    if (saved) setEditingRate(false);
  };

  const saveTitle = async (event: FormEvent<HTMLFormElement>) => {
    event.preventDefault();
    const saved = await save(async () => {
      await httpClient.put(`/job/${jobId}/title`, { title: title.title });
      return {
        ...currentJob,
        title: title.title ?? "",
        lastUpdateAt: new Date().toISOString()
      };
    });
    if (saved) setEditingTitle(false);
  };

  const saveJobSite = async (event: FormEvent<HTMLFormElement>) => {
    event.preventDefault();

    const saved = await save(async () => {
      await httpClient.put(`/job/${jobId}/site`, {
        latitude: jobSite.latitude,
        longitude: jobSite.longitude,
        addressLine1: jobSite.addressLine1,
        addressLine2: jobSite.addressLine2 || null,
        city: jobSite.city,
        state: jobSite.state,
        country: jobSite.country,
        postalCode: jobSite.postalCode
      });

      return {
        ...currentJob,
        jobSite: {
          location: {
            lat: jobSite.latitude,
            long: jobSite.longitude
          },
          address: {
            addressLine1: jobSite.addressLine1,
            addressLine2: jobSite.addressLine2 || null,
            city: jobSite.city,
            state: jobSite.state,
            country: jobSite.country,
            postalCode: jobSite.postalCode
          }
        },
        lastUpdateAt: new Date().toISOString()
      };
    });

    if (saved) setEditingSite(false);
  };

  const saveRequiredSkills = async (requiredSkills: JobSkill[]) => {
    const saved = await save(async () => {
      await httpClient.put(`/job/${jobId}/required-skills`, {
        requiredSkills
      });

      return {
        ...currentJob,
        requiredSkills,
        lastUpdateAt: new Date().toISOString()
      };
    });

    return saved;
  };

  const addSkill = async (event: FormEvent<HTMLFormElement>) => {
    event.preventDefault();

    const saved = await saveRequiredSkills([...currentJob.requiredSkills, skill]);
    if (saved) {
      setSkill(emptySkill);
      setAddingSkill(false);
    }
  };

  const startEditingSkill = (index: number) => {
    setSkill(currentJob.requiredSkills[index]);
    setAddingSkill(false);
    setEditingSkillIndex(index);
  };

  const cancelSkillEdit = () => {
    setSkill(emptySkill);
    setAddingSkill(false);
    setEditingSkillIndex(null);
  };

  const saveSkillEdit = async (event: FormEvent<HTMLFormElement>) => {
    event.preventDefault();
    if (editingSkillIndex === null) return;

    const saved = await saveRequiredSkills(currentJob.requiredSkills.map((item, index) => (
      index === editingSkillIndex ? skill : item
    )));

    if (saved) {
      cancelSkillEdit();
    }
  };

  const removeSkill = async (indexToRemove: number) => {
    await saveRequiredSkills(currentJob.requiredSkills.filter((_, index) => index !== indexToRemove));
  };

  return (
    <section className="job-details">
      <header className="job-details-header">
        <div>
          <p className="job-details-kicker">Job</p>
          <h2 className="job-details-title">{currentJob.hourlyRate.currency} {currentJob.hourlyRate.amount}/hr</h2>
        </div>
        <dl className="job-details-meta">
          <div>
            <dt>Created</dt>
            <dd>{formatDate(currentJob.createdAt)}</dd>
          </div>
          <div>
            <dt>Updated</dt>
            <dd>{formatDate(currentJob.lastUpdateAt)}</dd>
          </div>
        </dl>
      </header>

      {error && <p className="job-details-error">{error}</p>}

      <section className="job-details-section">
        <div className="job-section-heading">
          <h3>Title</h3>
          {!readOnly && <button type="button" onClick={() => setEditingTitle(true)} disabled={saving || editingTitle}>Edit</button>}
        </div>

        {!readOnly && editingTitle ? (
          <form className="job-form job-rate-form" onSubmit={saveTitle}>
            <label>
              Title
              <input type="text" value={title.title ?? ""} onChange={event => setTitle({ ...title, title: event.target.value })} required />
            </label>
            <div className="job-form-actions">
              <button type="submit" disabled={saving}>Save</button>
              <button type="button" className="secondary" onClick={() => { setTitle(toTitleForm(currentJob)); setEditingTitle(false); }} disabled={saving}>Cancel</button>
            </div>
          </form>
        ) : (
          <p className="job-value">{currentJob.title}</p>
        )}
      </section>
      <section className="job-details-section">
        <div className="job-section-heading">
          <h3>Hourly Rate</h3>
          {!readOnly && <button type="button" onClick={() => setEditingRate(true)} disabled={saving || editingRate}>Edit</button>}
        </div>

        {!readOnly && editingRate ? (
          <form className="job-form job-rate-form" onSubmit={saveHourlyRate}>
            <label>
              Amount
              <input type="number" min="0" step="0.01" value={hourlyRate.amount} onChange={event => setHourlyRate({ ...hourlyRate, amount: event.target.value })} required />
            </label>
            <label>
              Currency
              <input value={hourlyRate.currency} onChange={event => setHourlyRate({ ...hourlyRate, currency: event.target.value })} required />
            </label>
            <div className="job-form-actions">
              <button type="submit" disabled={saving}>Save</button>
              <button type="button" className="secondary" onClick={() => { setHourlyRate(toHourlyRateForm(currentJob)); setEditingRate(false); }} disabled={saving}>Cancel</button>
            </div>
          </form>
        ) : (
          <p className="job-value">{currentJob.hourlyRate.amount} {currentJob.hourlyRate.currency} per hour</p>
        )}
      </section>

      <section className="job-details-section">
        <div className="job-section-heading">
          <h3>Site</h3>
          {!readOnly && <button type="button" onClick={() => setEditingSite(true)} disabled={saving || editingSite}>Edit</button>}
        </div>

        {!readOnly && editingSite ? (
          <form className="job-form job-site-form" onSubmit={saveJobSite}>
            <label>
              Latitude
              <input value={jobSite.latitude} onChange={event => setJobSite({ ...jobSite, latitude: event.target.value })} required />
            </label>
            <label>
              Longitude
              <input value={jobSite.longitude} onChange={event => setJobSite({ ...jobSite, longitude: event.target.value })} required />
            </label>
            <label className="full-width">
              Address line 1
              <input value={jobSite.addressLine1} onChange={event => setJobSite({ ...jobSite, addressLine1: event.target.value })} required />
            </label>
            <label className="full-width">
              Address line 2
              <input value={jobSite.addressLine2} onChange={event => setJobSite({ ...jobSite, addressLine2: event.target.value })} />
            </label>
            <label>
              City
              <input value={jobSite.city} onChange={event => setJobSite({ ...jobSite, city: event.target.value })} required />
            </label>
            <label>
              State
              <input value={jobSite.state} onChange={event => setJobSite({ ...jobSite, state: event.target.value })} required />
            </label>
            <label>
              Country
              <input value={jobSite.country} onChange={event => setJobSite({ ...jobSite, country: event.target.value })} required />
            </label>
            <label>
              Postal code
              <input value={jobSite.postalCode} onChange={event => setJobSite({ ...jobSite, postalCode: event.target.value })} required />
            </label>
            <div className="job-form-actions full-width">
              <button type="submit" disabled={saving}>Save</button>
              <button type="button" className="secondary" onClick={() => { setJobSite(toJobSiteForm(currentJob)); setEditingSite(false); }} disabled={saving}>Cancel</button>
            </div>
          </form>
        ) : (
          <div className="job-details-list">
            <p><span>Address</span>{addressSummary}</p>
            <p><span>Location</span>{currentJob.jobSite.location.lat}, {currentJob.jobSite.location.long}</p>
          </div>
        )}
      </section>

      <section className="job-details-section">
        <div className="job-section-heading">
          <h3>Required Skills</h3>
          {!readOnly && <button type="button" onClick={() => { setSkill(emptySkill); setEditingSkillIndex(null); setAddingSkill(true); }} disabled={saving || addingSkill}>Add</button>}
        </div>

        {!readOnly && addingSkill && (
          <form className="job-form job-skill-form" onSubmit={addSkill}>
            <label>
              Name
              <input value={skill.name} onChange={event => setSkill({ ...skill, name: event.target.value })} required />
            </label>
            <label>
              Field
              <input value={skill.field} onChange={event => setSkill({ ...skill, field: event.target.value })} required />
            </label>
            <label className="full-width">
              Description
              <textarea value={skill.description} onChange={event => setSkill({ ...skill, description: event.target.value })} required />
            </label>
            <label>
              Level
              <select value={skill.skillLevel} onChange={event => setSkill({ ...skill, skillLevel: Number(event.target.value) })}>
                {skillLabels.map((label, index) => <option key={label} value={index}>{label}</option>)}
              </select>
            </label>
            <div className="job-form-actions full-width">
              <button type="submit" disabled={saving}>Save</button>
              <button type="button" className="secondary" onClick={() => { setSkill(emptySkill); setAddingSkill(false); }} disabled={saving}>Cancel</button>
            </div>
          </form>
        )}

        {!readOnly && editingSkillIndex !== null && (
          <form className="job-form job-skill-form" onSubmit={saveSkillEdit}>
            <label>
              Name
              <input value={skill.name} onChange={event => setSkill({ ...skill, name: event.target.value })} required />
            </label>
            <label>
              Field
              <input value={skill.field} onChange={event => setSkill({ ...skill, field: event.target.value })} required />
            </label>
            <label className="full-width">
              Description
              <textarea value={skill.description} onChange={event => setSkill({ ...skill, description: event.target.value })} required />
            </label>
            <label>
              Level
              <select value={skill.skillLevel} onChange={event => setSkill({ ...skill, skillLevel: Number(event.target.value) })}>
                {skillLabels.map((label, index) => <option key={label} value={index}>{label}</option>)}
              </select>
            </label>
            <div className="job-form-actions full-width">
              <button type="submit" disabled={saving}>Save</button>
              <button type="button" className="secondary" onClick={cancelSkillEdit} disabled={saving}>Cancel</button>
            </div>
          </form>
        )}

        {currentJob.requiredSkills.length ? (
          <div className="job-skills">
            {currentJob.requiredSkills.map((item, index) => (
              <article className="job-skill" key={`${item.name}-${item.field}-${index}`}>
                {!readOnly && <div className="job-skill-actions">
                  <button type="button" onClick={() => startEditingSkill(index)} disabled={saving || editingSkillIndex === index}>Edit</button>
                  <button type="button" className="danger" onClick={() => void removeSkill(index)} disabled={saving}>Remove</button>
                </div>}
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
      </section>
    </section>
  );
}

export default JobDetailsComponent;
