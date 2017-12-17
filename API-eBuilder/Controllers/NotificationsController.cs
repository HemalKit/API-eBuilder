using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using DataAccess;

namespace API_eBuilder.Controllers
{
    public class NotificationsController : ApiController
    {
        public IEnumerable<notification> Get()
        {
            using(ebuilderEntities entities = new ebuilderEntities())
            {
                return entities.notifications.ToList();
            }
        }

        public HttpResponseMessage Get(int id)
        {
            try
            {
                using (ebuilderEntities entities = new ebuilderEntities())
                {
                    var entity = entities.notifications.FirstOrDefault(n => n.NID == id);
                    if(entity != null)
                    {
                        return Request.CreateResponse(HttpStatusCode.OK, entity);
                    }
                    else
                    {
                        return Request.CreateResponse(HttpStatusCode.NotFound, "The Notification with the NID" + id + "not found");

                    }
                }
            }
            catch(Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ex);
            }
        }

        public HttpResponseMessage Get(DateTime date, string EID = "all")
        {
            try
            {
                using(ebuilderEntities entites = new ebuilderEntities())
                {
                    if( EID == "all")
                    {
                        return Request.CreateResponse(HttpStatusCode.OK, entites.notifications.Where(n => n.date == date).ToList());

                    }
                    else
                    {
                        // Need to get the notifications related to a particular employee
                        return Request.CreateResponse(HttpStatusCode.OK, entites.notifications.Where(n => n.date == date).ToList());                       
                    }
                }
            }
            catch(Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ex);
            }
        }

        public HttpResponseMessage Post([FromBody]notification not )
        {
            try
            {
                using(ebuilderEntities entities = new ebuilderEntities())
                {
                    entities.notifications.Add(not);
                    entities.SaveChanges();
                    var message = Request.CreateResponse(HttpStatusCode.OK, not);
                    message.Headers.Location = new Uri(Request.RequestUri + not.NID.ToString());
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
                    var entity = entities.notifications.FirstOrDefault(n => n.NID == id);
                    if (entity != null)
                    {
                        entities.notifications.Remove(entity);
                        entities.SaveChanges();
                        return Request.CreateResponse(HttpStatusCode.OK);
                    }
                    else
                    {
                        return Request.CreateErrorResponse(HttpStatusCode.NotFound, "A notfication with the NID" + id + "not found to delete");
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
