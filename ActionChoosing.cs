using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace StudChat
{
    //Взаимодействие с выбранным пользователем
    public partial class ActionChoosing : Form
    {
        Account your_account;
        MySQLConnection connection;
        ChatForm chat;
        MainMenu menu;
        User companion;//Выбраный пользователь
        List<User> users;//Список найденных пользователей
        public ActionChoosing(Account user,MySQLConnection con,MainMenu main_menu,User companion_,List<User> users_)
        {
            your_account = user;
            connection = con;
            users = users_;
            menu = main_menu;
            companion = companion_;
            InitializeComponent();
        }

        private void ActionChoosing_Load(object sender, EventArgs e)
        { 
            //Вывод имени пользователя на форму
            Message.Text = "This is " + companion.name + " " + companion.surname + " (" + companion.nickname + ")";
            if (companion.friend == true)
            {
                Add_Delete.Text = "Delete from friends";
            }
            else
            {
                Add_Delete.Text = "Add to friends";
            }
        }
        //Начать беседу с выбранным пользователем
        private void StartConversation_Click(object sender, EventArgs e)
        {

            menu.Visible = false;
            chat = new ChatForm(your_account,connection, menu,companion);//Переход к форме беседы
            chat.Enabled = true;
            chat.Visible = true;
            Close();

        }
        //Добавление/удаление из списка друзей(в зависимости от текущих отношений)
        private void Add_Delete_Click(object sender, EventArgs e)
        {
            List<User> CheckList = new List<User>();//Буффер для хранения полученных пользователей
            connection.InitializeFriends(your_account.ID,CheckList);//Получение пользователей из списка друзей
            bool trigger = false;
            for (int m = 0; m < CheckList.Count; m++)//Проверка наличия выбранного пользователя в списке друзей
            {
                if (companion.ID == CheckList[m].ID)//Если присутствует
                {
                    trigger = true;
                    connection.DeleteFromFriends(your_account.ID, companion.ID);//Удаление из таблицы
                    // Изменение отношения между текущим пользователем и данным пользователем в списке найденных пользователей
                    for (int y=0;y<users.Count;y++)
                    {
                        if (companion.ID==users[y].ID)
                        {
                            users[y].friend = false;//Более не является другом
                        }
                    }
                    menu.FillTable();//Обновление списка выведенных пользователей
                    break;
                }
             
            }
            if (trigger==false)//Добавление в список друзей
            {
                connection.AddToFriends(your_account.ID, companion.ID);//Добавление в таблицу
                //Установление статуса друга в списке объектов
                for (int y = 0; y < users.Count; y++)
                {
                    if (companion.ID == users[y].ID)
                    {
                        users[y].friend = true;
                    }
                }
                menu.FillTable();//Обновление списка выведенных пользователей


            }
            menu.Enabled = true;            
            Close();
        }
        //Закрытие формы
        private void ActionChoosing_FormClosing(object sender, FormClosingEventArgs e)
        {
            menu.Enabled = true;
        }
    }
}
