# The Standard Core — Rules

## Rule Set

### THEORY

**core-001** [ERROR] Every system must be understood through three lenses: Purpose, Dependency, Exposure (Tri-Nature).
**core-002** [ERROR] The Tri-Nature pattern is fractal — it repeats at system, sub-system, service, validation, and exposure level.
**core-003** [WARNING] Design every part with awareness that it will become someone else's dependency or exposure.

### ENGINEERING SEQUENCE

**core-010** [ERROR] The engineering sequence is fixed: Purposing → Modeling → Simulation. Never skip or reorder.
**core-011** [ERROR] Never start implementation when purpose is unclear.
**core-012** [ERROR] Never start modeling when the problem observation has not been completed.

### PURPOSING

**core-020** [ERROR] Start every design session with observation: identify the real blocker, constraint, or unmet need.
**core-021** [ERROR] Articulate the problem clearly before proposing a solution.
**core-022** [ERROR] Solutioning must honor: readability, configurability, longevity, optimization, and maintainability.
**core-023** [ERROR] Reaching the goal the wrong way is a violation — never cut corners.
**core-024** [ERROR] If purpose is unclear, stop and clarify before proceeding to modeling or implementation.

### MODELING

**core-030** [ERROR] Model only what the purpose requires — no speculative attributes.
**core-031** [ERROR] Do not attract irrelevant attributes into the model.
**core-032** [ERROR] Prefer the most generic valid name that still fits the problem scope.
**core-033** [ERROR] Primary models must be self-sufficient — they must not physically depend on another model to exist.
**core-034** [ERROR] Secondary models must reference or nest within a primary model.
**core-035** [ERROR] Relational models connect exactly two primary models; they must not contain unrelated details.
**core-036** [WARNING] Hybrid models are allowed only when the business flow truly requires mixing relational + additional details.
**core-037** [ERROR] Integration models = brokers. Processing models = services. Exposure models = exposers. Configuration models = startup/middleware.

### SIMULATION

**core-040** [ERROR] Simulation must stay inside the purpose and model boundaries.
**core-041** [ERROR] Functions, methods, and routines are the simulation mechanisms — they must not cross model ownership.

### SIMPLICITY

**core-050** [ERROR] Simplicity is mandatory, not optional.
**core-051** [ERROR] Do not use more than one level of inheritance (excessive inheritance is prohibited).
**core-052** [ERROR] No Utils, no Commons, no Helpers — these create horizontal entanglement.
**core-053** [ERROR] No shared models across independent flows.
**core-054** [ERROR] No local base components that create hidden coupling (vertical entanglement).
**core-055** [WARNING] Prefer duplication over cross-flow entanglement when duplication preserves autonomy.

### AUTONOMOUS COMPONENTS

**core-060** [ERROR] Every component must be self-sufficient — it owns its validations, tooling, and utilities.
**core-061** [ERROR] Components may depend on injected dependencies, not hidden helpers.
**core-062** [WARNING] Duplication is permitted when it preserves ownership and autonomy.

### NO MAGIC

**core-070** [ERROR] No hidden routines.
**core-071** [ERROR] No magical extensions that require chasing references to understand.
**core-072** [ERROR] No runtime tricks that make the system hard to understand.
**core-073** [ERROR] Validation, exception handling, tracing, security, and localization must be in plain sight.

### REWRITABILITY

**core-080** [ERROR] Every system must be easy to understand, modify, and fully rewrite.
**core-081** [ERROR] No hidden dependencies.
**core-082** [ERROR] No unknown prerequisites.
**core-083** [ERROR] No injected routines that obscure behavior.

### LEVEL 0

**core-090** [ERROR] Code must be understandable by entry-level engineers.
**core-091** [ERROR] If new engineers cannot follow the system, the system is too complex.

### ALL-IN / ALL-OUT

**core-100** [ERROR] The Standard must be embraced fully or rejected fully — partial adoption is not standardization.
**core-101** [ERROR] Outdated partial adherence does not constitute a Standardized system.

### READABILITY OVER OPTIMIZATION

**core-110** [ERROR] When in doubt, choose readability over performance.
**core-111** [ERROR] Unreadable "optimal" software is not truly optimum.

### AIRPLANE MODE

**core-120** [ERROR] The system must be runnable locally without mandatory cloud dependency.
**core-121** [WARNING] Develop tooling that bridges cloud resources to local stand-ins.

### NO TOASTERS

**core-130** [ERROR] Do not enforce Standard compliance via style cops or analyzers as the primary mechanism.
**core-131** [ERROR] Teach The Standard person-to-person, not through coercion tools.

### LAST DAY

**core-140** [WARNING] Work every day as if it might be your last day on the project.
**core-141** [WARNING] End each engineering day at a good stopping point so another engineer can continue seamlessly.
