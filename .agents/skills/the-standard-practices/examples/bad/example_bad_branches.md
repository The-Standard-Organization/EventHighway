# Bad Example: Branch Name Violations

## Violation 1 — Missing username (prac-010, prac-011)
```
# BAD: No users/ prefix, no username
FOUNDATIONS-student-add
feature/student-service
```

**Fix:**
```
users/hassanhabib/FOUNDATIONS-student-add
```

---

## Violation 2 — Non-Standard category (prac-012)
```
# BAD: Uses non-Standard categories
users/hassanhabib/feature-student-service
users/hassanhabib/bugfix-student-null
users/hassanhabib/refactor-student-validation
```

**Fix:**
```
users/hassanhabib/FOUNDATIONS-student-create
users/hassanhabib/PATCH-student-null-check
users/hassanhabib/MINOR FOUNDATIONS-student-validations-refactor
```

---

## Violation 3 — No entity or action (prac-013, prac-014)
```
# BAD: Missing entity and action
users/hassanhabib/FOUNDATIONS
users/hassanhabib/DATA-fix
```

**Fix:**
```
users/hassanhabib/FOUNDATIONS-student-add
users/hassanhabib/MINOR DATA-course-add-credit-hours
```

---

## Violation 4 — Wrong size suffix (prac-021)
```
# BAD: Using MAJOR when only 1 test is modified
users/hassanhabib/MAJOR FOUNDATIONS-student-add-null-check
```

**Fix:** `MAJOR` requires 5+ tests. For 1 test, use `MINOR`:
```
users/hassanhabib/MINOR FOUNDATIONS-student-add-null-check
```

---

## Violation 5 — Camel or lowercase category (prac-012)
```
# BAD: Category not in expected casing/format
users/hassanhabib/foundations-student-add
users/hassanhabib/Foundation-student-add
```

**Fix:** Category must be UPPER-CASE:
```
users/hassanhabib/FOUNDATIONS-student-add
```
