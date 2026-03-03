using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiModels
{
    public class Login
    {
        public string UserName { get; set; }
        public string UserPassword { get; set; }
        public string Device { get; set; }
    }
}
