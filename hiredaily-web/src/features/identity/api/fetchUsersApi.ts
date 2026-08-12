import { httpClient } from "../../../api/httpClient";
import type { UserProfile } from "../../Profile/types/UserProfile";

type UserProfileResponse = Omit<UserProfile, "userId"> & {
    userId: string | { value: string };
};

export async function fetchUserApi(
    id: string): Promise<UserProfile> {
    try {
        const response = await httpClient.get<UserProfileResponse>(`/user/${id}`);
        const profile = response.data;

        return {
            ...profile,
            userId: typeof profile.userId === "string"
                ? profile.userId
                : profile.userId.value
        };
    } catch (error) {
        console.error("Failed to fetch user profile:", error);
        throw error;
    }
}
