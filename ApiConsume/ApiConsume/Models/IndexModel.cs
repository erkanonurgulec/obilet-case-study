using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiConsume.Models
{
    public class IndexModel
    {
        public IndexModel()
        {
            Origins = new List<SelectListItem>();
            Destinations = new List<SelectListItem>();
        }

        public int? Origin { get; set; }
        public List<SelectListItem> Origins { get; set; }
        public int? Destination { get; set; }
        public List<SelectListItem> Destinations { get; set; }

        [BindProperty]
        public DateTime? DepartureDate { get; set; }
        public bool IsCurrentDate { get; set; }
        public bool IsTomorrowtDate { get; set; }
    }
}