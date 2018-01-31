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
    public class AttendanceController : ApiController
    {

        /// <summary>
        /// Get Attendance by AID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public HttpResponseMessage Get(int id)
        {
            using (ebuilderEntities entities = new ebuilderEntities())
            {
                var entity = entities.attendances.FirstOrDefault(a => a.AID == id);
                if(entity != null)
                {
                    attendanceWithWorkingHours att = new attendanceWithWorkingHours(entity);                    

                    return Request.CreateResponse(HttpStatusCode.OK, att);
                }
                else
                {
                    return Request.CreateErrorResponse(HttpStatusCode.NotFound, "Attendance with AID " + id.ToString() + " not found");

                }

            }
        }


        /// <summary>
        /// Get the attendance by date or EID, all parameters are optional
        /// </summary>
        /// <param name="date"></param>
        /// <param name="EID"></param>
        /// <returns></returns>
        public HttpResponseMessage Get(DateTime? date = null, string EID = "all")
        {
            try
            {
                using(ebuilderEntities entities = new ebuilderEntities())
                {
                    string parameters = "";
                    parameters += date == null ? "0" : "1";
                    parameters += EID == "all" ? "0" : "1";

                    var entity = new List<attendance>();

                    switch (parameters)
                    {
                        case "00":
                            entity = entities.attendances.ToList();
                            break;
                        case "01":
                            entity = entities.attendances.Where(a => a.EID == EID).ToList();
                            break;
                        case "10":
                            entity = entities.attendances.Where(a => a.date == date).ToList();
                            break;
                        case "11":
                            entity = entities.attendances.Where(a => a.date == date && a.EID == EID).ToList();
                            break;
                    }

                    List<attendanceWithWorkingHours> attList = new List<attendanceWithWorkingHours>();

                    foreach(var a in entity)
                    {
                        attendanceWithWorkingHours att = new attendanceWithWorkingHours(a);
                        attList.Add(att);
                    }
                    return Request.CreateResponse(HttpStatusCode.OK, attList);                   
                }
            }
            catch(Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ex);
            }
        }


        /// <summary>
        ///  Get the attendance of an EID within a given range
        /// </summary>
        /// <param name="EID"></param>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <returns></returns>
        [HttpGet]
        public HttpResponseMessage Get(string EID, DateTime startDate, DateTime endDate)
        {
            try
            {
                using(ebuilderEntities entities = new ebuilderEntities())
                {
                    var entity = entities.attendances.Where(a => a.EID == EID && (DateTime.Compare(startDate, a.date) < 0 && DateTime.Compare(a.date, endDate) < 0)).ToList();

                    List<attendanceWithWorkingHours> attList = new List<attendanceWithWorkingHours>();

                    foreach (var a in entity)
                    {
                        attendanceWithWorkingHours att = new attendanceWithWorkingHours(a);                        
                        attList.Add(att);
                    }

                    return Request.CreateResponse(HttpStatusCode.OK, attList);
                }
            }
            catch(Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ex);
            }
        }


        /// <summary>
        /// Get the list of attendance of managed employees within a given range by providing EID of the manager as ManagerID
        /// </summary>
        /// <param name="ManagerID"></param>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/Attendance/GetManaged")]
        public HttpResponseMessage GetManaged(string ManagerID, DateTime startDate, DateTime endDate)
        {
            try
            {
                using(ebuilderEntities entities = new ebuilderEntities())
                {
                    var manager = entities.employees.FirstOrDefault(e => e.EID == ManagerID);
                    if (manager == null)
                    {
                        return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "No manager exists with the given EID");
                    }

                    entities.Entry(manager).Collection("employee1").Load();

                    var entity = new List<attendanceWithWorkingHours>();

                    foreach(var emp in manager.employee1)
                    {
                        var attList = entities.attendances.Where(a => a.EID == emp.EID && (DateTime.Compare(startDate, a.date) < 0 && DateTime.Compare(a.date, endDate) < 0)).ToList();
                        foreach( var a in attList)
                        {
                            var attWithWH = new attendanceWithWorkingHours(a);                            
                            entity.Add(attWithWH);
                        }                        
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
        /// Add an attendance to the database
        /// </summary>
        /// <param name="att"></param>
        /// <returns></returns>
        public HttpResponseMessage Post([FromBody] attendance att)
        {
            try
            {
                using (ebuilderEntities entities = new ebuilderEntities())
                {
                    if (att.EID == null || att.checkIn == null || att.checkOut == null)
                    {
                        return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Some fields are empty");
                    }
                    var entity = entities.attendances.FirstOrDefault(a => a.EID == att.EID && a.date == att.date);
                    if(entity!= null)
                    {
                        return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Attendance the given EID and date already exists.");
                    }

                    entities.attendances.Add(att);
                    entities.SaveChanges();
                    var message = Request.CreateResponse(HttpStatusCode.OK, att);
                    message.Headers.Location = new Uri(Request.RequestUri + att.AID.ToString());
                    return message;
                }
            }
            catch(Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, att.ToString(),ex);
            }
        }

        /// <summary>
        /// Delete an attendance
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
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


        [HttpPost]
        [Route("api/Attendance/MinWorkingHours")]
        public HttpResponseMessage MinWorkingHours([FromBody]TimeSpan minWH)
        {
            try
            {
                attendanceWithWorkingHours.MinRequiredWH = minWH;
                return Request.CreateResponse(HttpStatusCode.OK,"Success");
            }
            catch(Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ex);
            }
        }

    }
}
