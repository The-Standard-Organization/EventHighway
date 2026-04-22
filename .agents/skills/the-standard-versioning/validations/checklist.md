# The Standard Versioning — Validation Checklist

Run this checklist before releasing a version, creating versioned files, or approving a versioning-related PR.
Each item is binary: PASS or FAIL.

---

## RELEASE VERSIONING

- [ ] **version-001** Release version follows exact format `v1.2.3.4`.
- [ ] **version-002** Model change increments segment 1 and resets segments 2, 3, 4 to zero.
- [ ] **version-003** Service/routine change increments segment 2 and resets segments 3, 4 to zero.
- [ ] **version-004** Bug fix/config change increments segment 3 and resets segment 4 to zero.
- [ ] **version-005** Automated build increments segment 4 only.
- [ ] **version-006** When multiple changes occur, highest-order change wins and lower segments reset.

---

## FILE VERSIONING

- [ ] **version-010** Initial files (V0) exist without version folder.
- [ ] **version-011** New model version creates `Models/.../V{n}/{Entity}V{n}.cs`.
- [ ] **version-012** New service version creates `Services/.../V{n}/{Entity}V{n}Service.cs`.
- [ ] **version-013** Versioned exceptions exist at `Models/.../V{n}/Exceptions/`.
- [ ] **version-014** Earlier-version files remain available (additive, not destructive).

---

## FILE NAMING

- [ ] **File naming** Model files follow pattern `{Entity}V{n}.cs`.
- [ ] **File naming** Service files follow pattern `{Entity}V{n}Service.cs`.
- [ ] **File location** Vn folders are subfolders of original V0 location.

---

## API VERSIONING

- [ ] **version-020** V0 API route is `api/{Resource}`.
- [ ] **version-021** Model version route is `api/V{n}/{Resource}`.
- [ ] **version-022** Model + behavior version route is `api/V{n}.{m}/{Resource}`.
- [ ] **version-023** Route version communicates intended model and behavior pairing.
- [ ] **version-024** API route aligns with underlying model/service pair versioning.

---

## DEPRECATION

- [ ] **version-030** Deprecated APIs expose deprecation metadata (sunset date, warning, migration link).
- [ ] **version-031** Deprecated code marked with `[Obsolete("message")]`.
- [ ] **version-032** Consumers given adequate notice before sunset.
- [ ] **version-033** Deprecation metadata includes sunset date.
- [ ] **version-034** Deprecation metadata includes migration guidance link.

---

## CAPABILITIES

- [ ] **Capabilities** Capabilities endpoint or metadata exists for version discovery.
- [ ] **Capabilities** Supported model versions are discoverable.
- [ ] **Capabilities** Supported behavior versions are discoverable.
- [ ] **Capabilities** Unsupported features are explicitly indicated.

---

## CONSISTENCY

- [ ] **Consistency** No model changes hidden in service segment.
- [ ] **Consistency** No service changes hidden in bug/config segment.
- [ ] **Consistency** Build increment never used to represent code changes.
- [ ] **Consistency** No overwriting of earlier versions.
- [ ] **Consistency** All version information remains discoverable.

---

## RESULT

| Category | PASS / FAIL |
|---|---|
| Release Versioning | |
| File Versioning | |
| File Naming | |
| API Versioning | |
| Deprecation | |
| Capabilities | |
| Consistency | |

**Overall: PASS only when every row is PASS.**