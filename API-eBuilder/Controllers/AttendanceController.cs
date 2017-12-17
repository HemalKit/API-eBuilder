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
        public IEnumerable<attendance> Get()
        {
            using (ebuilderEntities entities = new ebuilderEntities())
            {
                return entities.attendances.ToList();

            }
        }

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

        public HttpResponseMessage Get(DateTime date,string EID = "all")
        {
            try
            {
                using(ebuilderEntities entities = new ebuilderEntities())
                {
                    if(EID == "all")
                    {
                        return Request.CreateResponse(HttpStatusCode.OK,entities.attendances.Where(a => a.date == date).ToList());
                    }
                    else
                    {
                        return Request.CreateResponse(HttpStatusCode.OK,entities.attendances.Where(a => a.date == date && a.EID == EID).ToList());
                    }

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
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ex);
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
