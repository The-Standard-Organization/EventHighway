// ---------------------------------------------------------------
// BAD EXAMPLE: Class naming, field, and instantiation violations
// ---------------------------------------------------------------

namespace Examples.Bad
{
    // cs-060 VIOLATION: Model has type suffix
    public class StudentModel { }
    public class StudentDTO { }
    public class StudentEntity { }

    // cs-061 VIOLATION: Service is plural
    public class StudentsService { }

    // cs-061 VIOLATION: Service uses non-standard suffix
    public class StudentBusinessLogic { }
    public class StudentBL { }

    // cs-062 VIOLATION: Broker is plural
    public class StudentsBroker { }

    // cs-063 VIOLATION: Controller is singular
    public class StudentController : ControllerBase { }

    public class BadFieldExamples
    {
        // cs-070 VIOLATION: underscore prefix
        private readonly IStorageBroker _storageBroker;

        // cs-070 VIOLATION: PascalCase field
        private readonly IStorageBroker StorageBroker;

        public BadFieldExamples(IStorageBroker storageBroker)
        {
            // cs-072 VIOLATION: No this. prefix for private field assignment
            _storageBroker = storageBroker;
        }
    }

    public class BadInstantiationExamples
    {
        public void Demonstrate()
        {
            // cs-080 VIOLATION: Literal without alias
            var student = new Student("Josh", 150);

            // cs-082 VIOLATION: Target-typed new
            Student student2 = new (...);

            // cs-083 VIOLATION: Instantiation order doesn't match class declaration order
            // Student declares: Id, Name, CreatedDate, UpdatedDate
            var student3 = new Student
            {
                Name = "Elbek",       // wrong order — Name before Id
                Id = Guid.NewGuid(),
                UpdatedDate = DateTimeOffset.UtcNow,   // wrong order — UpdatedDate before CreatedDate
                CreatedDate = DateTimeOffset.UtcNow
            };
        }
    }
}
