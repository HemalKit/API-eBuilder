using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace API_eBuilder.Models
{
    public class attendanceWithWorkingHours
    {
        public int AID { get; set; }
        public System.DateTime date { get; set; }
        public Nullable<System.TimeSpan> checkIn { get; set; }
        public Nullable<System.TimeSpan> checkOut { get; set; }
        public string EID { get; set; }
        public TimeSpan workingHours { get; set; }
    }
}