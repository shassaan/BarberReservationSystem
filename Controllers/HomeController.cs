using BarberReservationSystem.Models;
using BarberReservationSystem.ViewModels;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BarberReservationSystem.Controllers
{
    public class HomeController : Controller
    {
        private readonly barberEntities _db = new barberEntities();

        /* 
            on load this method will be called
            and I am sending all data including "ratings",
            "today's reservations" and "current 
            user data (id,name,picture)"
        */
        public ActionResult Index(string userId = "",string uName = "" , string profile = "")
        {
            //validate that user creditials are valid or not
            if (userId == "" || uName == "" || profile == "")
            {
                return View("Error");
            }
            //initializing the current user with the data coming through GET request
            var u = new User { userId   = userId, userName = uName, dp = profile };
            //Getting all reservations for today 
            var res = _db.reservations.Where(r => r.startDate.Value.Day == DateTime.Now.Day
                                            && r.startDate.Value.Month == DateTime.Now.Month
                                            && r.startDate.Value.Year == DateTime.Now.Year
            ).ToList();
            //Getting the all types of services barber is providing
            var serv = _db.Services.ToList();
            //Grouping the names of services with its rating average
            //and the number of  ratings on which the average is based
            var ratings = _db.servieRatings
            .GroupBy(s => s.serviceType)
            .Select(r => new
            {
                serviceType = r.Key,
                count = r.Count(),
                rating = r.Average(a => a.rating)
            }).ToList();
            //as the group by query above returns list of anonymous objects so I am 
            //creating the list of service ratings and copying group by results to service
            //rating list
            List<servieRating> serviceRatings = new List<servieRating>();
            var allServiceRatings = _db.servieRatings.ToList();
            foreach (var rat in ratings)
            {
                serviceRatings.Add(new servieRating
                {
                    rating = rat.rating,
                    serviceType = $"{rat.serviceType};{rat.count}"//concating rating with its count 
                }) ;
            }


            //now wraping all above data in a single view model and sending to the view
            var model = new MainPageViewModel
            {
                currentUser = u,
                reservations = res,
                services = serv,
                servieRatings = serviceRatings,
                allRatings = allServiceRatings
            };

            return View(model);
        }

        [HttpPost]
        public ActionResult SaveRatings(List<servieRating> servieRatings)
        {
            _db.servieRatings.AddRange(servieRatings);
            _db.SaveChanges();
            return Json("",JsonRequestBehavior.AllowGet);
        }

        public ActionResult SaveReservation(reservation reservation)
        {
            /*
             * DateTime Slot should'nt be between the start and end date of 
             * all today's reservations that is why verifying
               that datetime  
             */
            var result = _db.reservations.Where(r => r.endDate.Value.Day >= reservation.startDate.Value.Day
                                            && r.endDate.Value.Month >= reservation.startDate.Value.Month
                                            && r.endDate.Value.Year >= reservation.startDate.Value.Year
                                            && r.endDate.Value.Hour >= reservation.startDate.Value.Hour
                                            && r.endDate.Value.Minute >= reservation.startDate.Value.Minute
                                            && r.startDate.Value.Day <= reservation.startDate.Value.Day
                                            && r.startDate.Value.Month <= reservation.startDate.Value.Month
                                            && r.startDate.Value.Year <= reservation.startDate.Value.Year
                                            && r.startDate.Value.Hour <= reservation.startDate.Value.Hour
                                            && r.startDate.Value.Minute <= reservation.startDate.Value.Minute
                                            && r.status == 1
                                            ).FirstOrDefault();
            //validating fields entered by user
            if (string.IsNullOrWhiteSpace(reservation.startDate.ToString())
                ||
                string.IsNullOrWhiteSpace(reservation.services)
                )
            {
                return Json("Please Provide Complete Data.");
            }
            else if (reservation.startDate < DateTime.Now)
            {
                return Json("Selected Date Time slot has Passed.");
            }
            else if(result != null)
            {
                return Json("Already the appointment is reserved in the selected Date Time Slot.");
            }
            else
            {
                reservation.endDate = reservation.startDate;
                //adding 1 hour by default 
                reservation.endDate = reservation.endDate.Value.AddMinutes(60);
                /*
                  if status is "1" then reservation is "schecduled"

                  if status is "2" then reservation  is "in progress"

                  if status is "3" then reservation is "delivered"

                  if status is "0" then reservation is "cancelled"
                 
                 */
                reservation.status = 1;
                _db.reservations.Add(reservation);
                _db.SaveChanges();
                return Json("1");//sending "1" as a success bit
            }

            
        }

        /*
         This method takes "status code" and "id" of reservation
         to change the status to "in Progress" or "scheduled" etc
             */
        public ActionResult ChangeStatus(int status = -1,int id = -1)
        {
            //validating the request
            if (status == -1 || id == -1)
            {
                return Json("err",JsonRequestBehavior.AllowGet);
            }
            else
            {
                //updating the reservation status
                var res = _db.reservations.Where(r => r.id == id).FirstOrDefault();
                res.status = status;
                _db.Entry(res).State = EntityState.Modified;
                _db.SaveChanges();
                return Json("ok", JsonRequestBehavior.AllowGet);
            }
        }
        
        
    }
}