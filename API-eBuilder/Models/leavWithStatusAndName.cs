using DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace API_eBuilder.Models
{
    public class leavWithStatusAndName
    {

        public leavWithStatusAndName(leav leave)
        {
            using(ebuilderEntities entities = new ebuilderEntities())
            {
                this.LID = leave.LID;
                this.EID = leave.EID;
                this.date = leave.date.Date;
                this.reason = leave.reason;
                this.jobCategory = leave.jobCategory;
                this.leaveCategory = leave.leaveCategory;


                var entity = entities.approvals.Where(a => a.LID == this.LID).Select(ap => ap.status).ToList();
                if (entity.Any(e => e == "rejected"))
                {
                    this.status = "rejected";
                }
                else if (entity.Any(e => e == "pending"))
                {
                    this.status = "pending";
                }
                if (entity.All(e => e == "accepted"))
                {
                    this.status = "accepted";
                }

                var emp = entities.employees.FirstOrDefault(e => e.EID == this.EID);
                this.fName = emp.fName;
                this.lName = emp.lName;                
            }
        }

        public int LID { get; set; }
        public System.DateTime date { get; set; }
        public string reason { get; set; }
        public string leaveCategory { get; set; }
        public string jobCategory { get; set; }
        public string EID { get; set; }
        public string status { get; set; }
        public string fName { get; set; }
        public string lName { get; set; }

    }
}