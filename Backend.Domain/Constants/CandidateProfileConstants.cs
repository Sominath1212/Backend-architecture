using System;
using System.Collections.Generic;
using System.Text;

namespace Backend.Domain.Constants
{
    public static class CandidateProfileConstants
    {
        public const int MaxFirstNameLength = 100;
        public const int MaxLastNameLength = 100;
        public const int MaxEmailLength = 256;
        public const int MaxMobileLength = 20;
        public const int MaxLocationLength = 200;
        public const int MaxDesignationLength = 150;
        public const int MaxCompanyLength = 200;
    }

    public static class EducationConstants
    {
        public const int MaxDegreeLength = 100;
        public const int MaxInstitutionLength = 200;
        public const int MaxSpecializationLength = 150;
        public const int MaxGradeLength = 50; // To accommodate "8.4 CGPA" or "85%"
    }
}
