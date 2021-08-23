using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Text;

namespace TimeRegister.Functions.Entities
{
    public class TimeregisterEntity : TableEntity
    {
        public int EmployeeId { get; set; }
        public DateTime? EntryDate { get; set; }
        public DateTime? DepartureDate { get; set; }
        public int TypeEntry { get; set; }
        public bool Consolidated { get; set; }

    }
}
