using DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using API_eBuilder.Models;

namespace API_eBuilder.Controllers
{
    public class DutyLeavesController : ApiController
    {
       /* public IEnumerable<duty_leave> Get()
        {
            using (ebuilderEntities entities = new ebuilderEntities())
            {
                return entities.duty_leave.ToList();

            }
        }*/


        //Get the duty leave by providing DLID
        public HttpResponseMessage Get(int id)
        {
            using (ebuilderEntities entities = new ebuilderEntities())
            {
                var entity = entities.duty_leave.FirstOrDefault(dl => dl.DLID == id);
                if (entity != null)
                {
                    dutyLeaveWithName dlName = new dutyLeaveWithName();
                    dlName.DLID = entity.DLID;
                    dlName.EID = entity.EID;
                    dlName.date = entity.date;
                    dlName.appointmentTime = entity.appointmentTime;
                    dlName.endTime = entity.endTime;
                    dlName.location = entity.location;
                    dlName.purpose = entity.purpose;

                    employee emp = entities.employees.FirstOrDefault(e => e.EID == entity.EID);
                    dlName.fName = emp.fName;
                    dlName.lName = emp.lName;

                    return Request.CreateResponse(HttpStatusCode.OK, dlName );
                }
                else
                {
                    return Request.CreateErrorResponse(HttpStatusCode.NotFound, "DutyLeave with the DLID " + id.ToString() + " not Found");
                }

            }
        }


        //Get the list of duty leaves of an employees or all employees within a time range. All parameters are optional
        public HttpResponseMessage Get(string EID = "all", DateTime? startDate = null, DateTime? endDate = null)
        {
            try
            {
                using (ebuilderEntities entities = new ebuilderEntities())
                {
                    var entity = new List<duty_leave>();
                    var parameters = "";
                    parameters += EID == "all" ? "0" : "1";
                    parameters += startDate == null ? "0" : "1";
                    parameters += endDate == null ? "0" : "1";

                    switch (parameters)
                    {
                        case "000":
                            entity = entities.duty_leave.ToList();
                            break;
                        case "001":
                            entity = entities.duty_leave.Where(dl => DateTime.Compare(dl.date,(DateTime)endDate)<0).ToList();
                            break;
                        case "010":
                            entity = entities.duty_leave.Where(dl => DateTime.Compare((DateTime)startDate,dl.date)<0).ToList();
                            break;
                        case "011":
                            entity = entities.duty_leave.Where(dl => DateTime.Compare(dl.date, (DateTime)endDate) < 0 && DateTime.Compare((DateTime)startDate, dl.date) < 0).ToList();
                            break;
                        case "100":
                            entity = entities.duty_leave.Where(dl => dl.EID == EID).ToList();
                            break;
                        case "101":
                            entity = entities.duty_leave.Where(dl => dl.EID == EID && DateTime.Compare(dl.date, (DateTime)endDate) < 0).ToList();
                            break;
                        case "110":
                            entity = entities.duty_leave.Where(dl => dl.EID == EID && DateTime.Compare((DateTime)startDate, dl.date) < 0).ToList();
                            break;
                        case "111":
                            entity = entities.duty_leave.Where(dl => dl.EID == EID && DateTime.Compare(dl.date, (DateTime)endDate) < 0 && DateTime.Compare((DateTime)startDate, dl.date) < 0).ToList();
                            break;
                    }

                    List<dutyLeaveWithName> dlNameList = new List<dutyLeaveWithName>();
                    foreach(var e in entity)
                    {
                        dutyLeaveWithName dlName = new dutyLeaveWithName();
                        dlName.DLID = e.DLID;
                        dlName.EID = e.EID;
                        dlName.date = e.date;
                        dlName.appointmentTime = e.appointmentTime;
                        dlName.endTime = e.endTime;
                        dlName.location = e.location;
                        dlName.purpose = e.purpose;

                        employee emp = entities.employees.FirstOrDefault(em => em.EID == e.EID);
                        dlName.fName = emp.fName;
                        dlName.lName = emp.lName;
                        dlNameList.Add(dlName);
                    }
                     return Request.CreateResponse(HttpStatusCode.OK,  dlNameList );
                }
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ex);
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
                        entity.endTime = dutyLeave.endTime;
                       
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
