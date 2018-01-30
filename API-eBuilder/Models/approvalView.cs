using DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace API_eBuilder.Models
{
    public class approvalView
    {
        public approvalView(approval app)
        {
            using(ebuilderEntities entities = new ebuilderEntities())
            {
                this.LID = app.LID;
                this.APID = app.APID;
                this.ManagerID = app.ManagerID;
                this.status = app.status;


                var leave = entities.leavs.FirstOrDefault(l => l.LID == this.LID);
                this.leaveCategory = leave.leaveCategory;
                this.reason = leave.reason;
                this.date = leave.date;

                var emp = entities.employees.FirstOrDefault(e => e.EID == leave.EID);

                this.fName = emp.fName;
                this.lName = emp.lName;
            }
        }

        public int APID { get; set; }
        public int LID { get; set; }
        public string status { get; set; }
        public string ManagerID { get; set; }
        public string lName { get; set; }
        public string fName { get; set; }
        public string leaveCategory { get; set; }
        public string reason { get; set; }
        public DateTime date { get; set; }
    }
}