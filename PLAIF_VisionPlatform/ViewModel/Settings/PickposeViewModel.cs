using Microsoft.Toolkit.Mvvm.Input;
using Microsoft.Win32;
using PLAIF_VisionPlatform.Interface;
using PLAIF_VisionPlatform.Model;
using PLAIF_VisionPlatform.View.Settings_View;
using PLAIF_VisionPlatform.Work;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using YamlDotNet.Core;

namespace PLAIF_VisionPlatform.ViewModel.Settings
{
    internal class PickposeViewModel : INotifyPropertyChanged, Observer
    {
        public ICommand? ImportPlyClick { get; set; }
        public ICommand? AddClick { get; set; }
        public ICommand? DelClick { get; set; }
        public ICommand? ModClick { get; set; }

        public PickposeViewModel()
        {
            Document.Instance.updater.Add(this);

            ImportPlyClick = new RelayCommand(ImportPlyCommand);
            AddClick = new RelayCommand(AddCommand);
            DelClick = new RelayCommand(DelCommand);
            ModClick = new RelayCommand(ModCommand);
            _pickPoses = new ObservableCollection<Pickpose>();
            
            this.UpdateFromJson();
        }

        public void ImportPlyCommand()
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "Polygon Files(*.ply)|*.PLY|All files (*.*)|*.*";
            if (ofd.ShowDialog() == true)
            {
                MessageBox.Show("공사중입니다..");
            }
        }

        PickPoseDefineViewService _pickPoseDefineViewService = new PickPoseDefineViewService();

        private readonly ObservableCollection<Pickpose> _pickPoses;
        public ObservableCollection<Pickpose> PickPoses { get => _pickPoses; }

        private void AddCommand()
        {
            _pickPoseDefineViewService.CreateWindow();
        }

        private void DelCommand()
        {
        }

        private void ModCommand()
        {
            _pickPoseDefineViewService.CreateWindow();
        }

        public void UpdateFromJson()
        {
            PickPoses.Clear();
            foreach (var pp in Document.Instance.pickPoses)
            {
                PickPoses.Add(pp);
            }
        }

        public void UpdateToJson()
        {

        }

        public event PropertyChangedEventHandler? PropertyChanged;
        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
