using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TourAgency
{
    public class Client
    {
        [Key]
        public int ID { get; set; }
        public string Address { get; set; }
        public string Status { get; set; }

        public int IDOverseasPassport { get; set; }
        [ForeignKey("IDOverseasPassport")]
        public OverseasPassport OverseasPassport { get; set; }

        public int IDAccount { get; set; }
        [ForeignKey("IDAccount")]
        public Account Account { get; set; }
    }
}
