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
    public partial class LogInForm : Form
    {
        RegistrationForm registration;
        MySQLConnection connection;
        MainMenu menu;
        Account user;
        public LogInForm(RegistrationForm reg,MySQLConnection con)
        {
            registration = reg;
            connection = con;
            InitializeComponent();
        }

        private void LogInForm_Load(object sender, EventArgs e)
        {

        }
        //Подтверждение авторизационных данных
        private void ApplyRegistrationButton_Click(object sender, EventArgs e)
        {
            bool entered_data_validation = true;
            //Проверка на правильность введённых данных по длинне строк
            if (LogInNicknameRgistration.Text.Length > 30 || LogInPasswordRegistration.Text.Length > 15)
            {
                MessageBox.Show("One of the fields out of limit");
                LogInNicknameRgistration.Text = null;
                LogInPasswordRegistration.Text = null;
                entered_data_validation = false;
            }
            else//Проверка на правильность введённых данных на наличие недопустимых символов
            {
                TextBox[] checking_buffer = {LogInPasswordRegistration,LogInNicknameRgistration };
                for (int u=0;u<checking_buffer.Length;u++)
                {
                    if (checking_buffer[u].Text.Contains("'") || checking_buffer[u].Text.Contains(" ") || checking_buffer[u].Text == "")
                    {
                        MessageBox.Show("Inpappropriate symbol or no data at all was entered");
                        LogInNicknameRgistration.Text = null;
                        LogInPasswordRegistration.Text = null;
                        entered_data_validation = false;
                        break;
                    }
                }

            }
            //Если проверка на правильность введённых данных пройдена
            if (entered_data_validation == true)
            {
                string ID = null;//ID
                string message_box = null;
                //Проверка существования учётной записи
                bool validation = connection.CheckInformation(LogInNicknameRgistration.Text, LogInPasswordRegistration.Text, out message_box, out ID);
                MessageBox.Show(message_box);//Сообщение о результате авторизации
                if (validation == true)//Если авторизация прошла успешно
                {
                    string[] receiver = connection.InitializeUser(ID);//Получение информации из базы данных
                    user = new Account(receiver[0], receiver[1], receiver[2], receiver[3], receiver[4]);
                    Visible = false;
                    Enabled = false;
                    menu = new MainMenu(connection, user, registration);//Переход к форме меню
                    menu.Enabled = true;
                    menu.Visible = true;
                    Close();
                }
            }
           
        }
        //Возврат к форме регистрации
        private void BackToRegistrationButton_Click(object sender, EventArgs e)
        {
            Enabled = false;
            Visible = false;
            registration.Enabled = true;
            registration.Visible = true;
        }


    }
}
