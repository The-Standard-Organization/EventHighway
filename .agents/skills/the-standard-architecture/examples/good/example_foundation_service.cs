// ---------------------------------------------------------------
// Copyright (c) Coalition of the Good-Hearted Engineers
// FREE TO USE TO CONNECT THE WORLD
// ---------------------------------------------------------------

// GOOD EXAMPLE: Standard-compliant Foundation Service
// Demonstrates: business language, single entity, validates before delegating,
// circuit-breaking, continuous validation, exception categorization, logging.

using System;
using System.Threading.Tasks;
using Xeptions;

namespace MyProject.Services.Foundations.Students
{
    // arch-022: Business language (Add, Retrieve, Modify, Remove)
    // arch-020: Pure-primitive — returns Student, takes Student
    // arch-021: Single entity (Student only)
    public partial class StudentService : IStudentService
    {
        private readonly IStorageBroker storageBroker;
        private readonly ILoggingBroker loggingBroker;

        public StudentService(
            IStorageBroker storageBroker,
            ILoggingBroker loggingBroker)
        {
            this.storageBroker = storageBroker;
            this.loggingBroker = loggingBroker;
        }

        // arch-022: Business language
        public ValueTask<Student> AddStudentAsync(Student student) =>
            TryCatch(async () =>
            {
                // arch-023: Validate before delegating
                ValidateStudentOnAdd(student);

                return await this.storageBroker.InsertStudentAsync(student);
            });

        // arch-015: Asynchronization Abstraction — method returns ValueTask<IQueryable<Student>>
        // even though the underlying call is not a network round-trip. The public contract
        // must be uniformly async so callers never need to change if the implementation evolves.
        public ValueTask<IQueryable<Student>> RetrieveAllStudentsAsync() =>
            TryCatch(async () =>
                await this.storageBroker.SelectAllStudentsAsync());

        public ValueTask<Student> RetrieveStudentByIdAsync(Guid studentId) =>
            TryCatch(async () =>
            {
                // arch-024: Circuit-breaking — null id stops immediately
                ValidateStudentId(studentId);

                Student maybeStudent =
                    await this.storageBroker.SelectStudentByIdAsync(studentId);

                // arch-026: External validation — check existence
                ValidateStorageStudent(maybeStudent, studentId);

                return maybeStudent;
            });

        public ValueTask<Student> ModifyStudentAsync(Student student) =>
            TryCatch(async () =>
            {
                ValidateStudentOnModify(student);

                Student maybeStudent =
                    await this.storageBroker.SelectStudentByIdAsync(student.Id);

                // arch-026: External validation
                ValidateStorageStudent(maybeStudent, student.Id);
                ValidateAgainstStorageStudentOnModify(inputStudent: student, storageStudent: maybeStudent);

                return await this.storageBroker.UpdateStudentAsync(student);
            });

        public ValueTask<Student> RemoveStudentByIdAsync(Guid studentId) =>
            TryCatch(async () =>
            {
                ValidateStudentId(studentId);

                Student maybeStudent =
                    await this.storageBroker.SelectStudentByIdAsync(studentId);

                ValidateStorageStudent(maybeStudent, studentId);

                return await this.storageBroker.DeleteStudentAsync(maybeStudent);
            });
    }
}

// ---------------------------------------------------------------
// Validations partial (StudentService.Validations.cs)
// ---------------------------------------------------------------

namespace MyProject.Services.Foundations.Students
{
    public partial class StudentService
    {
        private void ValidateStudentOnAdd(Student student)
        {
            // arch-024: Circuit-breaking — null entity stops immediately
            ValidateStudentIsNotNull(student);

            // arch-025, arch-028: Continuous validation — collect all field errors
            Validate(
                (Rule: IsInvalid(student.Id), Parameter: nameof(Student.Id)),
                (Rule: IsInvalid(student.Name), Parameter: nameof(Student.Name)),
                (Rule: IsInvalid(student.CreatedDate), Parameter: nameof(Student.CreatedDate)),
                (Rule: IsInvalid(student.UpdatedDate), Parameter: nameof(Student.UpdatedDate)),
                (Rule: IsNotSame(student.CreatedDate, student.UpdatedDate, nameof(Student.UpdatedDate)),
                    Parameter: nameof(Student.UpdatedDate)),
                (Rule: IsNotRecent(student.CreatedDate), Parameter: nameof(Student.CreatedDate)));
        }

        private static void ValidateStudentIsNotNull(Student student)
        {
            if (student is null)
            {
                throw new NullStudentException(message: "Student is null.");
            }
        }

        private static dynamic IsInvalid(Guid id) => new
        {
            Condition = id == Guid.Empty,
            Message = "Id is required"
        };

        private static dynamic IsInvalid(string text) => new
        {
            Condition = string.IsNullOrWhiteSpace(text),
            Message = "Text is required"
        };

        private static dynamic IsInvalid(DateTimeOffset date) => new
        {
            Condition = date == default,
            Message = "Date is required"
        };

        private static dynamic IsNotSame(
            DateTimeOffset firstDate,
            DateTimeOffset secondDate,
            string secondDateName) => new
        {
            Condition = firstDate != secondDate,
            Message = $"Date is not the same as {secondDateName}"
        };

        private dynamic IsNotRecent(DateTimeOffset date)
        {
            var (isNotRecent, startDate, endDate) = IsDateNotRecent(date);

            return new
            {
                Condition = isNotRecent,
                Message = $"Date is not recent. Expected a value between {startDate} and {endDate}"
            };
        }

        // arch-028: Upsertable continuous validation — collects ALL errors before throwing
        private static void Validate(params (dynamic Rule, string Parameter)[] validations)
        {
            var invalidStudentException = new InvalidStudentException(
                message: "Invalid student. Please correct the errors and try again.");

            foreach ((dynamic rule, string parameter) in validations)
            {
                if (rule.Condition)
                {
                    invalidStudentException.UpsertDataList(
                        key: parameter,
                        value: rule.Message);
                }
            }

            invalidStudentException.ThrowIfContainsErrors();
        }
    }
}

// ---------------------------------------------------------------
// Exception Handling partial (StudentService.Exceptions.cs)
// ---------------------------------------------------------------

namespace MyProject.Services.Foundations.Students
{
    public partial class StudentService
    {
        // arch-029: Catch, localize, categorize, and log all broker exceptions
        private delegate ValueTask<Student> ReturningStudentFunction();

        private async ValueTask<Student> TryCatch(ReturningStudentFunction returningStudentFunction)
        {
            try
            {
                return await returningStudentFunction();
            }
            catch (NullStudentException nullStudentException)
            {
                throw await CreateAndLogValidationException(nullStudentException);
            }
            catch (InvalidStudentException invalidStudentException)
            {
                throw await CreateAndLogValidationException(invalidStudentException);
            }
            catch (NotFoundStudentException notFoundStudentException)
            {
                throw await CreateAndLogValidationException(notFoundStudentException);
            }
            catch (DuplicateKeyException duplicateKeyException)
            {
                var alreadyExistsStudentException =
                    new AlreadyExistsStudentException(
                        message: "Student with the same Id already exists.",
                        innerException: duplicateKeyException);

                // arch-030: DependencyValidationException for conflict errors
                throw await CreateAndLogDependencyValidationException(alreadyExistsStudentException);
            }
            catch (DbUpdateConcurrencyException dbUpdateConcurrencyException)
            {
                var lockedStudentException =
                    new LockedStudentException(
                        message: "Student is locked, please try again.",
                        innerException: dbUpdateConcurrencyException);

                throw await CreateAndLogDependencyValidationException(lockedStudentException);
            }
            catch (DbUpdateException dbUpdateException)
            {
                var failedStudentStorageException =
                    new FailedStudentStorageException(
                        message: "Failed student storage error occurred, contact support.",
                        innerException: dbUpdateException);

                // arch-030: DependencyException for non-critical storage errors
                throw await CreateAndLogDependencyException(failedStudentStorageException);
            }
            catch (Exception serviceException)
            {
                var failedStudentServiceException =
                    new FailedStudentServiceException(
                        message: "Unexpected service error occurred. Contact support.",
                        innerException: serviceException);

                // arch-030: ServiceException for all unexpected errors
                throw await CreateAndLogServiceException(failedStudentServiceException);
            }
        }

        // arch-015: CreateAndLog* helpers are async because ILoggingBroker.LogErrorAsync
        // returns ValueTask. Callers use `throw await CreateAndLog*(...)`.
        // WRONG: private StudentValidationException CreateAndLogValidationException(...)
        //        { this.loggingBroker.LogError(...); }
        // CORRECT: async ValueTask<T> + await this.loggingBroker.LogErrorAsync(...)
        private async ValueTask<StudentValidationException> CreateAndLogValidationException(
            Xeption exception)
        {
            var studentValidationException =
                new StudentValidationException(
                    message: "Student validation error occurred, please fix the errors and try again.",
                    innerException: exception);

            await this.loggingBroker.LogErrorAsync(studentValidationException);

            return studentValidationException;
        }

        private async ValueTask<StudentDependencyValidationException>
            CreateAndLogDependencyValidationException(Xeption exception)
        {
            var studentDependencyValidationException =
                new StudentDependencyValidationException(
                    message: "Student dependency validation error occurred, fix the errors.",
                    innerException: exception);

            await this.loggingBroker.LogErrorAsync(studentDependencyValidationException);

            return studentDependencyValidationException;
        }

        private async ValueTask<StudentDependencyException> CreateAndLogDependencyException(
            Xeption exception)
        {
            var studentDependencyException =
                new StudentDependencyException(
                    message: "Student dependency error occurred, contact support.",
                    innerException: exception);

            await this.loggingBroker.LogErrorAsync(studentDependencyException);

            return studentDependencyException;
        }

        private async ValueTask<StudentServiceException> CreateAndLogServiceException(
            Xeption exception)
        {
            var studentServiceException =
                new StudentServiceException(
                    message: "Student service error occurred, contact support.",
                    innerException: exception);

            await this.loggingBroker.LogErrorAsync(studentServiceException);

            return studentServiceException;
        }
    }
}
