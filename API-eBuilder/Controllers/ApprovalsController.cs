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
        /*public IEnumerable<approval> Get()
        {
            using (ebuilderEntities entities = new ebuilderEntities())
            {
                return entities.approvals.ToList();
            }
        }*/

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


        public HttpResponseMessage Get(int LID=0 , string ManagerID="all", string status = "all")
        {
            try
            {
                using (ebuilderEntities entities = new ebuilderEntities())
                {
                    string parameters = "";
                    parameters += LID == 0 ? "0" : "1";
                    parameters += ManagerID == "all" ? "0" : "1";
                    parameters += status == "all" ? "0" : "1";

                    var entity = new List<approval>();

                    switch (parameters)
                    {
                        case "000":
                            entity = entities.approvals.ToList();
                            break;
                        case "001":
                            entity = entities.approvals.Where(a => a.status == status).ToList();
                            break;
                        case "010":
                            entity = entities.approvals.Where(a => a.ManagerID == ManagerID).ToList();
                            break;
                        case "011":
                            entity = entities.approvals.Where(a => a.ManagerID == ManagerID && a.status == status).ToList();
                            break;
                        case "100":
                            entity = entities.approvals.Where(a => a.LID == LID).ToList();
                            break;
                        case "101":
                            entity = entities.approvals.Where(a => a.LID == LID && a.status == status).ToList();
                            break;                       
                        case "110":
                            entity = entities.approvals.Where(a => a.LID == LID && a.ManagerID == ManagerID).ToList();
                            break;
                        case "111":
                            entity = entities.approvals.Where(a => a.LID == LID && a.ManagerID == ManagerID && a.status == status).ToList();
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
