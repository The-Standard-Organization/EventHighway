# The Standard Code CSharp — Rules

## FILES

**cs-001** [ERROR] File names must use PascalCase and end with `.cs` — e.g., `Student.cs`, `StudentService.cs`.
**cs-002** [ERROR] Partial class files must use dot-notation — e.g., `StudentService.Validations.cs`, `StudentService.Exceptions.cs`.
**cs-003** [ERROR] Partial class files must NOT use concatenation — `StudentServiceValidations.cs` is forbidden.
**cs-004** [ERROR] Partial class files must NOT use underscore — `StudentService_Validations.cs` is forbidden.

## VARIABLES — NAMING

**cs-010** [ERROR] Variable names must be full and descriptive. Single letters and abbreviations are forbidden: `s`, `stdnt`, `st`.
**cs-011** [ERROR] Lambda parameter names must be full: `students.Where(student => ...)` not `students.Where(s => ...)`.
**cs-012** [ERROR] Plural collections use natural plural: `students` not `studentList`, `studentCollection`, `studentArray`.
**cs-013** [ERROR] No type suffix in variable names: `student` not `studentModel`, `studentObj`, `studentEntity`.
**cs-014** [ERROR] Variables with null/default values must signal intent: `Student noStudent = null` not `Student student = null`.
**cs-015** [ERROR] Zero-value numeric variables must signal intent: `int noChangeCount = 0` not `int changeCount = 0`.

## VARIABLES — DECLARATIONS

**cs-020** [ERROR] When the right-side type is clear (e.g., `new Student()`), use `var`.
**cs-021** [ERROR] When the right-side type is NOT clear (e.g., method call returning concrete type), declare with explicit type.
**cs-022** [WARNING] When the right-side type is anonymous, `var` is required.
**cs-023** [ERROR] Single-property object: assign property after construction (not in initializer). Multi-property: use initializer.

## VARIABLES — ORGANIZATION

**cs-030** [ERROR] Variable declaration exceeding 120 characters must break after the `=` sign.
**cs-031** [ERROR] Multi-line declarations must have a blank line before AND after them.
**cs-032** [ERROR] Single-line declarations must have NO blank lines between consecutive single-line declarations.

## METHODS — NAMING

**cs-040** [ERROR] Method names must contain a verb: `GetStudents()` not `Students()`.
**cs-041** [ERROR] Async methods must end with `Async` suffix: `GetStudentsAsync()` not `GetStudents()`.
**cs-042** [ERROR] Input parameter names must be fully qualified: `studentName` not `name`, `text`, `value`.
**cs-043** [ERROR] Action parameters must identify what property is acted on: `GetStudentByIdAsync(Guid studentId)` not `GetStudentAsync(Guid studentId)`.
**cs-044** [ERROR] When calling a method with a matching variable name (full or partial), no alias is needed. When passing literals or non-matching names, the alias is required: `GetStudentByNameAsync(studentName: "Todd")` not `GetStudentByNameAsync("Todd")`.

## METHODS — ORGANIZATION

**cs-050** [ERROR] Methods with exactly one line of code must use fat arrow syntax: `=> this.storageBroker.GetStudents()`.
**cs-051** [ERROR] Methods with multiple lines of code must use a scope body `{ }`.
**cs-052** [ERROR] Single-liner exceeding 120 characters must break after `=>` with one extra tab indentation.
**cs-053** [ERROR] Multi-liner methods with chaining must use a scope body (not fat arrow).
**cs-054** [ERROR] Multi-line methods must have a blank line between the last logic statement and the `return` statement.
**cs-055** [ERROR] If multiple consecutive calls are under 120 chars, they may stack without blank lines unless the final call is a `return`.
**cs-056** [ERROR] If any call exceeds 120 chars, blank line separation is required between it and adjacent calls.
**cs-057** [ERROR] Method declarations exceeding 120 characters must break parameters onto the next line.
**cs-058** [ERROR] When multiple parameters are broken onto new lines, each parameter must be on its own line.
**cs-059** [ERROR] Method chaining (uglification-beautification): first call on same line as subject, each subsequent call indented by one extra tab.

## CLASSES — NAMING

**cs-060** [ERROR] Model class names carry no type suffix: `Student` not `StudentModel`, `StudentDTO`, `StudentEntity`.
**cs-061** [ERROR] Service class names are singular PascalCase + Service: `StudentService` not `StudentsService`, `StudentBL`.
**cs-062** [ERROR] Broker class names are singular PascalCase + Broker: `StudentBroker` not `StudentsBroker`.
**cs-063** [ERROR] Controller class names are plural PascalCase + Controller: `StudentsController` not `StudentController`.

## CLASSES — FIELDS

**cs-070** [ERROR] Class fields are named in camelCase: `private readonly string studentName` not `StudentName`, `_studentName`.
**cs-071** [ERROR] Class fields must follow the same naming rules as variables (descriptive, no abbreviation, no type suffix).
**cs-072** [ERROR] Private class fields must be referenced using `this.`: `this.studentName = studentName`.

## CLASSES — INSTANTIATION

**cs-080** [ERROR] When passing literals directly, named aliases are required: `new Student(name: "Josh")` not `new Student("Josh")`.
**cs-081** [ERROR] When variable names match constructor parameter aliases, aliases are not required: `new Student(name, score)` is valid if variables are `name` and `score`.
**cs-082** [ERROR] Target-typed `new()` is forbidden: `Student student = new (...)` is not allowed.
**cs-083** [ERROR] Instantiation property assignment order must match the class declaration property order.

## COMMENTS

**cs-090** [ERROR] Copyright block must use the exact Standard format: `// ------` dash lines, `// Copyright (c)...`, `// FREE TO USE...`.
**cs-091** [ERROR] XML-style copyright comments (`<copyright>`) are forbidden.
**cs-092** [ERROR] Block comment copyright (`/* ... */`) is forbidden.
**cs-093** [WARNING] Methods whose code is not accessible at dev-time should document: Purpose, Incomes, Outcomes, Side Effects.
