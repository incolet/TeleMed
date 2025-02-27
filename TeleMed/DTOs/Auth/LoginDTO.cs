﻿using System.ComponentModel.DataAnnotations;

namespace TeleMed.DTOs.Auth
{
    public class LoginDto
    {
        [Required, DataType(DataType.EmailAddress), EmailAddress]
        public string Email { get; set; } = string.Empty;
        [Required, DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;

        public int Role { get; set; } = 1;

    }
}
