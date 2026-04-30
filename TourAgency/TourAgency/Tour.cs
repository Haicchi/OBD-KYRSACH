using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TourAgency
{
    public class Tour
    {
        [Key]
        public int ID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Photo { get; set; }
        public DateTime DepartureDate { get; set; }
        public DateTime ArrivalDate { get; set; }
        public int TotalSeats { get; set; }

        public int IDHotel { get; set; }
        [ForeignKey("IDHotel")]
        public Hotel Hotel { get; set; }

        public int IDType { get; set; }
        [ForeignKey("IDType")]
        public TourType TourType { get; set; }

        public int IDFood { get; set; }
        [ForeignKey("IDFood")]
        public Food Food { get; set; }
    }
}
