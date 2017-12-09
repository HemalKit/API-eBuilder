using DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace API_eBuilder.Controllers
{
    public class TasksController : ApiController
    {
        public IEnumerable<task> Get()
        {
            using (ebuilderEntities entities = new ebuilderEntities())
            {
                return entities.tasks.ToList();

            }
        }

        public HttpResponseMessage Get(int id)
        {
            using (ebuilderEntities entities = new ebuilderEntities())
            {
                var entity = entities.tasks.FirstOrDefault(t => t.TID == id);
                if (entity != null)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, entity);
                }
                else
                {
                    return Request.CreateErrorResponse(HttpStatusCode.NotFound, "Task with the TID " + id.ToString() + " not Found");
                }

            }
        }

        public HttpResponseMessage Post([FromBody] task t)
        {
            try
            {

                using (ebuilderEntities entities = new ebuilderEntities())
                {

                    entities.tasks.Add(t);
                    entities.SaveChanges();
                    var message = Request.CreateResponse(HttpStatusCode.OK, t);
                    message.Headers.Location = new Uri(Request.RequestUri + t.TID.ToString());
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
                    var entity = entities.tasks.FirstOrDefault(tsk => tsk.TID == id);
                    if (entity != null)
                    {
                        entities.tasks.Remove(entity);
                        entities.SaveChanges();
                        return Request.CreateResponse(HttpStatusCode.OK);
                    }
                    else
                    {
                        return Request.CreateErrorResponse(HttpStatusCode.NotFound, "No Task with the TID " + id.ToString() + " to delete");

                    }

                }
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ex);
            }
        }

        public HttpResponseMessage Put(int id, [FromBody]task t)
        {
            try
            {
                using (ebuilderEntities entities = new ebuilderEntities())
                {
                    var entity = entities.tasks.FirstOrDefault(tsk => tsk.TID == id);

                    if (entity == null)
                    {
                        return Request.CreateErrorResponse(HttpStatusCode.NotFound, "Task with TID " + id.ToString() + " not found");

                    }
                    else
                    {
                        entity.time = t.time;
                        entity.date = t.date;
                        entity.activity = t.activity;
                        entity.status = t.status;
                        

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
