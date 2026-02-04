# Rental Marketplace - Project Specifications & Study Plan

**Goal:** Build a full-featured Rental Marketplace to master advanced .NET 8 concepts, architecture, and design patterns.
**Target OS:** macOS
**Learning Style:** Learning by doing (Code -> Review -> Refactor).

---

## Phase 0: Prerequisites & Environment Setup
**Goal:** Prepare the macOS environment for .NET development without polluting the system unnecessarily.

### Tasks
1.  **Install .NET 8 SDK**
    *   Action: Download and install the official .NET 8 SDK for macOS (Arm64 for Apple Silicon).
    *   Verification: Run `dotnet --list-sdks` in terminal.
2.  **Install Docker**
    *   Action: Install Docker Desktop, OrbStack, or Colima. This is required to run our Database (SQL Server) and Cache (Redis) without installing them directly on your Mac.
    *   Verification: Run `docker ps` in terminal.
3.  **Setup IDE (VS Code)**
    *   Action: Install VS Code if not present.
    *   Extensions: Install "C# Dev Kit", "Docker", and "SQL Server" (mssql) extensions.
    *   *Why*: C# Dev Kit provides the Solution Explorer view, which is critical for managing multi-project solutions similar to Visual Studio on Windows.
4.  **Database Tools**
    *   Action: Install Azure Data Studio (optional but recommended for macOS) or rely on the VS Code mssql extension.

---

## Phase 1: Foundation - Skeleton & Core Practices
**Goal:** Establish a "Clean Architecture" solution. This is the industry standard for scalable .NET apps, separating the "Domain" (logic) from the "Infrastructure" (database/web).

### Design Pattern: Clean Architecture (Onion Architecture)
Unlike a simple MVC (Model-View-Controller) single project, we will split our code into 4 projects to enforce "Separation of Concerns".
*   **Domain**: The core. Just C# classes (Entities). No dependencies.
*   **Application**: The logic. Interfaces, DTOs (Data Transfer Objects). Depends on *Domain*.
*   **Infrastructure**: The plumbing. Database contexts, file system access. Depends on *Application*.
*   **API (Presentation)**: The entry point. Controllers. Depends on *Application* components.

### Detailed Tasks
1.  **Initialize Git Repository**
    *   Command: `git init`
    *   Action: Create a standard `.gitignore` for .NET to avoid committing build artifacts (`bin`, `obj`).
2.  **Create Solution Structure (CLI Steps)**
    *   Create a solution file: `dotnet new sln -n RentalMarket`
    *   **Presentation Layer**: Create Web API project.
        *   `dotnet new webapi -n RentalMarket.Api -o src/RentalMarket.Api`
    *   **Application Layer**: Create Class Library.
        *   `dotnet new classlib -n RentalMarket.Application -o src/RentalMarket.Application`
    *   **Domain Layer**: Create Class Library.
        *   `dotnet new classlib -n RentalMarket.Domain -o src/RentalMarket.Domain`
    *   **Infrastructure Layer**: Create Class Library.
        *   `dotnet new classlib -n RentalMarket.Infrastructure -o src/RentalMarket.Infrastructure`
    *   **Tests**: Create Unit Test project.
        *   `dotnet new xunit -n RentalMarket.UnitTests -o tests/RentalMarket.UnitTests`
3.  **Link Projects (Dependency Implementation)**
    *   *Rule*: Inner layers (Domain) never know about outer layers (API/Infra).
    *   Add reference: `Api` -> `Application`
    *   Add reference: `Infrastructure` -> `Application`
    *   Add reference: `Application` -> `Domain`
    *   Add reference: `Infrastructure` -> `Domain` (optional, usually implied via Application but explicit is okay for EF)
    *   Add all projects to the `.sln` file.
4.  **Infrastructure Setup (Docker)**
    *   Create `docker-compose.yml` file in root.
    *   Define service: `sql-server` (Image: `mcr.microsoft.com/mssql/server:2022-latest`).
    *   Define service: `redis` (Image: `redis:alpine`).
    *   Action: Run `docker-compose up -d` to verify they start.
5.  **First Feature: Listing CRUD**
    *   **Domain**: Create `Listing` class (Id, Title, Price, Location).
    *   **Infrastructure**: Install `Microsoft.EntityFrameworkCore.SqlServer`. Create `ApplicationDbContext`.
    *   **Application**: Create `IListingRepository` interface.
    *   **API**: Create `ListingsController`.
6.  **CI Pipeline**
    *   Create `.github/workflows/dotnet.yml`.
    *   Step: Restore dependencies.
    *   Step: Build projects.
    *   Step: Run tests.

---

## Phase 2: Authentication, Authorization & Roles
**Goal:** Secure the API.
*   **Pattern**: **Identity**. We will use ASP.NET Core Identity, which creates user tables (AspNetUsers, AspNetRoles) managed by EF Core.
*   **Pattern**: **JWT (JSON Web Token)**. Stateless auth. The client sends a token in the generic `Authorization: Bearer <token>` header.

### Detailed Tasks
1.  **Install Identity Packages**: Add `Microsoft.AspNetCore.Identity.EntityFrameworkCore` to Infrastructure.
2.  **Configure Auth Context**: Inherit DbContext from `IdentityDbContext<ApplicationUser>`.
3.  **Implement Auth Service**:
    *   Create `IAuthService` in *Application*.
    *   Implement `LoginAsync` and `RegisterAsync`.
    *   Generate JWT String using `System.IdentityModel.Tokens.Jwt`.
4.  **Secure Endpoints**: Add `[Authorize]` attribute to `ListingsController` (create/delete).

---

## Phase 3: Booking Domain (Concurrency)
**Goal:** Handle real business logic where two people try to book the same house at the same time.

### Detailed Tasks
1.  **Domain Mockup**: Create `Booking` entity with `StartDate`, `EndDate`, `ListingId`, `GuestId`.
2.  **Availability Logic**:
    *   Concept: "Overlapping intervals".
    *   Task: Write a SQL Stored Procedure `sp_CreateBooking` that checks `WHERE NOT (ExistingStart < NewEnd AND ExistingEnd > NewStart)` inside a TRANSACTION.
3.  **EF Core Integration**:
    *   Map the Stored Proc to a method in `ApplicationDbContext`.
4.  **Testing Race Conditions**:
    *   Write a test that spawns 5 parallel tasks trying to book the same slot. Validation: Only 1 should succeed.

---

## Phase 4: Search & Performance
**Goal:** Make reading data fast.

### Detailed Tasks
1.  **Search DTO**: Create `SearchListingDto` (simple flat object, no heavy logic).
2.  **Dapper Implementation**:
    *   Install `Dapper` (micro-ORM).
    *   Write raw SQL query: `SELECT * FROM Listings WHERE Price < @MaxPrice`.
    *   *Why*: Dapper is faster than EF Core for read-only lists.
3.  **Caching (Decorator Pattern)**:
    *   Create interface `IListingService`.
    *   Create class `ListingService` (fetches from DB).
    *   Create class `CachedListingService` (checks Redis -> if miss, calls `ListingService` -> saves to Redis).
    *   *Why*: This is the **Decorator Pattern**. It adds behavior (caching) without changing the original class code.

---

## Phase 5: Advanced Architecture (CQRS)
**Goal:** Explicitly separate "Doing things" (Commands) from "Asking for things" (Queries).

### Detailed Tasks
1.  **Install MediatR**: A library to send messages in the app.
2.  **Refactor**:
    *   Move `CreateListing` logic into a `CreateListingCommand` class.
    *   Move `GetListings` logic into a `GetListingsQuery` class.
3.  **Domain Events**:
    *   Create event `BookingCreatedEvent`.
    *   Create a handler `EmailNotificationHandler` that listens for this event.
    *   *Why*: Decouples the "Booking" logic from the "Emailing" logic.

---

## Phase 6: Real-time & Polish
**Goal:** Add "Wow" factors.

### Detailed Tasks
1.  **SignalR Hub**: Create `NotificationHub`.
    *   Task: When a booking is made, push a message to the Host's connected web client.
2.  **Payment Stub**:
    *   Create `IPaymentGateway`.
    *   Implement `StripeMockPaymentGateway` that just returns "Success" after 500ms delay.
