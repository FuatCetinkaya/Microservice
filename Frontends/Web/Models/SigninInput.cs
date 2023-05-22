using System.ComponentModel.DataAnnotations;

namespace Web.Models;

public class SigninInput
{
    [Required]
    [Display(Name = "Email Adres")]
    public string Email { get; set; }

    [Required]
    [Display(Name = "Şifre")]
    public string Password { get; set; }

    [Display(Name = "Beni Hatırla")]
    public bool IsRemember { get; set; }
}
