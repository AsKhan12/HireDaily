# User

- Id (Guid)
- Name 
- Phone Number (Unique)
- Email (Unique)
- UserType {Job Applicant / OrganizationAdmin}
- CreatedAt
- Status { Active, Paused. Suspended} 

# WorkerProfile

- Id (GUID)
- UserId (FK → User)
- Skills (list / normalized table)
- IsAvailable (bool)
- CurrentLocation (lat, long)
- Rating (float)
- CompletedJobsCount

# EmployerProfile

- Id (GUID)
- UserIds (FK → User, UserType -> organizationAdmin)
- CompanyName
- Rating
- CreatedAt

# Job

- Id (GUID)
- EmployerId (FK → EmployerProfile)
- Title
- Description
- RequiredSkills
- Location (lat, long)
- WageAmount
- Status (Open / Assigned / InProgress / Completed / Cancelled / Disputed)
- CreatedAt
- StartTime (expected)
- ExpiryTime (optional)

# JobApplication
- Id (GUID)
- JobId (FK → Job)
- WorkerId (FK → WorkerProfile)
- Status (Applied / Selected / Rejected)
- AppliedAt

# Assignment
- Id (GUID)
- JobId (FK → Job) UNIQUE
- WorkerId (FK → WorkerProfile)
- Status (PendingAcceptance / Accepted / Rejected / Expired)
- AssignedAt
- AcceptedAt

# Payment
- Id (GUID)
- JobId (FK → Job)
- EmployerId
- WorkerId
- Amount
- Status (Locked / Released / Refunded / Disputed)
- LockedAt
- ReleasedAt

# Dispute
- Id (GUID)
- JobId
- RaisedBy (Worker / Employer)
- Reason
- Status (Open / UnderReview / Resolved)
- ResolutionNotes
- CreatedAt


User
 ├── WorkerProfile (1:1)
 └── EmployerProfile (1:1)

EmployerProfile → Job (1:N)

Job → JobApplication (1:N)
Job → Assignment (1:1)
Job → Payment (1:1)
Job → Dispute (0..1 or more)

WorkerProfile → JobApplication (1:N)
WorkerProfile → Assignment (1:N)