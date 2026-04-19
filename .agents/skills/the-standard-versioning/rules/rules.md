# The Standard Versioning — Rules

## RELEASE VERSIONING (v1.2.3.4 Format)

**version-001** [ERROR] All releases MUST follow the v1.2.3.4 format (major.minor.patch.build).
**version-002** [ERROR] Model changes MUST increment major version and reset all lower segments to 0.
**version-003** [ERROR] Service changes MUST increment minor version and reset patch and build to 0.
**version-004** [ERROR] Bug fixes or configuration changes MUST increment patch version and reset build to 0.
**version-005** [ERROR] Build/pipeline changes MUST increment build version only.
**version-006** [ERROR] When multiple changes occur, the highest-order change determines version increment (model > service > patch > build).

## FILE VERSIONING (Vn Folder Structure)

**version-010** [ERROR] Default version is V0 — version is inferred and MUST NOT be included in the filename.
**version-011** [ERROR] Model changes MUST require creation of a new Vn folder (e.g., V1, V2).
**version-012** [ERROR] Service changes MUST require creation of a new Vn folder.
**version-013** [ERROR] Previous version files MUST NOT be overwritten — always create new versioned folders.
**version-014** [ERROR] File versioning MUST be applied to maintain backward compatibility.

## API VERSIONING (Route Versioning)

**version-020** [ERROR] V0 APIs have no version prefix in the route.
**version-021** [ERROR] Model version changes MUST use Vn prefix in routes (e.g., /V1/students).
**version-022** [ERROR] Service version changes MUST use Vn.m prefix in routes (e.g., /V1.1/students).
**version-023** [ERROR] API routes are immutable — existing routes MUST NOT be modified or removed.
**version-024** [ERROR] API versioning MUST reflect the underlying model or service version.

## DEPRECATION

**version-030** [ERROR] Deprecated APIs MUST include sunset metadata indicating when they will be removed.
**version-031** [ERROR] Deprecated code MUST include warning messages to notify consumers.
**version-032** [ERROR] Deprecated code MUST use the `[Obsolete]` attribute in C# or equivalent in other languages.
**version-033** [ERROR] Deprecation period MUST provide adequate time for consumers to migrate (recommended minimum: 6 months).
**version-034** [ERROR] Deprecation notices MUST be communicated through documentation, API responses, and release notes.
