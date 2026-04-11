# The Standard Core — Validation Checklist

Run this checklist before approving any design, model, or architecture decision.
Each item is binary: PASS or FAIL. A single FAIL must be resolved before proceeding.

---

## PURPOSING

- [ ] **core-020** The real blocker or unmet need has been identified through observation (not assumption).
- [ ] **core-021** The problem has been articulated clearly in writing.
- [ ] **core-022** The solution honors readability, configurability, longevity, optimization, and maintainability.
- [ ] **core-023** No corners have been cut to reach the goal.
- [ ] **core-024** Purpose is fully clear — no ambiguity remains before proceeding.

---

## ENGINEERING SEQUENCE

- [ ] **core-010** Purposing was completed before modeling began.
- [ ] **core-011** Modeling was completed before simulation/implementation began.
- [ ] **core-012** No implementation code was written while purpose was still unclear.

---

## MODELING

- [ ] **core-030** Every model attribute was required by the purpose — no speculative fields.
- [ ] **core-031** No irrelevant attributes were added to any model.
- [ ] **core-032** All model names are generic, clear, and problem-scoped (no `Model`, `Object`, `Data` suffixes).
- [ ] **core-033** All primary models are self-sufficient (no physical external model dependency).
- [ ] **core-034** All secondary models correctly reference a primary model.
- [ ] **core-035** All relational models connect exactly two primary models with no unrelated fields.
- [ ] **core-036** Hybrid models are used only when the business flow absolutely requires mixed behavior.
- [ ] **core-037** Every operational model is correctly assigned: broker / service / exposer / configuration.

---

## SIMPLICITY & ENTANGLEMENT

- [ ] **core-050** No unnecessary complexity was introduced.
- [ ] **core-051** No class uses more than one level of inheritance.
- [ ] **core-052** No `Utils`, `Commons`, or `Helper` classes exist in the codebase.
- [ ] **core-053** No model is shared across independent flows.
- [ ] **core-054** No local base classes with hidden coupling exist.

---

## AUTONOMOUS COMPONENTS

- [ ] **core-060** Every component owns its validations, tooling, and utilities.
- [ ] **core-061** No component depends on hidden helpers — only on injected dependencies.

---

## NO MAGIC

- [ ] **core-070** No hidden routines exist.
- [ ] **core-071** No extension methods or magical abstractions that require reference-chasing.
- [ ] **core-072** No runtime tricks that obscure system behavior.
- [ ] **core-073** Validation, exception handling, tracing, and security are in plain sight.

---

## REWRITABILITY

- [ ] **core-080** The system can be fully rewritten by any engineer with The Standard knowledge.
- [ ] **core-081** No hidden dependencies exist.
- [ ] **core-082** No unknown prerequisites exist.
- [ ] **core-083** No injected routines obscure system behavior.

---

## LEVEL 0

- [ ] **core-090** An entry-level engineer can understand the system without mentorship.
- [ ] **core-091** No component is so complex that a new team member cannot follow it.

---

## AIRPLANE MODE

- [ ] **core-120** The system runs locally without mandatory cloud connectivity.
- [ ] **core-121** Local stand-ins or mocks exist for all cloud resources.

---

## ALL-IN / ALL-OUT

- [ ] **core-100** All Standard rules are applied — no selective adoption.
- [ ] **core-101** No outdated partial adherence is claimed as compliance.

---

## RESULT

| Check | PASS / FAIL |
|---|---|
| Purposing | |
| Engineering Sequence | |
| Modeling | |
| Simplicity & Entanglement | |
| Autonomous Components | |
| No Magic | |
| Rewritability | |
| Level 0 | |
| Airplane Mode | |
| All-In / All-Out | |

**Overall: PASS only if every row above is PASS.**
