﻿
namespace Domain.Entities
{
    public class Contact
    {
        public int ContactId { get; set; }
        public string Name { get; set;}
        public string Email { get; set;}
        public string Message { get; set;}
        public DateTime CreatedDate { get; set; }
    }
}
