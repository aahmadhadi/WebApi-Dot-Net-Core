using System;

namespace BasicWebApi.Models.Entity
{
    public class Book
    {
        public int Id { get; set;}
        public string Title { get; set; }
        public string Genre { get; set; }
        public DateTime Date { get; set; }
    }
}