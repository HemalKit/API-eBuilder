using API_eBuilder.Models;
using DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace API_eBuilder.Controllers
{
    public class AccessController : ApiController
    {
        public bool Get(Login login)
        {
            using (ebuilderEntities entities = new ebuilderEntities())
            {
                var emp = entities.employees.Where(e => e.EID == login.EID).FirstOrDefault();
                if (emp != null)
                {
                    if (string.Compare(Crypto.Hash(login.password), emp.password) == 0)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }

                }
                else
                {
                    return false;
                }
            }

        }

    }
}
