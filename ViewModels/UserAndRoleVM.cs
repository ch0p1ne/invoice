using CommunityToolkit.Mvvm.Input;
using invoice.Utilities;
using invoice.ViewModels.SubViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace invoice.ViewModels
{
    public partial class UserAndRoleVM : VMBase
    {
        // Fields
        // Field for the current sub-view model : can be UserVM or RoleVM
        private VMBase _currentSubViewModel = new UsersVM();

        // Properties
        // Property for the current sub-view model
        public VMBase CurrentSubViewModel
        {
            get => _currentSubViewModel;
            set
            {
                SetProperty(ref _currentSubViewModel, value);
                ShowUsersViewCommand.NotifyCanExecuteChanged();
                ShowRolesViewCommand.NotifyCanExecuteChanged();
            }
        }



        // Commands
        [RelayCommand(CanExecute = nameof(CanShowUsersView))]
        public void ShowUsersView()
        {
            CurrentSubViewModel = new UsersVM();
        }
        [RelayCommand(CanExecute = nameof(CanShowRolesView))]
        public void ShowRolesView()
        {
            CurrentSubViewModel = new RolesVM();
        }


        // CanExecute methods
        private bool CanShowUsersView()
        {
            return CurrentSubViewModel is not UsersVM;
        }
        private bool CanShowRolesView()
        {
            return CurrentSubViewModel is not RolesVM;
        }
    }
}
