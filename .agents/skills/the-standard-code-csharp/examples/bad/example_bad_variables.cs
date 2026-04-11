// ---------------------------------------------------------------
// BAD EXAMPLE: Variable violations
// Each violation annotated with the rule it breaks.
// ---------------------------------------------------------------

namespace Examples.Bad
{
    public class BadVariableExamples
    {
        public void DemonstrateViolations()
        {
            // cs-010 VIOLATION: single-letter abbreviation
            var s = new Student();

            // cs-010 VIOLATION: abbreviation in lambda
            var filtered = students.Where(s => s.IsActive);

            // cs-011 VIOLATION: abbreviated lambda param
            var result = students.Where(s => s.Name == "Todd");

            // cs-012 VIOLATION: type suffix in plural
            var studentList = new List<Student>();

            // cs-013 VIOLATION: type suffix in variable name
            var studentModel = new Student();
            var studentObj = new Student();

            // cs-014 VIOLATION: null variable doesn't signal intent
            Student student = null;

            // cs-015 VIOLATION: zero value doesn't signal intent
            int changeCount = 0;

            // cs-020 VIOLATION: right-side clear but no var
            Student student2 = new Student();

            // cs-021 VIOLATION: right-side unclear but using var
            var student3 = GetStudent(); // GetStudent() return type not obvious from var

            // cs-031 VIOLATION: multi-line declaration missing blank lines
            Student anotherStudent = GetStudent();
            List<Student> washingtonSchoolsStudents =
                await GetAllWashingtonSchoolsStudentsAsync();
            School school = await GetSchoolAsync();
            // ^ no blank lines around the multi-line declaration

            // cs-023 VIOLATION: single-property uses initializer (should be post-assign)
            var inputStudentEvent = new StudentEvent
            {
                Student = inputProcessedStudent  // should be: var e = new StudentEvent(); e.Student = ...;
            };

            // cs-023 VIOLATION: multi-property uses post-assignment (should use initializer)
            var studentEvent = new StudentEvent();
            studentEvent.Student = someStudent;
            studentEvent.Date = someDate;
        }
    }
}
