Gym Management API – Clean Architecture (.NET 8)

This project is a production-ready Gym Management System built using Clean Architecture, Repository & Unit of Work patterns, Entity Framework Core, ASP.NET Identity, and RESTful API standards.

It provides a complete backend solution for managing gym operations, including users, memberships, trainers, classes, schedules, bookings, payments, and performance reports.

⸻⸻⸻⸻⸻⸻⸻⸻⸻⸻⸻⸻⸻⸻⸻⸻

1. Features

Authentication & Authorization
	•	User registration and login using JWT
	•	Role-based access control: Admin, Trainer, Member
	•	ASP.NET Core Identity integration

User & Profile Management
	•	Member CRUD
	•	Trainer profile CRUD (linked to User)
	•	Assign membership plan to a member

Membership Plans
	•	Create, update, soft delete membership plans
	•	Optimistic Concurrency Control using ETag (RowVersion)
	•	Filtering, searching, sorting, paging

Classes & Sessions
	•	Create gym classes and assign trainers
	•	Create class sessions with configurable capacity

Booking & Attendance
	•	Book a session
	•	Prevent double booking
	•	Enforce capacity limits
	•	Check-in and attendance tracking

Payments
	•	Member purchases a membership plan
	•	Automatic expiration calculation
	•	Revenue data collection

Reporting
	•	Revenue reports
	•	Trainer performance analytics
	•	Class utilization statistics
	•	Member activity summary
	•	CSV export for revenue reports

System
	•	Centralized logging with Serilog
	•	Health checks
	•	Swagger UI (development)

⸻
