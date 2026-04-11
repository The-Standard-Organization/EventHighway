# The Standard Architecture — Anti-Patterns

---

## AP-ARCH-001: Business Logic in a Broker

**What it is:** A broker contains `if`/`switch`/`for` statements or makes decisions.

**Example:**
```csharp
public async ValueTask<Student> InsertStudentAsync(Student student)
{
    if (student.Name.Length > 100) // VIOLATION: flow control
        throw new Exception("Name too long");

    return await this.dbContext.Students.AddAsync(student);
}
```

**Why harmful:** Brokers are translation layers only. Business logic in a broker means it cannot be independently tested, and it contaminates the infrastructure layer with business concerns.

**How to fix:** Move all validation to the service's validation partial class.

---

## AP-ARCH-002: Exception Handling in a Broker

**What it is:** A broker catches exceptions and swallows, transforms, or re-throws them.

**Example:**
```csharp
public async ValueTask<Student> InsertStudentAsync(Student student)
{
    try
    {
        return await this.dbContext.Students.AddAsync(student);
    }
    catch (Exception ex) // VIOLATION
    {
        throw new Exception("Database failed", ex);
    }
}
```

**Why harmful:** Raw exceptions from infrastructure carry the original context (type, message, inner exception) that the service needs to categorize correctly. Wrapping in the broker destroys that context and prevents correct Standard exception mapping.

**How to fix:** Remove all try/catch from brokers. Let exceptions bubble to the service's `TryCatch` exception handler.

---

## AP-ARCH-003: Foundation Service Handling Multiple Entities

**What it is:** A single foundation service manages more than one entity type.

**Example:**
```csharp
public class StudentCourseService
{
    public ValueTask<Student> AddStudentAsync(Student student) { ... }
    public ValueTask<Course> AddCourseAsync(Course course) { ... }
}
```

**Why harmful:** Foundation services must be pure-primitive (same entity in/out). Mixing entity types makes validation, exception handling, and testing ambiguous. It violates single responsibility.

**How to fix:** Create `StudentService` and `CourseService` as separate, independent foundation services.

---

## AP-ARCH-004: Same-Layer Service Calling Another Same-Layer Service

**What it is:** A foundation service calls another foundation service directly.

**Example:**
```csharp
public class StudentService
{
    private readonly ICourseService courseService; // VIOLATION

    public async ValueTask<Student> AddStudentAsync(Student student)
    {
        var courses = await this.courseService.RetrieveAllCoursesAsync(); // VIOLATION
        ...
    }
}
```

**Why harmful:** Foundation services must only communicate upward. Same-layer calls create circular dependency risks, make the system harder to test, and violate the strictly layered architecture.

**How to fix:** If cross-entity coordination is needed, create an orchestration service that depends on both foundation services.

---

## AP-ARCH-005: Business Logic in a Controller

**What it is:** A controller contains `if`/`switch` logic, calculations, or decisions beyond mapping.

**Example:**
```csharp
[HttpPost]
public async Task<ActionResult> PostStudentAsync(Student student)
{
    if (student.Age < 18) // VIOLATION: business logic in controller
        return BadRequest("Student must be 18 or older");

    var addedStudent = await this.studentService.AddStudentAsync(student);
    return Created(addedStudent);
}
```

**Why harmful:** Controllers are pure mapping layers. Business rules in controllers are untestable through service unit tests and create duplicate validation that diverges from service-level validation.

**How to fix:** Move age validation to the foundation service's structural/logical validation. The controller maps the resulting `StudentValidationException` to HTTP 400.

---

## AP-ARCH-006: Not Logging Exceptions in the Service

**What it is:** A service catches and re-throws exceptions without logging them.

**Example:**
```csharp
catch (DuplicateKeyException duplicateKeyException)
{
    var alreadyExistsException = new AlreadyExistsStudentException(duplicateKeyException);
    throw new StudentDependencyValidationException(alreadyExistsException);
    // VIOLATION: no this.loggingBroker.LogError(...)
}
```

**Why harmful:** Without logging, production errors are invisible. The Standard requires every caught exception to be logged before being rethrown.

**How to fix:** Always call `this.loggingBroker.LogError(...)` on the categorized exception before returning it.

---

## AP-ARCH-007: Processing Service with Multiple Foundation Dependencies

**What it is:** A processing service depends on two or more foundation services.

**Example:**
```csharp
public class StudentProcessingService
{
    private readonly IStudentService studentService;
    private readonly ICourseService courseService; // VIOLATION
}
```

**Why harmful:** Processing services must depend on exactly one foundation service. Multi-foundation processing services are orchestration services in disguise and violate the layer contracts.

**How to fix:** If the logic truly requires both entities, promote this to an orchestration service with the correct naming and exception handling.

---

## AP-ARCH-008: Aggregation Service Asserting Call Order

**What it is:** Tests for an aggregation service verify the order in which dependencies are called.

**Example:**
```csharp
// In aggregation service test — VIOLATION
var sequence = new MockSequence();
mockStudentService.InSequence(sequence).Setup(...);
mockCourseService.InSequence(sequence).Setup(...);
```

**Why harmful:** Aggregation services by definition have no ordering contract. Asserting order creates a brittle test that will break if the implementation legitimately reorders calls for performance reasons.

**How to fix:** Remove sequence assertions. Test only the contract: the correct inputs were passed and the correct output was returned.
