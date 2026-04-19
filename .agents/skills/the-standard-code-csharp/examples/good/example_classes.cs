// ---------------------------------------------------------------
// Copyright (c) Coalition of the Good-Hearted Engineers
// FREE TO USE TO CONNECT THE WORLD
// ---------------------------------------------------------------

// GOOD EXAMPLE: Standard-compliant class naming, fields, and instantiation

namespace Examples.Good
{
    // cs-060: Model — no type suffix
    public class Student
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public DateTimeOffset CreatedDate { get; set; }
        public DateTimeOffset UpdatedDate { get; set; }
    }

    // cs-061: Service — singular + Service suffix
    public class StudentService : IStudentService
    {
        // cs-070: camelCase field name (no underscore, no PascalCase)
        private readonly IStorageBroker storageBroker;
        private readonly ILoggingBroker loggingBroker;

        public StudentService(
            IStorageBroker storageBroker,
            ILoggingBroker loggingBroker)
        {
            // cs-072: Reference with this.
            this.storageBroker = storageBroker;
            this.loggingBroker = loggingBroker;
        }
    }

    // cs-062: Broker — singular + Broker suffix
    public partial class StorageBroker : IStorageBroker
    {
        private readonly IConfiguration configuration;

        public StorageBroker(IConfiguration configuration)
        {
            this.configuration = configuration;
        }
    }

    // cs-063: Controller — plural + Controller suffix
    public class StudentsController : ControllerBase
    {
        private readonly IStudentService studentService;

        public StudentsController(IStudentService studentService)
        {
            this.studentService = studentService;
        }
    }

    // cs-083: Instantiation property order matches class declaration order
    public class InstantiationExamples
    {
        public void Demonstrate()
        {
            // Student has: Id, Name, CreatedDate, UpdatedDate
            // Initializer must follow that order:
            var student = new Student
            {
                Id = Guid.NewGuid(),
                Name = "Elbek",
                CreatedDate = DateTimeOffset.UtcNow,
                UpdatedDate = DateTimeOffset.UtcNow
            };

            // cs-080: Literal arguments require named aliases
            var namedStudent = new Student(id: Guid.NewGuid(), name: "Hassan");

            // cs-081: Variable names match → aliases optional
            Guid id = Guid.NewGuid();
            string name = "Hassan";
            var studentFromVars = new Student(id, name);
        }
    }
}
