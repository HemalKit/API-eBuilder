using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace API_eBuilder.Models
{
    public class trackingWithEID
    {
        public int TRID { get; set; }
        public double longitude { get; set; }
        public double latitude { get; set; }
        public System.TimeSpan time { get; set; }
        public string EID { get; set; }
    }
}