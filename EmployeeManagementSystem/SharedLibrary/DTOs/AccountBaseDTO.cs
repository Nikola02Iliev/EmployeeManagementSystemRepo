﻿using System.ComponentModel.DataAnnotations;

namespace SharedLibrary.DTOs
{
    public class AccountBaseDTO
    {
        [DataType(DataType.EmailAddress)]
        [EmailAddress]
        [Required]
        public string Email { get; set; } = string.Empty;

        [DataType(DataType.Password)]
        [Required]
        public string Password { get; set; } = string.Empty;
    }
}
