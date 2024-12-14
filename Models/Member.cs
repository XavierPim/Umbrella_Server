﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Umbrella_Server.Models
{
    public class Member
    {
        [Required]
        public Guid GroupID { get; set; } // FK to Group Table

        [Required]
        public Guid UserID { get; set; } // FK to User Table

        // Store multiple roles for the user in this group as a bitmask
        [Required]
        public int Roles { get; set; } = (int)UserRole.Attendee; // Default role is Attendee

        // Navigation Properties
        public Group Group { get; set; } = null!; // Relationship to Group

        public User User { get; set; } = null!; // Relationship to User
    }
}
