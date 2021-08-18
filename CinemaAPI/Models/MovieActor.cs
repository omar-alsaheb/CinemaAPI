using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace CinemaAPI.Models
{
    public class MovieActor
    {
        public long Id { get; set; }
        [Required, StringLength(150)]
        public int ActorId { get; set; }
        [ForeignKey("ActorId")]
        public Actor actor { get; set; }
        [Required]

        public long MovieId { get; set; }
        [ForeignKey("MovieId")]
        public Movie movie { get; set; }
    }
}
