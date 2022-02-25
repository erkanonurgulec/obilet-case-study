using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiConsume.Models
{
    public class JourneyModel
    {
        public string Origin { get; set; }
        public string OriginName { get; set; }
        public string Destiniation { get; set; }
        public string DestiniationName { get; set; }
        public string Departure { get; set; }
        public string Arrival { get; set; }
        public string Price { get; set; }
        public string Currency { get; set; }
    }
}