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

        /// <summary>
        /// Get the duty leave by providing DLID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Get the list of duty leaves of an employees or all employees within a time range. All parameters are optional
        /// </summary>
        /// <param name="EID"></param>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <returns></returns>
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


        /// <summary>
        /// Get the list of duty leaves of employees managed by a manager when EID of manager is given. Range is optional
        /// </summary>
        /// <param name="EID"></param>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <returns></returns>
        [Route("api/DutyLeaves/GetManaged")]
        public HttpResponseMessage GetManaged(string EID, DateTime? startDate = null, DateTime? endDate = null)
        {
            try
            {
                using(ebuilderEntities entities = new ebuilderEntities())
                {
                    var entity = new List<duty_leave>();
                    var parameters = "";
                    parameters += startDate == null ? "0" : "1";
                    parameters += endDate == null ? "0" : "1";

                    //Get the manager for the given EID
                    var manager = entities.employees.FirstOrDefault(e => e.EID == EID);

                    if(manager == null)
                    {
                        return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "No employee available for the given EID");
                    }

                    //Get the list of employees managed by the manager
                    entities.Entry(manager).Collection("employee1").Load();

                    foreach(var e in manager.employee1)
                    {
                        switch (parameters)
                        {
                            case "00":
                                entities.duty_leave.Where(dl => dl.EID == e.EID).ToList().ForEach( dulv=> entity.Add(dulv));
                                break;
                            case "01":
                                entities.duty_leave.Where(dl => dl.EID == e.EID && DateTime.Compare(dl.date, (DateTime)endDate) < 0).ToList().ForEach(dulv=>entity.Add(dulv));
                                break;
                            case "10":
                                entities.duty_leave.Where(dl => dl.EID == e.EID && DateTime.Compare((DateTime)startDate, dl.date) < 0).ToList().ForEach(dulv => entity.Add(dulv));
                                break;
                            case "11":
                                entities.duty_leave.Where(dl => dl.EID == e.EID && DateTime.Compare((DateTime)startDate, dl.date) < 0 && DateTime.Compare(dl.date, (DateTime)endDate) < 0).ToList().ForEach(dulv => entity.Add(dulv));
                                break;
                        }
                    }
                    List<dutyLeaveWithName> dlNameList = new List<dutyLeaveWithName>();
                    foreach (var e in entity)
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

                    return Request.CreateResponse(HttpStatusCode.OK, dlNameList);

                }
            }
            catch(Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ex);
            }
        }

        /// <summary>
        /// Add an dutyleave. One dutyleave is allowed for one employee in a single day
        /// </summary>
        /// <param name="dutyLeave"></param>
        /// <returns></returns>
        public HttpResponseMessage Post([FromBody] duty_leave dutyLeave)
        {
            try
            {
                using (ebuilderEntities entities = new ebuilderEntities())
                {
                    if (dutyLeave.EID == null || dutyLeave.location == null || dutyLeave.date == null)
                    {
                        return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Some fieds are empty");
                    }

                    if (DateTime.Compare(dutyLeave.date,DateTime.Today)<0 || TimeSpan.Compare(dutyLeave.appointmentTime,dutyLeave.endTime)>=0)
                    {
                        return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Invalid date or time");
                    }
                    var entity = entities.duty_leave.FirstOrDefault(dl => dl.EID == dutyLeave.EID && dl.date == dutyLeave.date);
                    if (entity != null)
                    {
                        return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Duty leave for given EID and date alredy exists");
                    }

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

        /// <summary>
        /// Delete a dutyleave by providing DLID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Update appointmnet time, date and end time of a dutyleave by providing DLID
        /// </summary>
        /// <param name="id"></param>
        /// <param name="dutyLeave"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Get the duty leaves percentage by weekday
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("api/DutyLeaves/Weekly")]
        public HttpResponseMessage GetWeekly()
        {
            try
            {
                using (ebuilderEntities entities = new ebuilderEntities())
                {
                    var allDutyLeaves = entities.duty_leave.Where(l => l.date.Year == DateTime.Now.Year).ToList();
                    var dutyLeavePercentageByWeeklyDay = new appliedLeavesPercentView();
                    for (int dayOfWeek = 1; dayOfWeek <= 7; dayOfWeek++)
                    {
                        switch (dayOfWeek)
                        {
                            case 1:
                                int leaveCountMonday = allDutyLeaves.Where(l => l.date.DayOfWeek == DayOfWeek.Monday).ToList().Count;
                                dutyLeavePercentageByWeeklyDay.Monday = Math.Round((((double)leaveCountMonday) / allDutyLeaves.Count * 100), 2);
                                break;
                            case 2:
                                int leaveCountTuesday = allDutyLeaves.Where(l => l.date.DayOfWeek == DayOfWeek.Tuesday).ToList().Count;
                                dutyLeavePercentageByWeeklyDay.Tuesday = Math.Round((((double)leaveCountTuesday) / allDutyLeaves.Count * 100), 2);
                                break;
                            case 3:
                                int leaveCountWednesday = allDutyLeaves.Where(l => l.date.DayOfWeek == DayOfWeek.Wednesday).ToList().Count;
                                dutyLeavePercentageByWeeklyDay.Wednesday = Math.Round((((double)leaveCountWednesday) / allDutyLeaves.Count * 100), 2);
                                break;
                            case 4:
                                int leaveCountThursday = allDutyLeaves.Where(l => l.date.DayOfWeek == DayOfWeek.Thursday).ToList().Count;
                                dutyLeavePercentageByWeeklyDay.Thursday = Math.Round((((double)leaveCountThursday) / allDutyLeaves.Count * 100), 2);
                                break;
                            case 5:
                                int leaveCountFriday = allDutyLeaves.Where(l => l.date.DayOfWeek == DayOfWeek.Friday).ToList().Count;
                                dutyLeavePercentageByWeeklyDay.Friday = Math.Round((((double)leaveCountFriday) / allDutyLeaves.Count * 100), 2);
                                break;
                            case 6:
                                int leaveCountSaturday = allDutyLeaves.Where(l => l.date.DayOfWeek == DayOfWeek.Saturday).ToList().Count;
                                dutyLeavePercentageByWeeklyDay.Saturday = Math.Round((((double)leaveCountSaturday) / allDutyLeaves.Count * 100), 2);
                                break;
                            case 7:
                                int leaveCountSunday = allDutyLeaves.Where(l => l.date.DayOfWeek == DayOfWeek.Sunday).ToList().Count;
                                dutyLeavePercentageByWeeklyDay.Sunday = Math.Round((((double)leaveCountSunday) / allDutyLeaves.Count * 100), 2);
                                break;
                        }
                    }
                    return Request.CreateResponse(HttpStatusCode.OK, dutyLeavePercentageByWeeklyDay);
                }
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ex);
            }
        }


        /// <summary>
        /// Get the duty keaves percentage by month
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("api/DutyLeaves/Monthly")]
        public HttpResponseMessage GetMonthly()
        {
            try
            {
                using (ebuilderEntities entities = new ebuilderEntities())
                {
                    var allDutyLeaves = entities.duty_leave.Where(l => l.date.Year == DateTime.Now.Year).ToList();
                    var dutyLeavePercentageByMonth = new appliedLeavesPercentViewMonthly();

                    for (int month = 1; month <= 12; month++)
                    {
                        switch (month)
                        {
                            case 1:
                                int leaveCountJan = allDutyLeaves.Where(l => l.date.Month == 1).ToList().Count;
                                dutyLeavePercentageByMonth.January = Math.Round(((double)leaveCountJan / allDutyLeaves.Count * 100), 2);
                                break;
                            case 2:
                                int leaveCountFeb = allDutyLeaves.Where(l => l.date.Month == 2).ToList().Count;
                                dutyLeavePercentageByMonth.February = Math.Round(((double)leaveCountFeb / allDutyLeaves.Count * 100), 2);
                                break;
                            case 3:
                                int leaveCountMar = allDutyLeaves.Where(l => l.date.Month == 3).ToList().Count;
                                dutyLeavePercentageByMonth.March = Math.Round(((double)leaveCountMar / allDutyLeaves.Count * 100), 2);
                                break;
                            case 4:
                                int leaveCountApr = allDutyLeaves.Where(l => l.date.Month == 4).ToList().Count;
                                dutyLeavePercentageByMonth.April = Math.Round(((double)leaveCountApr / allDutyLeaves.Count * 100), 2);
                                break;
                            case 5:
                                int leaveCountMay = allDutyLeaves.Where(l => l.date.Month == 5).ToList().Count;
                                dutyLeavePercentageByMonth.May = Math.Round(((double)leaveCountMay / allDutyLeaves.Count * 100), 2);
                                break;
                            case 6:
                                int leaveCountJun = allDutyLeaves.Where(l => l.date.Month == 6).ToList().Count;
                                dutyLeavePercentageByMonth.June = Math.Round(((double)leaveCountJun / allDutyLeaves.Count * 100), 2);
                                break;
                            case 7:
                                int leaveCountJul = allDutyLeaves.Where(l => l.date.Month == 7).ToList().Count;
                                dutyLeavePercentageByMonth.July = Math.Round(((double)leaveCountJul / allDutyLeaves.Count * 100), 2);
                                break;
                            case 8:
                                int leaveCountAug = allDutyLeaves.Where(l => l.date.Month == 8).ToList().Count;
                                dutyLeavePercentageByMonth.August = Math.Round(((double)leaveCountAug / allDutyLeaves.Count * 100), 2);
                                break;
                            case 9:
                                int leaveCountSep = allDutyLeaves.Where(l => l.date.Month == 9).ToList().Count;
                                dutyLeavePercentageByMonth.September = Math.Round(((double)leaveCountSep / allDutyLeaves.Count * 100), 2);
                                break;
                            case 10:
                                int leaveCountOct = allDutyLeaves.Where(l => l.date.Month == 10).ToList().Count;
                                dutyLeavePercentageByMonth.October = Math.Round(((double)leaveCountOct / allDutyLeaves.Count * 100), 2);
                                break;
                            case 11:
                                int leaveCountNov = allDutyLeaves.Where(l => l.date.Month == 11).ToList().Count;
                                dutyLeavePercentageByMonth.November = Math.Round(((double)leaveCountNov / allDutyLeaves.Count), 2);
                                break;
                            case 12:
                                int leaveCountDec = allDutyLeaves.Where(l => l.date.Month == 12).ToList().Count;
                                dutyLeavePercentageByMonth.December = Math.Round(((double)leaveCountDec / allDutyLeaves.Count * 100), 2);
                                break;
                        }
                    }
                    return Request.CreateResponse(HttpStatusCode.OK, dutyLeavePercentageByMonth);
                }
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ex);
            }
        }



    }

}
