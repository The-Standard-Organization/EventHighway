---
name: The Standard Versioning
description: Enforces Standard Versioning discipline.
the standard version: v2.12.0
skill version: v0.3.0.0
---

# The Standard — Versioning

## What this skill is

This skill defines the required versioning approach for:

- release versioning
- file versioning (models, services, exceptions)
- API versioning
- deprecation signaling
- discovery capabilities

This skill MUST follow the explicit versioning model defined here and MUST NOT substitute alternative semantic versioning strategies, compatibility theories, or inferred conventions.

The skill exists to keep version changes:

- explicit
- structured
- non-destructive
- discoverable
- safe for consumers

---

## When to use

Use this skill whenever:
- creating a new release version
- introducing a model change that requires versioning
- introducing a service or routine change that requires versioning
- creating new versioned files or folders
- defining API routes for versioned resources
- marking code or APIs as deprecated
- implementing discovery capabilities
- reviewing version-related changes in PRs

---

## Explicit coverage map

This skill covers these primary areas:

1. **Release versioning**
2. **Code versioning**
3. **Deprecation**
4. **Discovery capabilities**

Code versioning in this skill includes:

- file versioning
- API versioning
- deprecation markers
- discovery capability signaling that helps consumers understand supported behavior

---

## Release Versioning

### Release Format

Release versions MUST use the following exact format:

```text
v1.2.3.4
```

### Segment Meaning

| Segment | Meaning |
|---|---|
| 1 | Model change |
| 2 | Service or routine change |
| 3 | Bug fix or configuration change |
| 4 | Automated build version |

These meanings are fixed for this skill and MUST NOT be reinterpreted.

### Release Increment Rules

0. A release version MUST remain in the exact `v1.2.3.4` shape.

1. A **model change** MUST increment segment `1` and MUST reset segments `2`, `3`, and `4` to zero.

2. A **service or routine change** MUST increment segment `2` and MUST reset segments `3` and `4` to zero.

3. A **bug fix or configuration change** MUST increment segment `3` and MUST reset segment `4` to zero.

4. A **new automated build** for the same code MUST increment segment `4` only.

5. If multiple change types occur together, the **highest-order change MUST win** and all lower segments MUST reset to zero.

### Release Examples

Given current version:

```text
v1.2.3.4
```

Expected next versions:

- model change → `v2.0.0.0`
- service or routine change → `v1.3.0.0`
- bug fix or configuration change → `v1.2.4.0`
- automated build of the same code → `v1.2.3.5`
- model + service change together → `v2.0.0.0`

### Release Guidance

Use the release version to describe **what kind of change happened**, not merely that a change happened.

That means:

- a model change is never hidden inside the service segment
- a service change is never hidden inside the bug/config segment
- a build increment is never used to represent code change

---

## File Versioning

### V0 Convention

A file without a version folder is treated as **V0**.

Examples:

```text
Models/Foundations/Students/Student.cs
Services/Foundations/Students/StudentService.cs
Models/Foundations/Students/Exceptions/
```

This means the starting shape is unversioned on disk, but version zero is still logically assumed.

### Model Changes

When a model changes, a new versioned file MUST be created under a version folder that is a subfolder of the original location.

Example:

```text
Models/Foundations/Students/V1/StudentV1.cs
```

This pattern means:

- the original V0 model remains where it is
- the new model version lives in a nested `V1` folder
- the version number becomes explicit in both folder and type name

### Service Changes

When a service changes, a new versioned service MUST be created under a version folder that is a subfolder of the original location.

Example:

```text
Services/Foundations/Students/V1/StudentV1Service.cs
```

Related exceptions MUST align with the matching versioned model path.

Example:

```text
Models/Foundations/Students/V1/Exceptions/
```

### File Rules

0. No version folder means **V0 is implied**.

1. A new version MUST be introduced by creating new versioned files and folders, not by overwriting the earlier implementation.

2. A model change MUST produce a versioned model file under a `Vn` subfolder.

3. A service change MUST produce a versioned service file under a `Vn` subfolder.

4. Versioned exceptions MUST live beneath the corresponding versioned model path.

5. Existing earlier-version code MUST remain available unless there is an explicit deprecation/removal strategy outside the introduction of the new version.

### Naming Convention

Versioned file/type naming MUST follow this pattern:

- model → `{Entity}V{n}.cs`
- service → `{Entity}V{n}Service.cs`

Examples:

- `StudentV1.cs`
- `StudentV1Service.cs`

---

## API Versioning

### Default API Route

The initial API is exposed without a version prefix:

```text
api/Students
```

This is the route for V0.

### Model Version Route

If the model changes, the route MUST surface the model version:

```text
api/V1/Students
```

Here, `V1` tells the consumer that this endpoint represents the version 1 model.

### Service Behavior Route

If the model is already at version 1 and the service behavior changes for that model, the route MUST surface both the model version and the behavior version:

```text
api/V1.1/Students
```

In this shape:

- `1` before the dot denotes the model version
- `1` after the dot denotes the behavior/service version

This is a **model + behavior pairing** and MUST be interpreted that way.

If only behaviour changed but the model stayed the same, the new route will be `api/V0.1/Students` — the model version is still 0, but the behavior version has incremented to 1.

### API Rules

0. `api/{resource}` denotes V0.

1. `api/Vn/{resource}` denotes model version `n`.

2. `api/Vn.m/{resource}` denotes model version `n` with behavior version `m`.

3. A route version MUST communicate the intended model and behavior pairing.

4. The API route MUST remain aligned with the versioning story of the underlying model/service pair.

### API Example Flow

Starting point:

```text
api/Students
```

After model change:

```text
api/V1/Students
```

After additional service behavior change for the V1 model:

```text
api/V1.1/Students
```

---

## Deprecation

Deprecation MUST be clear enough that consumers can see:

- that an API is deprecated
- when it is expected to sunset
- where to go for migration guidance

Consumers MUST be given enough time to act before the deprecated version is sunset.

### Deprecation Signaling

A valid approach is to surface deprecation metadata through headers or attributes that emit those headers.

Example using a controller action attribute:

```csharp
[ApiController]
[Route("api/[controller]")]
public class SampleController : ControllerBase
{
    [HttpGet]
    [DeprecatedApi(
        Sunset = "2024-12-31",
        Warning = "This API is deprecated. Please migrate to v2.",
        Link = "https://example.com/deprecation-info")]
    public IActionResult GetSampleData()
    {
        return Ok(new { message = "Sample data" });
    }
}
```

#### Key Libraries

| Package                | Purpose                                           |
| ---------------------- | -------------------------------------------------------------------------------------------------------------------------------------- |
| `Attrify`              | A NuGet library for controlling API visibility and lifecycle (e.g. deprecation) through attributes applied to controllers and actions        |

---

#### Code Deprecation

Code that is no longer maintained or supported SHOULD be marked with:

```csharp
[Obsolete("This version is deprecated and no longer maintained.")]
```

Use the obsolete marker to make deprecation visible to maintainers and consumers at compile time.

---

## Capabilities

Capabilities are recommended so consumers can discover what is supported in each version before invoking a feature.

This is especially useful where model versions and service behaviors differ by version or by provider.

### Capability Goals

A capabilities surface should help consumers discover:

- what features are supported
- what model versions are supported
- what behavior versions are supported
- what optional features are unavailable

### Capability Surface Options

Capabilities MAY be exposed through:

- Swagger
- middleware-based metadata
- a bespoke capabilities endpoint
- a provider-specific capabilities endpoint

### Provider Example

In a provider-based architecture such as a SPAL provider, each concrete provider can expose a 
capabilities endpoint that tells consumers which operations are supported. 

This pattern allows consumers to safely determine whether a provider supports a given operation **before invoking it**, avoiding runtime failures.

---

#### 1. Operation Attribute

```csharp
[AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
public sealed class StudentOperationAttribute : Attribute
{
    public string OperationName { get; }

    public StudentOperationAttribute(string operationName) =>
        OperationName = operationName ?? string.Empty;

    public StudentOperationAttribute() =>
        OperationName = string.Empty;
}
```

---

#### 2. Capabilities Models

```csharp
public sealed class ResourceCapabilities
{
    public string ResourceName { get; init; } = string.Empty;
    public IReadOnlyCollection<string> SupportedOperations { get; init; } =
        Array.Empty<string>();
}

public sealed class ProviderCapabilities
{
    public string ProviderName { get; init; } = string.Empty;
    public IReadOnlyCollection<ResourceCapabilities> SupportedResources { get; init; } =
        Array.Empty<ResourceCapabilities>();
}
```

---

#### 3. Student Provider Contract

```csharp
public interface IStudentProvider
{
    string ProviderName { get; }

    ProviderCapabilities Capabilities { get; }

    IStudentResource Students { get; }
}
```

---

#### 4. Resource Contract

```csharp
public interface IStudentResource
{
    ValueTask<Student> AddAsync(Student student);
}
```

---

#### 5. Provider Base (Simplified Capability Declaration)

```csharp
public abstract class StudentProviderBase : IStudentProvider
{
    public abstract string ProviderName { get; }

    public abstract IStudentResource Students { get; }

    public virtual ProviderCapabilities Capabilities =>
        new ProviderCapabilities
        {
            ProviderName = ProviderName,
            SupportedResources = new[]
            {
                new ResourceCapabilities
                {
                    ResourceName = "Student",
                    SupportedOperations = GetSupportedOperations(Students)
                }
            }
        };

    private static IReadOnlyCollection<string> GetSupportedOperations(object resource)
    {
        if (resource is null)
            return Array.Empty<string>();

        return resource.GetType()
            .GetMethods()
            .Select(method =>
                method.GetCustomAttributes(typeof(StudentOperationAttribute), true)
                      .FirstOrDefault() as StudentOperationAttribute)
            .Where(attribute => attribute is not null)
            .Select(attribute => attribute!.OperationName)
            .Where(name => !string.IsNullOrWhiteSpace(name))
            .Distinct()
            .ToArray();
    }
}
```

---

#### 6. Example Implementation

```csharp
public sealed class StudentResource : IStudentResource
{
    [StudentOperation("Add")]
    public ValueTask<Student> AddAsync(Student student)
    {
        return ValueTask.FromResult(student);
    }
}

public sealed class StudentProvider : StudentProviderBase
{
    public override string ProviderName => "DefaultStudentProvider";
    public override IStudentResource Students { get; } = new StudentResource();
}
```

---

#### 7. Capability Check Extension

```csharp
public static class StudentProviderCapabilitiesExtensions
{
    public static bool SupportsOperation(
        this IStudentProvider provider,
        string resourceName,
        string operationName)
    {
        var resource = provider.Capabilities.SupportedResources
            .FirstOrDefault(r => r.ResourceName == resourceName);

        if (resource is null)
            return false;

        return resource.SupportedOperations.Contains(operationName);
    }
}
```

---

#### 8. Usage

```csharp
IStudentProvider provider = new StudentProvider();

if (!provider.SupportsOperation("Student", "Add"))
{
    // avoid call
    return;
}

await provider.Students.AddAsync(new Student());
```

---

#### Key Principle

Capabilities turn runtime uncertainty into a deterministic check:

- Without capabilities → call → fail → handle exception  
- With capabilities → check → decide → call safely  

This aligns with the Standard principle:

> Avoid invalid operations rather than reacting to them


---
