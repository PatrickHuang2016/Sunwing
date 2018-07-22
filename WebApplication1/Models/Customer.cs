//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace WebApplication1.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public partial class Customer : Person
    {
        [Required(ErrorMessage = "Please input birthDay")]
        public Nullable<System.DateTime> BirthDay { get; set; }
        [Required(ErrorMessage ="Please input email")]
        [EmailAddress(ErrorMessage ="Not a valid email")]
        public string Email { get; set; }
    }
}
