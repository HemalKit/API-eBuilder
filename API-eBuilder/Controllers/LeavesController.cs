using DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace API_eBuilder.Controllers
{
    public class LeavesController : ApiController
    {
        /*public IEnumerable<leav> Get()
        {
            using (ebuilderEntities entities = new ebuilderEntities())
            {
                return entities.leavs.ToList();

            }
        }*/

        public HttpResponseMessage Get(int id)
        {
            using (ebuilderEntities entities = new ebuilderEntities())
            {
                var entity = entities.leavs.FirstOrDefault(l => l.LID == id);
                if (entity != null)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, new { leaves = entity });
                }
                else
                {
                    return Request.CreateErrorResponse(HttpStatusCode.NotFound, "Leave with the LID " + id.ToString() + " not Found");
                }

            }
        }

        public HttpResponseMessage Get(DateTime? date = null, string leaveCategory="all", string jobCategory ="all", string EID = "all")
        {
            try
            {
                using (ebuilderEntities entities = new ebuilderEntities())
                {
                    string parameters = "";
                    parameters += date == null ? "0" : "1";
                    parameters += leaveCategory == "all" ? "0" : "1";
                    parameters += jobCategory == "all" ? "0" : "1";
                    parameters += EID == "all" ? "0" : "1";
                    
                    var entity = new List<leav>() ;
                    switch (parameters)
                    {
                        case "0000":                         
                             entity = entities.leavs.ToList();
                             break;                            
                        case "0001":
                            entity = entities.leavs.Where(l => l.EID == EID).ToList();
                            break;
                        case "0010":                       
                            entity = entities.leavs.Where(l => l.jobCategory == jobCategory).ToList();
                            break;
                        case "0011":                       
                            entity = entities.leavs.Where(l => l.jobCategory == jobCategory && l.EID == EID).ToList();
                            break;
                        case "0100":                                             
                            entity = entities.leavs.Where(l => l.leaveCategory == leaveCategory).ToList();
                            break;
                        case "0101":                                                
                            entity = entities.leavs.Where(l => l.leaveCategory == leaveCategory && l.EID == EID).ToList();
                            break;
                        case "0110":
                            entity = entities.leavs.Where(l => l.leaveCategory == leaveCategory && l.jobCategory == jobCategory).ToList();
                            break;
                        case "0111":
                            entity = entities.leavs.Where(l => l.leaveCategory == leaveCategory && l.jobCategory == jobCategory && l.EID == EID).ToList();
                            break;
                        case "1000":
                            entity = entities.leavs.Where(l => l.date == date).ToList();
                            break;
                        case "1001":
                            entity = entities.leavs.Where(l => l.date == date && l.EID == EID).ToList();
                            break;
                        case "1010":
                            entity = entities.leavs.Where(l =>  l.date == date && l.jobCategory == jobCategory).ToList();
                            break;
                        case "1011":
                            entity = entities.leavs.Where(l => l.date == date && l.jobCategory == jobCategory && l.EID == EID).ToList();
                            break;
                        case "1100":
                            entity = entities.leavs.Where(l => l.date == date && l.leaveCategory == leaveCategory).ToList();
                            break;
                        case "1101":
                            entity = entities.leavs.Where(l => l.date == date && l.leaveCategory == leaveCategory && l.EID == EID).ToList();
                            break;
                        case "1111":
                            entity = entities.leavs.Where(l =>l.date == date && l.leaveCategory == leaveCategory && l.jobCategory == jobCategory && l.EID == EID).ToList();
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



        [HttpGet]
        public HttpResponseMessage Get(string EID, DateTime startDate, DateTime endDate)
        {
            try
            {
                using(ebuilderEntities entities = new ebuilderEntities())
                {
                    var entity = entities.leavs.Where(l => l.EID == EID && (DateTime.Compare(startDate, l.date) < 0 && DateTime.Compare(l.date, endDate) < 0)).ToList();
                    return Request.CreateResponse(HttpStatusCode.OK, entity);
                }
            }
            catch(Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ex);
            }
        }


        public HttpResponseMessage Post([FromBody]leav leave)
        {
            try
            {

                using (ebuilderEntities entities = new ebuilderEntities())
                {
                    var maxLeaves = entities.leave_type.FirstOrDefault(lt => lt.leaveCategory == leave.leaveCategory && lt.jobCategory == leave.jobCategory).maxAllowed;

                    if (entities.employees.FirstOrDefault(e => e.EID == leave.EID).jobCategory != leave.jobCategory)
                    {
                        return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Inalid Data");
                    }


                    if (maxLeaves == 0)
                    {
                        return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Incompatible leave type");
                    }

                    var leavesTaken = entities.leavs.Where(l => l.EID == leave.EID && l.leaveCategory == leave.leaveCategory && l.date.Year == leave.date.Year).Count();
                    if (leavesTaken >= maxLeaves)
                    {
                        return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "No leaves available");
                    }

                    entities.leavs.Add(leave);
                    
                    var man = entities.employees.FirstOrDefault(e => e.EID == leave.EID);

                    // List<employee> man = (List<employee>)entities.Entry(emp).Property("employee").CurrentValue;
                    entities.Entry(man).Collection("employees").Load();
                    if (man.employees.Any())
                    {
                        foreach (var m in man.employees)
                        {
                            approval app = new approval();
                            app.ManagerID = m.EID;
                            app.LID = leave.LID;
                            app.status = "pending";
                            entities.approvals.Add(app);
                        }
                        var message = Request.CreateResponse(HttpStatusCode.OK, leave);
                        entities.SaveChanges();
                        message.Headers.Location = new Uri(Request.RequestUri + leave.LID.ToString());
                        return message;
                    }
                    else
                    {
                        var message = Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Error occured");
                        return message;
                    }
                    
                }
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ex);
            }
        }


        public HttpResponseMessage Delete(int id)
        {
            try
            {
                using (ebuilderEntities entities = new ebuilderEntities())
                {
                    var entity = entities.leavs.FirstOrDefault(l => l.LID == id);
                    if (entity != null)
                    {
                        entities.leavs.Remove(entity);
                        entities.SaveChanges();
                        return Request.CreateResponse(HttpStatusCode.OK);
                    }
                    else
                    {
                        return Request.CreateErrorResponse(HttpStatusCode.NotFound, "No Leave with the LID " + id.ToString() + " to delete");
                    }
                }
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ex);
            }
        }

        public HttpResponseMessage Put(int id, [FromBody]leav leave)
        {
            try
            {
                using (ebuilderEntities entities = new ebuilderEntities())
                {
                    var entity = entities.leavs.FirstOrDefault(l => l.LID == id);

                    if (entity == null)
                    {
                        return Request.CreateErrorResponse(HttpStatusCode.NotFound, "Leave with LID " + id.ToString() + " not found");
                    }
                    else
                    {
                        entity.date = leave.date;
                        entity.leaveCategory = leave.leaveCategory;
                        entity.reason = leave.reason;

                        entities.SaveChanges();
                        return Request.CreateResponse(HttpStatusCode.OK, entity);
                    }
                }
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ex);
            }
        }


    }
}
