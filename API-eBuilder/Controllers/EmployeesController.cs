﻿using System;
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
      
       /* public IEnumerable<employee> Get()
        {
            using(ebuilderEntities entities = new ebuilderEntities())
            {
                return entities.employees.ToList();

            }
        }*/

        public HttpResponseMessage Get(string id)
        {
            using (ebuilderEntities entities = new ebuilderEntities())
            {
                var entity = entities.employees.FirstOrDefault(e => e.EID == id);
                if(entity != null)
                {
                    
                    return Request.CreateResponse(HttpStatusCode.OK, entity);
                }
                else
                {
                    return Request.CreateErrorResponse(HttpStatusCode.NotFound, "Employee with the EID " + id + " not Found");
                }

            }
        }

        public HttpResponseMessage Get(string gender="all",string jobCategory = "all")
        {
            try
            {
                using(ebuilderEntities entities = new ebuilderEntities())
                {
                    var parameters = "";
                    parameters += gender == "all" ? "0" : "1";
                    parameters += jobCategory == "all" ? "0" : "1";

                    var entity = new List<employee>();

                    switch (parameters)
                    {
                        case "00":
                            entity = entities.employees.ToList();
                            break;
                        case "01":
                            entity = entities.employees.Where(e => e.jobCategory == jobCategory).ToList();
                            break;
                        case "10":
                            entity = entities.employees.Where(e => e.gender == gender).ToList();
                            break;
                        case "11":
                            entity = entities.employees.Where(e => e.gender == gender && e.jobCategory == jobCategory).ToList();
                            break;
                    }
                    return Request.CreateResponse(HttpStatusCode.OK, entity);
                }
            }
            catch(Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ex);
            }
        }


        
        public HttpResponseMessage Post([FromBody] employee emp)
        {
            try
            {

                using (ebuilderEntities entities = new ebuilderEntities())
                {
                    emp.password = Crypto.Hash(emp.password);
                    entities.employees.Add(emp);
                    entities.SaveChanges();
                    var message = Request.CreateResponse(HttpStatusCode.Created, emp);
                    message.Headers.Location = new Uri(Request.RequestUri + emp.EID);
                    return message;
                }
            }
            catch(Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ex);
            }
        }

       
        public HttpResponseMessage Delete(string id)
        {
            try
            {
                using (ebuilderEntities entities = new ebuilderEntities())
                {
                    var entity = entities.employees.FirstOrDefault(e => e.EID == id);
                    if(entity != null)
                    {
                        entities.employees.Remove(entity);
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
