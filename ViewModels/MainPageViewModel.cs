using BarberReservationSystem.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BarberReservationSystem.ViewModels
{
    public class MainPageViewModel
    {
        public List<servieRating> servieRatings { get; set; }
        public List<reservation> reservations { get; set; }

        public List<Service> services { get; set; }
        public User currentUser { get; set; }
    }
}