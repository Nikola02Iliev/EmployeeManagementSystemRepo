using System.Text.Json.Serialization;

namespace SharedLibrary.Entities
{
    public class Town : BaseEntity
    {
        [JsonIgnore]
        public List<Employee> Employees { get; set; } = new List<Employee>();
    }
}
