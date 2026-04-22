# Bad Example: Commit Message Violations

## Violation 1 — No FAIL/PASS prefix (prac-030, prac-031, prac-032)
```
# BAD: No discipline prefix
git commit -m "add student null check test"
git commit -m "implement null student validation"
```

**Fix:**
```
[FAIL]: ShouldThrowValidationExceptionOnAddIfStudentIsNullAndLogItAsync
[PASS]: AddStudentAsync — null student validation
```

---

## Violation 2 — FAIL commit without verified failure (prac-033)
```
# BAD: Developer wrote test and committed without running it
[FAIL]: ShouldAddStudentAsync
```
*...but the test was never run. It might be broken, have a typo, or pass immediately.*

**Fix:** Always run the test before committing a FAIL. Confirm the test runner shows RED.

---

## Violation 3 — PASS commit with failing tests (prac-034)
```
# BAD: Committed [PASS] but other tests in the suite are failing
[PASS]: AddStudentAsync — invalid student fields validation
```
*Other tests broke due to the new validation logic, but the developer didn't run the full suite.*

**Fix:** Run the FULL relevant test suite before any PASS commit. All tests must be GREEN.

---

## Violation 4 — Multiple tests in one FAIL commit (prac-035)
```
# BAD: Two tests committed in one FAIL commit
[FAIL]: ShouldAddStudentAsync + ShouldThrowValidationExceptionOnAddIfStudentIsNullAndLogItAsync
```

**Fix:** One test per FAIL commit:
```
[FAIL]: ShouldAddStudentAsync
```
Then after PASS:
```
[FAIL]: ShouldThrowValidationExceptionOnAddIfStudentIsNullAndLogItAsync
```

---

## Violation 5 — Vague commit messages
```
# BAD: No context, not machine-readable
git commit -m "fix"
git commit -m "working on student service"
git commit -m "WIP"
```

**Fix:** Commit messages must be descriptive and, for TDD work, use FAIL/PASS format.

---

## Violation 6 — Secrets in a commit (prac-003)
```
# BAD: Committed appsettings.json with real connection string
appsettings.json: "ConnectionStrings": { "Default": "Server=prod.db;Password=real_password" }
```

**Fix:**
1. Remove secret from code immediately.
2. Add `appsettings.local.json` to `.gitignore`.
3. Rotate the compromised secret.
4. Use environment variables or Azure Key Vault.
