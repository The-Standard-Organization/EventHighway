# The Standard Practices — Rules

## CONTRIBUTIONS

**prac-001** [ERROR] All contributions must be structured through the Standard workflow: fork → branch → commit → PR.
**prac-002** [ERROR] Contributions must be intentional — no uncommitted work-in-progress, no "WIP" commits reaching main.
**prac-003** [ERROR] Sensitive configuration (secrets, API keys, connection strings) must never be committed to source control in plain text.

## BRANCH NAMING

**prac-010** [ERROR] Branch names must follow: `users/[username]/[CATEGORY]-[entity]-[action]`.
**prac-011** [ERROR] `[username]` must be the GitHub username of the contributor.
**prac-012** [ERROR] `[CATEGORY]` must be from the Standard category list (INFRA, DATA, BROKERS, FOUNDATIONS, etc.).
**prac-013** [ERROR] `[entity]` must identify the entity or resource being worked on.
**prac-014** [ERROR] `[action]` must describe the specific action being performed.
**prac-015** [ERROR] Branch name parts must be separated by `/` between `users/username/` and the category block.

## CATEGORY SELECTION

**prac-020** [ERROR] Category must accurately reflect the type of work:
  - `INFRA` / `MAJOR|MEDIUM|MINOR INFRA`: project setup or configuration changes
  - `DATA` / `MAJOR|MEDIUM|MINOR DATA`: data model creation or modification
  - `MIGRATION` / `MAJOR|MEDIUM|MINOR MIGRATION`: data transformation or movement
  - `BROKERS` / `MAJOR|MEDIUM|MINOR BROKERS`: broker creation or modification
  - `FOUNDATIONS` / `MAJOR|MEDIUM|MINOR FOUNDATIONS`: foundation service creation or modification
  - `PROCESSINGS` / `MAJOR|MEDIUM|MINOR PROCESSINGS`: processing service creation or modification
  - `ORCHESTRATIONS` / `MAJOR|MEDIUM|MINOR ORCHESTRATIONS`: orchestration service creation or modification
  - `COORDINATIONS` / `MAJOR|MEDIUM|MINOR COORDINATIONS`: coordination service creation or modification
  - `MANAGEMENTS` / `MAJOR|MEDIUM|MINOR MANAGEMENTS`: management service creation or modification
  - `AGGREGATIONS` / `MAJOR|MEDIUM|MINOR AGGREGATIONS`: aggregation service creation or modification
  - `CONTROLLERS` / `MAJOR|MEDIUM|MINOR CONTROLLERS`: controller creation or modification
  - `CLIENTS`: creating a client library
  - `PROVIDERS`: creating a SPAL provider
  - `EXPOSERS` / `MAJOR|MEDIUM|MINOR EXPOSERS`: other exposer work (Program.cs, etc.)
  - `VIEWS` / `MAJOR|MEDIUM|MINOR VIEWS`: view service creation or modification
  - `COMPONENTS` / `MAJOR|MEDIUM|MINOR COMPONENTS`: UI component creation or modification
  - `PAGES` / `MAJOR|MEDIUM|MINOR PAGES`: page creation or modification
  - `FEATURES`: end-to-end feature (all layers)
  - `PATCH`: small non-TDD fixes
  - `RELEASES`: release infrastructure

**prac-021** [ERROR] Size suffix must reflect test count: MAJOR = 5+, MEDIUM = 3-4, MINOR = 1-2.
**prac-022** [WARNING] When unsure between MAJOR/MEDIUM/MINOR, choose based on the number of tests added or modified.

## TDD COMMIT DISCIPLINE

**prac-030** [ERROR] Every TDD commit follows FAIL/PASS discipline.
**prac-031** [ERROR] FAIL commit prefix: test is written and verified to fail. Commit message: `[FAIL]: ...`
**prac-032** [ERROR] PASS commit prefix: implementation written, all tests passing. Commit message: `[PASS]: ...`
**prac-033** [ERROR] Never commit a FAIL without running the test and confirming it shows red.
**prac-034** [ERROR] Never commit a PASS without running the full relevant suite and confirming all green.
**prac-035** [ERROR] FAIL and PASS commits must be atomic — one test per FAIL, one implementation unit per PASS.

## PULL REQUESTS

**prac-040** [ERROR] Pull request titles must include the Standard category: `[CATEGORY]: [Entity] - [Description]`.
**prac-041** [ERROR] Pull requests must not include sensitive data in the diff.
**prac-042** [ERROR] Pull requests must show passing CI (build + tests) before merge.
**prac-043** [ERROR] A human reviewer must approve before merging. AI may suggest but not merge.
**prac-044** [WARNING] Pull requests should be focused on one category of work — avoid mixing FEATURE + INFRA in one PR.

## CONFIGURATION HANDLING

**prac-050** [ERROR] Never hard-code environment-specific values (URLs, connection strings, API keys) in source code.
**prac-051** [ERROR] Use environment variables or a secrets management system for all sensitive values.
**prac-052** [ERROR] Local development configuration files (`.env`, `appsettings.local.json`) must be in `.gitignore`.
**prac-053** [ERROR] GitHub Actions CI/CD must use repository secrets, not plain text values in workflow files.
**prac-054** [ERROR] Configuration model classes must not contain default hard-coded production values.

## FORKING WORKFLOW

**prac-060** [ERROR] Open-source contributions must use the forking workflow: fork → local clone → branch → PR from fork.
**prac-061** [ERROR] Contributors must never push directly to the official repository unless they are the maintainer.
**prac-062** [WARNING] Pull requests from forks must be reviewed and build-verified before the maintainer merges.
