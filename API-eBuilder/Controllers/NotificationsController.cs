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
        /// <summary>
        /// Get a notification by NID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Get the notiifcations for a paticular employee with or without providing date
        /// </summary>
        /// <param name="EID"></param>
        /// <param name="date"></param>
        /// <returns></returns>
        public HttpResponseMessage Get(string EID ,DateTime? date = null)
        {
            try
            {
                using (ebuilderEntities entites = new ebuilderEntities())
                {

                    var entity = new List<notification>();
                    if (date == null)
                    {
                        var allNotifications = entites.notifications.ToList();
                        foreach (var notf in allNotifications)
                        {
                            entites.Entry(notf).Collection("employees").Load();
                            if (notf.employees.Any(e => e.EID == EID))
                            {
                                entity.Add(notf);
                            }

                        }
                    }
                    else
                    {
                        var allNotifications = entites.notifications.Where(n=>n.date==date).ToList();
                        foreach (var notf in allNotifications)
                        {
                            entites.Entry(notf).Collection("employees").Load();
                            if (notf.employees.Any(e => e.EID == EID))
                            {
                                entity.Add(notf);
                            }
                        }
                    }
                    return Request.CreateResponse(HttpStatusCode.OK, entity);
                }
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ex);
            }
        }

        /// <summary>
        /// Add an notification 
        /// </summary>
        /// <param name="not"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Delete a notification by providing NID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
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
