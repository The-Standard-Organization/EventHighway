# System Design Template (Standard-Compliant)

Use this template when starting any new system, service, or feature.
Complete every section before writing any code.

---

## 1. PURPOSING

### 1.1 Observation
> Describe the real blocker, constraint, or unmet need you have observed.
> Be specific. Use facts, not assumptions.

**Observed problem:**
[Describe the actual, observable problem here]

**Affected parties:**
- [Who is affected and how]

**Current state (without solution):**
[What happens today without the solution]

**Constraints:**
- [List hard constraints: technical, legal, time, team size]

---

### 1.2 Articulation
> Re-state the problem in one clear sentence.

**Problem statement:**
> [One sentence that completely captures the problem]

---

### 1.3 Solutioning
> Describe the solution path. Include: what will be built, what principles apply.

**Proposed solution:**
[Describe the approach]

**Principles honored:**
- [ ] Readability — an entry-level engineer can follow this
- [ ] Configurability — environment-specific values are injectable
- [ ] Longevity — this system can be maintained 2 years from now
- [ ] Optimization — performance is addressed without sacrificing readability
- [ ] Maintainability — another engineer can pick this up on their "last day"
- [ ] Airplane Mode — system runs locally without cloud connectivity

---

## 2. MODELING

### 2.1 Data Carrier Models

| Model Name | Type | Justification |
|---|---|---|
| [ModelName] | Primary / Secondary / Relational / Hybrid | [Why this model exists] |

**Model attributes (only what purposing requires):**

```csharp
// [ModelName].cs
public class [ModelName]
{
    public Guid Id { get; set; }
    // [only attributes identified in purposing]
}
```

---

### 2.2 Operational Models

| Model Name | Tri-Nature Role | Responsibility |
|---|---|---|
| [BrokerName]Broker | Integration (Broker) | Wraps [external resource] |
| [Entity]Service | Processing (Service) | [Business logic summary] |
| [Entities]Controller | Exposure (Exposer) | REST API for [entity] |

---

## 3. SIMULATION

### 3.1 Flow Description
> Describe how the models interact to produce the system's output.

```
[Entity Input]
  → [Exposer receives request]
  → [Service validates and processes]
  → [Broker writes/reads from external resource]
  → [Response flows back up the chain]
```

### 3.2 Methods/Routines Required

| Method | Owner | Signature |
|---|---|---|
| Add[Entity]Async | [Entity]Service | `ValueTask<[Entity]> Add[Entity]Async([Entity] entity)` |
| Retrieve[Entity]ByIdAsync | [Entity]Service | `ValueTask<[Entity]> Retrieve[Entity]ByIdAsync(Guid entityId)` |
| Modify[Entity]Async | [Entity]Service | `ValueTask<[Entity]> Modify[Entity]Async([Entity] entity)` |
| Remove[Entity]ByIdAsync | [Entity]Service | `ValueTask<[Entity]> Remove[Entity]ByIdAsync(Guid entityId)` |

---

## 4. VALIDATION GATE

Before writing any code, confirm:

- [ ] Purpose is documented and agreed upon
- [ ] All models contain only purposing-required attributes
- [ ] No speculative fields have been added
- [ ] Operational model roles are assigned (broker / service / exposer)
- [ ] The Tri-Nature applies to every layer
- [ ] Local execution (airplane mode) is possible
- [ ] An entry-level engineer can understand the design (Level 0)
