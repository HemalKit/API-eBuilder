using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace API_eBuilder.Models
{
    public class changePasswordCredential
    {
        public string EID { get; set; }
        public string oldPassword { get; set; }
        public string newPassword { get; set; }
    }
}