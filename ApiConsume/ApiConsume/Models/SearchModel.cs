using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiConsume.Models
{
    public class SearchModel
    {
        public int OriginId { get; set; }
        public string OriginName { get; set; }
        public int DestinationId { get; set; }
        public string DestinationName { get; set; }
        public DateTime DepartureDate { get; set; }

    }
}