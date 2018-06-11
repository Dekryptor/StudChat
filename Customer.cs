using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Linq.Mapping;

namespace StudChat
{
    public abstract class Customer
    {
        public string ID { get; set; }//ID 
        public string name { get; set; }//имя
        public string surname { get; set; }//фамилия
        public string nickname { get; set; }
        public string password { get; set; }//пароль
    }
}
