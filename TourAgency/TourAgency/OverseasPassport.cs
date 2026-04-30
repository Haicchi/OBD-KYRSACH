using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TourAgency
{
    public class OverseasPassport
    {
        [Key]
        public int ID { get; set; }
        public string NameTransliterated { get; set; }
        public string PassportNumber { get; set; }
        public string SurnameTransliterated { get; set; }
        public string Nationality { get; set; }
        public string Sex { get; set; }
        public DateTime DateOfBirth { get; set; }
        public DateTime IssueDate { get; set; }
        public DateTime ExpiryDate { get; set; }
    }
}
