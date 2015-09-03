﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace _20LHWebPortal.Models
{
    public class CreateHangoutViewModel
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Name")]
        public string Name { get; set; }

        [Display(Name = "Description")]
        public string Description { get; set; }

        [Display(Name = "Location")]
        public string Location { get; set; }

        [Display(Name = "Address")]
        public string Address { get; set; }

        [Required]
        [Display(Name = "Party Size")]
        [Range(4, 12, ErrorMessage = "Part size must be between 4 and 12")]
        public int PartySize { get; set; }

        [Display(Name = "Duration")]
        public int Duration { get; set; }

        [Display(Name = "Contact Info")]
        public string ContactInfo { get; set; }


        [Required]
        [Display(Name = "Date")]
        public DateTime? Date { get; set; }

        [Required]
        [Display(Name = "GenderRatio")]
        public bool GenderRatio { get; set; }


        public string UserId { get; set; }

    }
}
