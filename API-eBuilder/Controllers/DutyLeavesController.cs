using DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace API_eBuilder.Controllers
{
    public class DutyLeavesController : ApiController
    {
       /* public IEnumerable<duty_leave> Get()
        {
            using (ebuilderEntities entities = new ebuilderEntities())
            {
                return entities.duty_leave.ToList();

            }
        }*/

        public HttpResponseMessage Get(int id)
        {
            using (ebuilderEntities entities = new ebuilderEntities())
            {
                var entity = entities.duty_leave.FirstOrDefault(dl => dl.DLID == id);
                if (entity != null)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, new { dutyLeaves = entity });
                }
                else
                {
                    return Request.CreateErrorResponse(HttpStatusCode.NotFound, "DutyLeave with the DLID " + id.ToString() + " not Found");
                }

            }
        }

        public HttpResponseMessage Get(string EID = "all", DateTime? date = null)
        {
            try
            {
                using (ebuilderEntities entities = new ebuilderEntities())
                {
                    var entity = new List<duty_leave>();
                    var parameters = "";
                    parameters += EID == "all" ? "0" : "1";
                    parameters += date == null ? "0" : "1";

                    switch (parameters)
                    {
                        case "00":
                            entity = entities.duty_leave.ToList();
                            break;
                        case "01":
                            entity = entities.duty_leave.Where(dl => dl.date == date).ToList();
                            break;
                        case "10":
                            entity = entities.duty_leave.Where(dl => dl.EID == EID).ToList();
                            break;
                        case "11":
                            entity = entities.duty_leave.Where(dl => dl.EID == EID && dl.date == date).ToList();
                            break;
                    }                    
                     return Request.CreateResponse(HttpStatusCode.OK, new { dutyLeaves = entity });
                }
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ex);
            }
        }


        public HttpResponseMessage Post([FromBody] duty_leave dutyLeave)
        {
            try
            {
                using (ebuilderEntities entities = new ebuilderEntities())
                {

                    entities.duty_leave.Add(dutyLeave);
                    entities.SaveChanges();
                    var message = Request.CreateResponse(HttpStatusCode.OK, dutyLeave);
                    message.Headers.Location = new Uri(Request.RequestUri + dutyLeave.DLID.ToString());
                    return message;
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
                    var entity = entities.duty_leave.FirstOrDefault(dl => dl.DLID == id);
                    if (entity != null)
                    {
                        entities.duty_leave.Remove(entity);
                        entities.SaveChanges();
                        return Request.CreateResponse(HttpStatusCode.OK);
                    }
                    else
                    {
                        return Request.CreateErrorResponse(HttpStatusCode.NotFound, "No Duty Leave with the DLID " + id.ToString() + " to delete");

                    }

                }
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ex);
            }
        }

        public HttpResponseMessage Put(int id, [FromBody]duty_leave dutyLeave)
        {
            try
            {
                using (ebuilderEntities entities = new ebuilderEntities())
                {
                    var entity = entities.duty_leave.FirstOrDefault(dl => dl.DLID == id);

                    if (entity == null)
                    {
                        return Request.CreateErrorResponse(HttpStatusCode.NotFound, "Duty Leave with DLID " + id.ToString() + " not found");

                    }
                    else
                    {
                        entity.date = dutyLeave.date;
                        entity.appointmentTime = dutyLeave.appointmentTime;
                        entity.duration = dutyLeave.duration;
                       
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
