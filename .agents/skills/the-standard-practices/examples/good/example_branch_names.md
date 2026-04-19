# Good Example: Branch Names

## Pattern: `users/[username]/[CATEGORY]-[entity]-[action]`

---

### Creating a Foundation Service for Student
```
users/hassanhabib/FOUNDATIONS-student-add
```
- username: `hassanhabib`
- category: `FOUNDATIONS` (creating a new foundation service)
- entity: `student`
- action: `add`

---

### Modifying an existing Foundation Service (5+ tests changed)
```
users/hassanhabib/MAJOR FOUNDATIONS-student-validations
```
- username: `hassanhabib`
- category: `MAJOR FOUNDATIONS` (5+ test changes)
- entity: `student`
- action: `validations`

---

### Creating a Data Model with Migration
```
users/cjdutoit/DATA-course-create
```
- username: `cjdutoit`
- category: `DATA` (new data model + migration)
- entity: `course`
- action: `create`

---

### Modifying an existing data model (2 property changes)
```
users/cjdutoit/MINOR DATA-course-add-description
```
- Size MINOR because only 1-2 property changes.

---

### Creating a REST Controller
```
users/hassanhabib/CONTROLLERS-students-create
```
- category: `CONTROLLERS` (new controller)
- entity: `students` (plural because controllers are plural)
- action: `create`

---

### Initial infrastructure setup
```
users/hassanhabib/INFRA-project-setup
```
- category: `INFRA`
- entity: `project`
- action: `setup`

---

### Small bug fix (non-TDD)
```
users/cjdutoit/PATCH-student-name-null-check
```
- category: `PATCH` (small non-TDD fix)

---

### End-to-end feature
```
users/hassanhabib/FEATURES-enrollment-complete
```
- category: `FEATURES` (full feature, all layers)
