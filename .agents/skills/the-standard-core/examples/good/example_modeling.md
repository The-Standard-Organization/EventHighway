# Good Example: Modeling

## Context

After purposing, the team models the student enrollment system.

## Correct Model Design

### Primary Models (self-sufficient, no external dependencies)

```csharp
// Student.cs — Primary model. Self-sufficient.
public class Student
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public DateTimeOffset CreatedDate { get; set; }
    public DateTimeOffset UpdatedDate { get; set; }
}

// Course.cs — Primary model. Self-sufficient.
public class Course
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public DateTimeOffset CreatedDate { get; set; }
    public DateTimeOffset UpdatedDate { get; set; }
}
```

**Why correct:**
- `Student` and `Course` carry only what the problem requires.
- No speculative fields (no `PhoneNumber`, `Address` unless purposing identified them).
- No type suffix (`StudentModel` is forbidden).

---

### Relational Model (connects two primary models)

```csharp
// StudentCourse.cs — Relational model connecting Student ↔ Course
public class StudentCourse
{
    public Guid StudentId { get; set; }
    public Student Student { get; set; }
    public Guid CourseId { get; set; }
    public Course Course { get; set; }
}
```

**Why correct:**
- Connects exactly two primary models.
- Contains only references, no unrelated attributes.

---

### Secondary Model (depends on a primary model)

```csharp
// Enrollment.cs — Secondary model. References Student and Course.
public class Enrollment
{
    public Guid Id { get; set; }
    public Guid StudentId { get; set; }
    public Student Student { get; set; }
    public Guid CourseId { get; set; }
    public Course Course { get; set; }
    public EnrollmentStatus Status { get; set; }
    public DateTimeOffset CreatedDate { get; set; }
}
```

**Why correct:**
- Depends on `Student` and `Course` (primary models).
- Carries only enrollment-specific attributes (`Status`, `CreatedDate`).

---

### Operational Models

```
IStorageBroker        → Integration model (broker)
IStudentService       → Processing model (service)
StudentsController    → Exposure model (exposer)
Startup / Program.cs  → Configuration model
```

**Why correct:**
- Each operational model plays exactly one Tri-Nature role.
- No model crosses its designated responsibility.
