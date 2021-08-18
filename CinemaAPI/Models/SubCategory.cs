using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace CinemaAPI.Models
{
    public class SubCategory
    {
        public int Id { get; set; }
        [Required, StringLength(150)]
        public string SubCategoryName { get; set; }

        [Required]

        public int CategoryId { get; set; }
        [ForeignKey("CategoryId")]
        public Category category { get; set; }
    }
}
