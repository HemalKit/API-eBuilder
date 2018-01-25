﻿using DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using API_eBuilder.Models;

namespace API_eBuilder.Controllers
{
    public class TrackingsController : ApiController
    {
        public IEnumerable<tracking> Get()
        {
            using(ebuilderEntities entities = new ebuilderEntities())
            {
                return entities.trackings.ToList();
            }
        }

        public HttpResponseMessage Get(int DLID )
        {
            try
            {
                using(ebuilderEntities entities = new ebuilderEntities())
                {
                    var entity = entities.trackings.Where(t => t.DLID == DLID).ToList();
                    if(entity != null)
                    {
                        return Request.CreateResponse(HttpStatusCode.OK, entity);
                    }
                    else
                    {
                        return Request.CreateResponse(HttpStatusCode.NotFound, "Trackings for DLID " + DLID.ToString() + "not found");
                    }
                }
            }
            catch(Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ex);
            }
        }


        public HttpResponseMessage Post([FromBody]trackingWithEID tr)
        {
            try
            {
                using (ebuilderEntities entities = new ebuilderEntities())
                {
                    var DLID = entities.duty_leave.FirstOrDefault(dl => dl.EID == tr.EID && dl.date.Year == DateTime.Now.Year
                     && dl.date.Month == DateTime.Now.Month && dl.date.Day == DateTime.Now.Day).DLID;
                    tracking newTracking = new tracking();
                    newTracking.DLID = DLID;
                    newTracking.latitude = tr.latitude;
                    newTracking.longitude = tr.longitude;
                    newTracking.time = new TimeSpan(DateTime.Now.Hour+5, DateTime.Now.Minute+30, DateTime.Now.Second);


                    entities.trackings.Add(newTracking);
                    entities.SaveChanges();
                    var message = Request.CreateResponse(HttpStatusCode.OK, newTracking);
                    message.Headers.Location = new Uri(Request.RequestUri + tr.TRID.ToString());
                    return message;
                }
            }
            catch(Exception ex)
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
                    var entity = entities.trackings.FirstOrDefault(t => t.TRID == id);
                    if(entity != null)
                    {
                        entities.trackings.Remove(entity);
                        entities.SaveChanges();
                        return Request.CreateResponse(HttpStatusCode.OK);
                    }
                    else
                    {
                        return Request.CreateResponse(HttpStatusCode.NotFound, "Tracking with TRID " + id.ToString() + " not found to delete");
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
