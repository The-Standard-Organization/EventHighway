# The Standard Practices — Anti-Patterns

---

## AP-PRAC-001: Non-Standard Branch Names

**What it is:** Branches that don't follow `users/[username]/[CATEGORY]-[entity]-[action]`.

**Examples:**
```
feature/student-service
bugfix/null-check
student-service-update
main-fix
```

**Why harmful:** Non-Standard branch names make it impossible to understand the scope, owner, and category of work at a glance. Tooling, contribution point tracking, and code review workflows depend on the Standard branch pattern.

**How to fix:**
```
users/hassanhabib/FOUNDATIONS-student-create
users/cjdutoit/PATCH-student-null-check
```

---

## AP-PRAC-002: Committing Without FAIL/PASS Verification

**What it is:** Writing a FAIL commit where the test was never actually run and confirmed as red, or writing a PASS commit where the full suite was never run.

**Example:**
```
git commit -m "[FAIL]: ShouldAddStudentAsync"
# Engineer never ran the test — it may have a typo, compile error, or even pass immediately
```

**Why harmful:** The entire value of TDD is the red/green cycle. A FAIL commit that was never red means the engineer may have written a test that will never actually validate anything. A PASS commit that was never fully green means regressions go undetected.

**How to fix:** Before every FAIL commit: run the test, confirm red. Before every PASS commit: run the full suite, confirm green.

---

## AP-PRAC-003: Bundling Multiple Tests in One FAIL Commit

**What it is:** Committing two or more test implementations in a single FAIL commit.

**Example:**
```
[FAIL]: ShouldAddStudentAsync + ShouldThrowValidationExceptionOnAddIfStudentIsNullAndLogItAsync
```

**Why harmful:** Atomic commits enable bisecting. If something breaks, you need to find exactly which test or implementation caused it. A bundled FAIL commit obscures the cause and makes `git bisect` and `git revert` unpredictable.

**How to fix:** One test per FAIL commit, one implementation unit per PASS commit.

---

## AP-PRAC-004: Sensitive Data Committed to Source Control

**What it is:** Secrets, connection strings, API keys, or passwords appear in committed files.

**Example:**
```json
// appsettings.json — VIOLATION
{
  "ConnectionStrings": {
    "Default": "Server=prod-db.mycompany.com;User Id=admin;Password=SuperSecret123;"
  }
}
```

**Why harmful:** Once committed, a secret is in git history forever — even after deletion. Anyone with repo access (or who ever cloned it) can retrieve it. Secrets in CI pipelines are exposed to anyone who can read workflow logs.

**How to fix:**
1. Immediately rotate the exposed secret.
2. Add the configuration file to `.gitignore`.
3. Use environment variables, Azure Key Vault, AWS Secrets Manager, or GitHub Secrets.
4. Use `appsettings.local.json` (gitignored) for local development.

---

## AP-PRAC-005: Wrong Category Size

**What it is:** Using MAJOR when only 1-2 tests are changed, or MINOR when 5+ tests are changed.

**Examples:**
```
# VIOLATION: MAJOR used for 1 test change
users/hassanhabib/MAJOR FOUNDATIONS-student-add-null-check

# VIOLATION: MINOR used for 6 test changes
users/cjdutoit/MINOR FOUNDATIONS-student-all-validations
```

**Why harmful:** Size categories are used for contribution point calculations and effort estimation. Incorrect sizing distorts measurements and makes contribution tracking meaningless.

**How to fix:** Count the tests being added or modified:
- 1-2 tests → `MINOR`
- 3-4 tests → `MEDIUM`
- 5+ tests → `MAJOR`

---

## AP-PRAC-006: Non-Category PR Titles

**What it is:** PR titles that don't include the Standard category prefix.

**Examples:**
```
Add student service
Fix null check bug
Update student validations
```

**Why harmful:** The PR title is the primary signal for reviewers, contributors, and tooling to understand what category of work is being reviewed. Without it, code reviews become ambiguous and contribution tracking is broken.

**How to fix:**
```
FOUNDATIONS: Student - Add foundation service with CRUD operations
PATCH: Student - Fix null reference in name formatter
MEDIUM FOUNDATIONS: Student - Update validation logic
```

---

## AP-PRAC-007: Pushing Directly to Official Repository

**What it is:** A non-maintainer contributor pushes directly to the official repository branch instead of using a fork.

**Why harmful:** Bypasses the PR review process. Code is never reviewed before reaching the official codebase. Breaks the open-source contribution model that The Standard promotes.

**How to fix:** Fork the repository. Clone your fork. Create a branch on your fork. Open a PR from your fork to the official repository.

---

## AP-PRAC-008: Mixing Categories in One PR

**What it is:** A single PR that contains INFRA changes, DATA changes, and FOUNDATIONS changes.

**Example:**
```
PR: "Add student feature"
Changes:
- Added Student model + migration (DATA)
- Added StorageBroker changes (BROKERS)
- Added StudentService (FOUNDATIONS)
- Added Program.cs registration (INFRA)
```

**Why harmful:** Reviewers cannot focus their review on one category. Regressions are harder to isolate. If the PR needs to be reverted, all work is lost together. Contribution points cannot be accurately assigned.

**How to fix:** Break the work into separate PRs per category. Use feature flags or partial merges if coordination is needed.
