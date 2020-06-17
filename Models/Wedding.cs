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
    public class Wedding
    {
        [Key]
        public int WeddingId {get;set;}
        public int UserId {get;set;}
        public User Host {get;set;}
        [Required(ErrorMessage="Must have a Wedder")]
        [MinLength(2, ErrorMessage="Must be longer then 2 characters")]
        public string Wedder1 {get;set;}
        [Required(ErrorMessage="Must have a Wedder")]
        [MinLength(2, ErrorMessage="Must be longer then 2 characters")]
        public string Wedder2 {get;set;}
        [Required(ErrorMessage="Must have a wedding date")]
        public DateTime Date {get;set;}
        [Required(ErrorMessage="Must have a venue address")]
        public string VenueAddress {get;set;}
        public List<Guest> GuestList {get;set;}
        public DateTime CreatedAt {get;set;} = DateTime.Now;
        public DateTime UpdatedAt {get;set;} = DateTime.Now;

    }
}