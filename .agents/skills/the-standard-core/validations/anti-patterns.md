# The Standard Core — Anti-Patterns

Each anti-pattern below includes: what it is, why it is harmful, and how to fix it.

---

## AP-CORE-001: Skipping Purposing

**What it is:**
An engineer or AI agent starts writing models or code before articulating the problem.

**Example:**
```
// No problem statement. Immediately writes Student model.
public class Student { public Guid Id; public string Name; }
```

**Why harmful:**
- Builds the wrong thing with confidence.
- Creates rework that is more expensive than the original work.
- Results in features that do not solve the real user problem.

**How to fix:**
Stop. Write the observation, articulation, and solutioning sections of the design template first. Only proceed when purpose is clear.

---

## AP-CORE-002: Speculative Modeling

**What it is:**
Adding model attributes or models that were not identified in purposing.

**Example:**
```csharp
public class Student
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string LinkedInUrl { get; set; }   // never identified in purposing
    public bool IsPremium { get; set; }        // speculative
}
```

**Why harmful:**
- Creates noise that future engineers must maintain.
- Leads to validation and database schema complexity for unused fields.
- Pollutes the model's purpose.

**How to fix:**
Remove all attributes not explicitly required by purposing. If a field is "nice to have", do not add it — wait until purposing requires it.

---

## AP-CORE-003: Horizontal Entanglement (Utils/Helpers)

**What it is:**
Creating `Utils`, `Helper`, `Common`, or `Shared` classes that multiple flows depend on.

**Example:**
```csharp
public static class ValidationHelper
{
    public static void ValidateStudent(Student s) { ... }
    public static void ValidateCourse(Course c) { ... }
}
```

**Why harmful:**
- A change to `ValidationHelper` can break unrelated flows.
- Destroys component autonomy.
- Makes the system impossible to rewrite in isolation.
- A bug in the helper cascades across the entire system.

**How to fix:**
Each service owns its own validation logic in its own partial class (e.g., `StudentService.Validations.cs`). Duplicate validation logic rather than share it.

---

## AP-CORE-004: Vertical Entanglement (Local Base Classes)

**What it is:**
Creating local base classes that inject hidden behavior into derived classes.

**Example:**
```csharp
public abstract class ServiceBase
{
    protected void ValidateEntity(object entity) { ... }
    protected void LogError(Exception ex) { ... }
}

public class StudentService : ServiceBase { ... }
```

**Why harmful:**
- Engineers reading `StudentService` cannot understand it without also reading `ServiceBase`.
- Hidden behavior in base classes violates No Magic.
- Creates deep coupling that makes rewriting one service dependent on all others.

**How to fix:**
Inject dependencies explicitly. Move validation into the service's own partial class. Move logging into an injected `ILoggingBroker`.

---

## AP-CORE-005: Excessive Inheritance (>1 Level)

**What it is:**
Using inheritance chains deeper than one level.

**Example:**
```csharp
public class EntityBase { }
public class AuditableEntity : EntityBase { }
public class Student : AuditableEntity { }  // 2 levels deep — VIOLATION
```

**Why harmful:**
- Deep inheritance chains obscure what a class actually is and does.
- Makes refactoring dangerous (changing a base class ripples unpredictably).
- Violates Level 0 — new engineers cannot follow the inheritance chain without extensive study.

**How to fix:**
Use composition over inheritance. If `Student` needs audit fields, add them directly to `Student`. Do not share them through inheritance.

---

## AP-CORE-006: Partial Adoption

**What it is:**
Applying some Standard rules but not others, then claiming the system is Standard-compliant.

**Example:**
```
"We use The Standard naming conventions, but we don't do TDD,
and we allow Helper classes because it's faster."
```

**Why harmful:**
- Breaks the system-level consistency that makes The Standard valuable.
- Creates confusion for new engineers who expect the full Standard.
- The "partial" system is not rewritable or predictable.

**How to fix:**
Adopt The Standard fully or do not claim Standard compliance. All-In or All-Out.

---

## AP-CORE-007: Optimization Before Readability

**What it is:**
Choosing a clever or performant solution that is harder to read, without measuring whether the performance was actually needed.

**Example:**
```csharp
// "optimized" but unreadable — parallel bit manipulation
int result = (a ^ b) & ~((a ^ b) - 1);
```

**Why harmful:**
- Introduces bugs that are hard to find.
- Future engineers cannot maintain or modify the code safely.
- The optimization is often premature and unnecessary.

**How to fix:**
Write the readable version first. Only optimize when a measured performance problem exists, and only if the optimization does not cross the readability threshold.

---

## AP-CORE-008: Cloud-Mandatory Development

**What it is:**
Building a system that cannot be run locally without a live cloud connection.

**Example:**
```
// All tests require live Azure Storage connection.
// Local development requires VPN + production credentials.
```

**Why harmful:**
- Engineers cannot work offline.
- Tests are non-deterministic and slow.
- Onboarding new engineers requires access provisioning before a single test can run.

**How to fix:**
Use broker abstraction with local stand-ins. Mock external cloud services in tests. Use local emulators (Azurite for Azure Storage, LocalStack for AWS).
