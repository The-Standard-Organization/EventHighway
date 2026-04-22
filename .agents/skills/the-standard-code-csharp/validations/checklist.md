# The Standard Code CSharp — Validation Checklist

Run this checklist on every C# file before committing or approving.
Each item is binary: PASS or FAIL.

---

## FILES

- [ ] **cs-001** File name is PascalCase ending in `.cs`.
- [ ] **cs-002** Partial class files use dot-notation (e.g., `StudentService.Validations.cs`).
- [ ] **cs-003** No partial class files use concatenated naming.
- [ ] **cs-004** No partial class files use underscore separation.

---

## VARIABLES — NAMING

- [ ] **cs-010** All variable names are full and descriptive (no single letters, no abbreviations).
- [ ] **cs-011** All lambda parameters are full names (not `s`, `x`, `e`).
- [ ] **cs-012** Collections use natural plural (no `List`, `Collection`, `Array` suffix).
- [ ] **cs-013** No variable has a type suffix (`Model`, `Obj`, `Entity`, `DTO`).
- [ ] **cs-014** Variables initialized to `null` signal that intent in the name (`noStudent`).
- [ ] **cs-015** Variables initialized to zero signal that intent in the name (`noChangeCount`).

---

## VARIABLES — DECLARATIONS

- [ ] **cs-020** Clear right-side type uses `var`.
- [ ] **cs-021** Unclear right-side type (method return) uses explicit type declaration.
- [ ] **cs-023** Single-property objects assign post-construction. Multi-property objects use initializer.

---

## VARIABLES — ORGANIZATION

- [ ] **cs-030** No variable declaration exceeds 120 characters without a break after `=`.
- [ ] **cs-031** Multi-line declarations have blank lines before AND after.
- [ ] **cs-032** Consecutive single-line declarations have no blank lines between them.

---

## METHODS — NAMING

- [ ] **cs-040** Every method name contains a verb.
- [ ] **cs-041** All async methods end with `Async`.
- [ ] **cs-042** All input parameters are fully qualified with entity context.
- [ ] **cs-043** Action-specific methods identify the property: `ByName`, `ById`.
- [ ] **cs-044** Literals passed as arguments use named aliases.

---

## METHODS — ORGANIZATION

- [ ] **cs-050** One-line methods use fat arrow `=>`.
- [ ] **cs-051** Multi-line methods use scope body `{ }`.
- [ ] **cs-052** Fat arrow methods exceeding 120 chars break after `=>` with extra tab.
- [ ] **cs-053** Chaining methods use scope body (not fat arrow).
- [ ] **cs-054** Multi-line methods have blank line before `return`.
- [ ] **cs-055** Stacked calls under 120 chars have no blank lines (unless final is return).
- [ ] **cs-056** Calls exceeding 120 chars have blank line separation.
- [ ] **cs-057** Method declarations exceeding 120 chars break parameters to next line.
- [ ] **cs-058** Broken parameters each occupy their own line.
- [ ] **cs-059** Chained calls: first on same line as subject, subsequent calls +1 tab.

---

## CLASSES — NAMING

- [ ] **cs-060** Model classes have no type suffix.
- [ ] **cs-061** Service classes are singular PascalCase + `Service`.
- [ ] **cs-062** Broker classes are singular PascalCase + `Broker`.
- [ ] **cs-063** Controller classes are plural entity + `Controller`.

---

## CLASSES — FIELDS

- [ ] **cs-070** All class fields are camelCase (no underscore, no PascalCase).
- [ ] **cs-072** Private fields are always accessed via `this.`.

---

## CLASSES — INSTANTIATION

- [ ] **cs-080** Literal arguments use named aliases.
- [ ] **cs-082** No target-typed `new()` usage.
- [ ] **cs-083** Initializer property order matches class declaration order.

---

## COMMENTS

- [ ] **cs-090** Copyright block follows Standard format exactly.
- [ ] **cs-091** No XML-style copyright.
- [ ] **cs-092** No block-comment copyright.

---

## RESULT

| Category | PASS / FAIL |
|---|---|
| Files | |
| Variables — Naming | |
| Variables — Declarations | |
| Variables — Organization | |
| Methods — Naming | |
| Methods — Organization | |
| Classes — Naming | |
| Classes — Fields | |
| Classes — Instantiation | |
| Comments | |

**Overall: PASS only when every row is PASS.**
