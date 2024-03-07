using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.userAuthDto;
public class RegisterDto
{
    [Required]
    public string email { get; set; }
    [Required]
    public string password { get; set; }

}
