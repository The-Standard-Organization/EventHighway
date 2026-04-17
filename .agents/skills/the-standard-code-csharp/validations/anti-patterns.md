# The Standard Code CSharp — Anti-Patterns

---

## AP-CS-001: Abbreviated Variable Names

**What it is:** Using single letters or abbreviations for variable names.

**Examples:**
```csharp
var s = new Student();              // cs-010
var stdnt = new Student();          // cs-010
students.Where(s => s.IsActive);    // cs-011
```

**Why harmful:** Abbreviated names make code harder to read, especially in refactoring tools, search, and code review. The Standard enforces Level 0 readability — abbreviations fail that test.

**How to fix:**
```csharp
var student = new Student();
students.Where(student => student.IsActive);
```

---

## AP-CS-002: Type Suffix Pollution

**What it is:** Adding type information to variable or class names that is already implied.

**Examples:**
```csharp
var studentModel = new Student();    // cs-013
var studentList = new List<Student>(); // cs-012
public class StudentModel { }        // cs-060
public class StudentDTO { }          // cs-060
```

**Why harmful:** Adds noise. The type is already visible from `new Student()` or `List<Student>`. It creates a maintenance burden when the type changes (the variable name also needs updating).

**How to fix:**
```csharp
var student = new Student();
var students = new List<Student>();
public class Student { }
```

---

## AP-CS-003: Null Intent Hidden

**What it is:** Naming a null-initialized variable as if it has a value.

**Example:**
```csharp
Student student = null;   // cs-014 — looks like it has a value
```

**Why harmful:** When a future engineer reads `student`, they expect it to hold a student. The null initialization is hidden. This leads to null-reference bugs that are hard to trace.

**How to fix:**
```csharp
Student noStudent = null;
```

---

## AP-CS-004: Fat Arrow for Multi-Line Methods

**What it is:** Using fat arrow `=>` for methods that contain multiple logical lines.

**Example:**
```csharp
// cs-051/cs-053 VIOLATION
public Student AddStudent(Student student) =>
    this.storageBroker.InsertStudent(student)
        .WithLogging();
```

**Why harmful:** Fat arrows are for single-expression methods. Using them for multi-step logic obscures the fact that the method does more than one thing. It also prevents adding breakpoints, local variable inspection, and future steps.

**How to fix:**
```csharp
public Student AddStudent(Student student)
{
    return this.storageBroker.InsertStudent(student)
        .WithLogging();
}
```

---

## AP-CS-005: Missing Blank Line Before Return

**What it is:** Placing `return` immediately after the last logic statement without a blank line.

**Example:**
```csharp
// cs-054 VIOLATION
public List<Student> GetStudents()
{
    StudentsClient studentsApiClient = InitializeStudentApiClient();
    return studentsApiClient.GetStudents();  // missing blank line
}
```

**Why harmful:** Makes the method harder to read at a glance. The return is not visually separated from the logic that produces the return value.

**How to fix:**
```csharp
public List<Student> GetStudents()
{
    StudentsClient studentsApiClient = InitializeStudentApiClient();

    return studentsApiClient.GetStudents();
}
```

---

## AP-CS-006: Underscore Field Prefix

**What it is:** Using `_fieldName` for private class fields.

**Example:**
```csharp
// cs-070 VIOLATION
private readonly IStorageBroker _storageBroker;

public StudentService(IStorageBroker storageBroker)
{
    _storageBroker = storageBroker;  // cs-072 VIOLATION: no this.
}
```

**Why harmful:** Underscore is a C# style from earlier eras. The Standard uses `this.` to distinguish private fields from local variables. Underscores are redundant and visually noisy.

**How to fix:**
```csharp
private readonly IStorageBroker storageBroker;

public StudentService(IStorageBroker storageBroker)
{
    this.storageBroker = storageBroker;
}
```

---

## AP-CS-007: Positional Literal Arguments

**What it is:** Passing literal values to a constructor or method without named aliases.

**Example:**
```csharp
// cs-080 VIOLATION
var student = new Student("Josh", 150);
await GetStudentByNameAsync("Todd");
```

**Why harmful:** At the call site, it is impossible to know what `"Josh"` and `150` represent without jumping to the constructor signature. Named aliases document the call in place.

**How to fix:**
```csharp
var student = new Student(name: "Josh", score: 150);
await GetStudentByNameAsync(studentName: "Todd");
```

---

## AP-CS-008: Instantiation Order Mismatch

**What it is:** Initializing properties in a different order than they are declared in the class.

**Example:**
```csharp
// cs-083 VIOLATION
// Class declares: Id, Name, CreatedDate, UpdatedDate
var student = new Student
{
    Name = "Elbek",        // wrong — Name before Id
    Id = Guid.NewGuid(),
    UpdatedDate = DateTimeOffset.UtcNow,  // wrong — Updated before Created
    CreatedDate = DateTimeOffset.UtcNow
};
```

**Why harmful:** Inconsistency between the class definition and instantiation sites creates cognitive overhead. Engineers must constantly verify alignment between two locations.

**How to fix:** Always match instantiation order exactly to the class declaration order.

---

## AP-CS-009: Non-Standard Copyright Block

**What it is:** Using XML-style or block-comment copyright.

**Examples:**
```csharp
// cs-091 VIOLATION
//----------------------------------------------------------------
// <copyright file="StudentService.cs" company="OpenSource">
//      Copyright (C) Coalition of the Good-Hearted Engineers
// </copyright>
//----------------------------------------------------------------

// cs-092 VIOLATION
/* ============================================================
 * Copyright (c) Coalition of the Good-Hearted Engineers
 * ============================================================ */
```

**Why harmful:** The Standard prescribes a specific copyright format for consistency across all compliant codebases. Non-Standard formats create visual noise and break tooling that scans for Standard-compliant files.

**How to fix:**
```csharp
// ---------------------------------------------------------------
// Copyright (c) Coalition of the Good-Hearted Engineers
// FREE TO USE TO CONNECT THE WORLD
// ---------------------------------------------------------------
```
