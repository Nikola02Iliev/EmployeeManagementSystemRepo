using System.Text.Json.Serialization;

namespace SharedLibrary.Entities
{
    public class Department : BaseEntity
    {
        [JsonIgnore]
        public List<Employee> Employees { get; set; } = new List<Employee>();
    }
}
