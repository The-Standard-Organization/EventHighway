---
name: The Standard Core
description: Enforces the theory, purpose, modeling, simulation flow, and non-negotiable core values of The Standard.
the standard version: v2.12.0
skill version: v0.3.0.0
---

# The Standard Core

## What this skill is

This skill is the governing layer for every other Standard skill.
It defines the theory, operating model, and core values that must shape all design, implementation, testing, review, and planning decisions.

The Standard is technology-agnostic.
Apply it regardless of language, framework, runtime, or hosting model.
Use C# and .NET examples only as materializations of the theory, not as the theory itself.

## Explicit coverage map

This skill explicitly covers the foundational material of The Standard:

- 0. Introduction
- 0.0 The Theory
  - 0.0.0 Introduction
  - 0.0.1 Finding Answers
  - 0.0.2 Tri-Nature
  - 0.0.2.0 Purpose
  - 0.0.2.1 Dependency
  - 0.0.2.2 Exposure
  - 0.0.3 Everything is Connected
  - 0.0.4 Fractal Pattern
  - 0.0.5 Systems Design & Architecture
  - 0.0.6 Conclusion
- 0.1 Purposing, Modeling, and Simulation
  - 0.1.0 Introduction
  - 0.1.1 Purposing
  - 0.1.1.0 Observation
  - 0.1.1.1 Articulation
  - 0.1.1.2 Solutioning
  - 0.1.2 Modeling
  - 0.1.2.0 Model Types
  - 0.1.2.0.0 Data Carrier Models
  - 0.1.2.0.0.0 Primary Models
  - 0.1.2.0.0.1 Secondary Models
  - 0.1.2.0.0.2 Relational Models
  - 0.1.2.0.0.3 Hybrid Models
  - 0.1.2.0.1 Operational Models
  - 0.1.2.0.1.0 Integration Models (Brokers)
  - 0.1.2.0.1.1 Processing Models (Services)
  - 0.1.2.0.1.2 Exposure Models (Exposers)
  - 0.1.2.0.2 Configuration Models
  - 0.1.3 Simulation
  - 0.1.4 Summary
- 0.2 Principles
  - 0.2.0 People-First
  - 0.2.0.0 Simplicity
  - 0.2.0.0.0 Excessive Inheritance
  - 0.2.0.0.1 Entanglement
  - 0.2.0.0.1.0 Horizontal Entanglement
  - 0.2.0.0.1.1 Vertical Entanglement
  - 0.2.0.0.2 Autonomous Components
  - 0.2.0.0.2.0 No Magic
  - 0.2.1 Rewritability
  - 0.2.2 Mono-Micro
  - 0.2.3 Level 0
  - 0.2.4 Open Code
  - 0.2.5 Airplane Mode (Cloud-Foreign)
  - 0.2.6 No Toasters
  - 0.2.7 Pass Forward
  - 0.2.8 All-In/All-Out
  - 0.2.9 Readability over Optimization
  - 0.2.10 Last Day

## When to use

Use this skill always when any software-related request is present.
Use it before architecture, before coding, before testing, before planning, and before review.
If another Standard skill applies, activate this one first and let it govern the rest.

## Identity and theory

0. Every system is governed by theory, whether explicit or implicit.
1. The Standard is powered by the Tri-Nature theory.
2. Every system must be understood through three lenses:
   - Purpose
   - Dependency
   - Exposure
3. Tri-Nature is fractal.
   - The same pattern repeats at system level, sub-system level, service level, validation level, and exposure level.
4. Everything is connected.
   - Design every part with awareness that it will become someone else’s dependency or exposure.

## Mandatory engineering sequence

Follow this order intentionally:

0. Purposing
1. Modeling
2. Simulation

This order is mandatory at initiation.
Iteration is allowed later, but do not skip purpose to jump into code.

## Purposing rules

0. Start with observation.
   - Identify the real blocker, constraint, or unmet need.
1. Articulate the problem clearly.
   - Use words, diagrams, or figures when helpful.
   - A well-described problem is halfway solved.
2. Include solutioning in the purpose.
   - Do not treat the path to the goal as trivial.
   - A solution must also honor readability, configurability, longevity, optimization, and maintainability.
3. Never cut corners to reach the goal.
   - Reaching the goal the wrong way is a violation.
4. Always keep purpose visible.
   - If the purpose is unclear, stop and clarify before modeling or implementation.

## Modeling rules

0. Model only what the purpose requires.
1. Do not attract irrelevant attributes into the model.
2. Treat models as classes that express only the data required for the problem.
3. Prefer the most generic valid name that still fits the problem scope.

### Data carrier model rules

0. Primary models are self-sufficient.
   - They do not physically depend on another model to exist.
1. Secondary models depend on primary models.
   - They usually reference another model or are nested within one.
2. Relational models connect two primary models.
   - They exist to materialize many-to-many relationships.
   - They should hold references, not unrelated details.
3. Hybrid models are allowed only when necessary.
   - They mix relational behavior with additional details about the relationship.
   - Prefer purity first; use hybrid models only when the business flow truly requires it.

### Operational model rules

0. Integration models are brokers.
1. Processing models are services.
2. Exposure models are exposers.
3. Configuration models are startup, composition, middleware, or entry-point models.

## Simulation rules

0. Simulation is how models interact.
1. A model may act on another model.
2. A model may be acted upon by another model.
3. A model may act on itself.
4. Functions, methods, and routines are simulation mechanisms.
5. Keep simulation inside the purpose and the model boundaries.

## Core principles and non-negotiables

### People-First

0. Build for both users and future maintainers.
1. Favor human understanding over cleverness.
2. Favor systems that mainstream engineers can own, evolve, and rewrite.

### Simplicity

0. Simplicity is mandatory, not optional.
1. Simplicity creates rewritability.
2. Simplicity favors modular monoliths and clear decomposition.

#### Excessive Inheritance

0. Do not use more than one level of inheritance.
1. More than one level is excessive and prohibited, except when vertical versioning of flows absolutely requires it.

#### Entanglement

0. Avoid horizontal entanglement.
   - No Utils.
   - No Commons.
   - No Helpers that pretend to simplify everything.
   - No shared models across independent flows.
   - No shared exceptions or shared validation rules across unrelated flows.
1. Avoid vertical entanglement.
   - No local base components unless they are native or external.
   - No local inheritance chains that create hidden coupling.
2. Prefer duplication over cross-flow entanglement when duplication preserves autonomy.

### Autonomous Components

0. Every component should be self-sufficient.
1. Every component owns its validations, tooling, and utilities in one of its dimensions.
2. Components may depend on injected dependencies, not hidden helpers.
3. Duplication is permitted when it preserves ownership and autonomy.

### No Magic

0. What you see is what you get.
1. No hidden routines.
2. No magical extensions that require chasing references.
3. No runtime tricks that make the system hard to understand.
4. Put validation, exception handling, tracing, security, localization, and behavior in plain sight.

### Rewritability

0. Every system must be easy to understand, modify, and fully rewrite.
1. A Standard-compliant system should be forkable, clonable, buildable, and testable with minimal surprise.
2. No hidden dependencies.
3. No unknown prerequisites.
4. No injected routines that obscure behavior.

### Mono-Micro

0. Build monoliths with a microservice mindset.
1. Every flow should be autonomous enough to be extracted later.
2. Modularize aggressively without splitting prematurely.

### Level 0

0. Code must be understandable by entry-level engineers.
1. Level 0 understanding is the measure of success.
2. If new engineers cannot follow the system, the system is too complex.

### Open Code

0. Prefer code, tooling, platforms, and patterns that are visible and accessible.
1. Do not make source proprietary solely for personal or organizational gain.
2. Exceptions exist only when safety, security, or contractual obligations require it.

### Airplane Mode (Cloud-Foreign)

0. The system should be runnable locally without mandatory cloud dependency.
1. Develop tooling that bridges cloud resources to local stand-ins.
2. Favor local testing, local development, and mocked external systems.

### No Toasters

0. Do not force Standard compliance via style cops or analyzers as the primary mechanism.
1. Teach the Standard person-to-person.
2. Favor conviction and understanding over coercion.
3. AI-assisted coding (including “vibe coding”) is acceptable, 
   provided a human remains actively involved and retains full responsibility for the final code.
4. AI may be used for code reviews and suggestions, but the final authority for approval 
   and merge decisions must always rest with a human.

### Pass Forward

0. The Standard is to be taught at no cost.
1. Do not profiteer from teaching The Standard.
2. Do not use The Standard to belittle others.
3. Pass it forward freely.

### All-In / All-Out

0. The Standard must be embraced fully or rejected fully.
1. Partial adoption is not standardization.
2. Outdated partial adherence is not enough to claim a Standardized system.

### Readability over Optimization

0. When in doubt, choose readability.
1. Unreadable “optimal” software is not truly optimum.

### Last Day

0. Work every day as if it might be your last day on the project.
1. End each engineering day at a good stopping point.
2. Ensure another engineer can pick up the work seamlessly the next day.
3. Apply this to design, development, documentation, and automation.

## Mandatory operating behavior for agents

0. Never skip purpose.
1. Never start with implementation when purpose or models are unclear.
2. Never optimize at the expense of readability.
3. Never recommend hidden shared abstractions as a first move.
4. Never recommend analyzers or “toasters” as a substitute for understanding.
5. Reject vague or chaotic designs.
6. Prefer explicitness, autonomy, and rewritability.
7. Use the language of The Standard consistently.
8. If there is a conflict between generic convention and The Standard, The Standard wins.
