import type { EntityId } from "./EntityId";
import type { JobSite } from "./JobSite";
import type { JobSkill } from "./JobSkill";
import type { Money } from "./Money";

export interface Job {
  jobId: EntityId;
  organizationId: EntityId;
  hourlyRate: Money;
  jobSite: JobSite;
  requiredSkills: JobSkill[];
  createdAt: string;
  lastUpdateAt: string | null;
  title: string | null
}


