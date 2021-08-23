using System;
using System.Collections.Generic;
using System.Text;

namespace TimeRegister.Common.Models
{
   public class Timeregister
    {
        public int EmployeeId { get; set; }
        public DateTime? EntryDate { get; set; }
        public DateTime? DepartureDate { get; set; }
        public int TypeEntry { get; set; }
        public bool Consolidated { get; set; }

    }
}
