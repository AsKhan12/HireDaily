import { useCallback, useEffect, useMemo, useState } from "react";
import { httpClient } from "../../../api/httpClient";
import { useAuth } from "../../identity/context/AuthContext";
import OrganizationProfile from "../../Profile/pages/OrganizationProfile";
import type { Job } from "../../../types/Job";
import type { OrganizationJobsResponse } from "../../Profile/types/OrganizationJobsResponse";
import "./OrganizationHomePage.css";
import { ProfileHeader } from "../../Profile/components/ProfileHeader";
import JobDetailsComponent from "../../Jobs/components/JobDetailsComponent";
import CreateJobComponent from "../../Jobs/components/CreateJobComponent";

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

export default function OrganizationHomePage() {
  const { user, isLoading } = useAuth();
  const [jobs, setJobs] = useState<Job[]>([]);
  const [selectedJobId, setSelectedJobId] = useState<string | null>(null);
  const [loadingJobs, setLoadingJobs] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [showCreateJob, setShowCreateJob] = useState(false);
  const [showOrganizationProfile, setShowOrganizationProfile] = useState(false);

  const selectedJob = useMemo(
    () => jobs.find(job => job.jobId.value === selectedJobId) ?? null,
    [jobs, selectedJobId]
  );

  const loadOrganizationJobs = useCallback(async (organizationId: string) => {
    setLoadingJobs(true);
    setError(null);

    try {
      const response = await httpClient.get<OrganizationJobsResponse>(`/job/organization/${organizationId}`);
      const organizationJobs = response.data.jobs ?? [];

      setJobs(organizationJobs);
      // setSelectedJobId(current => (
      //   current && organizationJobs.some(job => job.jobId.value === current)
      //     ? current
      //     : organizationJobs[0]?.jobId.value ?? null
      // ));
    } catch (err) {
      setJobs([]);
      setSelectedJobId(null);
      setError(err instanceof Error ? err.message : "Failed to load organization jobs");
    } finally {
      setLoadingJobs(false);
    }
  }, []);

  useEffect(() => {
    if (isLoading) return;

    if (!user) {
      setJobs([]);
      setSelectedJobId(null);
      setLoadingJobs(false);
      return;
    }

    void loadOrganizationJobs(user.userId);
  }, [isLoading, loadOrganizationJobs, user]);

  const updateJob = (updatedJob: Job) => {
    setJobs(current => current.map(job => (
      job.jobId.value === updatedJob.jobId.value ? updatedJob : job
    )));
  };

  if (isLoading || loadingJobs) {
    return <main className="organization-home-page"><p className="organization-home-status">Loading jobs...</p></main>;
  }

  if (!user) {
    return <main className="organization-home-page"><p className="organization-home-status">Sign in to view organization jobs.</p></main>;
  }

  return (
    <main className="organization-home-page">
      <>
        <ProfileHeader
          name={user.name}
          username={user.username}
        />
        <div className="organization-home-heading">
          <div>
            <h1>Jobs</h1>
          </div>
          <span>{jobs.length} total</span>
        </div>
        <section className="organization-home-panel">
          {error && <p className="organization-home-error">{error}</p>}

          {jobs.length ? (
            <div className="organization-jobs-table-wrap">
              <table className="organization-jobs-table">
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
            <p className="organization-home-empty">No jobs found for this organization.</p>
          )}
        </section>

        {selectedJob && (
          <div className="organization-modal-backdrop" role="presentation">
            <section className="organization-modal" role="dialog" aria-modal="true" aria-label="Create job">
              <button type="button" className="organization-modal-close" onClick={() => setSelectedJobId(null)}>Close</button>
              <div className="organization-modal-body">
                <JobDetailsComponent job={selectedJob} onJobChange={updateJob} />
              </div>
            </section>
          </div>
        )}

        <div className="organization-home-actions" aria-label="Organization actions">
          <button
            type="button"
            className="organization-fab"
            onClick={() => setShowOrganizationProfile(true)}
          >
            Profile
          </button>
          <button
            type="button"
            className="organization-fab primary"
            onClick={() => setShowCreateJob(true)}
          >
            Create
          </button>
        </div>

        {showCreateJob && (
          <div className="organization-modal-backdrop" role="presentation">
            <section className="organization-modal" role="dialog" aria-modal="true" aria-label="Create job">
              <button type="button" className="organization-modal-close" onClick={() => setShowCreateJob(false)}>Close</button>
              <div className="organization-modal-body">
                <CreateJobComponent
                  organizationId={user.userId}
                  onCreated={async () => {
                    await loadOrganizationJobs(user.userId);
                    setShowCreateJob(false);
                  }}
                />
              </div>
            </section>
          </div>
        )}

        {showOrganizationProfile && (
          <div className="organization-modal-backdrop" role="presentation">
            <section className="organization-modal profile-modal" role="dialog" aria-modal="true" aria-label="Organization profile">
              <button type="button" className="organization-modal-close" onClick={() => setShowOrganizationProfile(false)}>Close</button>
              <div className="organization-modal-body">
                <OrganizationProfile onComplete={() => setShowOrganizationProfile(false)} onCompleteText="Exit"/>
              </div>
            </section>
          </div>
        )}
      </>
    </main>
  );
}
