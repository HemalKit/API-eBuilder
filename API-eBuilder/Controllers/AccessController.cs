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
        public HttpResponseMessage Post([FromBody]Login login)
        {
            using (ebuilderEntities entities = new ebuilderEntities())
            {
                var emp = entities.employees.Where(e => e.email == login.email).FirstOrDefault();
                if (emp != null)
                {

                    if (string.Compare(Crypto.Hash(login.password), emp.password) == 0)
                    {
                        entities.Entry(emp).Collection("employees").Load();
                        var employeeData = new AccessView(emp);
                        var response =  Request.CreateResponse(HttpStatusCode.OK, employeeData);
                        response.Headers.Location = new Uri("http://localhost:61355/api/Employees/" + emp.EID);
                        return response;
                    }
                    else
                    {
                        
                        var response = Request.CreateResponse(HttpStatusCode.NotFound, new AccessView(new employee()));
                        return response;
                    }

                }
                else
                {
                    
                    return Request.CreateResponse(HttpStatusCode.NotFound, new AccessView(new employee()));
                }
            }

        }

    }
}
