﻿using System.ComponentModel.DataAnnotations;

namespace TeleMed.DTOs.Auth
{
    public class RegisterDto : LoginDto
    {
        [Required]
        public string Name { get; set; } = string.Empty;
        
    }
}
