import { useCallback, useEffect, useMemo, useState } from "react";
import { httpClient } from "../../../api/httpClient";
import { useAuth } from "../../identity/context/AuthContext";
import UserProfile from "../../Profile/pages/UserProfile";
import type { Job } from "../../../types/Job";
import "./UserHomePage.css";
import { ProfileHeader } from "../../Profile/components/ProfileHeader";
import JobDetailsComponent from "../../Jobs/components/JobDetailsComponent";

const formatDate = (value: string | null) => {
  if (!value) return "Not updated";

  const date = new Date(value);
  return Number.isNaN(date.getTime()) ? value : date.toLocaleDateString();
};

const getAddressSummary = (job: Job) => [
  job.jobSite.address.addressLine1,
  job.jobSite.address.city,
  job.jobSite.address.state,
  job.jobSite.address.country
].filter(Boolean).join(", ");

export default function UserHomepage() {
  const { user, isLoading } = useAuth();
  const [jobs, setJobs] = useState<Job[]>([]);
  const [selectedJobId, setSelectedJobId] = useState<string | null>(null);
  const [loadingJobs, setLoadingJobs] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [showUserProfile, setShowUserProfile] = useState(false);
  const [userLocation, setUserLocation] = useState<{ lat: string; long: string } | null>(null);
  const [userSkills, setUserSkills] = useState<string[]>([]);

  const selectedJob = useMemo(
    () => jobs.find(job => job.jobId.value === selectedJobId) ?? null,
    [jobs, selectedJobId]
  );

  const fetchUserProfile = useCallback(async (userId: string) => {
    try {
      const response = await httpClient.get<any>(`/user/${userId}`);
      const userData = response.data;
      
      if (userData.address?.locatoin) {
        setUserLocation({
          lat: userData.address.locatoin.lat,
          long: userData.address.locatoin.long
        });
      }
      
      if (userData.skills && Array.isArray(userData.skills)) {
        setUserSkills(userData.skills.map((skill: any) => skill.name));
      }
    } catch (err) {
      console.error("Failed to fetch user profile:", err);
    }
  }, []);

  const loadFeed = useCallback(async () => {
    setLoadingJobs(true);
    setError(null);

    try {
      const response = await httpClient.post<Job[]>("/feed/search", {
        location: userLocation,
        skills: userSkills.length > 0 ? userSkills : null
      });
      const feedJobs = Array.isArray(response.data) ? response.data : [];

      setJobs(feedJobs);
    } catch (err) {
      setJobs([]);
      setSelectedJobId(null);
      setError(err instanceof Error ? err.message : "Failed to load job feed");
    } finally {
      setLoadingJobs(false);
    }
  }, [userLocation, userSkills]);

  useEffect(() => {
    if (isLoading) return;

    if (!user) {
      setJobs([]);
      setSelectedJobId(null);
      setLoadingJobs(false);
      return;
    }

    void fetchUserProfile(user.userId);
  }, [isLoading, fetchUserProfile, user]);

  useEffect(() => {
    if (!user || userLocation === null) return;

    void loadFeed();
  }, [user, userLocation, userSkills, loadFeed]);

  if (isLoading || loadingJobs) {
    return <main className="user-home-page"><p className="user-home-status">Loading jobs...</p></main>;
  }

  if (!user) {
    return <main className="user-home-page"><p className="user-home-status">Sign in to view job feed.</p></main>;
  }

  return (
    <main className="user-home-page">
      <>
        <ProfileHeader
          name={user.name}
          username={user.username}
        />
        <div className="user-home-heading">
          <div>
            <h1>Job Feed</h1>
          </div>
          <span>{jobs.length} available</span>
        </div>
        <section className="user-home-panel">
          {error && <p className="user-home-error">{error}</p>}

          {jobs.length ? (
            <div className="user-jobs-table-wrap">
              <table className="user-jobs-table">
                <thead>
                  <tr>
                    <th>Rate</th>
                    <th>Location</th>
                    <th>Skills</th>
                    <th>Created</th>
                    <th>Updated</th>
                  </tr>
                </thead>
                <tbody>
                  {jobs.map(job => (
                    <tr
                      key={job.jobId.value}
                      className={job.jobId.value === selectedJobId ? "selected" : undefined}
                      onClick={() => setSelectedJobId(job.jobId.value)}
                      style={{ cursor: "pointer" }}
                    >
                      <td>{job.hourlyRate.amount} {job.hourlyRate.currency}/hr</td>
                      <td>{getAddressSummary(job)}</td>
                      <td>{job.requiredSkills.length}</td>
                      <td>{formatDate(job.createdAt)}</td>
                      <td>{formatDate(job.lastUpdateAt)}</td>
                    </tr>
                  ))}
                </tbody>
              </table>
            </div>
          ) : (
            <p className="user-home-empty">No jobs found. Try adjusting your search criteria.</p>
          )}
        </section>

        {selectedJob && (
          <div className="user-modal-backdrop" role="presentation">
            <section className="user-modal" role="dialog" aria-modal="true" aria-label="Job details">
              <button type="button" className="user-modal-close" onClick={() => setSelectedJobId(null)}>Close</button>
              <div className="user-modal-body">
                <JobDetailsComponent job={selectedJob} onJobChange={() => {}} readOnly={true} />
              </div>
            </section>
          </div>
        )}

        <div className="user-home-actions" aria-label="User actions">
          <button
            type="button"
            className="user-fab primary"
            onClick={() => setShowUserProfile(true)}
          >
            Profile
          </button>
        </div>

        {showUserProfile && (
          <div className="user-modal-backdrop" role="presentation">
            <section className="user-modal profile-modal" role="dialog" aria-modal="true" aria-label="User profile">
              <button type="button" className="user-modal-close" onClick={() => setShowUserProfile(false)}>Close</button>
              <div className="user-modal-body">
                <UserProfile />
              </div>
            </section>
          </div>
        )}
      </>
    </main>
  );
}