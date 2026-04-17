# Good Example: Purposing

## Context

A team needs to build a student enrollment system for a university.

## WRONG approach (jumping to code)

```
// Immediately starts writing Student model and database tables.
// No problem statement, no observation, no constraints identified.
```

**Why this is wrong:** Skips the observation phase. Team has not identified: Who are the users? What problem is being solved? What are the constraints?

---

## CORRECT approach: Full purposing cycle

### Step 0: Observation

> "Students cannot register for courses without first verifying their prerequisite completions. The current system requires a staff member to manually check prerequisites, which takes 2-3 days and causes late enrollments."

**Real blocker identified:** Manual prerequisite verification is a bottleneck.

### Step 1: Articulation

> **Problem:** The enrollment system has no automated prerequisite check. This forces manual intervention for every enrollment, creating delays that cause students to miss course deadlines.

> **Affected parties:** Students (wait 2-3 days), Staff (repetitive manual work), Administrators (enrollment metrics are delayed).

> **Constraint:** Prerequisite rules are defined per-course and may change each semester.

### Step 2: Solutioning

> **Goal:** Build an automated prerequisite verification service that:
> - Retrieves a student's completed courses
> - Compares them against the required prerequisites for the target course
> - Returns an approval or rejection with specific unmet prerequisites listed
> - Is readable, maintainable, and rewritable by any engineer on the team
> - Does not require cloud connectivity to run locally (Airplane Mode)

### Result

Purpose is clear. The team can now move to Modeling.

**What this enables:**
- The Student model needs: Id, Name, CompletedCourses
- The Course model needs: Id, Name, Prerequisites
- The Enrollment model (secondary) needs: StudentId, CourseId, Status
- A Foundation Service for Student, Course, and Enrollment
- A Processing Service for prerequisite checking logic
