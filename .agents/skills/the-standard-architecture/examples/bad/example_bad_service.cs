// ---------------------------------------------------------------
// BAD EXAMPLE: Non-Standard Service
// Each violation is annotated with the rule it breaks.
// ---------------------------------------------------------------

namespace MyProject.Services
{
    // VIOLATION arch-021: Handles more than one entity type (Student AND Course)
    // VIOLATION arch-022: Uses infrastructure language (Insert, Select) not business language
    // VIOLATION arch-080: Calls another foundation-level service directly (CourseService)
    public class StudentService
    {
        private readonly IStorageBroker storageBroker;
        private readonly ICourseService courseService; // arch-080 VIOLATION: same-layer call

        public StudentService(IStorageBroker storageBroker, ICourseService courseService)
        {
            this.storageBroker = storageBroker;
            this.courseService = courseService;
        }

        // VIOLATION arch-022: Infrastructure language (Insert) instead of business (Add)
        // VIOLATION arch-023: No validation before delegating
        // VIOLATION arch-024: No null check (structural validation) — no circuit-breaking
        public async Task<Student> InsertStudent(Student student)
        {
            // arch-023 VIOLATION: Directly inserts without any validation
            return await this.storageBroker.InsertStudentAsync(student);
        }

        // VIOLATION arch-025: No logical validation
        // VIOLATION arch-028: Does not collect all errors — throws on first failure only
        public async Task<Student> UpdateStudent(Student student)
        {
            // arch-024 VIOLATION: No null check
            // arch-025 VIOLATION: No logical validation
            if (student.Name == null)
            {
                throw new Exception("Name is null"); // not a Standard exception
            }

            // arch-080 VIOLATION: calling peer service from foundation service
            var courses = await this.courseService.RetrieveAllCoursesAsync();

            return await this.storageBroker.UpdateStudentAsync(student);
        }

        // VIOLATION arch-029: Raw exception is rethrown without localization or categorization
        // VIOLATION arch-030: Exception is not categorized into Standard exception types
        public async Task<Student> GetStudentById(Guid id)
        {
            try
            {
                return await this.storageBroker.SelectStudentByIdAsync(id);
            }
            catch (Exception ex)
            {
                // arch-029 VIOLATION: Not localized, not categorized, not logged
                throw new Exception($"Database error: {ex.Message}");
            }
        }
    }
}
