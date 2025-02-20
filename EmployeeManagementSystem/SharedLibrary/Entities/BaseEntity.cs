using System.Text.Json.Serialization;

namespace SharedLibrary.Entities
{
    public class BaseEntity
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }
}
