using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication1.Models.DTO
{
    public class SupplierDTO
    {
        public long Id { get; set; }
        public string FullName { get; set; }
        public string First { get; set; }
        public string Last { get; set; }
        public Name name { get; set; }
        //public DateTime BirthDay { get; set; }
        public string Telephone { get; set; }
      

        public long Name_Id { get; set; }
    }
}