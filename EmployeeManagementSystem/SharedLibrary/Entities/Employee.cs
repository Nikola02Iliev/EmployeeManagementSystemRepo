namespace SharedLibrary.Entities
{
    public class Employee : BaseEntity
    {
        public string CivilId { get; set; } = string.Empty;
        public string FileNumber { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string JobName { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string Photo { get; set; } = string.Empty;
        public string Other { get; set; } = string.Empty;

        public GeneralDepartment GeneralDepartment { get; set; } = new GeneralDepartment();
        public int GeneralDepartmentId { get; set; }

        public Department Department { get; set; } = new Department();
        public int DepartmentId { get; set; }

        public Branch Branch { get; set; } = new Branch();
        public int BranchId { get; set; }

        public Town Town { get; set; } = new Town();
        public int TownId { get; set; }



    }
}
