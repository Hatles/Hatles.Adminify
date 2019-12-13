using System.ComponentModel.DataAnnotations;

namespace Hatles.Adminify.Users.Dto
{
    public class ChangeUserLanguageDto
    {
        [Required]
        public string LanguageName { get; set; }
    }
}