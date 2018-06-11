using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace StudChat
{
    public partial class MainMenu : Form
    {
        Account your_account;
        MySQLConnection connection;
        ActionChoosing choosing_form;
        RegistrationForm registration;
        public List<User> founded_people=new List<User>();//Список найденных пользователей
        public MainMenu(MySQLConnection con, Account Account,RegistrationForm reg)
        {
            connection = con;
            your_account = Account;
            registration = reg;
            InitializeComponent();
        }

        private void MainMenu_Load(object sender, EventArgs e)
        {

            NoResults.Enabled = false;
            NoResults.Enabled = false;
           
        }
        //Вывод списка друзей текущего пользователя
        private void ListOfFriends_Click(object sender, EventArgs e)
        {
            NoResults.Visible = false;
            FoundedPeople.Items.Clear();//Очистка списка найденных пользователей на форме
            founded_people.Clear();//Очистка списка найденных пользователей
            connection.InitializeFriends(your_account.ID.ToString(),founded_people);//Получение пользователей из списка друзей
            if (founded_people.Count == 0)//Если пользователей-друзей не найдено
            {
                NoResults.Visible = true;//Вывод на форму сообщения об отсутствии результатов
            }
            else//Если пользователи-друзья найдены
            {
                FillTable();//Заполнение списка найденных пользователей на форме
            }                      
        }
        //Выбор пользователя из списка формы
        private void Act(object sender, EventArgs e)
        {
            User companion=null;//Протопип объекта выбранного пользователя
            for (int r=0;r<FoundedPeople.Items.Count;r++)//Поиск объекта пользователя в списке формы
            {
                //Если строка выделена
                if (FoundedPeople.Items[r].Selected==true)
                {
                    companion = founded_people[r];//Присвоение объекта 
                    break;                  
                }
            }
            Enabled = false;
            //Переход к форме взаимодействия с выбранным пользователем
            choosing_form = new ActionChoosing(your_account, connection,this,companion,founded_people);
            choosing_form.Enabled = true;
            choosing_form.Visible = true;


        }

        private void FindPeople_Click(object sender, EventArgs e)
        {
            SeekForPeople();//Поиск пользователей по введённым данным

        }
        public void SeekForPeople()//Поиск пользователей по введённым данным
        {
            NoResults.Visible = false;
            if (SearchBar.Text != null)
            {
                FoundedPeople.Items.Clear();//Очистка списка найденных пользователей на форме
                founded_people.Clear();//Очистка списка найденных пользователей
                //Поиск пользователей
                founded_people = connection.FindUsers(SearchBar.Text, founded_people, your_account.ID.ToString());
                if (founded_people.Count == 0)//Если пользователей-друзей не найдено
                {
                    NoResults.Visible = true;//Вывод на форму сообщения об отсутствии результатов
                }
                else//Если пользователи-друзья найдены
                {
                    FillTable();// Заполнение списка найденных пользователей на форме
                }
            }
            else//Если поле поиска пустое
            {
                NoResults.Visible = true;
            }
        }
        //Вывод результатов поиска на форму
        public void FillTable()
        {
            FoundedPeople.Items.Clear();//Очистка списка формы
            for (int f = 0; f < founded_people.Count; f++)
            {
                string name_item;
                string name_subitem = null;
                if (founded_people[f].friend == true)//Определение отношения текущего пользователя к данному пользователю
                {
                    name_subitem = "Friend";
                }
                //Запись имени,фамилии и ника пользователя
                name_item = founded_people[f].name + " " + founded_people[f].surname + "(" + founded_people[f].nickname + ")";
                //Создание объекта строки
                ListViewItem item = new ListViewItem() { Text = name_item, SubItems = { name_subitem } };
                //Добавление строки
                FoundedPeople.Items.Add(item);
            }

        }
        //Завершение работы программы
        private void Exit_Click(object sender, EventArgs e)
        {
            registration.Close();
        }
        //Выйти из учётной записи текущего пользователя
        private void LogOff_Click(object sender, EventArgs e)
        {
            registration.Enabled = true;
            registration.Visible = true;
            Close();
        }

     
    }
}
