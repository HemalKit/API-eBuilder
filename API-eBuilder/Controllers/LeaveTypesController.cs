using DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace API_eBuilder.Controllers
{
    public class LeaveTypesController : ApiController
    {
        /*public IEnumerable<leave_type> Get()
        {
            using (ebuilderEntities entities = new ebuilderEntities())
            {
                return entities.leave_type.ToList();

            }
        }*/
        
        public HttpResponseMessage Get([FromUri]string jobCategory = "all", [FromUri] string leaveCategory="all")
        {
            try
            {
                using (ebuilderEntities entities = new ebuilderEntities())
                {
                    var parameters = "";
                    parameters += jobCategory == "all" ? "0" : "1";
                    parameters += leaveCategory == "all" ? "0" : "1";

                    var entity = new List<leave_type>();

                    switch (parameters)
                    {
                        case "00":
                            entity = entities.leave_type.ToList();
                            break;
                        case "01":
                            entity = entities.leave_type.Where(lt => lt.leaveCategory == leaveCategory).ToList();
                            break;
                        case "10":
                            entity = entities.leave_type.Where(lt => lt.jobCategory == jobCategory).ToList();
                            break;
                        case "11":
                            entity = entities.leave_type.Where(lt => lt.jobCategory == jobCategory && lt.leaveCategory == leaveCategory).ToList();
                            break;
                    }
                    if (entity.Any())
                    {

                        return Request.CreateResponse(HttpStatusCode.OK, entity );
                    }
                    else
                    {
                        return Request.CreateErrorResponse(HttpStatusCode.NotFound, "No Leave Types");

                    }

                }
            }
            catch(Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ex);
            }
        }

        public HttpResponseMessage Post([FromBody] leave_type leaveType)
        {
            try
            {

                using (ebuilderEntities entities = new ebuilderEntities())
                {

                    entities.leave_type.Add(leaveType);
                    entities.SaveChanges();
                    var message = Request.CreateResponse(HttpStatusCode.OK, leaveType);
                    message.Headers.Location = new Uri(Request.RequestUri + leaveType.jobCategory + leaveType.leaveCategory);
                    return message;
                }
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ex);
            }
        }

        public HttpResponseMessage Put(string jobCategory, string leaveCategory ,[FromBody]leave_type leaveType)
        {
            try
            {
                using (ebuilderEntities entities = new ebuilderEntities())
                {
                    var entity = entities.leave_type.FirstOrDefault(lt => lt.jobCategory == jobCategory && lt.leaveCategory == leaveCategory);

                    if (entity == null)
                    {
                        return Request.CreateErrorResponse(HttpStatusCode.NotFound, "Leave type " + leaveCategory +"for"+ jobCategory +" not found");
                    }
                    else
                    {
                        entity.maxAllowed = leaveType.maxAllowed;

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

        public HttpResponseMessage Delete(string jobCategory, string leaveCategory)
        {
            try
            {
                using (ebuilderEntities entities = new ebuilderEntities())
                {
                    var entity = entities.leave_type.FirstOrDefault(lt => lt.jobCategory == jobCategory && lt.leaveCategory == leaveCategory);
                    if (entity != null)
                    {
                        entities.leave_type.Remove(entity);
                        entities.SaveChanges();
                        return Request.CreateResponse(HttpStatusCode.OK);
                    }
                    else
                    {
                        return Request.CreateErrorResponse(HttpStatusCode.NotFound, "No Leave Type "+ leaveCategory +"for"+jobCategory+ " to delete");

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
