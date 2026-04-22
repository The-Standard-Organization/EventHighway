---
name: The Standard Code CSharp
description: Enforces the C# naming, organization, method, variable, class, field, instantiation, and comment/documentation rules of The Standard.
the standard version: v2.12.0
skill version: v0.3.0.0
---

# The Standard Code CSharp

## What this skill is

This skill is the canonical C# coding-style and organization layer for The Standard.
Use it whenever generating, reviewing, refactoring, or evaluating C# code.

## Explicit coverage map

This skill explicitly covers all supplied C# style rule files:

- Files
- Variables
- Methods
- Classes and Interfaces
- Comments and Documentation

It also preserves the implementation-profile naming conventions supplied in the implementation specification.

## When to use

Use this skill for any C# or .NET code, including models, brokers, services, controllers, tests, and support code.

## Mandatory operating behavior

0. Follow these rules exactly.
1. Prefer explicit readability over personal taste.
2. Use Standard naming even when team-local habits differ.
3. Reject abbreviations, ambiguous names, noisy wrappers, and inconsistent formatting.
4. Keep methods at level 0 of detail whenever possible.
5. Break down code when chaining or long declarations become cognitively expensive.
6. Preserve property order when instantiating models.
7. Use `this.` for private fields.
8. Maintain partial-file conventions for multi-dimensional classes.

## Canonical rule set

## 0 Files
The following are the naming conventions and guidance for naming C# files.

### 0.0 Naming
File names should follow the PascalCase convention followed by the file extension `.cs`.

##### Do
```csharp
Student.cs
```

##### Also, Do
```csharp
StudentService.cs
```

##### Don't
```csharp
student.cs
```

##### Also, Don't
```csharp
studentService.cs
```

##### Also, Don't
```csharp
Student_Service.cs
```

### 0.1 Partial Class Files
Partial class files are files that contain nested classes for a root file. For instance:

- StudentService.cs
    - StudentService.Validations.cs
    - StudentService.Exceptions.cs

Both validations and exceptions are partial classes to display a different aspect of any given class in a multi-dimensional space.

##### Do
```csharp
StudentService.Validations.cs
```

##### Also, Do
```csharp
StudentService.Validations.Add.cs
```

##### Don't
```csharp
StudentServiceValidations.cs
```

##### Also, Don't
```csharp
StudentService_Validations.cs
```

## 0. Variables

### 0.0 Naming
Variable names should be concise and representative of nature and the quantity of the value it holds or will potentially hold.

#### 0.0.0 Names
##### Do
```cs
var student = new Student();
```
##### Don't
```cs
var s = new Student();
```
##### Also, Don't
```cs
var stdnt = new Student();
```

The same rule applies to lambda expressions:
##### Do
```cs
students.Where(student => student ... );
```
##### Don't
```cs
students.Where(s => s ... );
```
<br />

#### 0.0.1 Plurals 
##### Do
```cs
var students = new List<Student>();
```
##### Don't
```cs
var studentList = new List<Student>();
```
<br />

#### 0.0.2 Names with Types

##### Do
```cs
var student = new Student();
```
##### Don't
```cs
var studentModel = new Student();
```
##### Also, Don't
```cs
var studentObj = new Student();
```
<br />

#### 0.0.3 Nulls or Defaults
If a variable value is it's default such as ```0``` for ```int``` or ```null``` for strings and you are not planning on changing that value (for testing purposes for instance) then the name should identify that value. 
##### Do
```cs
Student noStudent = null;
```
##### Don't
```cs
Student student = null;
```
##### Also, Do
```cs
int noChangeCount = 0;
```

##### But, Don't
```cs
int changeCount = 0;
```
<br /> <br />

### 0.1 Declarations
Declaring a variable and instantiating it should indicate the immediate type of the variable, even if the value is to be determined later.
#### 0.1.0 Clear Types
If the right side type is clear, then use ```var``` to declare your variable
##### Do
```cs
var student = new Student();
```
##### Don't
```cs
Student student = new Student();
```
<br /> <br />

#### 0.1.1 Semi-Clear Types
If the right side isn't clear (but known) of the returned value type, then you must explicitly declare your variable with it's type.
##### Do
```cs
Student student = GetStudent();
```
##### Don't
```cs
var student = GetStudent();
```
<br />

#### 0.1.2 Unclear Types 
If the right side isn't clear and unknown (such as an anonymous types) of the returned value type, you may use ```var``` as your variable type.
##### Do
```cs
var student = new
{
    Name = "Hassan",
    Score = 100
};
```
<br /> <br />

#### 0.1.3 Single-Property Types

Assign properties directly if you are declaring a type with one property. 

#### Do 

```cs
var inputStudentEvent = new StudentEvent();
inputStudentEvent.Student = inputProcessedStudent;
```

#### Don't

```cs
var inputStudentEvent = new StudentEvent
{
    Student = inputProcessedStudent
};
```

#### Also, Do
```cs
var studentEvent = new StudentEvent
{
    Student = someStudent,
    Date = someDate
}
```

#### Don't
```cs
var studentEvent = new StudentEvent();
studentEvent.Student = someStudent;
studentEvent.Date = someDate;
```

### 0.2 Organization

#### 0.2.0 Breakdown
If a variable declaration exceeds 120 characters, break it down starting from the equal sign.

##### Do
```cs
List<Student> washingtonSchoolsStudentsWithGrades = 
    await GetAllWashingtonSchoolsStudentsWithTheirGradesAsync();

```
##### Don't 
```cs
List<Student> washgintonSchoolsStudentsWithGrades = await GetAllWashingtonSchoolsStudentsWithTheirGradesAsync();
```
<br />

#### 0.2.1 Multiple Declarations
Declarations that occupy two lines or more should have a new line before and after them to separate them from previous and next variables declarations.

##### Do
```cs
Student student = GetStudent();

List<Student> washingtonSchoolsStudentsWithGrades = 
    await GetAllWashingtonSchoolsStudentsWithTheirGradesAsync();

School school = await GetSchoolAsync();
```

##### Don't
```cs
Student student = GetStudent();
List<Student> washgintonSchoolsStudentsWithGrades = 
    await GetAllWashingtonSchoolsStudentsWithTheirGradesAsync();
School school = await GetSchoolAsync();
```
Also, declarations of variables that are of only one line should have no new lines between them.

##### Do
```cs
Student student = GetStudent();
School school = await GetSchoolAsync();
```

##### Don't
```cs
Student student = GetStudent();

School school = await GetSchoolAsync();

```
<br />

## 1 Methods

### 1.0 Naming
Method names should be a summary of what the method is doing, it needs to stay percise and short and representative of the operation with respect to synchrony.

#### 1.0.0 Verbs
Method names must contain verbs in them to represent the action it performs.
##### Do
```cs
public List<Student> GetStudents()
{
	...
}

```
##### Don't
```cs
public List<Student> Students()
{
	...
}
```
<br />

#### 1.0.1 Asynchronousy
Asynchronous methods should be postfixed by the term ```Async``` such as methods returning ```Task``` or ```ValueTask``` in general.
##### Do
```cs
public async ValueTask<List<Student>> GetStudentsAsync()
{
	...
}
```
##### Don't
```cs
public async ValueTask<List<Student>> GetStudents()
{
	...
}
```
<br />

#### 1.0.2 Input Parameters
Input parameters should be explicit about what property of an object they will be assigned to, or will be used for any action such as search.
##### Do
```cs
public async ValueTask<Student> GetStudentByNameAsync(string studentName)
{
	...
}
```
##### Don't
```cs
public async ValueTask<Student> GetStudentByNameAsync(string text)
{
	...
}
```
##### Also, Don't
```cs
public async ValueTask<Student> GetStudentByNameAsync(string name)
{
	...
}
```
<br />

#### 1.0.3 Action Parameters
If your method is performing an action with a particular parameter specify it.
##### Do
```cs
public async ValueTask<Student> GetStudentByIdAsync(Guid studentId)
{
	...
}

```
##### Don't
```cs
public async ValueTask<Student> GetStudentAsync(Guid studentId)
{
	...
}
```
<br />

#### 1.0.4 Passing Parameters
When utilizing a method, if the input parameters aliases match the passed in variables in part or in full, then you don't have to use the aliases, otherwise you must specify your values with aliases.

Assume you have a method:
```csharp
Student GetStudentByNameAsync(string studentName);
```

##### Do
```cs
string studentName = "Todd";
Student student = await GetStudentByNameAsync(studentName);

```
##### Also, Do
```cs
Student student = await GetStudentByNameAsync(studentName: "Todd");
```

##### Also, Do
```cs
Student student = await GetStudentByNameAsync(toddName);
```

##### Don't
```cs
Student student = await GetStudentByNameAsync("Todd");
```

##### Don't
```cs
Student student = await GetStudentByNameAsync(todd);
```

<br /><br />

### 1.1 Organization
In general encapsulate multiple lines of the same logic into their own method, and keep your method at level 0 of details at all times.

#### 1.1.0 Single/Multiple-Liners

##### 1.1.0.0 One-Liners
Any method that contains only one line of code should use fat arrows
##### Do
```cs
public List<Student> GetStudents() => this.storageBroker.GetStudents();

```
##### Don't
```cs
public List<Student> Students()
{
	return this.storageBroker.GetStudents();
}
```

If a one-liner method exceeds the length of 120 characters then break after the fat arrow with an extra tab for the new line.

##### Do
```cs
public async ValueTask<List<Student>> GetAllWashingtonSchoolsStudentsAsync() => 
	await this.storageBroker.GetStudentsAsync();
```

##### Don't
```cs
public async ValueTask<List<Student>> GetAllWashingtonSchoolsStudentsAsync() => await this.storageBroker.GetStudentsAsync();
```
<br />

##### 1.1.0.1 Multiple-Liners
If a method contains multiple liners separated or connected via chaining it must have a scope. Unless the parameters are going on the next line then a one-liner method with multi-liner params is allowed.

##### Do
```cs
public Student AddStudent(Student student)
{
	ValidateStudent(student);

	return this.storageBroker.InsertStudent(student);
}
```

##### Also, Do
```cs
public Student AddStudent(Student student)
{
	return this.storageBroker.InsertStudent(student)
		.WithLogging();
}
```

##### Also, Do
```cs
public Student AddStudent(
	Student student)
{
	return this.storageBroker.InsertStudent(student);
}
```

##### Don't
```cs
public Student AddStudent(Student student) =>
	this.storageBroker.InsertStudent(student)
		.WithLogging();
```

##### Also, Don't
```cs
public Student AddStudent(
	Student student) =>
		this.storageBroker.InsertStudent(student);
```

#### 1.1.1 Returns
For multi-liner methods, take a new line between the method logic and the final return line (if any).
##### Do
```cs
public List<Student> GetStudents()
{
	StudentsClient studentsApiClient = InitializeStudentApiClient();

	return studentsApiClient.GetStudents();
}
```

##### Don't
```cs
public List<Student> GetStudents()
{
	StudentsClient studentsApiClient = InitializeStudentApiClient();
	return studentsApiClient.GetStudents();
}
```
<br />

#### 1.1.2 Multiple Calls
With mutliple method calls, if both calls are less than 120 characters then they may stack unless the final call is a method return, otherwise separate with a new line.
##### Do
```cs
public List<Student> GetStudents()
{
	StudentsClient studentsApiClient = InitializeStudentApiClient();
	List<Student> students = studentsApiClient.GetStudents();

	return students; 
}
```

##### Don't
```cs
public List<Student> GetStudents()
{
	StudentsClient studentsApiClient = InitializeStudentApiClient();

	List<Student> students = studentsApiClient.GetStudents();

	return students; 
}
```
##### Also, Do

```cs
public async ValueTask<List<Student>> GetStudentsAsync()
{
	StudentsClient washingtonSchoolsStudentsApiClient = 
		await InitializeWashingtonSchoolsStudentsApiClientAsync();

	List<Student> students = studentsApiClient.GetStudents();

	return students; 
}
```
##### Don't

```cs
public async ValueTask<List<Student>> GetStudentsAsync()
{
	StudentsClient washingtonSchoolsStudentsApiClient = 
		await InitializeWashingtonSchoolsStudentsApiClientAsync();
	List<Student> students = studentsApiClient.GetStudents();

	return students; 
}
```
<br />

#### 1.1.3 Declaration
A method declaration should not be longer than 120 characters.
##### Do
```cs
public async ValueTask<List<Student>> GetAllRegisteredWashgintonSchoolsStudentsAsync(
	StudentsQuery studentsQuery)
{
	...
}
```

##### Don't
```cs
public async ValueTask<List<Student>> GetAllRegisteredWashgintonSchoolsStudentsAsync(StudentsQuery studentsQuery)
{
	...
}
```
<br />

#### 1.1.4 Multiple Parameters
If you are passing multiple parameters, and the length of the method call is over 120 characters, you must break by the parameters, with **one** parameter on each line.
##### Do
```cs
List<Student> redmondHighStudents = await QueryAllWashingtonStudentsByScoreAndSchoolAsync(
	MinimumScore: 130,
	SchoolName: "Redmond High");
```

##### Don't
```cs
List<Student> redmondHighStudents = await QueryAllWashingtonStudentsByScoreAndSchoolAsync(
	MinimumScore: 130,SchoolName: "Redmond High");
```

#### 1.1.5 Chaining (Uglification/Beautification)
Some methods offer extensions to call other methods. For instance, you can call a `Select()` method after a `Where()` method. And so on until a full query is completed.

We will follow a process of Uglification Beautification. We uglify our code to beautify our view of a chain methods. Here's some examples:

##### Do
```csharp
	students.Where(student => student.Name is "Elbek")
		.Select(student => student.Name)
			.ToList();
```

##### Don't
```csharp
	students
		.Where(student => student.Name is "Elbek")
			.Select(student => student.Name)
				.ToList();
```

The first approach enforces simplifying and cutting the chaining short as more calls continues to uglify the code like this:

```csharp
	students.SomeMethod(...)
		.SomeOtherMethod(...)
			.SomeOtherMethod(...)
				.SomeOtherMethod(...)
					.SomeOtherMethod(...);
```
The uglification process forces breaking down the chains to smaller lists then processing it. The second approach (no uglification approach) may require additional cognitive resources to distinguish between a new statement and an existing one as follows:

```csharp
	student
		.Where(student => student.Name is "Elbek")
		.Select(student => student.Name)
		.OrderBy(student => student.Name)
		.ToList();
	
	ProcessStudents(students);
```

## 4 Classes

### 4.0 Naming
Classes that represent services or brokers in a Standard-Compliant architecture should represent the type of class in their naming convention, however that doesn't apply to models.

#### 4.0.0 Models
##### Do
```cs
class Student {
	...
}
```
##### Don't
```cs
class StudentModel {

}
```
<br />

#### 4.0.1 Services
In a singular fashion, for any class that contains business logic.
##### Do
```cs
class StudentService {
	....
}
```
##### Don't
```cs
class StudentsService{
	...
}
```
##### Also, Don't
```cs 
class StudentBusinessLogic {
	...
}
```
##### Also, Don't
```cs
class StudentBL {
	...
}
```
<br />

#### 4.0.2 Brokers
In a singular fashion, for any class that is a shim between your services and external resources.
##### Do
```cs
class StudentBroker {
	....
}
```
##### Don't
```cs
class StudentsBroker {
	...
}
```
<br />

#### 4.0.3 Controllers
In a plural fashion, to reflect endpoints such as ```/api/students``` to expose your logic via RESTful operations.
##### Do
```cs
class StudentsController {
	....
}
```
##### Don't
```cs
class StudentController {
	...
}
```

<br /> <br />
### 4.1 Fields
A field is a variable of any type that is declared directly in a class or struct. Fields are members of their containing type.

#### 4.1.0 Naming
Class fields are named in a camel cased fashion.
##### Do
```cs
class StudentsController {
	private readonly string studentName;
}
```
##### Don't
```cs
class StudentController {
	private readonly string StudentName;
}
```
##### Also, Don't
```cs
class StudentController {
	private readonly string _studentName;
}
```
Should follow the same rules for naming as mentioned in the Variables sections.

<br />

#### 4.1.1 Referencing
When referencing a class private field, use ```this``` keyword to distinguish private class member from a scoped method or constructor level variable.
##### Do
```cs
class StudentsController {
	private readonly string studentName;
	
	public StudentsController(string studentName) {
		this.studentName = studentName;
	}
}
```
##### Don't
```cs
class StudentController {
	private readonly string _studentName;

	public StudentsController(string studentName) {
		_studentName = studentName;
	}
}
```

<br /> <br />
### 4.2 Instantiations
#### 4.2.0 Input Params Aliases
If the input variables names match to input aliases, then use them, otherwise you must use the aliases, especially with values passed in.

##### Do
```cs
int score = 150;
string name = "Josh";

var student = new Student(name, score);

```

##### Also, Do
```cs
var student = new Student(name: "Josh", score: 150);

```

##### But, Don't
```cs
var student = new Student("Josh", 150);

```

##### Also, Don't
```cs
Student student = new (...);
```

#### 4.2.1 Honoring Property Order
When instantiating a class instance - make sure that your property assignment matches the properties order in the class declarations.

##### Do
```cs
public class Student
{
	public Guid Id {get; set;}
	public string Name {get; set;}
}

var student = new Student
{
	Id = Guid.NewGuid(),
	Name = "Elbek"
}
```

##### Also, Do
```cs
public class Student
{
	private readonly Guid id;
	private readonly string name;

	public Student(Guid id, string name)
	{
		this.id = id;
		this.name = name;
	}
}

var student = new Student (id: Guid.NewGuid(), name: "Elbek");
```
##### Don't
```cs
public class Student
{
	public Guid Id {get; set;}
	public string Name {get; set;}
}

var student = new Student
{
	Name = "Elbek",
	Id = Guid.NewGuid()
}
```

##### Also, Don't
```cs
public class Student
{
	private readonly Guid id;
	private readonly string name;

	public Student(string name, Guid id)
	{
		this.id = id;
		this.name = name;
	}
}

var student = new Student (id: Guid.NewGuid(), name: "Elbek");
```
##### Also, Don't
```cs
public class Student
{
	private readonly Guid id;
	private readonly string name;

	public Student(Guid id, string name)
	{
		this.id = id;
		this.name = name;
	}
}

var student = new Student (name: "Elbek", id: Guid.NewGuid());
```

## 12 Comments

### 12.0 Introduction
Comments can only be used to explain what code can't. Whether the code is visible or not.

### 12.1 Copyrights
Comments highlighting copyrights should follow this pattern:

##### Do
```csharp
    // ---------------------------------------------------------------
    // Copyright (c) Coalition of the Good-Hearted Engineers
    // FREE TO USE TO CONNECT THE WORLD
    // ---------------------------------------------------------------
```

##### Don't
```csharp

    //----------------------------------------------------------------
    // <copyright file="StudentService.cs" company="OpenSource">
    //      Copyright (C) Coalition of the Good-Hearted Engineers
    // </copyright>
    //----------------------------------------------------------------

```

##### Also, Don't
```csharp
   /* 
    * ==============================================================
    * Copyright (c) Coalition of the Good-Hearted Engineers
    * FREE TO USE TO CONNECT THE WORLD
    * ==============================================================
    */
```

### 12.2 Methods
Methods that have code that is not accessible at dev-time, or perform a complex function should contain the following details in their documentation.

- Purposing
- Incomes
- Outcomes
- Side Effects

## Implementation-profile naming addendum

## 10. Naming Conventions

| Element            | Pattern                                                       | Example                            |
| ------------------ | ------------------------------------------------------------- | ---------------------------------- |
| Broker interface   | `I{Resource}Broker`                                           | `IStorageBroker`, `IModernApiBroker`, `ILoggingBroker` |
| Broker class       | `{Resource}Broker`                                            | `StorageBroker`, `ModernApiBroker`, `LoggingBroker` |
| Broker method      | `{Action}{Entity}Async`                                       | `InsertLegacyUserAsync`, `PostPersonAsync` |
| Service interface  | `I{Entity}Service`                                            | `ILegacyUserService`               |
| Service class      | `{Entity}Service`                                             | `LegacyUserService`                |
| Service method     | `Add{Entity}Async`                                            | `AddLegacyUserAsync`               |
| Inner exception    | `{Adjective}{Entity}Exception`                                | `NullLegacyUserException`          |
| Outer exception    | `{Entity}{Category}Exception`                                 | `LegacyUserValidationException`    |
| Test class         | `{Entity}ServiceTests`                                        | `LegacyUserServiceTests`           |
| Test method        | `Should{Action}Async` / `ShouldThrow{Exception}On{Action}…`  | `ShouldAddLegacyUserAsync`         |

---
