---
description: Autonomous Development Loop (Conductor -> Code -> Test -> Fix)
---

# Autonomous Development Workflow

This workflow orchestrates a development cycle where the agent acts as a "Conductor", switching between implementation and testing roles to autonomously deliver features.

## 1. Plan and Design (Conductor Mode)
- **Goal**: Analyze the immediate requirement and break it down.
- **Action**: Create or update `implementation_plan.md` if the task is complex.
- **Command**: `task_boundary(TaskName="Planning Feature", Mode="PLANNING")`

## 2. Implementation (Developer Mode)
- **Goal**: Write the code to satisfy the requirement.
- **Action**: Implement changes in `src/`.
- **Command**: `task_boundary(TaskName="Implementing Feature", Mode="EXECUTION")`

## 3. Verification Loop (Tester Mode)
- **Goal**: Verify changes and fix bugs autonomously.
- **Action**:
    1. Create/Update Unit Tests in `tests/`.
    2. Run Tests: `dotnet test`
    3. **IF FAILURE**:
        - Read error output.
        - Analyze root cause.
        - Switch to **Fixer Mode**: Apply fixes to `src/` or `tests/`.
        - Re-run tests.
    4. **IF SUCCESS**:
        - Proceed to next task.

## 4. Final Review (Conductor Mode)
- **Goal**: Ensure quality before handing off to user.
- **Action**:
    - Remove temporary test files (if any).
    - Update `walkthrough.md` with results.
    - Call `notify_user` to report completion.

## Usage
Trigger this workflow when you have a specific feature request.
Example: "Implement a new API endpoint for GET /rentals/:id with robust error handling."
