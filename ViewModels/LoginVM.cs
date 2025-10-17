using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using invoice.Context;
using invoice.Models;
using invoice.Services;
using invoice.Utilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;
using System.Threading.Tasks;

namespace invoice.ViewModels
{
    public partial class LoginVM : VMBase
    {

        public readonly ISessionService _sessionService;
        public readonly INavigationService _navigationService;
        private string? _userName;
        private SecureString? _password;
        private string _errorMessage;

        public string? UserName {
            get=>  _userName;
            set
            {
                if (SetProperty(ref _userName, value))
                    DoLoginCommand.NotifyCanExecuteChanged();
            }
        }
        public SecureString? Password
        {
            get => _password;
            set
            {
                if (SetProperty(ref _password, value))
                    DoLoginCommand.NotifyCanExecuteChanged();
            }
        }

  
        public string ErrorMessage
        {
            get {  return _errorMessage; }
            set { SetProperty(ref _errorMessage, value); }
        }


        public LoginVM(INavigationService navigationService, ISessionService sessionService)
        {
            _sessionService = sessionService;
            _navigationService = navigationService;
            _errorMessage = "";
        }

        [RelayCommand(CanExecute = nameof(CanExecuteDoLogin))]
        public async Task DoLogin()
        {
            using var context = new ClimaDbContext();
            var user = await context.Users.SingleOrDefaultAsync(w => w.Account_name == UserName);
            if (user == null)
            {
                ErrorMessage = "Utilisateur Inconnue";
                return;
            }

            string plainpwd = SecureStringHelper.ToPlainString(Password!);
            string computedHash = PasswordHasher.HashPassword(plainpwd, user.Salt);

            if (computedHash != user.PasswordHash)
            {
                ErrorMessage = "Le Mots de passe est incorrect";
                return;
            }
            _sessionService.User = user;
            // user credentials are correct so launch mainview;
            _navigationService.NavigateTo<MainVM>();
            _navigationService.CloseWindow<LoginVM>();
        }

        [RelayCommand]
        public async Task<bool> DoRegister()
        {
            using var context = new ClimaDbContext();
            bool exits = await context.Users.AnyAsync(u => u.Account_name == "Thomas Souah");
            if(exits)
                return false;

            string plainpwd = SecureStringHelper.ToPlainString(Password!);
            string salt = PasswordHasher.GenerateSalt();
            string hash = PasswordHasher.HashPassword(plainpwd, salt);

            User user = new User()
            {
                Account_name = "Thomas Souah",
                Salt = salt,
                PasswordHash = hash,
                Email = "souahthomas@prot.com",
                Phone_number_one = "06685456"
            };

            context.Users.Add(user);
            await context.SaveChangesAsync();

            return true;
        }

        [RelayCommand]
        public void CloseWindow()
        {
            _navigationService.CloseWindow<LoginVM>();
        }

        public bool CanExecuteDoLogin()
        {
            if (String.IsNullOrWhiteSpace(UserName) || UserName?.Length <= 3 || Password == null || Password?.Length <= 3)
                return false;
            return true;
        }

    }
}
