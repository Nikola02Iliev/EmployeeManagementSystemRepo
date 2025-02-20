using System.Text.Json.Serialization;

namespace SharedLibrary.Entities
{
    public class GeneralDepartment : BaseEntity
    {
        [JsonIgnore]
        public List<Employee> Employees { get; set; } = new List<Employee>();
    }
}
