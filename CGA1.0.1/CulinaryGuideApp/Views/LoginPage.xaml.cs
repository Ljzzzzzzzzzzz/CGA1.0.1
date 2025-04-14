using CulinaryGuideApp.Models;
using CulinaryGuideApp.Services;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Dispatching;

namespace CulinaryGuideApp.Views
{
    public partial class LoginPage : ContentPage
    {
        private DatabaseService _db = new();

        public LoginPage()
        {
            InitializeComponent();
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await _db.Init(); // 确保数据库已初始化
        }

        private async void OnLoginClicked(object sender, EventArgs e)
        {
            Console.WriteLine("Login button clicked!");
            var username = UsernameEntry.Text?.Trim();
            var password = PasswordEntry.Text?.Trim();
            Console.WriteLine($"Username entered: '{username}'");
            Console.WriteLine($"Password entered: '{password}'");

            var user = await _db.GetUser(username, password);
            Console.WriteLine($"_db.GetUser returned: {(user == null ? "null" : "user object")}");

            if (user != null)
            {
                Console.WriteLine("Login successful! Navigating to MainPage using Shell.");
                await Shell.Current.GoToAsync($"//{nameof(MainPage)}");
            }
            else
            {
                Console.WriteLine("Login failed. Displaying error message.");
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    MessageLabel.Text = "Invalid username or password.";
                });
            }
        }

        private async void OnRegisterClicked(object sender, EventArgs e)
        {
            var username = UsernameEntry.Text?.Trim();
            var password = PasswordEntry.Text?.Trim();

            if (await _db.UserExists(username))
            {
                Console.WriteLine("Registration failed: Username already exists.");
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    MessageLabel.Text = "Username already exists.";
                    MessageLabel.TextColor = Colors.Red; // 确保错误消息是可见的颜色
                });
            }
            else
            {
                var newUser = new User { Username = username, Password = password }; // Password 在 AddUser 中会被处理
                await _db.AddUser(newUser);
                Console.WriteLine("Registration successful.");
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    MessageLabel.TextColor = Colors.Green;
                    MessageLabel.Text = "Registration successful. You can now log in.";
                    UsernameEntry.Text = string.Empty; // 清空输入框
                    PasswordEntry.Text = string.Empty; // 清空输入框
                });
            }
        }
    }
}