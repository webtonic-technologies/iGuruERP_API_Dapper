using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Communication_API.DTOs.Requests
{
    public class AddUpdateGroupRequest : IValidatableObject
    {
        public int GroupID { get; set; }

        [Required]
        public string GroupName { get; set; }

        [Required]
        public string AcademicYearCode { get; set; }

        [Required]
        public int TypeID { get; set; }  // 1 for Student, 2 for Employee

        [Required]
        public int InstituteID { get; set; }

        // For Student (TypeID = 1)
        public List<ClassSectionMapping>? ClassSectionMappings { get; set; }
        public List<int>? StudentIDs { get; set; }

        // For Employee (TypeID = 2)
        public List<DepartmentDesignationMapping>? DepartmentDesignationMappings { get; set; }
        public List<int>? EmployeeIDs { get; set; }

        // Custom validation logic to validate fields based on TypeID
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (TypeID == 1)
            {
                // Validate that ClassSectionMappings and StudentIDs are provided
                if (ClassSectionMappings == null || ClassSectionMappings.Count == 0)
                {
                    yield return new ValidationResult(
                        "ClassSectionMappings are required when TypeID is 1.",
                        new[] { nameof(ClassSectionMappings) });
                }

                if (StudentIDs == null || StudentIDs.Count == 0)
                {
                    yield return new ValidationResult(
                        "StudentIDs are required when TypeID is 1.",
                        new[] { nameof(StudentIDs) });
                }

                // Ensure no employee-related data is included when TypeID is 1
                if (DepartmentDesignationMappings != null || EmployeeIDs != null)
                {
                    yield return new ValidationResult(
                        "Employee-related data should not be provided when TypeID is 1.",
                        new[] { nameof(DepartmentDesignationMappings), nameof(EmployeeIDs) });
                }
            }
            else if (TypeID == 2)
            {
                // Validate that DepartmentDesignationMappings and EmployeeIDs are provided
                if (DepartmentDesignationMappings == null || DepartmentDesignationMappings.Count == 0)
                {
                    yield return new ValidationResult(
                        "DepartmentDesignationMappings are required when TypeID is 2.",
                        new[] { nameof(DepartmentDesignationMappings) });
                }

                if (EmployeeIDs == null || EmployeeIDs.Count == 0)
                {
                    yield return new ValidationResult(
                        "EmployeeIDs are required when TypeID is 2.",
                        new[] { nameof(EmployeeIDs) });
                }

                // Ensure no student-related data is included when TypeID is 2
                if (ClassSectionMappings != null || StudentIDs != null)
                {
                    yield return new ValidationResult(
                        "Student-related data should not be provided when TypeID is 2.",
                        new[] { nameof(ClassSectionMappings), nameof(StudentIDs) });
                }
            }
            else
            {
                yield return new ValidationResult("Invalid TypeID. It must be either 1 (Student) or 2 (Employee).");
            }
        }
    }

    public class ClassSectionMapping
    {
        public int ClassID { get; set; }
        public int SectionID { get; set; }
    }

    public class DepartmentDesignationMapping
    {
        public int DepartmentID { get; set; }
        public int DesignationID { get; set; }
    }
}
