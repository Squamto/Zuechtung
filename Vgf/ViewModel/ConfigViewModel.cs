// -----------------------------------------------------------------------
// <copyright file="ConfigViewModel.cs" company="IB Hermann">
// Copyright (c) IB Hermann Mirow. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
namespace Vgf.ViewModel
{
    using System;
    using System.Collections.ObjectModel;
    using System.Collections.Specialized;
    using Config;
    using Framework.Helper;
    using Framework.ViewModel;
    using Model;
    using Model.FG;

    /// <summary>
    /// Defines the config view model.
    /// </summary>
    public class ConfigViewModel : BaseViewModel
    {
        MainModel mainModel;

        /// <summary>
        /// Initializes a new instance of the <see cref="ConfigViewModel"/> class.
        /// </summary>
        public ConfigViewModel(MainModel mainModel)
        {
            this.mainModel = mainModel;
            this.mainModel.Channels.ControlStateChanged += this.OnChannelsControlStateChanged;
            this.ConfigurationTree = new TreeNodeViewModel("Configuration Tree");
            this.ConfigurationTree.Children = new DispatchedObservableCollection<TreeNodeViewModel>(this.Dispatcher);
            this.SaveConfigCommand = new RelayCommand(this.SaveConfig, (o) => this.mainModel.Channels.ControlState == ControlStates.Stop, Global.UserMsg);
            this.ReadConfigCommand = new RelayCommand(this.ReadConfig, (o) => this.mainModel.Channels.ControlState == ControlStates.Stop, Global.UserMsg);
            this.RefreshConfigCommand = new RelayCommand(this.mainModel.RefreshConfigValues, (o) => true, Global.UserMsg);
            this.EnableExeutionLog(Global.LogInfo);
            this.mainModel = mainModel;
        }

        public string FileName
        {
            get => this.Get<string>();
            set => this.Set(value);
        }

        public TreeNodeViewModel ConfigurationTree { get; private set; }

        public RelayCommand SaveConfigCommand { get; }

        public RelayCommand ReadConfigCommand { get; }

        public RelayCommand RefreshConfigCommand { get; }
        
        public void FillData()
        {
            Dictionary<string, string> names = new Dictionary<string, string>();
            foreach (NameValueItem item in Conf.I.Values)
            {
                names.Add(item.Name, item.Value);
            }

            if (this.ConfigurationTree.Children != null)
            {
                foreach (TreeNodeViewModel node in this.ConfigurationTree.Children)
                {
                    this.RemoveEventHandler(node);
                }
            }
            this.ConfigurationTree.DisposeChildren();
            this.ConfigurationTree.Children = new DispatchedObservableCollection<TreeNodeViewModel>(this.Dispatcher);
            this.ConfigurationTree.ConvertFromPointSeparatedStringList(names);
            foreach(TreeNodeViewModel node in this.ConfigurationTree.Children)
            {
                this.AddEventHandler(node);
            }
        }

        private void AddEventHandler(TreeNodeViewModel treeNodeViewModel)
        {
            treeNodeViewModel.ValueChanged += this.OnTreeNodeViewModelValueChanged;
            if (treeNodeViewModel.Children == null)
            {
                return;
            }

            foreach (TreeNodeViewModel node in treeNodeViewModel.Children)
            {
                this.AddEventHandler(node);
            }
        }

        private void RemoveEventHandler(TreeNodeViewModel treeNodeViewModel)
        {
            treeNodeViewModel.ValueChanged -= this.OnTreeNodeViewModelValueChanged;
            if (treeNodeViewModel.Children == null)
            {
                return;
            }
            foreach (TreeNodeViewModel node in treeNodeViewModel.Children)
            {
                this.RemoveEventHandler(node);
            }
        }

        private void ReadConfig()
        {
            string filename = Global.OpenFileDialog(true);
            if (string.IsNullOrEmpty(filename))
            {
                return;
            }
            Conf.I.ReadAll(filename);
            this.FileName = filename;
            this.FillData();
        }

        private void SaveConfig()
        {
            string filename = Global.SaveFileDialog(true);
            if (string.IsNullOrEmpty(filename))
            {
                return;
            }

            Dictionary<string, string> dict = new Dictionary<string, string>();
            foreach (TreeNodeViewModel node in this.ConfigurationTree.Children)
            {
                Dictionary<string, string> add = node.ReadAllChildren();
                foreach (KeyValuePair<string, string> item in add)
                {
                    Conf.I.Values.First(o => o.Name == item.Key).Value = item.Value;
                }
            }
            Conf.I.WriteAll(filename);
        }
        private void OnTreeNodeViewModelValueChanged(object? sender, KeyValuePair<string, string> e)
        {
            Conf.I.SetValue(e.Key, e.Value);
        }
        private void OnChannelsControlStateChanged(object? sender, ControlStates e)
        {
            BaseViewModel.RequeryCommands();
        }
    }
}
