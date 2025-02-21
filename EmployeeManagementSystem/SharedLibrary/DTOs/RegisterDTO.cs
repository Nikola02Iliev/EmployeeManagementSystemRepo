using System.ComponentModel.DataAnnotations;

namespace SharedLibrary.DTOs
{
    public class RegisterDTO : AccountBaseDTO
    {
        [Required]
        [MinLength(5)]
        [MaxLength(100)]
        public string FullName { get; set; } = string.Empty;

        [DataType(DataType.Password)]
        [Compare(nameof(Password))]
        [Required]
        public string ConfirmPassword { get; set; } = string.Empty;
    }
}
