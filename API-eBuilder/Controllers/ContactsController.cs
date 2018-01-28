using DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace API_eBuilder.Controllers
{
    public class ContactsController : ApiController
    {
        /// <summary>
        /// Get the list of contacts by EID
        /// </summary>
        /// <param name="EID"></param>
        /// <returns></returns>
        public HttpResponseMessage Get(string EID)
        {
            try
            {
                using(ebuilderEntities entities = new ebuilderEntities())
                {
                    var entity = entities.contacts.Where(c => c.EID == EID).ToList();
                    return Request.CreateResponse(HttpStatusCode.Accepted, entity);
                }
            }
            catch(Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ex);
            }

        }

        /// <summary>
        /// Add an contact
        /// </summary>
        /// <param name="con"></param>
        /// <returns></returns>
        public HttpResponseMessage Post(contact con)
        {
            try
            {
                using(ebuilderEntities entities = new ebuilderEntities())
                {
                    entities.contacts.Add(con);
                    entities.SaveChanges();
                    return Request.CreateResponse(HttpStatusCode.Created, con);
                }
            }
            catch(Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ex);
            }
        }

        /// <summary>
        /// Update a Phone NUmber of a contact
        /// </summary>
        /// <param name="id"></param>
        /// <param name="con"></param>
        /// <returns></returns>
        public HttpResponseMessage Put(int id, [FromBody] contact con)
        {
            try
            {
                using(ebuilderEntities entities = new ebuilderEntities())
                {
                    var entity = entities.contacts.FirstOrDefault(c => c.CID == id);
                    if (entity == null)
                    {
                        return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "No contact available with the given CID");
                    }
                    else
                    {
                        entity.phoneNo = con.phoneNo;
                        entities.SaveChanges();
                        return Request.CreateResponse(HttpStatusCode.OK, entity);
                    }
                }
            }
            catch(Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ex);
            }
        }

        /// <summary>
        /// Delete a contact
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public HttpResponseMessage Delete(int id)
        {
            try
            {
                using(ebuilderEntities entities = new ebuilderEntities())
                {
                    var entity = entities.contacts.FirstOrDefault(c => c.CID == id);
                    if(entity == null)
                    {
                        return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "No contact exists with the given id");
                    }
                    else
                    {
                        entities.contacts.Remove(entity);
                        entities.SaveChanges();
                        return Request.CreateResponse(HttpStatusCode.OK);
                    }
                }
            }
            catch(Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ex);
            }
        }
    }
}
