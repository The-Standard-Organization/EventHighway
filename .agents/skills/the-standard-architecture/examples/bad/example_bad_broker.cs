// ---------------------------------------------------------------
// BAD EXAMPLE: Non-Standard Broker
// Each violation is annotated with the rule it breaks.
// ---------------------------------------------------------------

using System;

namespace MyProject.Brokers
{
    // VIOLATION arch-001: No interface — broker cannot be mocked in tests
    // VIOLATION arch-008: Wraps TWO resources (storage AND email) in one broker
    public class StudentBroker
    {
        private readonly AppDbContext dbContext;
        private readonly SmtpClient emailClient;

        public StudentBroker(AppDbContext dbContext, SmtpClient emailClient)
        {
            this.dbContext = dbContext;
            this.emailClient = emailClient;
        }

        // VIOLATION arch-006: Uses business language (Add) instead of infrastructure (Insert)
        // VIOLATION arch-002: Contains flow control (if statement)
        // VIOLATION arch-003: Handles exceptions — brokers must never catch exceptions
        // VIOLATION arch-009: Not async
        public Student AddStudent(Student student)
        {
            // arch-002 VIOLATION: flow control in broker
            if (student == null)
            {
                throw new ArgumentNullException("student is null");
            }

            try
            {
                // arch-003 VIOLATION: exception handling in broker
                this.dbContext.Students.Add(student);
                this.dbContext.SaveChanges();

                // arch-008 VIOLATION: sending email from an entity broker
                this.emailClient.Send(new MailMessage("noreply@school.com", student.Email,
                    "Welcome", "You have been added."));

                return student;
            }
            catch (Exception ex)
            {
                // arch-003 VIOLATION: catching and rethrowing in broker
                throw new Exception($"Failed to add student: {ex.Message}");
            }
        }

        // VIOLATION arch-006: Uses business language (GetById) instead of (SelectStudentById)
        // VIOLATION arch-004: Reads connection string from environment directly
        //                      instead of owning configuration through injection
        public Student GetById(Guid id)
        {
            // arch-004 VIOLATION: broker does not own its configuration
            var connectionString = Environment.GetEnvironmentVariable("DB_CONNECTION");

            return this.dbContext.Students.Find(id);
        }
    }
}

// ---------------------------------------------------------------
// BAD EXAMPLE: Non-Standard Logging Broker
// ---------------------------------------------------------------

namespace MyProject.Brokers.Loggings
{
    public class LoggingBroker : ILoggingBroker
    {
        private readonly ILogger<LoggingBroker> logger;

        public LoggingBroker(ILogger<LoggingBroker> logger) =>
            this.logger = logger;

        // VIOLATION arch-009: Task.Run() wraps a synchronous ILogger<T> call,
        //   producing a heap-allocated Task and introducing thread-pool overhead
        //   for no benefit. ILogger<T> is already synchronous — no offloading needed.
        //   Use `async ValueTask` + direct call instead.
        public ValueTask LogWarningAsync(string message) =>
            new ValueTask(Task.Run(() => this.logger.LogWarning(message)));

        // VIOLATION arch-009: Same Task.Run() anti-pattern on every method.
        public ValueTask LogErrorAsync(Exception exception) =>
            new ValueTask(Task.Run(() => this.logger.LogError(exception, exception.Message)));

        public ValueTask LogCriticalAsync(Exception exception) =>
            new ValueTask(Task.Run(() => this.logger.LogCritical(exception, exception.Message)));

        public ValueTask LogInformationAsync(string message) =>
            new ValueTask(Task.Run(() => this.logger.LogInformation(message)));

        public ValueTask LogTraceAsync(string message) =>
            new ValueTask(Task.Run(() => this.logger.LogTrace(message)));

        public ValueTask LogDebugAsync(string message) =>
            new ValueTask(Task.Run(() => this.logger.LogDebug(message)));
    }
}
