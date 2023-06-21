using Domain.Entities;

namespace Contracts.DTOs
{
    public class UserDto
    {
        public string LastName { get; set; }
        public string FirstName { get; set; }
        public string UserName { get; set; }
        //public string Address { get; set; }
        //public string City { get; set; }
        public string Email { get; set; }
        public string State { get; set; }
        public string PhoneNumber { get; set; }
        public string? ImageUrl { get; set; }
        public DateTime CreatedDate { get; set; }
        //public ICollection<UserRole> UserRoles { get; set; }
        public bool IsActive { get; set; }
    }
}
