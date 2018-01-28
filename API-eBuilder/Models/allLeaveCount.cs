using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace API_eBuilder.Models
{
    public class allLeaveCount
    {

        public allLeaveCount()
        {
            this.left = new List<count>();
            this.taken = new List<count>();
        }
        public List<count> taken { get; set; }
        public List<count> left { get; set; }
    }
}