import type { FormEvent } from "react";
import type { JobSkill } from "../types/JobSkill";

type SkillsSectionProps = {
  skills: JobSkill[];
  skill: JobSkill;
  adding: boolean;
  saving: boolean;
  onSkillChange: (skill: JobSkill) => void;
  onAdd: () => void;
  onCancel: () => void;
  onSave: () => Promise<void>;
  onRemove: (skill: JobSkill) => Promise<void>;
};

const skillLabels = ["Beginner", "Intermediate", "Advanced", "Expert"];

export function SkillsSection({
  skills,
  skill,
  adding,
  saving,
  onSkillChange,
  onAdd,
  onCancel,
  onSave,
  onRemove
}: SkillsSectionProps) {
  const handleSubmit = async (event: FormEvent<HTMLFormElement>) => {
    event.preventDefault();
    await onSave();
  };

  return (
    <section className="profile-section editable-section" onClick={onAdd}>
      <div className="section-heading">
        <h2 className="section-title">Skills</h2>
        <span className="edit-hint">Click to add a skill</span>
      </div>
      {adding && (
        <form className="edit-form form-grid skill-form" onClick={event => event.stopPropagation()} onSubmit={handleSubmit}>
          <label>Name<input value={skill.name} onChange={event => onSkillChange({ ...skill, name: event.target.value })} required /></label>
          <label>Field<input value={skill.field} onChange={event => onSkillChange({ ...skill, field: event.target.value })} required /></label>
          <label className="full-width">Description<textarea value={skill.description} onChange={event => onSkillChange({ ...skill, description: event.target.value })} required /></label>
          <label>
            Level
            <select value={skill.skillLevel} onChange={event => onSkillChange({ ...skill, skillLevel: Number(event.target.value) })}>
              {skillLabels.map((label, index) => <option key={label} value={index}>{label}</option>)}
            </select>
          </label>
          <div className="form-actions full-width">
            <button disabled={saving} type="submit">Add skill</button>
            <button type="button" className="secondary" onClick={onCancel}>Cancel</button>
          </div>
        </form>
      )}
      {skills.length ? (
        <div className="skills-grid">
          {skills.map((item, index) => (
            <div key={`${item.name}-${index}`} className="skill-card" onClick={event => event.stopPropagation()}>
              <button className="remove-skill" disabled={saving} onClick={() => void onRemove(item)}>Remove</button>
              <h3 className="skill-name">{item.name}</h3>
              <p className="skill-field">{item.field}</p>
              <p className="skill-description">{item.description}</p>
              <div className="skill-level">
                <span className="skill-level-label">{skillLabels[item.skillLevel] ?? "Unknown"}</span>
                <div className="skill-level-bar">
                  <div className="skill-level-fill" style={{ width: `${((item.skillLevel + 1) / 4) * 100}%` }} />
                </div>
              </div>
            </div>
          ))}
        </div>
      ) : <p className="no-data">No skills added yet</p>}
    </section>
  );
}
