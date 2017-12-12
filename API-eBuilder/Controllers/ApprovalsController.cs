using DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace API_eBuilder.Controllers
{
    public class ApprovalsController : ApiController
    {
        public IEnumerable<approval> Get()
        {
            using (ebuilderEntities entities = new ebuilderEntities())
            {
                return entities.approvals.ToList();
            }
        }

        public HttpResponseMessage Get(int id)
        {
            using (ebuilderEntities entities = new ebuilderEntities())
            {
                var entity = entities.approvals.FirstOrDefault(a => a.APID == id);
                if (entity != null)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, entity);
                }
                else
                {
                    return Request.CreateErrorResponse(HttpStatusCode.NotFound, "Approal with the APID " + id.ToString() + " not Found");
                }

            }
        }

        

        public HttpResponseMessage Post([FromBody] approval appr)
        {
            try
            {

                using (ebuilderEntities entities = new ebuilderEntities())
                {

                    entities.approvals.Add(appr);
                    entities.SaveChanges();
                    var message = Request.CreateResponse(HttpStatusCode.OK, appr);
                    message.Headers.Location = new Uri(Request.RequestUri + appr.APID.ToString());
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
                    var entity = entities.approvals.FirstOrDefault(a => a.APID == id);
                    if (entity != null)
                    {
                        entities.approvals.Remove(entity);
                        entities.SaveChanges();
                        return Request.CreateResponse(HttpStatusCode.OK);
                    }
                    else
                    {
                        return Request.CreateErrorResponse(HttpStatusCode.NotFound, "No Approval with the APID " + id.ToString() + " to delete");

                    }

                }
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ex);
            }
        }

        public HttpResponseMessage Put(int id, [FromBody]approval appr)
        {
            try
            {
                using (ebuilderEntities entities = new ebuilderEntities())
                {
                    var entity = entities.approvals.FirstOrDefault(a => a.APID == id);

                    if (entity == null)
                    {
                        return Request.CreateErrorResponse(HttpStatusCode.NotFound, "Approval with APID " + id.ToString() + " not found");

                    }
                    else
                    {
                        entity.status = appr.status;

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
