using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace API_eBuilder.Models
{
    public class dutyLeaveWithName
    {
        public int DLID { get; set; }
        public System.DateTime date { get; set; }
        public System.TimeSpan appointmentTime { get; set; }
        public string location { get; set; }
        public string EID { get; set; }
        public string purpose { get; set; }
        public System.TimeSpan endTime { get; set; }
        public string fName { get; set; }
        public string lName { get; set; }
    }
}