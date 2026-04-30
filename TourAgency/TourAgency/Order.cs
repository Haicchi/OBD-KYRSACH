using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TourAgency
{
    public class Order
    {
        [Key]
        public int ID { get; set; }
        public string StatusOfPay { get; set; }
        public DateTime DateOfOrder { get; set; }
        public int NumberOfPeople { get; set; }
        public string TypeOfPayment { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalValue { get; set; }

        public int IDClient { get; set; }
        [ForeignKey("IDClient")]
        public Client Client { get; set; }

        public int IDWorker { get; set; }
        [ForeignKey("IDWorker")]
        public Staff Worker { get; set; }

        public int IDTour { get; set; }
        [ForeignKey("IDTour")]
        public Tour Tour { get; set; }
    }
}
