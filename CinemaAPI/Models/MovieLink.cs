using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace CinemaAPI.Models
{
    public class MovieLink
 
    {
        public long Id { get; set; }

        public long Quality { get; set; }
        public string Resolution { get; set; }
        [Required]
        public string MovieLinkDownload { get; set; }

        [Required]
        public long MoveId { get; set; }
        [ForeignKey("MoveId")]
        public Movie movie { get; set; }
    }
}
