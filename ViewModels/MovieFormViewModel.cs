using site101.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace site101.ViewModels
{
    public class MovieFormViewModel
    {

        public int Id { get; set; }
        [Required(ErrorMessage ="hmdaaa says: This is a required field"), StringLength(250)]

        public string Title { get; set; }

        public int Year { get; set; }

        [Range(1,10)]
        public double Rate { get; set; }

        [Required, StringLength(2500)]
        public string Storyline { get; set; }
        
        [Display(Name = "Choose poster")]
        public byte[] poster { get; set; }

        [Display (Name = "Genre")]
        public byte GenreId { get; set; }

        public IEnumerable<Genre> Genres { get; set; }

    }
}
