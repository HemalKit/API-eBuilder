using DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace API_eBuilder.Models
{


    public class attendanceWithWorkingHours
    {
        public attendanceWithWorkingHours(attendance att)
        {
            this.AID = att.AID;
            this.checkIn = att.checkIn;
            this.checkOut = att.checkOut;
            this.date = att.date;
            this.EID = att.EID;
            using(ebuilderEntities entities = new ebuilderEntities())
            {
                var emp = entities.employees.FirstOrDefault(e => e.EID == this.EID);
                this.fName = emp.fName;
                this.lName = emp.lName;
            }
        }

        public int AID { get; set; }
        public System.DateTime date { get; set; }
        public Nullable<System.TimeSpan> checkIn { get; set; }
        public Nullable<System.TimeSpan> checkOut { get; set; }
        public string EID { get; set; }
        public string fName { get; set; }
        public string lName { get; set; }
        public TimeSpan workingHours { get; set; }
    }
}