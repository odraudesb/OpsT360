using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace MiWebApi
{
    public class Login
    {
        [Required]
        public string UserName { get; set; }
        [Required]
        public string UserPassword { get; set; }
       
        public string Device { get; set; }

     

    }
}