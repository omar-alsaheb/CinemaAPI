using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CinemaAPI.ModelsView.Users
{
    public class EditUserModel
    {
        [Required]
        public string Id { get; set; }

        [StringLength(256), Required, DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }

        public string UserName { get; set; }
        public string PhoneNumber { get; set; }
        public string Country { get; set; }

    }
}
