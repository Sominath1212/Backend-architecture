Absolutely. For a **production-grade Naukri.com-like job portal**, you should not start by asking the implementation team only for “.NET Web API + Clean Architecture + SQL Server.” You need a proper **Product Requirements Document (PRD)** covering users, workflows, modules, functional requirements, security, search, notifications, admin, recruiter tools, APIs, non-functional requirements, and production considerations.

Below is a detailed PRD you can directly give to your implementation team as the baseline specification.

# Product Requirements Document (PRD)

## Production-Grade Job Portal

**Product Type:** Online Job Recruitment & Career Platform
**Reference Products:** Naukri.com / Indeed / LinkedIn Jobs
**Backend:** ASP.NET Core Web API
**Architecture:** Clean Architecture
**Database:** Microsoft SQL Server
**Frontend:** Web application — framework to be finalized
**Deployment:** Cloud/Production environment
**API Style:** RESTful APIs
**Authentication:** JWT + Refresh Token
**Primary Users:** Job Seekers, Recruiters/Employers, Administrators

---

# 1. Product Vision

The application will be a centralized employment platform where:

* Job seekers can create professional profiles and resumes.
* Job seekers can search and filter jobs.
* Job seekers can apply for jobs.
* Recruiters can create company profiles.
* Recruiters can post and manage job vacancies.
* Recruiters can search and shortlist candidates.
* Recruiters can manage applications and recruitment stages.
* Administrators can control users, companies, jobs, reports, subscriptions and platform settings.

The system should be designed as a **scalable, secure and production-grade recruitment platform**, rather than a basic CRUD job website.

---

# 2. User Roles

The system should initially support these roles:

### 2.1 Job Seeker

A candidate looking for employment.

Main capabilities:

* Registration
* Login
* Profile creation
* Resume upload
* Resume management
* Job search
* Job filtering
* Job application
* Saved jobs
* Job alerts
* Application tracking
* Recruiter communication
* Interview management
* Notifications

---

### 2.2 Recruiter

A company representative who recruits candidates.

Main capabilities:

* Recruiter registration
* Company creation
* Company verification
* Job posting
* Job management
* Candidate search
* Candidate filtering
* Application management
* Candidate shortlisting
* Interview scheduling
* Recruiter-candidate communication
* Recruitment pipeline
* Recruiter dashboard

---

### 2.3 Company/Employer

A company account can have one or more recruiters.

Example:

```text
ABC Technologies
       |
       |--- HR Manager
       |--- Recruiter 1
       |--- Recruiter 2
       |--- Recruiter 3
```

This is important for a production system because companies may eventually have multiple recruiters.

---

### 2.4 Super Admin

Platform administrator.

Capabilities:

* User management
* Recruiter management
* Company management
* Job management
* Category management
* Location management
* Reports
* Verification
* Content moderation
* Subscription management
* Platform configuration
* Audit logs

---

### 2.5 Support/Admin Staff

Optional role.

Can handle:

* User complaints
* Recruiter complaints
* Job reports
* Account issues
* Support tickets

Permissions should be restricted compared with Super Admin.

---

# 3. High-Level System Architecture

Recommended architecture:

```text
                    ┌─────────────────────┐
                    │      Web Client     │
                    │ Angular/React/etc.  │
                    └──────────┬──────────┘
                               │
                               │ HTTPS
                               ▼
                    ┌─────────────────────┐
                    │    API Gateway /    │
                    │ Reverse Proxy       │
                    └──────────┬──────────┘
                               │
                               ▼
              ┌────────────────────────────────┐
              │      ASP.NET Core Web API      │
              │                                │
              │ Authentication / Authorization │
              │ Jobs                           │
              │ Candidates                     │
              │ Recruiters                     │
              │ Applications                   │
              │ Notifications                  │
              └───────────────┬────────────────┘
                              │
                    Clean Architecture
                              │
          ┌───────────────────┼──────────────────┐
          │                   │                  │
          ▼                   ▼                  ▼
    Presentation        Application          Domain
                              │
                              ▼
                       Infrastructure
                              │
                    ┌─────────┴──────────┐
                    ▼                    ▼
              SQL Server             Redis
```

---

# 4. Clean Architecture

The implementation team should follow:

```text
Solution
│
├── JobPortal.API
│
├── JobPortal.Application
│
├── JobPortal.Domain
│
├── JobPortal.Infrastructure
│
└── JobPortal.Tests
```

### Domain

Contains:

* Entities
* Value Objects
* Domain Events
* Enums
* Business rules

Example:

```text
Job
User
CandidateProfile
Company
Application
Resume
Interview
Notification
Subscription
```

---

### Application

Contains:

* Use cases
* CQRS if adopted
* DTOs
* Validators
* Interfaces
* Application services
* Business workflows

---

### Infrastructure

Contains:

* Entity Framework Core
* SQL Server
* Repository implementations
* Authentication infrastructure
* Email service
* File storage
* Redis
* Search infrastructure
* External APIs

---

### API

Contains:

* Controllers
* Middleware
* Authentication
* Exception handling
* API configuration
* Swagger/OpenAPI

---

# 5. Authentication & Account Module

## 5.1 Candidate Registration

Fields:

```text
First Name
Last Name
Email
Mobile Number
Password
Confirm Password
Location
```

Optional:

```text
Date of Birth
Gender
Current Job Status
Experience Level
```

Registration should include:

* Email validation
* Mobile validation
* Password policy
* Duplicate email detection
* Terms & Conditions acceptance

---

# 6. Authentication

Required functionality:

### Login

```text
Email/Mobile
Password
```

### Additional functionality

* JWT access token
* Refresh token
* Logout
* Forgot password
* Reset password
* Email verification
* Mobile OTP verification
* Account lockout
* Login attempt tracking
* Session management

---

# 7. Candidate Profile Module

Candidate profile should contain:

## Personal Information

```text
First Name
Last Name
Profile Photo
Email
Mobile
Location
Date of Birth
Gender
```

## Professional Information

```text
Current Designation
Total Experience
Current Company
Current Salary
Expected Salary
Notice Period
Employment Status
Preferred Job Type
Preferred Location
```

## Skills

Example:

```text
C#
ASP.NET Core
SQL Server
Angular
Entity Framework
Azure
Git
```

Each skill should ideally have:

```text
Skill
Experience
Last Used
Proficiency
```

---

# 8. Education Module

Candidate should be able to add multiple education records.

Example:

```text
Degree
Institution
Specialization
Start Year
End Year
Percentage/CGPA
```

Example:

```text
B.Tech
MIT College
Computer Science
2021 - 2025
8.4 CGPA
```

---

# 9. Experience Module

Multiple employment records.

Fields:

```text
Company
Designation
Employment Type
Start Date
End Date
Currently Working
Location
Description
Skills Used
```

---

# 10. Resume Module

Candidate should be able to:

* Upload resume
* Replace resume
* Delete resume
* Download resume
* Set primary resume
* Maintain multiple resumes

Supported formats:

```text
PDF
DOC
DOCX
```

File size limit should be configurable.

---

# 11. Resume Builder

Optional but highly valuable.

Candidate can create resume using templates.

Sections:

```text
Personal Details
Summary
Skills
Experience
Education
Projects
Certifications
Achievements
Languages
```

The system should generate a downloadable PDF.

---

# 12. Job Search Module

This is one of the most important modules.

Search should support:

```text
Job Title
Skills
Company
Location
```

Example:

```text
Python Developer
```

---

# 13. Job Filters

Candidates should be able to filter by:

### Experience

```text
Fresher
0-1 Years
1-3 Years
3-5 Years
5-10 Years
10+ Years
```

### Salary

```text
Minimum Salary
Maximum Salary
```

### Location

```text
City
State
Country
Remote
Hybrid
On-site
```

### Job Type

```text
Full Time
Part Time
Contract
Internship
Freelance
```

### Other

```text
Date Posted
Company
Industry
Education
Skills
Department
Work Mode
```

---

# 14. Job Listing

Each job should display:

```text
Job Title
Company
Company Logo
Location
Salary
Experience
Job Type
Work Mode
Skills
Posted Date
Application Deadline
Number of Openings
```

Example:

```text
Python Developer

ABC Technologies

Pune, Maharashtra
₹5 - ₹8 LPA
2-4 Years
Full Time
Hybrid

Python | Django | SQL | REST API
```

---

# 15. Job Details Page

Job details should include:

### Job Description

### Responsibilities

### Required Skills

### Preferred Skills

### Qualifications

### Experience

### Salary

### Benefits

### Work Location

### Work Mode

### Company Information

### Application Deadline

### Number of Vacancies

---

# 16. Apply Job Module

Candidate clicks:

**Apply Now**

System should check:

```text
Is user logged in?
Does candidate profile exist?
Does candidate have resume?
Has candidate already applied?
Is application deadline passed?
Is job active?
```

Application form may include:

```text
Resume
Cover Letter
Expected Salary
Notice Period
Additional Questions
```

---

# 17. Application Tracking

Candidate should see:

```text
Applied
        ↓
Application Received
        ↓
Under Review
        ↓
Shortlisted
        ↓
Interview
        ↓
Selected / Rejected
```

Candidate dashboard:

```text
Total Applications
Applications Under Review
Shortlisted
Interviews
Rejected
Selected
```

---

# 18. Saved Jobs

Candidate can:

```text
Save Job
Remove Saved Job
View Saved Jobs
```

---

# 19. Job Alerts

Candidate can create alerts.

Example:

```text
Keyword: Python Developer
Location: Pune
Experience: 0-2 years
Salary: ₹4-8 LPA
```

System sends notifications when matching jobs are posted.

Notification channels can include:

```text
Email
In-App
Push Notification
```

---

# 20. Recruiter Registration

Recruiter registration:

```text
Name
Email
Mobile
Password
Designation
Company
```

Recruiter account should go through verification where required.

---

# 21. Company Module

Company profile:

```text
Company Name
Logo
Cover Image
Website
Industry
Company Size
Founded Year
Headquarters
Description
Benefits
Social Links
```

---

# 22. Company Verification

Admin should be able to verify companies.

Status:

```text
Pending
Under Review
Verified
Rejected
Suspended
```

Verified company can receive a verification badge.

---

# 23. Recruiter Dashboard

Dashboard should display:

```text
Active Jobs
Total Applications
New Applications
Shortlisted Candidates
Interviews
Jobs Expiring Soon
```

Charts:

```text
Applications per Job
Applications by Date
Candidate Sources
Hiring Funnel
```

---

# 24. Job Posting Module

Recruiter should be able to create jobs.

Fields:

```text
Job Title
Department
Employment Type
Experience Required
Salary Range
Location
Work Mode
Number of Openings
Skills
Education
Job Description
Responsibilities
Requirements
Benefits
Application Deadline
```

---

# 25. Job Lifecycle

Job status:

```text
Draft
Pending Approval
Published
Paused
Expired
Closed
Rejected
```

Production system should not simply delete jobs.

Use lifecycle/status management.

---

# 26. Recruiter Job Management

Recruiter can:

```text
Create Job
Edit Job
Duplicate Job
Pause Job
Publish Job
Close Job
Extend Deadline
View Applications
```

---

# 27. Candidate Search / Talent Search

Recruiters should be able to search candidates.

Search:

```text
Skills
Experience
Location
Designation
Education
Salary
Notice Period
Industry
```

Example:

```text
Python + Django
Pune
2-5 years
30 days notice
```

---

# 28. Candidate Search Results

Display:

```text
Name
Designation
Experience
Location
Skills
Current Company
Notice Period
Profile Completion
```

Recruiter can:

```text
View Profile
Download Resume
Shortlist
Contact
Add to Pipeline
```

---

# 29. Recruitment Pipeline

This is an important production feature.

Example:

```text
Applied
   ↓
Screening
   ↓
Shortlisted
   ↓
HR Interview
   ↓
Technical Interview
   ↓
Manager Interview
   ↓
Offer
   ↓
Hired
```

Recruiter should be able to move candidates between stages.

---

# 30. Application Management

Recruiter can view:

```text
All Applications
New Applications
Shortlisted
Rejected
Interview
Selected
```

Candidate details:

```text
Profile
Resume
Skills
Experience
Education
Application
Cover Letter
Recruiter Notes
```

---

# 31. Recruiter Notes

Recruiters should be able to add private notes.

Example:

```text
Strong backend experience.
Good communication.
Schedule technical interview.
```

Candidate must NOT see private recruiter notes.

---

# 32. Interview Management

Recruiter can schedule interviews.

Fields:

```text
Candidate
Interview Type
Date
Time
Duration
Interviewer
Meeting Link
Location
Instructions
```

Interview types:

```text
Phone
Video
Technical
HR
Manager
On-site
```

---

# 33. Candidate Interview Dashboard

Candidate should see:

```text
Upcoming Interviews
Past Interviews
Interview Date
Time
Company
Job
Interview Type
Meeting Link
```

---

# 34. Messaging Module

Candidate ↔ Recruiter communication.

Features:

```text
Send Message
Receive Message
Conversation
Unread Count
Message Timestamp
Attachments
```

Important:

Messages should only be allowed according to business rules, for example after application/shortlisting or recruiter-initiated contact.

---

# 35. Notifications Module

Central notification system.

Notifications:

```text
Job Application Submitted
Application Status Changed
Candidate Shortlisted
Interview Scheduled
Interview Rescheduled
New Message
Job Alert
Password Changed
Email Verification
Account Verification
```

---

# 36. Notification Center

Users should see:

```text
All
Unread
Read
```

Ability:

```text
Mark as Read
Mark All as Read
```

---

# 37. Admin Dashboard

Admin dashboard should include:

```text
Total Users
Total Candidates
Total Recruiters
Total Companies
Total Jobs
Active Jobs
Applications
Interviews
Reports
```

Charts:

```text
User Growth
Job Growth
Application Growth
Recruiter Growth
```

---

# 38. User Management

Admin can:

```text
Search User
View User
Edit User
Activate User
Deactivate User
Suspend User
Verify User
Reset Account
```

Admin should not normally have access to plaintext passwords.

---

# 39. Company Management

Admin:

```text
View Companies
Verify Company
Reject Company
Suspend Company
Edit Company
View Recruiters
View Company Jobs
```

---

# 40. Job Moderation

Admin can:

```text
Review Job
Approve Job
Reject Job
Pause Job
Close Job
Delete/Archive Job
```

Reason for rejection should be recorded.

---

# 41. Job/Company Reporting

Users can report:

```text
Fake Job
Fraud
Spam
Incorrect Information
Abusive Content
Company Issue
```

Admin manages reports.

Workflow:

```text
Reported
   ↓
Under Review
   ↓
Action Taken
   ↓
Resolved
```

---

# 42. Categories & Master Data

Admin should manage:

### Skills

```text
Python
Java
C#
Angular
React
SQL
```

### Industries

```text
IT
Finance
Healthcare
Education
Manufacturing
```

### Job Types

```text
Full Time
Part Time
Contract
Internship
```

### Locations

```text
Country
State
City
```

Avoid hardcoding these values in the frontend.

---

# 43. Search Engine

For production scale, simple SQL `LIKE` queries may eventually become insufficient.

Recommended architecture:

```text
SQL Server
+
Search Layer
```

Possible search technology:

```text
Elasticsearch
OpenSearch
Azure AI Search
```

Search should support:

* Keyword matching
* Skill matching
* Location matching
* Fuzzy search
* Relevance ranking
* Filters
* Pagination

---

# 44. Recommendation Engine

Candidate recommendations:

```text
Candidate Skills
+
Experience
+
Location
+
Job Preferences
+
Job Requirements
```

System generates:

```text
Recommended Jobs
```

Similarly recruiters can get:

```text
Recommended Candidates
```

This can initially be rule-based.

Example:

```text
Skill Match = 50%
Experience Match = 20%
Location Match = 15%
Preference Match = 15%
```

Later this can be upgraded to ML/AI.

---

# 45. Profile Completion

Candidate profile should show:

```text
Profile Completion: 80%
```

Missing information:

```text
Add Skills
Add Experience
Upload Resume
Add Education
Add Profile Photo
```

---

# 46. SEO Module

For a public job portal, SEO is extremely important.

Public pages:

```text
/jobs
/jobs/python-developer
/jobs/python-developer-in-pune
/company/abc-technologies
```

SEO metadata:

```text
Title
Meta Description
Canonical URL
Open Graph
Structured Data
Sitemap
Robots.txt
```

Job pages should use structured data where appropriate.

---

# 47. Admin CMS

Admin should be able to manage:

```text
Homepage Content
Banners
FAQs
About Us
Terms
Privacy Policy
Contact Information
Blogs
```

---

# 48. Blog Module

Optional but recommended for SEO.

Admin can create:

```text
Blog
Category
Author
Tags
Featured Image
Content
Publish Date
Status
```

---

# 49. Subscription / Monetization

If the platform will eventually generate revenue, design for subscriptions from the beginning.

### Recruiter Plans

Example:

```text
Free
Basic
Professional
Enterprise
```

Potential limits:

```text
Number of Job Posts
Candidate Views
Resume Downloads
Talent Search
Featured Jobs
```

Payment integration should be abstracted behind an interface.

Example:

```text
IPaymentService
```

So the payment provider can be changed later.

---

# 50. Featured Jobs

Recruiters can optionally promote jobs.

Job can appear:

```text
Featured
Sponsored
Normal
```

The commercial rules should be configurable.

---

# 51. Email System

Transactional email service:

```text
IEmailService
```

Events:

```text
Registration
Email Verification
Password Reset
Job Application
Application Status
Interview
Job Alert
Recruiter Notification
```

Production email provider can be:

```text
SendGrid
Amazon SES
Azure Communication Services
```

---

# 52. File Storage

Do not store large resume files directly inside SQL Server in production unless there is a specific reason.

Recommended:

```text
Application
      ↓
File Storage
      ↓
Azure Blob Storage / AWS S3
```

Database stores metadata:

```text
FileId
UserId
FileName
FileType
FileSize
StoragePath
CreatedAt
```

---

# 53. Database Design

Core tables could include:

```text
Users
Roles
UserRoles
CandidateProfiles
RecruiterProfiles
Companies
CompanyRecruiters
Skills
CandidateSkills
Jobs
JobSkills
JobApplications
ApplicationStatuses
Resumes
Education
Experience
SavedJobs
JobAlerts
Interviews
Messages
Conversations
Notifications
Subscriptions
Payments
Reports
AuditLogs
Categories
Locations
```

---

# 54. Important Relationships

Example:

```text
User
 │
 ├── CandidateProfile
 │       ├── Education
 │       ├── Experience
 │       ├── Skills
 │       └── Resumes
 │
 └── Applications
          │
          └── Job
```

Recruiter:

```text
Company
   │
   ├── Recruiters
   │
   └── Jobs
          │
          └── Applications
                 │
                 └── Candidate
```

---

# 55. Application Status History

Do not only store:

```text
Application.Status
```

Also maintain:

```text
ApplicationStatusHistory
```

Example:

```text
Application ID: 1001

Applied       10:00
Screening     12:30
Shortlisted   15:20
Interview     Next Day
```

This provides auditability and analytics.

---

# 56. Audit Logging

Production application should track important actions.

Example:

```text
UserId
Action
Entity
EntityId
Timestamp
IP Address
User Agent
Old Value
New Value
```

Examples:

```text
Admin suspended user
Recruiter published job
Recruiter changed application status
Candidate updated profile
```

---

# 57. Security Requirements

Minimum requirements:

### Authentication

```text
JWT
Refresh Token
Password Hashing
OTP/Email Verification
```

### Authorization

Use role/policy-based authorization.

Example:

```text
Candidate
Recruiter
CompanyAdmin
Support
SuperAdmin
```

---

# 58. API Security

Implement:

```text
HTTPS
CORS
Rate Limiting
Input Validation
Anti-forgery protections where applicable
Secure Headers
Request Size Limits
File Validation
SQL Injection Protection
```

Use parameterized queries/EF Core.

---

# 59. Password Security

Never store:

```text
Plain Password
```

Use:

```text
ASP.NET Core Identity
```

Prefer using established identity/security components instead of creating custom password hashing/authentication.

---

# 60. API Versioning

API should support versioning.

Example:

```text
/api/v1/auth/login
/api/v1/jobs
/api/v1/candidates
/api/v1/applications
```

Future:

```text
/api/v2/...
```

---

# 61. API Response Standard

Define a common response structure.

Example:

```json
{
  "success": true,
  "message": "Job created successfully",
  "data": {},
  "errors": [],
  "traceId": "..."
}
```

For validation:

```json
{
  "success": false,
  "message": "Validation failed",
  "errors": [
    {
      "field": "salary",
      "message": "Salary is required"
    }
  ]
}
```

---

# 62. Pagination

All large collections should support pagination.

Example:

```text
GET /api/v1/jobs?page=1&pageSize=20
```

Response:

```json
{
  "items": [],
  "page": 1,
  "pageSize": 20,
  "totalItems": 1250,
  "totalPages": 63
}
```

For very large datasets, consider cursor/keyset pagination where appropriate.

---

# 63. Sorting

Examples:

```text
Newest
Oldest
Salary Low-High
Salary High-Low
Relevance
```

---

# 64. Caching

Use Redis or another distributed cache for frequently accessed data.

Examples:

```text
Popular jobs
Categories
Skills
Locations
Company information
Search results where appropriate
```

Do not cache sensitive personalized data without careful isolation.

---

# 65. Background Jobs

Some operations should not block HTTP requests.

Use a background processing mechanism such as:

```text
Hangfire
Azure Service Bus
RabbitMQ
AWS SQS
```

Potential jobs:

```text
Email notifications
Job alerts
Resume processing
Search indexing
Expired job processing
Report generation
Analytics
```

---

# 66. Logging

Implement centralized structured logging.

Recommended:

```text
Serilog
```

Log:

```text
Request
Response status
Exception
User ID
Correlation ID
Duration
```

Never log:

```text
Password
JWT tokens
Sensitive personal information
```

---

# 67. Global Exception Handling

API should have centralized exception middleware.

Example:

```text
Controller
    ↓
Service
    ↓
Exception
    ↓
Global Exception Middleware
    ↓
Standard Error Response
```

Don't expose internal stack traces to production users.

---

# 68. Health Checks

Production APIs should expose health checks.

Example:

```text
/api/health
/api/health/live
/api/health/ready
```

Check:

```text
Application
SQL Server
Redis
Storage
External services
```

---

# 69. Performance Requirements

Target requirements should be finalized with expected traffic, but the implementation team should design for:

```text
Horizontal scalability
Stateless APIs
Database indexing
Caching
Async I/O
Pagination
Efficient queries
Connection pooling
Background processing
```

Avoid loading thousands of records into memory.

---

# 70. SQL Server Requirements

Database should include:

* Proper normalization
* Foreign keys
* Indexes
* Unique constraints
* Check constraints where appropriate
* Transactions
* Stored procedures only where justified
* Query optimization
* Backup strategy

Important indexes:

```text
Users.Email
Jobs.Title
Jobs.Status
Jobs.CreatedAt
Jobs.LocationId
Applications.JobId
Applications.CandidateId
```

Actual indexes should be determined from query patterns and production measurements.

---

# 71. Soft Delete

For important business entities, consider:

```text
IsDeleted
DeletedAt
DeletedBy
```

instead of immediate physical deletion.

Particularly:

```text
Users
Companies
Jobs
Applications
```

However, retention/privacy requirements must determine what data is actually retained.

---

# 72. Data Privacy

Candidate information is sensitive.

Implement:

```text
Privacy settings
Profile visibility
Resume visibility
Data export
Account deletion
Consent tracking
```

Candidate should control whether recruiters can discover their profile.

---

# 73. GDPR / Privacy Readiness

Even if the first market is India, design a privacy-aware system.

Support:

```text
Privacy Policy
Terms & Conditions
Consent
Data deletion requests
Data export
Cookie consent where applicable
```

Legal requirements should be reviewed with appropriate legal counsel for the jurisdictions served.

---

# 74. Fraud Prevention

Potential controls:

```text
Rate limiting
Email verification
Phone verification
Company verification
Job moderation
Report system
IP/device monitoring
Suspicious activity detection
```

---

# 75. CAPTCHA

Use CAPTCHA/risk-based bot protection on appropriate endpoints:

```text
Registration
Login after suspicious attempts
Password reset
Job posting
Contact forms
```

Don't add CAPTCHA everywhere unnecessarily because it hurts usability.

---

# 76. Analytics

Track business metrics such as:

```text
Registered Candidates
Active Candidates
Registered Recruiters
Active Recruiters
Jobs Posted
Jobs Applied
Applications per Job
Application Conversion
Interview Conversion
Hiring Conversion
```

Admin dashboard can visualize these.

---

# 77. Recommended API Modules

The API can be organized as:

```text
Auth
Users
Candidates
Recruiters
Companies
Jobs
Applications
Resumes
Skills
Education
Experience
Search
SavedJobs
JobAlerts
Interviews
Messages
Notifications
Reports
Subscriptions
Payments
Admin
Analytics
CMS
```

---

# 78. Example API Endpoints

### Authentication

```http
POST /api/v1/auth/register
POST /api/v1/auth/login
POST /api/v1/auth/refresh-token
POST /api/v1/auth/logout
POST /api/v1/auth/forgot-password
POST /api/v1/auth/reset-password
POST /api/v1/auth/verify-email
```

### Jobs

```http
GET    /api/v1/jobs
GET    /api/v1/jobs/{id}
POST   /api/v1/jobs
PUT    /api/v1/jobs/{id}
DELETE /api/v1/jobs/{id}
POST   /api/v1/jobs/{id}/publish
POST   /api/v1/jobs/{id}/pause
POST   /api/v1/jobs/{id}/close
```

### Applications

```http
POST /api/v1/jobs/{jobId}/apply
GET  /api/v1/candidate/applications
GET  /api/v1/applications/{id}
PUT  /api/v1/applications/{id}/status
```

### Candidates

```http
GET /api/v1/candidates/me
PUT /api/v1/candidates/me
GET /api/v1/candidates/{id}
GET /api/v1/candidates/search
```

---

# 79. Testing Strategy

The implementation team should not consider the project complete merely because APIs work manually.

Testing should include:

### Unit Testing

```text
Domain logic
Application services
Validators
```

### Integration Testing

```text
API + SQL Server
Authentication
Repositories
External services
```

### API Testing

```text
Postman
Swagger
Automated API tests
```

### Security Testing

```text
Authentication
Authorization
Injection
Rate limiting
File uploads
Access control
```

### Performance Testing

Use tools such as:

```text
k6
JMeter
```

---

# 80. CI/CD

Recommended pipeline:

```text
Developer
   ↓
Git
   ↓
Pull Request
   ↓
Build
   ↓
Unit Tests
   ↓
Security Scan
   ↓
Docker Build
   ↓
Deploy Staging
   ↓
Integration Tests
   ↓
Production
```

---

# 81. Environment Management

Separate:

```text
Development
Testing
Staging
Production
```

Never keep production secrets in source code.

Use:

```text
Environment Variables
Azure Key Vault
AWS Secrets Manager
```

or equivalent secret-management infrastructure.

---

# 82. Configuration

Use configuration for:

```text
Database
JWT
Email
Storage
Redis
Payment
Search
External APIs
```

Do not hardcode these.

---

# 83. Deployment

Possible production architecture:

```text
                    Internet
                       │
                       ▼
                 Load Balancer
                       │
            ┌──────────┴──────────┐
            ▼                     ▼
       API Instance 1        API Instance 2
            │                     │
            └──────────┬──────────┘
                       ▼
                  SQL Server
                       │
              ┌────────┴────────┐
              ▼                 ▼
            Redis          File Storage
```

The exact cloud stack can be decided later.

---

# 84. Backup & Disaster Recovery

Need:

```text
Automated SQL backups
Point-in-time recovery where supported
File storage backup/versioning
Database restore testing
Disaster recovery plan
```

Define:

```text
RPO
RTO
```

before production launch.

---

# 85. Non-Functional Requirements

The implementation team should treat these as first-class requirements.

### Performance

API response targets should be defined based on workload and endpoint type.

### Availability

Production availability target should be agreed upon.

### Scalability

System should support horizontal API scaling.

### Security

OWASP-aligned secure development.

### Maintainability

Clean Architecture + SOLID + coding standards.

### Observability

Logs + metrics + traces.

### Reliability

Retries/timeouts/circuit breakers for appropriate external services.

---

# 86. MVP vs Phase 2

Do **not** try to build every Naukri.com feature in Version 1.

## Phase 1 — MVP

Build:

```text
Authentication
Candidate Profile
Resume Upload
Recruiter Registration
Company Profile
Job Posting
Job Search
Job Filters
Job Application
Application Tracking
Saved Jobs
Recruiter Dashboard
Candidate Search
Application Management
Notifications
Admin Dashboard
User Management
Company Verification
Job Moderation
```

---

# 87. Phase 2

Add:

```text
Messaging
Interview Management
Job Alerts
Advanced Candidate Search
Recommendation Engine
Resume Builder
Company Reviews
Blogs
Advanced Analytics
```

---

# 88. Phase 3

Add:

```text
Subscriptions
Payments
Featured Jobs
AI Resume Parsing
AI Job Recommendations
AI Candidate Matching
Advanced Search Engine
Mobile Applications
Recruitment Automation
```

---

# 89. AI Features — Future Ready

The architecture should leave room for AI but **AI should not be a dependency for the basic recruitment workflow**.

Potential future features:

### Resume Parsing

Upload:

```text
Resume PDF
```

Extract:

```text
Name
Skills
Experience
Education
Designation
Companies
```

### Job Matching

```text
Candidate Resume
        +
Job Requirements
        ↓
Matching Engine
        ↓
Match Score
```

### Recruiter Candidate Recommendations

```text
Job
 ↓
Required Skills
 ↓
Candidate Search
 ↓
Matching
 ↓
Recommended Candidates
```

---

# 90. Core Business Workflow

## Candidate Workflow

```text
Register
   ↓
Verify Email/Mobile
   ↓
Create Profile
   ↓
Upload Resume
   ↓
Search Jobs
   ↓
View Job
   ↓
Apply
   ↓
Track Application
   ↓
Shortlisted
   ↓
Interview
   ↓
Offer / Rejection
```

---

# 91. Recruiter Workflow

```text
Register
   ↓
Create Company
   ↓
Company Verification
   ↓
Create Job
   ↓
Admin Approval (if required)
   ↓
Publish Job
   ↓
Receive Applications
   ↓
Screen Candidates
   ↓
Shortlist
   ↓
Interview
   ↓
Offer
   ↓
Hire
```

---

# 92. Admin Workflow

```text
Login
 ↓
Dashboard
 ↓
Monitor Platform
 ↓
Verify Companies
 ↓
Moderate Jobs
 ↓
Manage Users
 ↓
Handle Reports
 ↓
Monitor Analytics
 ↓
Manage Configuration
```

---

# 93. Production-Grade Definition of Done

A module should not be considered complete simply because its UI works.

For every module:

```text
Requirement
    ↓
Database
    ↓
Domain Logic
    ↓
Application Layer
    ↓
API
    ↓
Authorization
    ↓
Validation
    ↓
Error Handling
    ↓
Logging
    ↓
Unit Tests
    ↓
Integration Tests
    ↓
Swagger/API Documentation
    ↓
UI Integration
    ↓
Security Testing
```

---

# 94. Deliverables Expected From Implementation Team

You can explicitly ask the team to deliver:

### Backend

* ASP.NET Core Web API
* Clean Architecture
* EF Core
* SQL Server
* Authentication
* Authorization
* REST APIs
* Swagger/OpenAPI
* Validation
* Logging
* Exception handling
* Background jobs
* Caching
* File storage integration

### Database

* ER diagram
* Database schema
* Relationships
* Indexes
* Migration scripts
* Seed data
* Backup strategy

### Frontend

* Candidate portal
* Recruiter portal
* Admin portal
* Responsive design
* API integration
* Authentication
* Error handling
* Loading states
* Form validation

### DevOps

* Git repository
* Branching strategy
* CI/CD
* Docker
* Environment configuration
* Staging
* Production deployment
* Monitoring
* Logging

### Documentation

* PRD
* Architecture document
* ER diagram
* API documentation
* Deployment guide
* Environment setup guide
* Admin guide
* User guide
* Test cases

---

# 95. Recommended Initial Solution Structure

I would ask the .NET team to start approximately like this:

```text
JobPortal.sln
│
├── src
│   │
│   ├── JobPortal.API
│   │
│   ├── JobPortal.Application
│   │
│   ├── JobPortal.Domain
│   │
│   └── JobPortal.Infrastructure
│
├── tests
│   │
│   ├── JobPortal.UnitTests
│   │
│   └── JobPortal.IntegrationTests
│
├── docs
│   ├── architecture
│   ├── database
│   └── api
│
├── docker
│
└── README.md
```

---

# 96. Recommended Technology Stack

Your initial technology direction is reasonable, but I would make the stack explicit in the project requirements:

| Layer             | Technology                                               |
| ----------------- | -------------------------------------------------------- |
| Backend           | ASP.NET Core Web API                                     |
| Language          | C#                                                       |
| Architecture      | Clean Architecture                                       |
| ORM               | Entity Framework Core                                    |
| Database          | SQL Server                                               |
| Authentication    | ASP.NET Core Identity + JWT                              |
| API Documentation | Swagger/OpenAPI                                          |
| Validation        | FluentValidation                                         |
| Logging           | Serilog                                                  |
| Caching           | Redis                                                    |
| Background Jobs   | Hangfire or equivalent                                   |
| File Storage      | Azure Blob / S3 equivalent                               |
| Search            | SQL initially; dedicated search engine as scale requires |
| Testing           | xUnit                                                    |
| API Testing       | Postman/automated tests                                  |
| CI/CD             | GitHub Actions/Azure DevOps                              |
| Containers        | Docker                                                   |
| Monitoring        | Application Insights / OpenTelemetry-compatible stack    |

The exact cloud provider and frontend framework can be finalized separately.

---

# 97. Most Important Point for Your Implementation Team

When you hand this PRD to the team, **don't tell them only:**

> "Build a Naukri clone using .NET Web API."

Instead, tell them:

> **"Build a multi-role production-grade recruitment platform with Candidate, Recruiter/Company and Admin workflows, using ASP.NET Core Web API with Clean Architecture and SQL Server. The system must support authentication, candidate profiles, resumes, job posting, advanced job search, applications, recruitment pipelines, interviews, notifications, company verification, administration, audit logging, security, scalability and production deployment. The implementation must follow the requirements and workflows defined in this PRD."**

That gives the development team a **product specification**, rather than just a technology specification.

### One important architectural recommendation

Don't try to copy every feature of Naukri.com in the first release. Build the **core recruitment engine first**:

**Candidate → Job Search → Apply → Recruiter → Screen → Shortlist → Interview → Hire**

Everything else—AI matching, subscriptions, advanced analytics, resume builder, recommendations, mobile apps—can be layered on top of that foundation.

If this is going to an actual implementation team, the next document I would create from this PRD is a **complete SRS + module-wise user stories + acceptance criteria + SQL Server ER diagram + API specification**, because that is what developers can directly convert into Jira/implementation tasks.
