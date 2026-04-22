# Bad Example: Modeling Violations

## Violation 1 — Speculative Attributes (core-030, core-031)

```csharp
// BAD: StudentModel with attributes that purposing never required
public class StudentModel
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }          // purposing never mentioned email
    public string PhoneNumber { get; set; }    // purposing never mentioned phone
    public string EmergencyContact { get; set; } // speculative
    public string LinkedInProfile { get; set; }  // speculative
    public bool IsVip { get; set; }              // undefined business meaning
}
```

**Problems:**
- `email`, `PhoneNumber`, `EmergencyContact`, `LinkedInProfile`, `IsVip` were never identified in purposing.
- `StudentModel` uses the forbidden `Model` suffix (core-032 / naming convention).

---

## Violation 2 — Primary Model With External Dependency (core-033)

```csharp
// BAD: Student physically depends on School to exist
public class Student
{
    public Guid Id { get; set; }
    public string Name { get; set; }

    // Primary model must not physically depend on another model
    public School School { get; set; }  // VIOLATION: creates tight coupling
    public Guid SchoolId { get; set; }
}
```

**Problem:** `Student` is a primary model. It must be self-sufficient.
A `Student` should exist independently. The relationship belongs in
a relational or secondary model.

---

## Violation 3 — Relational Model with Unrelated Details (core-035)

```csharp
// BAD: StudentCourse has unrelated attributes
public class StudentCourse
{
    public Guid StudentId { get; set; }
    public Student Student { get; set; }
    public Guid CourseId { get; set; }
    public Course Course { get; set; }

    // Unrelated details in a relational model — VIOLATION
    public string StudentEmail { get; set; }
    public string CourseDescription { get; set; }
    public string InstructorName { get; set; }
}
```

**Problem:** A relational model must only hold references to the two primary models it connects.
Email belongs on `Student`. Description belongs on `Course`. Instructor is a separate entity.

---

## Violation 4 — Shared Utility Model (core-052, core-053)

```csharp
// BAD: Shared "helpers" violate autonomous components
public static class ModelHelper
{
    public static bool IsValid(Student student) { ... }
    public static bool IsValid(Course course) { ... }
    public static string GetDisplayName(object entity) { ... }
}
```

**Problem:** This is a `Helper` — horizontal entanglement. Each service must
own its own validation logic. Centralizing it here creates coupling between
unrelated flows and breaks autonomous component ownership.
