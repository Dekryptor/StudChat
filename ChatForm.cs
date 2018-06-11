using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;
using System.Net.Sockets;
using System.Net;

namespace StudChat
{
    public partial class ChatForm : Form
    {
        private delegate void printer(string data);
        private delegate void cleaner();
        printer Printer;
        cleaner Cleaner;
        private Socket server_socket;
        private Thread client_thread;
        private  string server_host; 
        private const int server_port = 9933;//Порт сервера
        private User companion;
        private string name;
        Account your_account;
        MySQLConnection connection;
        MainMenu menu;
        public ChatForm(Account user,MySQLConnection con,MainMenu main_menu,User companion_)
        {
            
            string Host = Dns.GetHostName();
            string IP = Dns.GetHostByName(Host).AddressList[0].ToString();//Определение IP-адреса сервера StudChatServer
            server_host = IP;
            your_account = user;
            connection = con;
            menu = main_menu;
            companion = companion_;
            InitializeComponent();         
            name = your_account.name + " " + your_account.surname + "(" + your_account.nickname + ")";
            Printer = new printer(Print);
            Cleaner = new cleaner(ClearChat);
            //Создание соединения с сервером
            Connection();
            //Запуск соединения с сервером через поток
            client_thread = new Thread(Listener);
            client_thread.IsBackground = true;
            client_thread.Start();

        }
        private void Listener()//Прослушивание сокета на предмет поступления сообщений
        {
            
            while (server_socket.Connected)
            {
                try
                {
                    byte[] buffer = new byte[8196];//Буффер приёма сообщений
                    int bytesRec = server_socket.Receive(buffer);//Размер полученных сообщений
                    string data = Encoding.UTF8.GetString(buffer, 0, bytesRec);//Получение сообщений
                    if (data.Contains("#updatechat"))//Если содержит префикс полученного сообщения
                    {
                        UpdateChat(data);
                        continue;
                    }
                }
                catch(SocketException error)
                {
                    MessageBox.Show(error.ToString());
                }
            
            }
        }
        private void Connection()//Создание соединения с сервером
        {
            try
            {
                IPHostEntry ipHost = Dns.GetHostEntry(server_host);
                IPAddress ipAddress = ipHost.AddressList[0];
                IPEndPoint ipEndPoint = new IPEndPoint(ipAddress, server_port);
                server_socket = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);//Создание сокета
                server_socket.Connect(ipEndPoint);//Подключение к серверу
            }
            catch { Print("Server not avalilable!"); }
        }
        private void ClearChat()//Очистка поля сообщений на форме
        {
            if (InvokeRequired)
            {
                Invoke(Cleaner);
                return;
            }
            chatBox.Clear();
        }
        private void UpdateChat(string data)//Обновление чата
        {
            ClearChat();//Очистка поля сообщений на форме
            string[] messages = data.Split('&')[1].Split('|');//Разделение полученного массива информации на отдельные сообщения
            int countMessages = messages.Length;
            if (countMessages <= 0) return;
            for (int i = 0; i < countMessages; i++)//Выведение сообщений на форму
            {
                try
                {
                    if (string.IsNullOrEmpty(messages[i])) continue;
                    Print(String.Format("[{0}]:{1}    {2}", messages[i].Split('~')[0], messages[i].Split('~')[1], messages[i].Split('~')[2]));
                }
                catch { continue; }
            }
        }
        private void Send(string data)
        {
            try
            {
                byte[] buffer = Encoding.UTF8.GetBytes(data);//Конвертация сообщения в массив байт
                int bytesSent = server_socket.Send(buffer);//Непосредственно отправка на сервер
            }
            catch { Print("Сonnection to the server was lost..."); }
        }
        private void Print(string msg)//Вывод сообщений на форму
        {
            if (InvokeRequired)
            {
                Invoke(Printer, msg);
                return;
            }
            if (chatBox.Text.Length == 0)
            {
                chatBox.AppendText(msg);
            }
            else
            {
                chatBox.AppendText(Environment.NewLine + msg);
            }             
        }
        private void SendMessage()
        {
            try
            {
                string data = TextBox.Text;//Получение сообщения из строки ввода на форме
                if (string.IsNullOrEmpty(data)) return;
                Send("#newmsg&" + data+"&"+DateTime.Now);//Создание и отправка сообщения с индексом нового сообщения
                TextBox.Text = string.Empty;
            }
            catch { MessageBox.Show("Message sending error!"); }
        }
        private void ChatForm_Load(object sender, EventArgs e)
        {
            //Имя,фамилия и ник собеседника
            CompanionName.Text= companion.name + " " + companion.surname + "(" + companion.nickname + ")";
            //Отправка сообщения для установки данных получателя сообщений на сервере
            Send("#setname&" + name + "&" + your_account.ID + "&" + companion.ID);
            Send("#newmsg&"+" & ");
        }

        private void Send_Message_Click(object sender, EventArgs e)
        {
            SendMessage();//Отправка сообщения
        }

        //Возвращение к форме меню
        private void BackToMenu_Click(object sender, EventArgs e)
        {
            menu.Enabled = true;
            menu.Visible = true;
            server_socket.Close();
            client_thread.Abort(); //Завершение соединения        
            Close();
        }

        //Отправка сообщения через нажатие клавиши на клавиатуре
        private void KeySend(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Enter)
            {
                SendMessage();//Отправка сообщения
            }               
        }
    }
}
