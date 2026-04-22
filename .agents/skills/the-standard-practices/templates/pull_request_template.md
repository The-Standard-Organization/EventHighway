# Pull Request Template

## Title Format
```
[CATEGORY]: [Entity] - [Short description of the change]
```

**Examples:**
```
FOUNDATIONS: Student - Add foundation service with Add, Retrieve, Modify, Remove
MAJOR FOUNDATIONS: Student - Update validation logic for date fields
MINOR DATA: Course - Add credit hours property
CONTROLLERS: Students - Create REST controller
PATCH: Student - Fix null reference in name formatter
```

---

## PR Description Template

### What does this PR do?
[One paragraph describing the change, the purpose it serves, and the layer it touches.]

### Category
- [ ] INFRA
- [ ] DATA / MIGRATION
- [ ] BROKERS
- [ ] FOUNDATIONS
- [ ] PROCESSINGS
- [ ] ORCHESTRATIONS
- [ ] AGGREGATIONS
- [ ] CONTROLLERS
- [ ] EXPOSERS
- [ ] VIEWS / COMPONENTS / PAGES
- [ ] FEATURES
- [ ] PATCH
- [ ] RELEASES

### Size
- [ ] MAJOR (5+ tests added/modified)
- [ ] MEDIUM (3-4 tests added/modified)
- [ ] MINOR (1-2 tests added/modified)
- [ ] N/A (INFRA, PATCH, etc.)

### Branch name follows Standard convention
- [ ] `users/[username]/[CATEGORY]-[entity]-[action]`

### TDD compliance
- [ ] All FAIL commits have been verified to fail (test runner was red)
- [ ] All PASS commits have been verified to pass (full suite was green)
- [ ] Commit history follows FAIL/PASS alternating pattern

### No sensitive data
- [ ] No secrets, API keys, or connection strings in the diff
- [ ] `.gitignore` covers local configuration files

### Quality gates
- [ ] CI build passes
- [ ] All tests pass
- [ ] Code follows The Standard Code CSharp rules
- [ ] Architecture follows The Standard Architecture rules

### Related issues
Closes #[issue number]

---

## Reviewer Checklist

- [ ] PR title follows `[CATEGORY]: [Entity] - [Description]`
- [ ] Branch name follows `users/[username]/[CATEGORY]-[entity]-[action]`
- [ ] Commit history shows FAIL/PASS pattern
- [ ] No sensitive data in diff
- [ ] CI is green
- [ ] Code complies with The Standard C# and Architecture rules
- [ ] Tests cover happy path + validation + exception scenarios
