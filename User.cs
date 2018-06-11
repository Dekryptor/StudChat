using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Linq.Mapping;

namespace StudChat
{
    //Класс объекта пользователя
    public class User:Customer
    {
        public bool friend;//отношения с текущим пользователем
         public User(string id, string Name, string Surname, string Nickname, string Password,bool friendship)
         {
             ID = id;
             name = Name;
             surname = Surname;
             nickname = Nickname;
             password = Password;
             friend = friendship;
         }
    }
}
