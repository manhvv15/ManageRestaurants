using System;
using System.Collections.Generic;

namespace ManageRestaurant.Models
{
    public partial class Promotion
    {
        public int PromotionId { get; set; }
        public int? RestaurantId { get; set; }
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public int? DiscountPercentage { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public virtual Restaurant? Restaurant { get; set; }
    }
}
