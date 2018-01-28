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
        /// <summary>
        /// Get a task by providing TID as id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
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


        /// <summary>
        /// Add a new Task
        /// </summary>
        /// <param name="newTask"></param>
        /// <returns></returns>
        public HttpResponseMessage Post([FromBody] task newTask)
        {
            try
            {

                using (ebuilderEntities entities = new ebuilderEntities())
                {

                    entities.tasks.Add(newTask);
                    entities.SaveChanges();
                    var message = Request.CreateResponse(HttpStatusCode.OK, newTask);
                    message.Headers.Location = new Uri(Request.RequestUri + newTask.TID.ToString());
                    return message;
                }
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ex);
            }
        }

        /// <summary>
        /// Delete a task
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Update time, date, activity or status of an existing task
        /// </summary>
        /// <param name="id"></param>
        /// <param name="t"></param>
        /// <returns></returns>
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
