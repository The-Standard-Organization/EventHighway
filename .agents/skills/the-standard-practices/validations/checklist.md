# The Standard Practices — Validation Checklist

Run this checklist when reviewing a PR, creating a branch, or making commits.

---

## BRANCH NAMING

- [ ] **prac-010** Branch name follows `users/[username]/[CATEGORY]-[entity]-[action]`.
- [ ] **prac-011** Username is the contributor's GitHub username.
- [ ] **prac-012** Category is from the Standard category list and is UPPER-CASE.
- [ ] **prac-013** Entity is identified in the branch name.
- [ ] **prac-014** Action is identified in the branch name.
- [ ] **prac-021** Size suffix (MAJOR/MEDIUM/MINOR) reflects actual test count change.

---

## TDD COMMIT DISCIPLINE

- [ ] **prac-030** TDD commits use FAIL/PASS pattern.
- [ ] **prac-031** Each `[FAIL]` commit contains exactly one failing test.
- [ ] **prac-032** Each `[PASS]` commit contains exactly one implementation unit.
- [ ] **prac-033** FAIL commits were preceded by a verified red test run.
- [ ] **prac-034** PASS commits were preceded by a verified green full suite run.
- [ ] **prac-035** No FAIL commit contains multiple tests. No PASS commit contains multiple unrelated implementation units.

---

## PULL REQUESTS

- [ ] **prac-040** PR title follows `[CATEGORY]: [Entity] - [Description]`.
- [ ] **prac-041** Diff contains no sensitive data (secrets, API keys, passwords).
- [ ] **prac-042** CI build and all tests pass.
- [ ] **prac-043** A human reviewer has approved (not AI-only approval).
- [ ] **prac-044** PR is focused on one category of work (no mixing).

---

## CONFIGURATION

- [ ] **prac-050** No environment-specific values hard-coded in source files.
- [ ] **prac-051** Sensitive values are in environment variables or secrets management.
- [ ] **prac-052** Local config files are in `.gitignore`.
- [ ] **prac-053** CI/CD workflows use repository secrets.
- [ ] **prac-054** No hard-coded production defaults in configuration model classes.

---

## FORKING WORKFLOW (Open Source)

- [ ] **prac-060** Contribution comes from a fork, not a direct branch on the official repo.
- [ ] **prac-061** Contributor did not push directly to the official repository.
- [ ] **prac-062** PR from fork has been reviewed and build is verified before merge.

---

## RESULT

| Category | PASS / FAIL |
|---|---|
| Branch Naming | |
| TDD Commit Discipline | |
| Pull Requests | |
| Configuration | |
| Forking Workflow | |

**Overall: PASS only when every row is PASS.**
