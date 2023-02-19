﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
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
using System.Windows.Shapes;
using ebay_api_inventory.Entities;
using ebay_api_inventory.Main.Pages;
using ebay_api_inventory.Main.ViewModels;

namespace ebay_api_inventory.Main;

public partial class MainWindow : Window
{
    private MainViewModel mainViewModel;
    private LoginViewModel loginViewModel; 

    public MainWindow(MainViewModel mainViewModel)
    {
        InitializeComponent();
        this.mainViewModel = mainViewModel;
        loginViewModel = new LoginViewModel(loggedIn: UserLoggedIn);
        LoginButton.IsSelected = true;
    }

    private void SideBar_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        var selected = SideBar.SelectedItem as NavButton;
        if (selected == LoginButton)
        {
            Login loginPage = new Login(loginViewModel);
            NavFrame.Navigate(loginPage);
        }
        else if (selected == SettingsButton)
        {
            SettingsPage settingsPage = new SettingsPage();
            NavFrame.Navigate(settingsPage);
        }
    }

    private void UserLoggedIn(object? sender, UserAccessToken userAccessToken)
    {
        Debug.WriteLine($"User Access Token: {userAccessToken.access_token}");
    }
}
