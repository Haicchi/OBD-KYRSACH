using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TourAgency
{
    public class Staff
    {
        [Key]
        public int ID { get; set; }
        public string Position { get; set; }
        public DateTime DateOfHire { get; set; }
        public string WorkPhone { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal PercentageOfSales { get; set; }

        public int IDAccount { get; set; }
        [ForeignKey("IDAccount")]
        public Account Account { get; set; }
    }
}
