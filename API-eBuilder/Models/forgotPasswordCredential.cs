using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace API_eBuilder.Models
{
    public class forgotPasswordCredential
    {
        public string email { get; set; }
        public string verificationCode { get; set; }
        public string newPassword { get; set; }
    }
}