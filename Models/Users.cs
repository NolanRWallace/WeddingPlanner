using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WeddingPlaner.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WeddingPlaner.Models
{
    public class User
    {
        [Key]
        public int UserId {get;set;}
        [Required(ErrorMessage="Must have a first name")]
        [MinLength(2,ErrorMessage="Must be longer than 2 characters")]
        public string FirstName {get;set;}
        [Required(ErrorMessage="Must have a last name")]
        [MinLength(2,ErrorMessage="Must be longer than 2 characters")]
        public string LastName {get;set;}
        [Required(ErrorMessage="Must have a Email Address")]
        [EmailAddress(ErrorMessage="Must be a valid email address")]
        public string Email {get;set;}
        [Required(ErrorMessage="Must have a password")]
        [MinLength(8,ErrorMessage="Password must be at least 8 characters long")]
        [DataType(DataType.Password)]
        public string Password {get;set;}
        [NotMapped]
        [Compare("Password")]
        [DataType(DataType.Password)]
        public string Confirm {get;set;}
        public List<Guest> UserReservations {get;set;}
        public DateTime CreatedAt {get;set;} = DateTime.Now;
        public DateTime UpdatedAt {get;set;} = DateTime.Now;

    }
}