Contract Monthly Claim System

A GUI Web Application built using ASP.NET Core MVC

📄 Overview

The Contract Monthly Claim System is a web-based application designed for educational institutions to streamline the submission, verification, approval, and processing of lecturer claims. The system supports lecturers, programme coordinators, academic managers, and HR personnel with dedicated interfaces and automated workflows.

This platform enhances productivity, transparency, and accuracy while reducing the risk of manual errors.

🚀 Features Implemented (Part 2 & Final Part)
1️⃣ Lecturer Claim Submission

Lecturers can submit their monthly claims quickly and easily.

✔ Features:

Clean, responsive form layout

Fields for:

Hours Worked

Hourly Rate

Additional Notes

Prominent Submit Claim button

Document upload (.pdf, .docx, .xlsx) with file size/type validation

Automatically displays uploaded filenames

Error handling for missing or invalid fields

🔄 Automation:

Auto-calculation of final payment (Hours × Rate) using JavaScript/jQuery

Client-side validation for numeric fields

Database storage using Entity Framework Core

2️⃣ Programme Coordinator & Academic Manager Verification

A separate view for staff members responsible for verifying claims.

✔ Features:

Table showing all pending claims

Displays claim details:

Lecturer name

Hours worked

Hourly rate

Final payment

Uploaded supporting documents

Approve and Reject buttons for each claim

Real-time status updates

🔄 Automation:

Automatic checking of claims against institutional rules

Approval workflow implemented using:

ASP.NET Core MVC Controller Actions

FluentValidation for rule validation

ASP.NET Identity for role-based access

3️⃣ Document Uploading

Lecturers can upload supporting documents for each claim.

✔ Features:

Upload button on claim form

File name preview

Secure file storage

File size and type restriction

File linked to its claim in the database

4️⃣ Claim Tracking

Users can track status updates across the claim lifecycle.

✔ Features:

Claim status displayed as:

Pending

Approved

Rejected

Automatic update when coordinator/manager takes action

Status visible to:

Lecturer

Coordinator

Manager

HR

5️⃣ Reliability & Consistency
✔ Unit Testing

Unit tests cover:

Claim submission

Claim validation

Document upload

Status update workflow

✔ Error Handling

The system includes:

Try-catch exception handling

Friendly error messages

Model validation messages

Logging of system-level errors

6️⃣ Version Control

✔ GitHub repository with multiple commits (15+)
✔ Descriptive commit messages such as:

“Added claim submission logic with EF Core”

“Implemented role-based access for coordinators”

“Added document upload validation”

“Created HR reporting module”

🔧 Application Enhancement (Final Part)
🧮 Lecturer Automation

Auto-calculation of total payment

Auto-validation of hours & rate

Smooth jQuery-powered form interactions

🗂 Coordinator & Manager Automation

Automated approval rules using FluentValidation

Workflow-based approval process

RESTful Web API endpoints for async operations

📊 HR Automation

Automatically generates:

Monthly invoices

Payment summaries

Reports (SSRS or LINQ reporting)

HR can manage lecturer information

Secure access using ASP.NET Identity

🖥️ Technologies Used
Layer	Technology
Frontend	HTML5, CSS3, Bootstrap, JavaScript, jQuery
Backend	ASP.NET Core MVC, ASP.NET Web API
Database	SQL Server + Entity Framework Core
Authentication	ASP.NET Identity
Reporting	LINQ, SSRS / Crystal Reports
Workflow	FluentValidation
Version Control	Git & GitHub
🧪 Unit Tests

Unit tests written using:

xUnit or NUnit

Moq for mocking dependencies

Tests cover:

Claim creation

Validation rules

Document uploads

Approval workflow
