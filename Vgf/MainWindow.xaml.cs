// -----------------------------------------------------------------------
// <copyright file="MainWindow.cs" company="IB Hermann">
// Copyright (c) IB Hermann Mirow. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
namespace Vgf
{
    using System;
    using System.Reflection;
    using System.Windows;
    using Model;

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            Global.SetApplicationVersion(Assembly.GetExecutingAssembly().GetName().Version);
            this.Title = Global.ApplicationTitle;
            this.DataContext = new MainViewModel();
        }
    }
}
