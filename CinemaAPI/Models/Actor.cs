using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CinemaAPI.Models
{
    public class Actor
    {
        public int Id { get; set; }
        [Required, StringLength(150)]

        public string ActorName { get; set; }
        public string ActorPicture { get; set; }
    }
}
