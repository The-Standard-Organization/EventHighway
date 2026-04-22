// ---------------------------------------------------------------
// Copyright (c) Coalition of the Good-Hearted Engineers
// FREE TO USE TO CONNECT THE WORLD
// ---------------------------------------------------------------

// GOOD EXAMPLE: Standard-compliant variable usage
// Each section references the rule it demonstrates.

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Examples.Good
{
    public class VariableExamples
    {
        public void DemonstrateNaming()
        {
            // cs-010: Full descriptive name
            var student = new Student();

            // cs-010: Full name in lambda
            var filteredStudents = students.Where(student => student.IsActive);

            // cs-012: Natural plural (not studentList)
            var students = new List<Student>();

            // cs-013: No type suffix (not studentModel)
            var student = new Student();

            // cs-014: Null intent signaled
            Student noStudent = null;

            // cs-015: Zero value intent signaled
            int noChangeCount = 0;
        }

        public async Task DemonstrateDeclarations()
        {
            // cs-020: Right-side clear → var
            var student = new Student();

            // cs-021: Right-side not clear → explicit type
            Student student = GetStudent();

            // cs-023: Single-property — assign after construction
            var inputStudentEvent = new StudentEvent();
            inputStudentEvent.Student = inputProcessedStudent;

            // cs-023: Multi-property — use initializer
            var studentEvent = new StudentEvent
            {
                Student = someStudent,
                Date = someDate
            };
        }

        public async Task DemonstrateOrganization()
        {
            // cs-031, cs-032: Single-line declarations stack without blank lines
            Student student = GetStudent();
            School school = await GetSchoolAsync();

            // cs-031: Multi-line declaration needs blank lines before AND after
            Student anotherStudent = GetStudent();

            List<Student> washingtonSchoolsStudentsWithGrades =
                await GetAllWashingtonSchoolsStudentsWithTheirGradesAsync();

            School anotherSchool = await GetSchoolAsync();
        }
    }
}
