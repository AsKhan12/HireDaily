import type { JobSkill } from "../../../types/JobSkill";
import type { UserAddress } from "../../identity/types/UserAddress";

export interface UserProfile {
  userId: string;
  name: string;
  username: string;
  address: UserAddress;
  skills: JobSkill[];
  createdAt: string;
  updatedAt: string | null;
}
