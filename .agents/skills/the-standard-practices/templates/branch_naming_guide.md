# Branch Naming Guide

## Pattern

```
users/[username]/[CATEGORY]-[entity]-[action]
```

## Fields

| Field | Description | Example |
|---|---|---|
| `username` | Your GitHub username | `hassanhabib`, `cjdutoit` |
| `CATEGORY` | The Standard category (UPPER-CASE) | `FOUNDATIONS`, `MAJOR DATA` |
| `entity` | The entity or resource you are working on | `student`, `course`, `enrollment` |
| `action` | The specific action being performed | `add`, `create`, `validations`, `setup` |

---

## Complete Category Reference

| Category | When to Use | Size Required |
|---|---|---|
| `INFRA` | Initial project setup, build scripts | No |
| `MAJOR INFRA` | Major configuration/infrastructure changes | Yes |
| `MEDIUM INFRA` | Medium configuration changes | Yes |
| `MINOR INFRA` | Minor configuration changes | Yes |
| `PROVISIONS` | Provision scripts | No |
| `RELEASES` | Release infrastructure | No |
| `DATA` | New data model + migration | No |
| `MAJOR DATA` | 5+ property changes to model | Yes |
| `MEDIUM DATA` | 3-4 property changes to model | Yes |
| `MINOR DATA` | 1-2 property changes to model | Yes |
| `MIGRATION` | New data migration/transformation | No |
| `MAJOR/MEDIUM/MINOR MIGRATION` | Modifying existing migration | Yes |
| `BROKERS` | New broker | No |
| `MAJOR/MEDIUM/MINOR BROKERS` | Modifying existing broker (test count) | Yes |
| `FOUNDATIONS` | New foundation service | No |
| `MAJOR FOUNDATIONS` | Modifying foundation service (5+ tests) | Yes |
| `MEDIUM FOUNDATIONS` | Modifying foundation service (3-4 tests) | Yes |
| `MINOR FOUNDATIONS` | Modifying foundation service (1-2 tests) | Yes |
| `PROCESSINGS` | New processing service | No |
| `MAJOR/MEDIUM/MINOR PROCESSINGS` | Modifying processing service | Yes |
| `ORCHESTRATIONS` | New orchestration service | No |
| `MAJOR/MEDIUM/MINOR ORCHESTRATIONS` | Modifying orchestration service | Yes |
| `COORDINATIONS` | New coordination service | No |
| `MAJOR/MEDIUM/MINOR COORDINATIONS` | Modifying coordination service | Yes |
| `MANAGEMENTS` | New management service | No |
| `MAJOR/MEDIUM/MINOR MANAGEMENTS` | Modifying management service | Yes |
| `AGGREGATIONS` | New aggregation service | No |
| `MAJOR/MEDIUM/MINOR AGGREGATIONS` | Modifying aggregation service | Yes |
| `CONTROLLERS` | New controller | No |
| `MAJOR/MEDIUM/MINOR CONTROLLERS` | Modifying controller | Yes |
| `CLIENTS` | Creating a client library | No |
| `MAJOR/MEDIUM/MINOR CLIENTS` | Modifying client library | Yes |
| `PROVIDERS` | Creating a SPAL provider | No |
| `EXPOSERS` | Other exposers (Program.cs) | No |
| `MAJOR/MEDIUM/MINOR EXPOSERS` | Modifying other exposers | Yes |
| `VIEWS` | New view service | No |
| `MAJOR/MEDIUM/MINOR VIEWS` | Modifying view service | Yes |
| `COMPONENTS` | New UI component | No |
| `MAJOR/MEDIUM/MINOR COMPONENTS` | Modifying UI component | Yes |
| `PAGES` | New page | No |
| `MAJOR/MEDIUM/MINOR PAGES` | Modifying page | Yes |
| `FEATURES` | Full end-to-end feature (all layers) | No |
| `PATCH` | Small non-TDD fix | No |
| `RELEASES` | Release artifacts | No |

## Size Reference

| Size | Test Count |
|---|---|
| `MAJOR` | 5+ tests added or modified |
| `MEDIUM` | 3-4 tests added or modified |
| `MINOR` | 1-2 tests added or modified |

## Quick Examples

```bash
# New foundation service
git checkout -b users/hassanhabib/FOUNDATIONS-student-create

# Modifying foundation service — 3 tests updated
git checkout -b users/cjdutoit/MEDIUM FOUNDATIONS-student-validations-update

# New data model
git checkout -b users/hassanhabib/DATA-course-create

# Adding 1 property to model
git checkout -b users/cjdutoit/MINOR DATA-student-add-email

# Infrastructure setup
git checkout -b users/hassanhabib/INFRA-project-initial-setup

# Small bug fix
git checkout -b users/cjdutoit/PATCH-student-date-null-fix

# Full feature
git checkout -b users/hassanhabib/FEATURES-enrollment-add-complete
```
