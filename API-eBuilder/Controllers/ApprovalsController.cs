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

        /// <summary>
        /// Get the approval by APID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
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


        /// <summary>
        /// Get the approvals by ManagerID or LID or status. All parameters are optional
        /// </summary>
        /// <param name="LID"></param>
        /// <param name="ManagerID"></param>
        /// <param name="status"></param>
        /// <returns></returns>
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


        /// <summary>
        /// Add an approval
        /// </summary>
        /// <param name="appr"></param>
        /// <returns></returns>
        public HttpResponseMessage Post([FromBody] approval appr)
        {
            try
            {
                using (ebuilderEntities entities = new ebuilderEntities())
                {
                    var entity = entities.approvals.FirstOrDefault(a => a.ManagerID == appr.ManagerID && a.LID == appr.LID);
                    if(entity != null)
                    {
                        return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Approval already available with given datails");
                    }

                    appr.status = "pending";
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


        /// <summary>
        /// Delete an approval
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
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


        /// <summary>
        /// Update the status of an approval
        /// </summary>
        /// <param name="id"></param>
        /// <param name="appr"></param>
        /// <returns></returns>
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
