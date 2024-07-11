using System;
using System.Collections.Generic;

namespace ManageRestaurant.Models
{
    public partial class Table
    {
        public Table()
        {
            Reserveds = new HashSet<Reserved>();
        }

        public int TableId { get; set; }
        public int? RestaurantId { get; set; }
        public int TableNumber { get; set; }
        public int Capacity { get; set; }
        public string? Status { get; set; }

        public virtual Restaurant? Restaurant { get; set; }
        public virtual ICollection<Reserved> Reserveds { get; set; }
    }
}
