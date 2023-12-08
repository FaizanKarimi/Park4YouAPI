using System.ComponentModel.DataAnnotations;

namespace ParkForYouAPI.APIRequestModels.User
{
    public class AddRoleRequest
    {
        [Required]
        public string RoleName { get; set; }
    }
}