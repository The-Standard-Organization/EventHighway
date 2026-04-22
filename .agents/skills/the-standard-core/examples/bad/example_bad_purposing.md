# Bad Example: Purposing Violations

## Violation 1 — Skipping Observation (core-020)

```
Engineer: "We need a student service. Let me just start with the model."
// Immediately creates Student.cs, StudentService.cs
```

**Problem:** No observation. No real blocker identified. The engineer assumed
what was needed rather than observing the actual constraint.

**Consequence:** Likely builds the wrong thing. Wasted work, rework, or
a feature that does not solve the actual problem.

---

## Violation 2 — Vague Articulation (core-021)

```
Engineer: "The system is slow. Make it faster."
// Jumps to adding Redis caching everywhere.
```

**Problem:** "Slow" is not an articulated problem. What is slow? For whom?
Under what conditions? What is the acceptable threshold?

**Consequence:** Adds complexity (caching) to a problem that was never
properly defined. May cache the wrong data or introduce cache invalidation bugs.

---

## Violation 3 — Solutioning Without Constraints (core-022)

```
Engineer: "We'll use microservices for this. Each entity gets its own service,
           its own database, and its own deployment pipeline."
// No consideration for team size, operational maturity, or readability.
```

**Problem:** The solution does not honor longevity, maintainability, or
the Level 0 readability principle. A 3-person team cannot maintain
30 microservices.

**Consequence:** Over-engineering, operational burden, and unmaintainability.

---

## Violation 4 — Cutting Corners (core-023)

```
Engineer: "I know the proper way is to validate inputs first, but the deadline
           is tomorrow. I'll just write straight to the database."
```

**Problem:** Reaching the goal the wrong way is a violation.
An unvalidated write creates a security vulnerability and data integrity problem.

**Consequence:** Security breach, corrupt data, future rework that is more
expensive than doing it right the first time.

---

## Violation 5 — Unclear Purpose Proceeding Anyway (core-024)

```
Engineer: "I'm not totally sure what this feature should do, but I'll start
           coding and we'll figure it out as we go."
```

**Problem:** If purpose is unclear, you must stop and clarify.
Code written without purpose will be thrown away or, worse, kept
as a maintenance liability.

**Consequence:** Technical debt, confusion, and wasted effort.
