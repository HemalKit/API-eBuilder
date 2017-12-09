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
        public IEnumerable<duty_leave> Get()
        {
            using (ebuilderEntities entities = new ebuilderEntities())
            {
                return entities.duty_leave.ToList();

            }
        }

        public HttpResponseMessage Get(int id)
        {
            using (ebuilderEntities entities = new ebuilderEntities())
            {
                var entity = entities.duty_leave.FirstOrDefault(dl => dl.DLID == id);
                if (entity != null)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, entity);
                }
                else
                {
                    return Request.CreateErrorResponse(HttpStatusCode.NotFound, "DutyLeave with the DLID " + id.ToString() + " not Found");
                }

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
