# .NET Rental App - Interview Notes & Concepts

Use this file to revise key concepts before an interview. It covers everything we implemented in the Rental Market app.

---

## Phase 0: Environment & Docker

### 1. .NET SDK vs Runtime
*   **SDK (Software Development Kit)**: Includes everything you need to *build* and *run* apps (compilers, CLI tools). Use this for dev.
*   **Runtime**: only includes what is needed to *run* the app. Use this for production servers.

### 2. Docker Concepts
*   **Image**: The "Blueprint" or "Recipe". It's read-only. (e.g., `mcr.microsoft.com/mssql/server`).
*   **Container**: The "Running Instance" of an image. You can have many containers from one image.
*   **Volume**: A way to persist data. If you delete a container, files inside it are gone *unless* they are stored in a Volume.
*   **Docker Compose**: A tool to define and run multiple containers (e.g., App + SQL + Redis) at once using a YAML file.

---

## Phase 1: Clean Architecture & Foundation

### 1. Clean Architecture (Onion Architecture)
The goal is to separate concerns and make the app testable. Dependencies point **inward**.

*   **1. Domain (Core)**: Requires NOTHING. Contains Entities (`Listing`, `User`) and Enums. The "heart" of the business.
*   **2. Application**: Depends on Domain. Contains Interfaces (`IListingRepository`) and Business Logic. It defines *what* needs to be done, not *how*.
*   **3. Infrastructure**: Depends on Application. Contains the implementation (`ListingRepository`, `DbContext`, EmailService). It talks to the database/files.
*   **4. API (Presentation)**: Depends on Application. The entry point (Controllers). It accepts requests and returns responses.

### 2. Dependency Injection (DI)
We don't create objects with `new Repository()`. We ask the container to give it to us.
*   **Benefit**: Decoupling. The API doesn't know we are using SQL Server; it just knows `IListingRepository`. We can swap SQL for a Mock for testing easily.
*   **Lifetimes**:
    *   **Transient**: A new instance is created *every time* it is requested.
    *   **Scoped**: A new instance is created *once per HTTP Request*. (Standard for `DbContext`).
    *   **Singleton**: A single instance is created *once* and shared forever. (Careful with thread safety!).

### 3. Repository Pattern
*   A localized abstraction over the database.
*   **Why?** Removes complicated LINQ/SQL queries from your Controller. Your controller just calls `GetAllAsync()`.

### 4. Entity Framework Core (EF Core)
*   **ORM (Object-Relational Mapper)**: Maps C# Classes (Entities) to SQL Tables.
*   **DbContext**: Represents a session with the database.
*   **EnsureCreated()**: Good for prototyping; creates DB schema if not exists. For production, use **Migrations**.

---

## Phase 2: Authentication & Authorization

### 1. AuthN vs AuthZ
*   **Authentication (AuthN)**: "Who are you?" (Verifying identity via Login).
*   **Authorization (AuthZ)**: "What are you allowed to do?" (Checking permissions/roles).

### 2. JWT (JSON Web Token)
*   **Stateless Auth**: The server doesn't store "sessions" in RAM. The user holds the "Token" which contains all their info.
*   **Anatomy**: `Header.Payload.Signature`.
*   **Signature**: The most important part. Encrypted with a Secret Key. If the user tampers with the token, the signature breaks, and the server rejects it.
*   **Bearer Token**: The standard way to send it in headers: `Authorization: Bearer <token_string>`.

### 3. ASP.NET Middleware Order
The order in `Program.cs` is CRITICAL.
1.  `app.UseAuthentication()`: Check the ID card. (Must come FIRST).
2.  `app.UseAuthorization()`: Check if they are on the guest list.

### 4. ASP.NET Core Identity
*   **IdentityUser**: A built-in class provided by Microsoft with standard fields (`Email`, `PasswordHash`, `SecurityStamp`).
*   **UserManager<T>**: A helper service to handle complex logic like Hashing Passwords (never store plain text!), validating emails, and creating users.

---

## Phase 3: Bookings & Concurrency

### 1. The Challenge: Double Bookings
*   **Race Condition**: Two users try to book the *same listing* for the *same dates* at the *exact same millisecond*.
*   **Naive Approach**: `if (IsAvailable) { Save() }`. This fails because both users pass the `if` check before either saves.

### 2. Optimistic Concurrency
*   **RowVersion (Timestamp)**: A special byte array column in SQL Server that changes *automatically* every time a row is updated.
*   **Mechanism**:
    1.  User A reads Row (Version 1).
    2.  User B reads Row (Version 1).
    3.  User A saves. DB checks: "Is Version still 1?". Yes -> Update (Version becomes 2).
    4.  User B saves. DB checks: "Is Version still 1?". **No (It's 2)** -> **REJECT**.

### 3. Serializable Transaction (The "Big Lock")
We used a **Stored Procedure** with `SET TRANSACTION ISOLATION LEVEL SERIALIZABLE`.
*   **Isolation Level**: Controls how much "interference" transactions see from each other.
*   **Serializable**: The strictest level. It behaves as if transactions happen one after another (In Series), never in parallel.
*   **Why use it?**: It places "Range Locks" on the `Bookings` table. If I am checking dates `Jan 1 - Jan 5`, NO ONE can insert a booking in that range until I am done.

### 4. Logic Flow (Controller -> Repo -> DB)
1.  **Controller**:
    *   Validate inputs (Start Date < End Date).
    *   Calculate Price (`Listing.Price * Days`).
    *   Get `Description` or Listing details for the UI.
2.  **Repository**:
    *   Doesn't run logic. It just marshals data to SQL.
    *   Calls `Sp_CreateBooking`.
    *   Catches `SqlException` (Error 50001) and translates it to a clean C# `InvalidOperationException`.
3.  **Database**:
    *   `IF EXISTS (...) THROW Error`.
    *   `INSERT INTO ...`.
    *   All wrapped in a Transaction.

---

## Phase 4: Search & Performance

### 1. Redis Caching
*   **What is it?**: An in-memory key-value store. It is much faster than SQL Server (Microseconds vs Milliseconds).
*   **Why use it?**: To store data that is requested often but changes rarely (like Listing Details).
*   **Distributed Cache**: `IDistributedCache` in .NET allows us to swap Redis for SQL Cache or Memory Cache easily.

### 2. The Decorator Pattern (Proxy Pattern)
This is a **Key Interview Concept**.
*   **Problem**: We want to add Caching to `ListingRepository`, but we don't want to clutter the SQL logic with Redis code.
*   **Solution**: Create a `CachedListingRepository` that implements the SAME interface (`IListingRepository`).
*   **How it works**:
    1.  The `CachedRepo` takes the `RealRepo` in its constructor.
    2.  When `GetById` is called, `CachedRepo` checks Redis.
    3.  If missing, it calls `RealRepo.GetById` (fetch from SQL).
    4.  It saves the result to Redis and returns it.
*   **Dependency Injection**: In `Program.cs`, we register the Decorator (`CachedListingRepository`) as the default implementation for `IListingRepository`. The Controller doesn't even know it's talking to a cache!

### 3. Search & Filtering
*   **IQueryable**: We use `IQueryable` to build the SQL query dynamically *before* sending it to the database.
*   **Where Clause**: `query.Where(l => l.Price <= maxPrice)` translates to `WHERE Price <= 100` in SQL.
*   **Execution**: The SQL is only sent when we call `ToListAsync()`. This is called **Deferred Execution**.
