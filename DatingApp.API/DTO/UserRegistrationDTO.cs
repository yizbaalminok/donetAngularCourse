using System.ComponentModel.DataAnnotations;

namespace DatingApp.API.DTO
{
    public class UserRegistrationDTO
    {
        [Required]
        [StringLength(15, MinimumLength = 4, ErrorMessage = "Username must be 4 to 15 characters")]
        public string username { get; set;}

        [Required]
        [StringLength(20, MinimumLength=6, ErrorMessage="Username must be 6 to 20 characters")]
        public string password {get; set;}

    }
}