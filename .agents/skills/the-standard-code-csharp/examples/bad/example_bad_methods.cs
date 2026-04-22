// ---------------------------------------------------------------
// BAD EXAMPLE: Method violations
// Each violation annotated with the rule it breaks.
// ---------------------------------------------------------------

namespace Examples.Bad
{
    public class BadMethodExamples
    {
        // cs-040 VIOLATION: No verb in method name
        public List<Student> Students() => this.storageBroker.GetStudents();

        // cs-041 VIOLATION: Async method missing Async suffix
        public async ValueTask<List<Student>> GetStudents() =>
            await this.storageBroker.GetStudentsAsync();

        // cs-042 VIOLATION: Input parameter not fully qualified (text instead of studentName)
        public async ValueTask<Student> GetStudentByNameAsync(string text) { ... }

        // cs-042 VIOLATION: Input parameter not entity-qualified (name instead of studentName)
        public async ValueTask<Student> GetStudentByNameAsync(string name) { ... }

        // cs-043 VIOLATION: Action parameter not described (GetStudentAsync instead of GetStudentByIdAsync)
        public async ValueTask<Student> GetStudentAsync(Guid studentId) { ... }

        // cs-044 VIOLATION: Literal passed without alias
        public async Task BadLiteralPass()
        {
            Student student = await GetStudentByNameAsync("Todd"); // should be studentName: "Todd"
        }

        // cs-050 VIOLATION: One-liner using scope body instead of fat arrow
        public List<Student> GetStudents()
        {
            return this.storageBroker.GetStudents();
        }

        // cs-051 VIOLATION: Multi-liner using fat arrow
        public Student AddStudent(Student student) =>
            this.storageBroker.InsertStudent(student)
                .WithLogging();

        // cs-054 VIOLATION: No blank line before return
        public List<Student> GetStudents2()
        {
            StudentsClient studentsApiClient = InitializeStudentApiClient();
            return studentsApiClient.GetStudents(); // missing blank line before return
        }

        // cs-059 VIOLATION: Chaining starts on new line (no uglification)
        public void BadChaining()
        {
            var result = students
                .Where(student => student.Name is "Elbek")
                .Select(student => student.Name)
                .ToList();
            // ^ should start Where on the same line as students
        }

        // cs-057 VIOLATION: Method declaration > 120 chars, no line break
        public async ValueTask<List<Student>> GetAllRegisteredWashingtonSchoolsStudentsAsync(StudentsQuery studentsQuery)
        {
            return await this.storageBroker.GetStudentsAsync(studentsQuery);
        }
    }
}
