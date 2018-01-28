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

        //Get Attendance by AID
        public HttpResponseMessage Get(int id)
        {
            using (ebuilderEntities entities = new ebuilderEntities())
            {
                var entity = entities.attendances.FirstOrDefault(a => a.AID == id);
                if(entity != null)
                {
                    attendanceWithWorkingHours att = new attendanceWithWorkingHours();
                    att.AID = entity.AID;
                    att.checkIn = entity.checkIn;
                    att.checkOut = entity.checkOut;
                    att.date = entity.date;
                    att.EID = entity.EID;
                    att.workingHours = GetWorkingHours(entity);

                    return Request.CreateResponse(HttpStatusCode.OK, att);
                }
                else
                {
                    return Request.CreateErrorResponse(HttpStatusCode.NotFound, "Attendance with AID " + id.ToString() + " not found");

                }

            }
        }

        //Get the attendance by date or EID, all parameters are optional
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
                        attendanceWithWorkingHours att = new attendanceWithWorkingHours();
                        att.AID = a.AID;
                        att.checkIn = a.checkIn;
                        att.checkOut = a.checkOut;
                        att.date = a.date;
                        att.EID = a.EID;
                        att.workingHours = GetWorkingHours(a);
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


        //Get the attendance of an EID within a given range
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
                        attendanceWithWorkingHours att = new attendanceWithWorkingHours();
                        att.AID = a.AID;
                        att.checkIn = a.checkIn;
                        att.checkOut = a.checkOut;
                        att.date = a.date;
                        att.EID = a.EID;
                        att.workingHours = GetWorkingHours(a);
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

        //Add an attendance to the database
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

        //Delete an attendance
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

        //Calculate working hours
        [NonAction]
        public static TimeSpan GetWorkingHours(attendance att)
        {
            TimeSpan checkIn = (TimeSpan)att.checkIn;
            TimeSpan checkOut = (TimeSpan)att.checkOut;
            var workingHours = checkOut.Subtract(checkIn);
            return workingHours;
        }

    }
}
