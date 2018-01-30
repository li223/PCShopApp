using Npgsql;
using PCShopApp.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace PCShopApp
{
    public partial class Window1 : Window
    {
        private DatabaseInteractivity _DbInter = new DatabaseInteractivity();
        private NpgsqlConnection _connection { get; set; }
        private IEnumerable<Login> _logins { get; set; }
        public Window1()
        {
            InitializeComponent();
            var config = DatabaseConfig.LoadConfig();
            _connection = _DbInter.CreateConnection(config.Database, config.Host, config.Password, config.Username, config.Port);
        }

        private async void Login_Button_Click(object sender, RoutedEventArgs e)
        {
            var un = Input_Username.Text;
            var pw = Input_Password.Password;
            var login = _logins.FirstOrDefault(x => x.Username == un && x.Password == pw);
            if (login != null)
            {
                var staff = await _DbInter.SelectAsync<Staff>("staff", _connection);
                var window = new MainWindow()
                {
                    Connection = _connection,
                    User = staff.FirstOrDefault(x => x.Id == login.StaffId),
                    DbInter = _DbInter
                };
                window.tbctrl.SelectedIndex = 4;
                Application.Current.MainWindow = window;
                this.Close();
                window.Show();
            }
            else
            {
                MessageBox.Show("Invalid Username or Password", "Error", MessageBoxButton.OK);
                Input_Password.Clear();
                Input_Username.Clear();
                string a = "2";
                int b = int.Parse(Convert.ToInt64(a).ToString());
            }
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e) => _logins = await _DbInter.SelectAsync<Login>("logins", _connection);

        private void Exit_Button_Click(object sender, RoutedEventArgs e) => Environment.Exit(0);
    }
}
