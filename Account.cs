using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Linq.Mapping;

namespace StudChat
{
    //Класс объекта текущего пользователя
    public class Account:Customer
    {
        public Account(string id,string Name,string Surname,string Nickname,string Password)
        {
            ID = id;
            name = Name;
            surname = Surname;
            nickname = Nickname;
            password = Password;
        }
    }
}
