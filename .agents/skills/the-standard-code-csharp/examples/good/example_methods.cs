// ---------------------------------------------------------------
// Copyright (c) Coalition of the Good-Hearted Engineers
// FREE TO USE TO CONNECT THE WORLD
// ---------------------------------------------------------------

// GOOD EXAMPLE: Standard-compliant method usage

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Examples.Good
{
    public class MethodExamples
    {
        // cs-040: Contains verb (Get)
        // cs-050: One line → fat arrow
        public List<Student> GetStudents() =>
            this.storageBroker.GetStudents();

        // cs-041: Async → Async suffix
        // cs-050: One line → fat arrow
        public async ValueTask<List<Student>> GetStudentsAsync() =>
            await this.storageBroker.GetStudentsAsync();

        // cs-042: Parameter fully qualified (studentName, not name or text)
        // cs-043: Action parameter explicit (ByName)
        public async ValueTask<Student> GetStudentByNameAsync(string studentName)
        {
            // cs-044: Variable matches alias → no alias needed
            return await this.storageBroker.GetStudentByNameAsync(studentName);
        }

        // cs-044: Literal → alias required
        public async ValueTask<Student> GetToddAsync()
        {
            return await this.storageBroker.GetStudentByNameAsync(studentName: "Todd");
        }

        // cs-051: Multi-line → scope body
        // cs-054: Blank line before return
        public List<Student> GetStudents()
        {
            StudentsClient studentsApiClient = InitializeStudentApiClient();

            return studentsApiClient.GetStudents();
        }

        // cs-052: One-liner > 120 chars → break after =>
        public async ValueTask<List<Student>> GetAllWashingtonSchoolsStudentsAsync() =>
            await this.storageBroker.GetStudentsAsync();

        // cs-057, cs-058: Method declaration > 120 chars → break params, one per line
        public async ValueTask<List<Student>> GetAllRegisteredWashingtonSchoolsStudentsAsync(
            StudentsQuery studentsQuery)
        {
            return await this.storageBroker.GetStudentsAsync(studentsQuery);
        }

        // cs-059: Chaining — first call on same line, subsequent indented one extra tab
        public void DemonstrateChaining()
        {
            var result = students.Where(student => student.Name is "Elbek")
                .Select(student => student.Name)
                    .ToList();
        }

        // cs-055: Multiple calls < 120 chars → may stack
        public async ValueTask<List<Student>> GetStudentsAsync()
        {
            StudentsClient studentsApiClient = InitializeStudentApiClient();
            List<Student> students = studentsApiClient.GetStudents();

            return students;
        }

        // cs-056: Call > 120 chars → blank line separation required
        public async ValueTask<List<Student>> GetStudentsWithLongNameAsync()
        {
            StudentsClient washingtonSchoolsStudentsApiClient =
                await InitializeWashingtonSchoolsStudentsApiClientAsync();

            List<Student> students = studentsApiClient.GetStudents();

            return students;
        }
    }
}
