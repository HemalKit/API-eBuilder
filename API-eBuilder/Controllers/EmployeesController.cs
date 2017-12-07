using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using DataAccess;
using API_eBuilder.Models;


namespace API_eBuilder.Controllers
{
    
    public class EmployeesController : ApiController
    {
      
        public IEnumerable<employee> Get()
        {
            using(ebuilderEntities entities = new ebuilderEntities())
            {
                return entities.employees.ToList();

            }
        }

        public employee Get(string id)
        {
            using (ebuilderEntities entities = new ebuilderEntities())
            {
                return entities.employees.FirstOrDefault(e => e.EID == id);

            }
        }


        
        public void Post([FromBody] employee emp)
        {
            using (ebuilderEntities entities = new ebuilderEntities())
            {
                emp.password = Crypto.Hash(emp.password);
                entities.employees.Add(emp);
                entities.SaveChanges();
            }
        }

       
        public HttpResponseMessage Delete(string id)
        {
            try
            {
                using (ebuilderEntities entities = new ebuilderEntities())
                {
                    var emp = entities.employees.FirstOrDefault(e => e.EID == id);
                    if(emp != null)
                    {
                        entities.employees.Remove(emp);
                        entities.SaveChanges();
                        return Request.CreateResponse(HttpStatusCode.OK);
                    }
                    else
                    {
                        return Request.CreateErrorResponse(HttpStatusCode.NotFound, "No employee with the EID " + id + " to delete");

                    }
                    
                }
            }
            catch(Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ex);
            }
        }

        public HttpResponseMessage Put(string id, [FromBody]employee emp)
        {
            try
            {
                using (ebuilderEntities entities = new ebuilderEntities())
                {
                    var entity = entities.employees.FirstOrDefault(e => e.EID == id);

                    if (entity == null)
                    {
                        return Request.CreateErrorResponse(HttpStatusCode.NotFound, "Employee with EID " + id + " not found");
                        
                    }
                    else
                    {
                        entity.fName = emp.fName;
                        entity.lName = emp.lName;
                        entity.street = emp.street;
                        entity.homeNo = emp.homeNo;
                        entity.city = emp.city;
                        entity.gender = emp.gender;
                        entity.dob = emp.dob;
                        entity.jobCategory = emp.jobCategory;

                        entities.SaveChanges();
                        return Request.CreateResponse(HttpStatusCode.OK, entity);
                    }
                }

            }
            catch(Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ex);
            }
        }


    }
}
