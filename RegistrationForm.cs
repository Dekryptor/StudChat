using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;

namespace StudChat
{
   
    public partial class RegistrationForm : Form
    {
        MySQLConnection connection = new MySQLConnection();//Объект взаимодей ствия с базой данных
        LogInForm log_in_form;
        MainMenu menu;
        Account user;//Прототип объекта учётной записи текущего пользователя
        public RegistrationForm()
        {
            InitializeComponent();
        }
        private void RegistrationForm_Load(object sender, EventArgs e)
        {
        }
        //Подтверждение регистрационных данных
        private void ApplyRegistrationButton_Click(object sender, EventArgs e)
        {
            bool entered_data_validation = true;
            //Проверка на правильность введённых данных по длинне строк
            if (NameRegistration.Text.Length > 20 || SurnameRegistration.Text.Length > 30 || NicknameRegistration.Text.Length > 30 || PasswordRegistration.Text.Length > 15)
            {
                MessageBox.Show("One of the fields out of limit");
                NameRegistration.Text = null;
                SurnameRegistration.Text = null;
                NicknameRegistration.Text = null;
                PasswordRegistration.Text = null;
                entered_data_validation = false;
            }
            else//Проверка на правильность введённых данных на наличие недопустимых символов
            {
                TextBox[] checking_buffer = { NameRegistration, SurnameRegistration , NicknameRegistration, PasswordRegistration };
                for (int u = 0; u < checking_buffer.Length; u++)
                {
                    if (checking_buffer[u].Text.Contains("'") || checking_buffer[u].Text.Contains(" ") || checking_buffer[u].Text=="")
                    {
                        MessageBox.Show("Inpappropriate symbol or no data at all was entered");
                        NameRegistration.Text = null;
                        SurnameRegistration.Text = null;
                        NicknameRegistration.Text = null;
                        PasswordRegistration.Text = null;
                        entered_data_validation = false;
                        break;
                    }
                }
            }
            //Если проверка на правильность введённых данных пройдена
            if(entered_data_validation==true)
            {
                string ID = null;//ID 
                string message_box = null;
                //Создание учётной записи в базе данных
                bool registration = connection.CreateAccount(NameRegistration.Text, SurnameRegistration.Text, NicknameRegistration.Text, PasswordRegistration.Text, out message_box, out ID);
                MessageBox.Show(message_box);//Сообщение о результате регистрации
                if (registration == true)//Если регистрация прошла успешно
                {
                    string[] receiver = connection.InitializeUser(ID);//Получение информации из базы данных
                    user = new Account(receiver[0], receiver[1], receiver[2], receiver[3], receiver[4]);//Создание объекта учётной записи текущего пользователя
                    Visible = false;
                    Enabled = false;
                    menu = new MainMenu(connection, user, this);//Переход к форме меню
                    menu.Enabled = true;
                    menu.Visible = true;
                }
            }
      
               
        }
        //Переход к форме авторизации
        private void LogInButton_Click(object sender, EventArgs e)
        {
            Enabled = false;
            Visible = false;
            log_in_form = new LogInForm(this,connection);
            log_in_form.Enabled = true;
            log_in_form.Visible = true;
        }


    }
}
