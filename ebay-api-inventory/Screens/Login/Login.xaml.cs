using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ebay_api_inventory;

public partial class MainWindow : Window
{
    private string usernameText = "";
    private string passwordText = "";

    public MainWindow()
    {
        InitializeComponent();
    }

    private void Username_TextChanged(object sender, RoutedEventArgs e)
    {
        usernameText = UsernameTextBox.Text;
    }

    private void Password_Changed(object sender, RoutedEventArgs e)
    {
        passwordText = PasswordBox.Password;
    }

    private void LogInButton_Clicked(object sender, RoutedEventArgs e)
    {
        Debug.WriteLine("Log In with username " + this.usernameText + " and password " + this.passwordText);
    }
}
