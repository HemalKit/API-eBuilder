using DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace API_eBuilder.Models
{
    public class AccessView
    {
        public AccessView(employee emp)
        {
            this.EID = emp.EID;
            this.password = emp.password;
            this.email = emp.email;
            this.emailVerified = emp.emailVerified;
            this.activationCode = emp.activationCode;
            this.dob = emp.dob;
            this.fName = emp.fName;
            this.lName = emp.lName;
            this.gender = emp.gender;
            this.homeNo = emp.homeNo;
            this.street = emp.street;
            this.city = emp.city;
            this.jobCategory = emp.jobCategory;
            this.managerId = emp.employees.Any() ? emp.employees.First().EID : null;
        }


        public string EID { get; set; }
        public string password { get; set; }
        public string email { get; set; }
        public Nullable<bool> emailVerified { get; set; }
        public string activationCode { get; set; }
        public System.DateTime dob { get; set; }
        public string fName { get; set; }
        public string lName { get; set; }
        public string gender { get; set; }
        public string homeNo { get; set; }
        public string street { get; set; }
        public string city { get; set; }
        public string jobCategory { get; set; }
        public string managerId { get; set; }
    }
}