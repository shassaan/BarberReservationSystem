//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace BarberReservationSystem
{
    using System;
    using System.Collections.Generic;
    
    public partial class reservation
    {
        public int id { get; set; }
        public string uid { get; set; }
        public string uname { get; set; }
        public string profile { get; set; }
        public string services { get; set; }
        public Nullable<System.DateTime> startDate { get; set; }
        public Nullable<System.DateTime> endDate { get; set; }
        public Nullable<int> status { get; set; }
        public string changedBy { get; set; }
    }
}
