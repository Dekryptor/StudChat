using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using System.Data;
using System.Data.Linq;
using System.IO;


namespace StudChat
{
    public class MySQLConnection
    {
        
        public MySQLConnection()//Установка соединения
        {
            //Определение IP-адреса сервера базы данных
            string Host = System.Net.Dns.GetHostName();
            string IP = System.Net.Dns.GetHostByName(Host).AddressList[0].ToString();
            string server_info = "server="+IP+";user=monty;database=users;password=some_pass;";
            string server_info1 = "server="+IP+";user=monty;database=users;password=some_pass;";
            //Создание подключений
            connection = new MySqlConnection(server_info);
            con = new MySqlConnection(server_info1);

        }

        //Создание учётной записи в базе данных
        public bool CreateAccount(string name,string surname,string nickname,string password,out string message_box,out string ID )
        {
            string command = "INSERT INTO users.users (name,surname,nickname,password) VALUES ('" + name + "','" + surname + "','" + nickname + "','" + password + "')";
            com = new MySqlCommand
            {
                CommandText = command,
                Connection = connection
            };
            com.Connection.Open();
            //Занесение введённой информации в базу данных и проверка на уникальность
            try
            {
                com.ExecuteNonQuery();
                com.Connection.Close();
            }
            catch(Exception e)
            {
                message_box = "Such name and/or password already exists"+e;
                ID = null;
                com.Connection.Close();
                return false;
            }

            //Получение сгенерированного базой данных ID учётной записи
            command = "SELECT ID FROM users.users WHERE nickname='" + nickname + "' AND password='" + password + "'";//
            com.CommandText = command;
            com.Connection.Open();
            ID=com.ExecuteScalar().ToString();
            com.Connection.Close();
            message_box = "Registration completed successfully!";
            return true;
        }
        //Проверка введённой информации при авторизации уже существующей учётной записи
        public bool CheckInformation( string nickname, string password, out string message_box,out string ID)
        {
             string command = "SELECT name FROM users.users WHERE nickname='"+nickname+"' AND password='"+password+"'";
             com = new MySqlCommand
             {
                 CommandText = command,
                 Connection = connection
             };
             com.Connection.Open();
            //Проверка существования запрашиваемой учётной записи
             try
             {
                 message_box ="Hello, "+ com.ExecuteScalar().ToString();
                 com.Connection.Close();
             }
             catch
             {
                 message_box = "Such account didn't exist";
                 ID = null;
                 com.Connection.Close();
                 return false;             
             }
            //Получение ID учётной записи
            command = "SELECT ID FROM users.users WHERE nickname='" + nickname + "' AND password='" + password + "'";
             com.CommandText = command;
             com.Connection.Open();
             ID = com.ExecuteScalar().ToString();
             com.Connection.Close();
             return true;
        }
        //Создание объектов учетных записей пользователей
        public string[] InitializeUser(string ID)
        {
            string[] buffer = new string[5];
            string command= "SELECT * FROM users.users WHERE ID='" + ID + "'"; ;
             com = new MySqlCommand
             {
                 CommandText = command,
                 Connection = connection
             };
            MySqlDataReader reader;
            com.Connection.Open();
            reader = com.ExecuteReader();
            while (reader.Read())
            {
                for (int i = 0; i < buffer.Length; i++)
                {
                    buffer[i] = reader[i].ToString();
                }
            } 

            reader.Close();
            com.Connection.Close();
            return buffer;
        }
        //Поиск и инициализация объектов пользователей из списка друзей текущего пользователя программы
        public void InitializeFriends(string ID,List<User> users)
        {
            string command = "SELECT * FROM users.friends WHERE user_ID='" + ID + "'";
            List<string[]> friends_buffer = new List<string[]>();
            com = new MySqlCommand
            {
                CommandText = command,
                Connection = con
            };
            MySqlDataReader reader;
            com.Connection.Close();
            com.Connection.Open();

            reader = com.ExecuteReader();
            //Получение данных об ID друзей текущего пользователя из таблицы friends
            while (reader.Read())
            {
                string[] buffer = new string[2];
                for (int i = 0; i < buffer.Length; i++)
                {
                    buffer[i] = reader[i].ToString();
                }
                friends_buffer.Add(buffer);
                //Получение данных о конкретном пользователе из списка друзей
                string[] receiver = InitializeUser(buffer[1]);
                //Пользователь - друг
                bool friend = true;
                //Создание объекта пользователя
                users.Add(new User(receiver[0],receiver[1],receiver[2],receiver[3],receiver[4],friend));
            }
            reader.Close();
            com.Connection.Close();

        }
       //Удаление пользователя из списка друзей
        public void DeleteFromFriends(string ID,string ID2)
        {
            string command = "DELETE FROM users.friends WHERE user_ID='" + ID + "' AND friend_ID='"+ID2+"'";
            com = new MySqlCommand
            {
                CommandText = command,
                Connection = connection
            };
            com.Connection.Open();
            com.ExecuteNonQuery();
            com.Connection.Close();

        }
        //Добавление пользователя в список друзей
        public void AddToFriends(string ID,string ID2)
        {
            string command = "INSERT INTO users.friends (user_ID,friend_ID) VALUES ('" + ID + "','" + ID2 + "')";
            com = new MySqlCommand
            {
                CommandText = command,
                Connection = connection
            };
            try
            {
                com.Connection.Open();
                com.ExecuteNonQuery();
                com.Connection.Close();
            }
            catch(Exception e)
            {

                com.Connection.Close();
            }
          
        }
        //Поиск пользователей по введённым данным
        public List<User> FindUsers(string request,List<User> users,string ID)
        {
            //буффер для хранения информации о пользователях из списка друзей
            List<User> friends_buffer = new List<User>();
            //Разделение введённого запроса по словам
            string[] buf = request.Split(' ');
            string command=null;
            if (buf.Length == 1)//Поиск по имени или фамилии или нику
            {
                command = "SELECT * FROM users.users WHERE name='" + buf[0] + "' OR surname='" + buf[0] + "' OR nickname='" + buf[0] + "'";
            }
            else if (buf.Length == 2)//Поиск по имени и фамилии
            {
                command = "SELECT * FROM users.users WHERE name='" + buf[0] + "' AND surname='" + buf[1] + "'";
            }
            else
            {
                return users;
            }        
            com = new MySqlCommand
            {
                CommandText = command,
                Connection = connection
            };
            MySqlDataReader reader;
            com.Connection.Open();
            reader = com.ExecuteReader();
            while (reader.Read())
            {
                string[] buff = new string[5];
                for (int i = 0; i < buff.Length; i++)
                {
                    
                    buff[i] = reader[i].ToString();

                }
                //если не является ID текущего пользователя программы
                if (buff[0]!=ID)
                {
                    bool friend = false;
                    //Создание объекта пользователя
                    users.Add(new User(buff[0],buff[1],buff[2],buff[3],buff[4],friend));
                }        

            }
            reader.Close();
            com.Connection.Close();
            //Сверка со списком друзей для определения наличия таковых в списку найденных
            InitializeFriends(ID,friends_buffer);
            //Перебираем списки для провкрки совпадения по ID
            for(int y=0;y<users.Count;y++)
            {
                for (int g=0;g<friends_buffer.Count;g++)
                {
                    //Если ID совпадают, значит друг присутствует в списке найденных
                    if (users[y].ID==friends_buffer[g].ID)
                    {
                        users[y].friend = true;
                    }
                }

            }
            return users;

        }
        MySqlConnection connection;
        MySqlConnection con;
        MySqlCommand com;

    }
}
