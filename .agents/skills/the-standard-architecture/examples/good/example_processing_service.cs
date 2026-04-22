// ---------------------------------------------------------------
// Copyright (c) Coalition of the Good-Hearted Engineers
// FREE TO USE TO CONNECT THE WORLD
// ---------------------------------------------------------------

// GOOD EXAMPLE: Standard-compliant Processing Service
// Demonstrates: one foundation dependency, used-data-only validation,
// higher-order logic (Ensure/Upsert), shifters, exception unwrapping.

using System;
using System.Threading.Tasks;

namespace MyProject.Services.Processings.Students
{
    // arch-040: Depends on exactly one foundation service
    public partial class StudentProcessingService : IStudentProcessingService
    {
        private readonly IStudentService studentService;
        private readonly ILoggingBroker loggingBroker;

        public StudentProcessingService(
            IStudentService studentService,
            ILoggingBroker loggingBroker)
        {
            this.studentService = studentService;
            this.loggingBroker = loggingBroker;
        }

        // arch-042: Higher-order logic — EnsureStudentExists (retrieve+add combination)
        // arch-044: Combination pattern
        public ValueTask<Student> EnsureStudentExistsAsync(Student student) =>
            TryCatch(async () =>
            {
                // arch-041: Validate only what this method uses
                ValidateStudentOnEnsure(student);

                IQueryable<Student> allStudents =
                    this.studentService.RetrieveAllStudents();

                bool isStudentExists = allStudents.Any(retrievedStudent =>
                    retrievedStudent.Id == student.Id);

                return isStudentExists switch
                {
                    false => await this.studentService.AddStudentAsync(student),
                    _ => await this.studentService.RetrieveStudentByIdAsync(student.Id)
                };
            });

        // arch-042: Higher-order logic — UpsertStudent (retrieve+modify or add)
        public ValueTask<Student> UpsertStudentAsync(Student student) =>
            TryCatch(async () =>
            {
                ValidateStudentOnUpsert(student);

                IQueryable<Student> allStudents =
                    this.studentService.RetrieveAllStudents();

                bool isStudentExists = allStudents.Any(retrievedStudent =>
                    retrievedStudent.Id == student.Id);

                return isStudentExists switch
                {
                    true => await this.studentService.ModifyStudentAsync(student),
                    false => await this.studentService.AddStudentAsync(student)
                };
            });

        // arch-043: Shifter — Student → bool
        public ValueTask<bool> VerifyStudentExistsAsync(Guid studentId) =>
            TryCatch(async () =>
            {
                ValidateStudentId(studentId);

                IQueryable<Student> allStudents =
                    this.studentService.RetrieveAllStudents();

                return allStudents.Any(student => student.Id == studentId);
            });

        // arch-046: Pass-through — no processing logic needed, delegates directly
        public ValueTask<Student> RetrieveStudentByIdAsync(Guid studentId) =>
            this.studentService.RetrieveStudentByIdAsync(studentId);
    }
}

// ---------------------------------------------------------------
// Exception Handling partial (StudentProcessingService.Exceptions.cs)
// ---------------------------------------------------------------

namespace MyProject.Services.Processings.Students
{
    public partial class StudentProcessingService
    {
        // arch-045: Unwrap foundation exceptions, re-wrap as processing exceptions
        private delegate ValueTask<Student> ReturningStudentFunction();
        private delegate ValueTask<bool> ReturningBoolFunction();

        private async ValueTask<Student> TryCatch(ReturningStudentFunction returningStudentFunction)
        {
            try
            {
                return await returningStudentFunction();
            }
            catch (StudentValidationException studentValidationException)
            {
                throw CreateAndLogValidationException(studentValidationException.InnerException as Xeption);
            }
            catch (StudentDependencyValidationException studentDependencyValidationException)
            {
                throw CreateAndLogDependencyValidationException(
                    studentDependencyValidationException.InnerException as Xeption);
            }
            catch (StudentDependencyException studentDependencyException)
            {
                throw CreateAndLogDependencyException(studentDependencyException.InnerException as Xeption);
            }
            catch (StudentServiceException studentServiceException)
            {
                throw CreateAndLogServiceException(studentServiceException.InnerException as Xeption);
            }
            catch (Exception serviceException)
            {
                var failedStudentProcessingServiceException =
                    new FailedStudentProcessingServiceException(
                        message: "Failed student processing service error occurred, contact support.",
                        innerException: serviceException);

                throw CreateAndLogServiceException(failedStudentProcessingServiceException);
            }
        }
    }
}
