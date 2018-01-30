﻿using API_eBuilder.Models;
using DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace API_eBuilder.Controllers
{
    public class LeavesController : ApiController
    {
        /// <summary>
        /// Get the leave by providing LID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public HttpResponseMessage Get(int id)
        {
            using (ebuilderEntities entities = new ebuilderEntities())
            {
                var entity = entities.leavs.FirstOrDefault(l => l.LID == id);
                if (entity != null)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, new { leaves = entity });
                }
                else
                {
                    return Request.CreateErrorResponse(HttpStatusCode.NotFound, "Leave with the LID " + id.ToString() + " not Found");
                }

            }
        }

        /* public HttpResponseMessage Get(DateTime? date = null, string leaveCategory="all", string jobCategory ="all", string EID = "all")
         {
             try
             {
                 using (ebuilderEntities entities = new ebuilderEntities())
                 {
                     string parameters = "";
                     parameters += date == null ? "0" : "1";
                     parameters += leaveCategory == "all" ? "0" : "1";
                     parameters += jobCategory == "all" ? "0" : "1";
                     parameters += EID == "all" ? "0" : "1";

                     var entity = new List<leav>() ;
                     switch (parameters)
                     {
                         case "0000":                         
                              entity = entities.leavs.ToList();
                              break;                            
                         case "0001":
                             entity = entities.leavs.Where(l => l.EID == EID).ToList();
                             break;
                         case "0010":                       
                             entity = entities.leavs.Where(l => l.jobCategory == jobCategory).ToList();
                             break;
                         case "0011":                       
                             entity = entities.leavs.Where(l => l.jobCategory == jobCategory && l.EID == EID).ToList();
                             break;
                         case "0100":                                             
                             entity = entities.leavs.Where(l => l.leaveCategory == leaveCategory).ToList();
                             break;
                         case "0101":                                                
                             entity = entities.leavs.Where(l => l.leaveCategory == leaveCategory && l.EID == EID).ToList();
                             break;
                         case "0110":
                             entity = entities.leavs.Where(l => l.leaveCategory == leaveCategory && l.jobCategory == jobCategory).ToList();
                             break;
                         case "0111":
                             entity = entities.leavs.Where(l => l.leaveCategory == leaveCategory && l.jobCategory == jobCategory && l.EID == EID).ToList();
                             break;
                         case "1000":
                             entity = entities.leavs.Where(l => l.date == date).ToList();
                             break;
                         case "1001":
                             entity = entities.leavs.Where(l => l.date == date && l.EID == EID).ToList();
                             break;
                         case "1010":
                             entity = entities.leavs.Where(l =>  l.date == date && l.jobCategory == jobCategory).ToList();
                             break;
                         case "1011":
                             entity = entities.leavs.Where(l => l.date == date && l.jobCategory == jobCategory && l.EID == EID).ToList();
                             break;
                         case "1100":
                             entity = entities.leavs.Where(l => l.date == date && l.leaveCategory == leaveCategory).ToList();
                             break;
                         case "1101":
                             entity = entities.leavs.Where(l => l.date == date && l.leaveCategory == leaveCategory && l.EID == EID).ToList();
                             break;
                         case "1111":
                             entity = entities.leavs.Where(l =>l.date == date && l.leaveCategory == leaveCategory && l.jobCategory == jobCategory && l.EID == EID).ToList();
                             break;
                     }
                     return Request.CreateResponse(HttpStatusCode.OK, entity);
                 }
             }
             catch(Exception ex)
             {
                 return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ex);
             }
         }  */

        /// <summary>
        /// Get list of accepted leaves for a given EID within a given range
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
                    var allLeaves = entities.leavs.Where(l => l.EID == EID && (DateTime.Compare(startDate, l.date) < 0 && DateTime.Compare(l.date, endDate) < 0)).ToList();
                    var entity = new List<leavWithStatusAndName>();
                    foreach( leav l in allLeaves)
                    {
                        foreach(approval app in entities.approvals.Where(a => a.LID == l.LID).ToList())
                        {
                            if(app.status == "accepted")
                            {
                                var levStatusName = new leavWithStatusAndName(l);                                
                                entity.Add(levStatusName); //select only the accepted leaves
                            }
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
        /// Get the available leaves left for the relevant time
        /// </summary>
        /// <param name="EID"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/Leaves/GetAvailable")]
        public HttpResponseMessage GetLeaves(string EID)
        {
            try
            {
                using(ebuilderEntities entities = new ebuilderEntities())
                {
                    var jCategory = entities.employees.FirstOrDefault(e => e.EID == EID).jobCategory;
                    var allLeavesTypes = entities.leave_type.Where(lt => lt.jobCategory == jCategory).ToList();

                    foreach(var lt in allLeavesTypes)
                    {
                        var leavesTakenCount = entities.leavs.Where(l => l.EID == EID && l.leaveCategory == lt.leaveCategory && l.date.Year == DateTime.Now.Year).Count();
                        lt.maxAllowed = lt.maxAllowed - leavesTakenCount;
                    }

                    return Request.CreateResponse(HttpStatusCode.OK, allLeavesTypes);
                }
            }
            catch(Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ex);
            }
        }


        /// <summary>
        /// Get the left and taken leave counts for the relevant time
        /// </summary>
        /// <param name="EID"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/Leaves/LeaveCount")]
        public HttpResponseMessage GetLeaveCount(string EID)
        {
            try
            {
                using(ebuilderEntities entities = new ebuilderEntities())
                {
                    var leavesApplied = entities.leavs.Where(l => l.EID == EID && l.date.Year == DateTime.Now.Year);
                    allLeaveCount allLeaves= new allLeaveCount();

                    //Get the accepted leaves only
                    var entity = (from l in leavesApplied join ap in entities.approvals on l.LID equals ap.LID where ap.status == "accepted" select l).ToList();

                    var leaveTypes = (from e in entities.employees join lt in entities.leave_type on e.jobCategory equals lt.jobCategory where e.jobCategory == lt.jobCategory  && e.EID == EID select lt).ToList();
                    foreach (var leaveType in leaveTypes)
                    {
                        count leaveCount = new count();
                        leaveCount.name = leaveType.leaveCategory;
                        foreach (var leave in entity)
                        {
                            if (leave.leaveCategory == leaveType.leaveCategory)
                            {
                                leaveCount.number++;                                
                            }
                            
                        }
                        allLeaves.taken.Add(leaveCount);
                        count leftLeaveCount = new count();
                        leftLeaveCount.name = leaveType.leaveCategory;
                        leftLeaveCount.number = leaveType.maxAllowed - leaveCount.number;

                        allLeaves.left.Add(leftLeaveCount);
                    }
                    return Request.CreateResponse(HttpStatusCode.OK, allLeaves);
                }
            }
            catch(Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest,ex);
            }
        }


        /// <summary>
        /// Get all the leaves of managed employees of a manager by providing EID of Manager
        /// </summary>
        /// <param name="ManagerID"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/Leaves/GetManaged")]
        public HttpResponseMessage GetManaged(string ManagerID)
        {
            try
            {
                using(ebuilderEntities entities = new ebuilderEntities())
                {
                    var manager = entities.employees.FirstOrDefault(e => e.EID == ManagerID);

                    if(manager == null)
                    {
                        return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "No Manager exists for the given EID");
                    }
                    entities.Entry(manager).Collection("employee1").Load();

                    var entity = new List<leavWithStatusAndName>();
                    foreach(var emp in manager.employee1)
                    {
                        var empLeaves = entities.leavs.Where(e => e.EID == emp.EID).ToList();
                        foreach(leav l in empLeaves)
                        {
                            var leavNameStatus = new leavWithStatusAndName(l);
                            entity.Add(leavNameStatus);
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
        /// Get a array of number of leaves applied in a day of a week in all months of the year
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("api/Leaves/LeavesAppliedWeekly")]
        public HttpResponseMessage GetLeavesWeekly()
        {
            try
            {
                using(ebuilderEntities entities = new ebuilderEntities())
                {
                    var allLeaves = entities.leavs.Where(l => l.date.Year == DateTime.Now.Year).ToList();
                    var leavePercentageByWeeklyDay = new appliedLeavesPercentView();
                    for(int dayOfWeek=1; dayOfWeek <= 7; dayOfWeek++)
                    {
                        switch (dayOfWeek)
                        {
                            case 1:
                                int leaveCountMonday = allLeaves.Where(l => l.date.DayOfWeek == DayOfWeek.Monday).ToList().Count;
                                leavePercentageByWeeklyDay.Monday = Math.Round((((double)leaveCountMonday) / allLeaves.Count * 100),2);
                                //leavePercentageByWeeklyDay.Add(new { Monday = MondayPercent });
                                break;
                            case 2:
                                int leaveCountTuesday = allLeaves.Where(l => l.date.DayOfWeek == DayOfWeek.Tuesday).ToList().Count;
                                leavePercentageByWeeklyDay.Tuesday = Math.Round((((double)leaveCountTuesday) / allLeaves.Count * 100),2);
                                //leavePercentageByWeeklyDay.Add(new { Tuesday = TuesdayPercent });
                                break;
                            case 3:
                                int leaveCountWednesday = allLeaves.Where(l => l.date.DayOfWeek == DayOfWeek.Wednesday).ToList().Count;
                                leavePercentageByWeeklyDay.Wednesday = Math.Round((((double)leaveCountWednesday) / allLeaves.Count * 100),2);
                                //leavePercentageByWeeklyDay.Add(new { Wednesday = WednesdayCount });
                                break;
                            case 4:
                                int leaveCountThursday = allLeaves.Where(l => l.date.DayOfWeek == DayOfWeek.Thursday).ToList().Count;
                                leavePercentageByWeeklyDay.Thursday = Math.Round((((double)leaveCountThursday) / allLeaves.Count * 100),2);
                                //leavePercentageByWeeklyDay.Add(new { Thursday = ThursdayCount });
                                break;
                            case 5:
                                int leaveCountFriday = allLeaves.Where(l => l.date.DayOfWeek == DayOfWeek.Friday).ToList().Count;
                                leavePercentageByWeeklyDay.Friday = Math.Round((((double)leaveCountFriday) / allLeaves.Count * 100),2);
                                //leavePercentageByWeeklyDay.Add(new { Friday = FridayCount });
                                break;
                            case 6:
                                int leaveCountSaturday = allLeaves.Where(l => l.date.DayOfWeek == DayOfWeek.Saturday).ToList().Count;
                                leavePercentageByWeeklyDay.Saturday = Math.Round((((float)leaveCountSaturday) / allLeaves.Count * 100),2);
                                //leavePercentageByWeeklyDay.Add(new { Saturday = SaturdayCount });
                                break;
                            case 7:
                                int leaveCountSunday = allLeaves.Where(l => l.date.DayOfWeek == DayOfWeek.Sunday).ToList().Count;
                                leavePercentageByWeeklyDay.Sunday = Math.Round((((float)leaveCountSunday) / allLeaves.Count * 100),2);
                                //leavePercentageByWeeklyDay.Add(new { Sunday = Sun });
                                break;
                        }
                    }
                    return Request.CreateResponse(HttpStatusCode.OK, leavePercentageByWeeklyDay);                    
                }
            }
            catch(Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ex);
            }
        }


        /// <summary>
        /// Add a leave for a future date in normal way
        /// </summary>
        /// <param name="leave"></param>
        /// <returns></returns>
        public HttpResponseMessage Post([FromBody]leav leave)
        {
            try
            {
                using (ebuilderEntities entities = new ebuilderEntities())
                {
                    
                    if (DateTime.Compare(leave.date, DateTime.Today) < 0)
                    {
                        return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Invalid field date");
                    }


                    //check whether a leave exists for the given day for the employee
                    var entity = entities.leavs.FirstOrDefault(l => l.EID == leave.EID && l.date == leave.date);
                                                            
                    if (entity != null)
                    {
                        return Request.CreateResponse(HttpStatusCode.BadRequest, "Leave Already exists");
                    }
                    
                    
                    leave.jobCategory = entities.employees.FirstOrDefault(e => e.EID == leave.EID).jobCategory;

                    //Check whether the leaveCategory exists
                    var maxLeaves = entities.leave_type.FirstOrDefault(lt => lt.leaveCategory == leave.leaveCategory && lt.jobCategory == leave.jobCategory).maxAllowed;                    
                    if (maxLeaves == 0)
                    {
                        return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Incompatible leave type");
                    }

                    //check whether the maximum allowed leaves for the period has been taken
                    var leavesTaken = entities.leavs.Where(l => l.EID == leave.EID && l.leaveCategory == leave.leaveCategory && l.date.Year == leave.date.Year).Count();
                    if (leavesTaken >= maxLeaves)
                    {
                        return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "No leaves available");
                    }


                    entities.leavs.Add(leave);                    
                    var man = entities.employees.FirstOrDefault(e => e.EID == leave.EID);

                    entities.Entry(man).Collection("employees").Load();
                    if (man.employees.Any())
                    {
                        foreach (var m in man.employees)
                        {
                            approval app = new approval();
                            app.ManagerID = m.EID;
                            app.LID = leave.LID;
                            app.status = "pending";
                            entities.approvals.Add(app);
                        }
                        var message = Request.CreateResponse(HttpStatusCode.OK, leave);
                        entities.SaveChanges();
                        message.Headers.Location = new Uri(Request.RequestUri + leave.LID.ToString());
                        return message;
                    }
                    else if(leave.jobCategory=="HR Admin")
                    {
                        var message = Request.CreateResponse(HttpStatusCode.OK, leave);
                        entities.SaveChanges();
                        message.Headers.Location = new Uri(Request.RequestUri + leave.LID.ToString());
                        return message;                        
                    }
                    else
                    {
                        var message = Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Error occured");
                        return message;
                    }
                }
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ex);
            }
        }

        /// <summary>
        /// Add a leave for a past date to cover not enough working hours
        /// </summary>
        /// <param name="leave"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/Leaves/ApplyEx")]
        public HttpResponseMessage PostEx([FromBody] leav leave)
        {
            try
            {
                using(ebuilderEntities entities = new ebuilderEntities())
                {
                    
                    if (DateTime.Compare(leave.date, DateTime.Today.AddDays(-30)) < 0)
                    {
                        return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "The date should be within last 30 days.");
                    }

                    //Check whether the a leave eixsts for the fiven date
                    var entity = entities.leavs.FirstOrDefault(l => l.EID == leave.EID && l.date == leave.date);                    
                    if (entity != null)
                    {
                        return Request.CreateResponse(HttpStatusCode.BadRequest, "Leave Already exists");
                    }

                    //get the jobCategory for the leave
                    leave.jobCategory = entities.employees.FirstOrDefault(e => e.EID == leave.EID).jobCategory;

                    //Check whether the leaveCategory exists
                    var maxLeaves = entities.leave_type.FirstOrDefault(lt => lt.leaveCategory == leave.leaveCategory && lt.jobCategory == leave.jobCategory).maxAllowed;
                    if (maxLeaves == 0)
                    {
                        return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Incompatible leave type");
                    }

                    //check whether the maximum allowed leaves for the period has been taken
                    var leavesTaken = entities.leavs.Where(l => l.EID == leave.EID && l.leaveCategory == leave.leaveCategory && l.date.Year == leave.date.Year).Count();
                    if (leavesTaken >= maxLeaves)
                    {
                        return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "No leaves available");
                    }

                    entities.leavs.Add(leave);
                    entities.SaveChanges();

                    return Request.CreateResponse(HttpStatusCode.Created, leave);

                }
            }
            catch(Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ex);
            }

        }

        /// <summary>
        /// Delete a leave by giving the LID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public HttpResponseMessage Delete(int id)
        {
            try
            {
                using (ebuilderEntities entities = new ebuilderEntities())
                {
                    var entity = entities.leavs.FirstOrDefault(l => l.LID == id);
                    if (entity != null)
                    {
                        entities.leavs.Remove(entity);
                        entities.SaveChanges();
                        return Request.CreateResponse(HttpStatusCode.OK);
                    }
                    else
                    {
                        return Request.CreateErrorResponse(HttpStatusCode.NotFound, "No Leave with the LID " + id.ToString() + " to delete");
                    }
                }
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ex);
            }
        }

        /// <summary>
        /// Update the date, leaveCategory and reason by providing LID
        /// </summary>
        /// <param name="id"></param>
        /// <param name="leave"></param>
        /// <returns></returns>
        public HttpResponseMessage Put(int id, [FromBody]leav leave)
        {
            try
            {
                using (ebuilderEntities entities = new ebuilderEntities())
                {
                    var entity = entities.leavs.FirstOrDefault(l => l.LID == id);

                    if (entity == null)
                    {
                        return Request.CreateErrorResponse(HttpStatusCode.NotFound, "Leave with LID " + id.ToString() + " not found");
                    }
                    else
                    {
                        entity.date = leave.date;
                        entity.leaveCategory = leave.leaveCategory;
                        entity.reason = leave.reason;

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
