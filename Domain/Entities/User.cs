﻿using Microsoft.AspNetCore.Identity;

namespace Domain.Entities
{
    public class User : IdentityUser
    {
        public string LastName { get; set; }
        public string FirstName { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string? ImageUrl { get; set; }
        public DateTime CreatedDate { get; set; }
        public bool IsActive { get; set; }

        public bool IsAuthenticated { get; set; }
        //public ICollection<UserRole>? UserRoles { get; set; }
    }
}
