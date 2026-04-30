using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TourAgency
{
    public class Town
    {
        [Key]
        public int ID { get; set; }
        public string Name { get; set; }
        public int IDCountry { get; set; }
        [ForeignKey("IDCountry")]
        public Country Country { get; set; }
    }
}
