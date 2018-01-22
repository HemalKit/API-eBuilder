using DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace API_eBuilder.Controllers
{
    public class AttendanceController : ApiController
    {
        /*public IEnumerable<attendance> Get()
        {
            using (ebuilderEntities entities = new ebuilderEntities())
            {
                return entities.attendances.ToList();

            }
        }*/

        public HttpResponseMessage Get(int id)
        {
            using (ebuilderEntities entities = new ebuilderEntities())
            {
                var entity = entities.attendances.FirstOrDefault(a => a.AID == id);
                if(entity != null)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, entity);
                }
                else
                {
                    return Request.CreateErrorResponse(HttpStatusCode.NotFound, "Attendance with AID " + id.ToString() + " not found");

                }

            }
        }


        public HttpResponseMessage Get(DateTime? date = null, string EID = "all")
        {
            try
            {
                using(ebuilderEntities entities = new ebuilderEntities())
                {
                    string parameters = "";
                    parameters += date == null ? "0" : "1";
                    parameters += EID == "all" ? "0" : "1";

                    var entity = new List<attendance>();

                    switch (parameters)
                    {
                        case "00":
                            entity = entities.attendances.ToList();
                            break;
                        case "01":
                            entity = entities.attendances.Where(a => a.EID == EID).ToList();
                            break;
                        case "10":
                            entity = entities.attendances.Where(a => a.date == date).ToList();
                            break;
                        case "11":
                            entity = entities.attendances.Where(a => a.date == date && a.EID == EID).ToList();
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
        //[Route("api/attendance/{EID}/{*startDate:datetime}/{*endDate:datetime}")]
        public HttpResponseMessage Get(string EID, DateTime startDate, DateTime endDate)
        {
            try
            {
                using(ebuilderEntities entities = new ebuilderEntities())
                {
                    var entity = entities.attendances.Where(a => a.EID == EID && (DateTime.Compare(startDate, a.date) < 0 && DateTime.Compare(a.date, endDate) < 0)).ToList();
                    return Request.CreateResponse(HttpStatusCode.OK, entity);
                }
            }
            catch(Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ex);
            }
        }



        public HttpResponseMessage Post([FromBody] attendance att)
        {
            try
            {
                using (ebuilderEntities entities = new ebuilderEntities())
                {
                    entities.attendances.Add(att);
                    entities.SaveChanges();
                    var message = Request.CreateResponse(HttpStatusCode.OK, att);
                    message.Headers.Location = new Uri(Request.RequestUri + att.AID.ToString());
                    return message;
                }
            }
            catch(Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, att.ToString(),ex);
            }
        }

        public HttpResponseMessage Delete(int id)
        {
            try
            {
                using (ebuilderEntities entities = new ebuilderEntities())
                {
                    var entity = entities.attendances.FirstOrDefault(a => a.AID == id);
                    if (entity != null)
                    {
                        entities.attendances.Remove(entity);
                        entities.SaveChanges();
                        return Request.CreateResponse(HttpStatusCode.OK);
                    }
                    else
                    {
                        return Request.CreateErrorResponse(HttpStatusCode.NotFound, "No attendnce with the AID " + id.ToString() + " to delete");

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
