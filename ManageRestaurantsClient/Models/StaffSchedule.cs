using System;
using System.Collections.Generic;

namespace ManageRestaurant.Models
{
    public partial class StaffSchedule
    {
        public int ScheduleId { get; set; }
        public int? UserId { get; set; }
        public int? RestaurantId { get; set; }
        public DateTime ShiftDate { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public string? Description { get; set; }

        public virtual Restaurant? Restaurant { get; set; }
        public virtual User? User { get; set; }
    }
}
