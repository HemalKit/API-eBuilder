﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using DataAccess;
using API_eBuilder.Models;
using System.Net.Mail;

namespace API_eBuilder.Controllers
{

    public class EmployeesController : ApiController
    {

        /// <summary>
        /// Get an employee's details by giving EID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public HttpResponseMessage Get(string id)
        {
            using (ebuilderEntities entities = new ebuilderEntities())
            {
                var entity = entities.employees.FirstOrDefault(e => e.EID == id);
                if (entity != null)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, entity);
                }
                else
                {
                    return Request.CreateErrorResponse(HttpStatusCode.NotFound, "Employee with the EID " + id + " not Found");
                }

            }
        }

        /// <summary>
        /// Get all the employees or the employees by gender or jobCategory, all parameters are optional
        /// </summary>
        /// <param name="gender"></param>
        /// <param name="jobCategory"></param>
        /// <returns></returns>
        public HttpResponseMessage Get(string gender = "all", string jobCategory = "all")
        {
            try
            {
                using (ebuilderEntities entities = new ebuilderEntities())
                {
                    var parameters = "";
                    parameters += gender == "all" ? "0" : "1";
                    parameters += jobCategory == "all" ? "0" : "1";

                    var entity = new List<employee>();

                    switch (parameters)
                    {
                        case "00":
                            entity = entities.employees.ToList();
                            break;
                        case "01":
                            entity = entities.employees.Where(e => e.jobCategory == jobCategory).ToList();
                            break;
                        case "10":
                            entity = entities.employees.Where(e => e.gender == gender).ToList();
                            break;
                        case "11":
                            entity = entities.employees.Where(e => e.gender == gender && e.jobCategory == jobCategory).ToList();
                            break;
                    }
                    return Request.CreateResponse(HttpStatusCode.OK, entity);
                }
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ex);
            }
        }

        /// <summary>
        /// Add an employee
        /// </summary>
        /// <param name="emp"></param>
        /// <returns></returns>
        public HttpResponseMessage Post([FromBody] employee emp)
        {
            try
            {
                if (emp.EID == null || emp.email == null || emp.fName == null || emp.lName == null || emp.jobCategory == null || emp.gender == null)
                {
                    return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Some fields are empty");
                }
                using (ebuilderEntities entities = new ebuilderEntities())
                {
                    var entity = entities.employees.FirstOrDefault(e => e.EID == emp.EID || e.email == emp.email);
                    if (entity != null)
                    {
                        return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Employee with the given details already exists");
                    }

                    emp.password = Crypto.Hash(emp.password);
                    entities.employees.Add(emp);
                    entities.SaveChanges();
                    var message = Request.CreateResponse(HttpStatusCode.Created, emp);
                    message.Headers.Location = new Uri(Request.RequestUri + emp.EID);
                    return message;
                }
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ex);
            }
        }

        /// <summary>
        /// Delete an employee by providing EID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public HttpResponseMessage Delete(string id)
        {
            try
            {
                using (ebuilderEntities entities = new ebuilderEntities())
                {
                    var entity = entities.employees.FirstOrDefault(e => e.EID == id);
                    if (entity != null)
                    {                        
                        entities.employees.Remove(entity);
                        entities.SaveChanges();
                        return Request.CreateResponse(HttpStatusCode.OK);
                    }
                    else
                    {
                        return Request.CreateErrorResponse(HttpStatusCode.NotFound, "No employee with the EID " + id + " to delete");
                    }
                }
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ex);
            }
        }

        /// <summary>
        /// Update the details of employee
        /// </summary>
        /// <param name="id"></param>
        /// <param name="emp"></param>
        /// <returns></returns>
        public HttpResponseMessage Put(string id, [FromBody]employee emp)
        {
            try
            {
                using (ebuilderEntities entities = new ebuilderEntities())
                {
                    var entity = entities.employees.FirstOrDefault(e => e.EID == id);

                    if (entity == null)
                    {
                        return Request.CreateErrorResponse(HttpStatusCode.NotFound, "Employee with EID " + id + " not found");

                    }
                    else
                    {
                        entity.fName = emp.fName;
                        entity.lName = emp.lName;
                        entity.street = emp.street;
                        entity.homeNo = emp.homeNo;
                        entity.city = emp.city;
                        entity.gender = emp.gender;
                        entity.jobCategory = emp.jobCategory;

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
        /// Change Password by providing EID and old password
        /// </summary>
        /// <param name="credential"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("api/Employees/ChangePassword")]
        public HttpResponseMessage PutPassword(changePasswordCredential credential)
        {
            try
            {
                using (ebuilderEntities entities = new ebuilderEntities())
                {
                    var entity = entities.employees.FirstOrDefault(e => e.EID == credential.EID);
                    if (entity == null)
                    {
                        return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "No employee exists with the given EID");
                    }
                    if (string.Compare(entity.password, Crypto.Hash(credential.oldPassword)) == 0)
                    {
                        entity.password = Crypto.Hash(credential.newPassword);

                        entities.SaveChanges();
                        return Request.CreateResponse(HttpStatusCode.OK, "Change Success");
                    }
                    else
                    {
                        return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Wrong Password");
                    }
                }
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ex);
            }

        }


        /// <summary>
        /// Send an verification code to the email to reset password
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("api/Employees/ForgotPassword")]
        public HttpResponseMessage ForgotPassword([FromBody]string email)
        {
            string verificationCode = Guid.NewGuid().ToString().Substring(0, 8);
            using (ebuilderEntities entities = new ebuilderEntities())
            {
                var entity = entities.employees.FirstOrDefault(e => e.email == email);
                if (entity == null)
                {
                    return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "No existing employee for the given email");
                }
                entity.activationCode = verificationCode;
                entities.SaveChanges();
            }
            var fromEmail = new MailAddress("technocoders.ebuilder@gmail.com");
            var toEmail = new MailAddress(email);
            var a = "";
            string subject = "Forgot Password";

            string body = "The verification code to get access again is <br/><h2>" + verificationCode + "</h2>";

            var smtp = new SmtpClient
            {
                Host = "smtp.gmail.com",
                Port = 587,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(fromEmail.Address, a)
            };

            using (var message = new MailMessage(fromEmail, toEmail)
            {
                Subject = subject,
                Body = body,
                IsBodyHtml = true
            })
                smtp.Send(message);
            return Request.CreateResponse(HttpStatusCode.OK, "Email with a verification code was sent to the given email.");
        }

        /// <summary>
        /// Reset password when forgot password by providing verification code sent in ForgotPassword action
        /// </summary>
        /// <param name="credential"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("api/Employees/ResetPassword")]
        public HttpResponseMessage ResetPassword(forgotPasswordCredential credential)
        {
            try
            {
                using(ebuilderEntities entities = new ebuilderEntities())
                {
                    var entity = entities.employees.FirstOrDefault(e => e.email == credential.email);
                    if (entity == null)
                    {
                        return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Bad Credentials");
                    }
                    if (entity.activationCode == credential.verificationCode)
                    {
                        entity.password = Crypto.Hash(credential.newPassword);
                        entities.SaveChanges();
                        return Request.CreateResponse(HttpStatusCode.OK, "Reset Success");
                    }
                    else
                    {
                        return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Bad Crededntials");
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
