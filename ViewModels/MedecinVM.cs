using CommunityToolkit.Mvvm.Input;
using invoice.Context;
using invoice.Models;
using invoice.Utilities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace invoice.ViewModels
{
    public partial class MedecinVM : VMBase
    {
        private int _medecinId;
        public int MedecinId
        {
            get => _medecinId;
            set { SetProperty(ref _medecinId, value); RefreshCanExecuteCommands(); }
        }
        private string _medecinFirstName = string.Empty;
        public string MedecinFirstName
        {
            get => _medecinFirstName;
            set { SetProperty(ref _medecinFirstName, InputValidator.CapitalizeEachWord(value) ?? value); RefreshCanExecuteCommands(); }
        }
        private string _medecinLastName = string.Empty;
        public string MedecinLastName
        {
            get => _medecinLastName;
            set { SetProperty(ref _medecinLastName, InputValidator.ToUpperString(value) ?? value); RefreshCanExecuteCommands(); }
        }
        private string _speciality = string.Empty;
        public string Speciality
        {
            get => _speciality;
            set { SetProperty(ref _speciality, value); RefreshCanExecuteCommands(); }
        }
        private string? _address;
        public string? Address
        {
            get => _address;
            set { SetProperty(ref _address, value); RefreshCanExecuteCommands(); }
        }
        private string? _phoneNumberOne;
        public string? PhoneNumberOne
        {
            get => _phoneNumberOne;
            set { SetProperty(ref _phoneNumberOne, InputValidator.FormatAndValidateInput(value)); RefreshCanExecuteCommands(); }
        }
        private string? _phoneNumberTwo;
        public string? PhoneNumberTwo
        {
            get => _phoneNumberTwo;
            set { SetProperty(ref _phoneNumberTwo, InputValidator.FormatAndValidateInput(value)); RefreshCanExecuteCommands(); }
        }
        private DateTime _startWork = DateTime.Now;
        public DateTime StartWork
        {
            get => _startWork;
            set { SetProperty(ref _startWork, value); RefreshCanExecuteCommands(); }
        }
        private DateTime? _endWork = null;
        public DateTime? EndWork
        {
            get => _endWork;
            set { SetProperty(ref _endWork, value); RefreshCanExecuteCommands(); }
        }
        private string? _email;
        public string? Email
        {
            get => _email;
            set { SetProperty(ref _email, value); RefreshCanExecuteCommands(); }
        }

        public ObservableCollection<Medecin> Medecins { get; } = new ObservableCollection<Medecin>();
        private Medecin? _selectedMedecin;
        public Medecin? SelectedMedecin
        { 
            get => _selectedMedecin;
            set
            {                 
                SetProperty(ref _selectedMedecin, value);
                updateSelectedMedecinFields();
                RefreshCanExecuteCommands();
            }
        } 


        // Constructor
        public MedecinVM() 
        {
            _ = LoadMedecins();
        }

        // Commands

        [RelayCommand(CanExecute = nameof(CanClearFields))]
        public async Task ClearFields()
        {
            MedecinId = 0;
            MedecinFirstName = string.Empty;
            MedecinLastName = string.Empty;
            Speciality = string.Empty;
            Address = null;
            PhoneNumberOne = null;
            PhoneNumberTwo = null;
            StartWork = DateTime.Now;
            EndWork = null;
            Email = null;
            await Task.CompletedTask;
        }
        [RelayCommand(CanExecute = nameof(CanCreateNewMedecin))]
        public async Task CreateNewMedecin()
        {
            var messageBox = new ModelOpenner();
            try
            {
                using var context = new ClimaDbContext();
                var newMedecin = new Medecin
                {
                    MedecinFirstName = this.MedecinFirstName,
                    MedecinLastName = this.MedecinLastName,
                    Speciality = this.Speciality,
                    Address = this.Address,
                    PhoneNumberOne = this.PhoneNumberOne,
                    PhoneNumberTwo = this.PhoneNumberTwo,
                    StartWork = this.StartWork,
                    EndWork = this.EndWork,
                    Email = this.Email
                };

                context.Medecins.Add(newMedecin);
                await context.SaveChangesAsync();
                Medecins.Add(newMedecin);
                await ClearFields();
                messageBox.Show("Succès", "Le médecin a été créé avec succès.", System.Windows.MessageBoxButton.OK);

            }
            catch (Exception ex)
            {
                messageBox.Show("Erreur", $"Une erreur est survenue lors de la création du médecin.\n{ex.Message}", System.Windows.MessageBoxButton.OK);
            }

        }
        [RelayCommand]
        public async Task LoadMedecins()
        {
            var messageBox = new ModelOpenner();
            try
            {
                using var context = new ClimaDbContext();
                Medecins.Clear();
                var medecins = await context.Medecins.ToListAsync();
                foreach (var medecin in medecins)
                {
                    Medecins.Add(medecin);
                }
            }
            catch (Exception ex)
            {

                messageBox.Show("Erreur", $"{ex.Message}", System.Windows.MessageBoxButton.OK);
            }
        }
        [RelayCommand(CanExecute = nameof(CanDeleteMedecin))]
        public async Task DeleteMedecin()
        {
            var messageBox = new ModelOpenner();
            try
            {
                if (SelectedMedecin == null)
                    return;
                using var context = new ClimaDbContext();
                var medecinToDelete = await context.Medecins.FindAsync(SelectedMedecin.MedecinId);
                if (medecinToDelete != null)
                {
                    context.Medecins.Remove(medecinToDelete);
                    await context.SaveChangesAsync();
                    Medecins.Remove(SelectedMedecin);
                    await ClearFields();
                    messageBox.Show("Succès", "Le médecin a été supprimé avec succès.", System.Windows.MessageBoxButton.OK);
                }
            }
            catch (Exception ex)
            {
                messageBox.Show("Erreur", $"Une erreur est survenue lors de la suppression du médecin.\n{ex.Message}", System.Windows.MessageBoxButton.OK);
            }
        }
        [RelayCommand(CanExecute = nameof(CanUpdateMedecin))]
        public async Task UpdateMedecin()
        {
            var messageBox = new ModelOpenner();
            try
            {
                if (SelectedMedecin == null)
                    return;
                using var context = new ClimaDbContext();
                var medecinToUpdate = await context.Medecins.FindAsync(SelectedMedecin.MedecinId);
                if (medecinToUpdate != null)
                {
                    medecinToUpdate.MedecinFirstName = this.MedecinFirstName;
                    medecinToUpdate.MedecinLastName = this.MedecinLastName;
                    medecinToUpdate.Speciality = this.Speciality;
                    medecinToUpdate.Address = this.Address;
                    medecinToUpdate.PhoneNumberOne = this.PhoneNumberOne;
                    medecinToUpdate.PhoneNumberTwo = this.PhoneNumberTwo;
                    medecinToUpdate.StartWork = this.StartWork;
                    medecinToUpdate.EndWork = this.EndWork;
                    medecinToUpdate.Email = this.Email;
                    await context.SaveChangesAsync();
                    // Update the SelectedMedecin in the ObservableCollection
                    var index = Medecins.IndexOf(SelectedMedecin);
                    Medecins[index] = medecinToUpdate;
                    SelectedMedecin = medecinToUpdate;
                    messageBox.Show("Succès", "Le médecin a été mis à jour avec succès.", System.Windows.MessageBoxButton.OK);
                }
            }
            catch (Exception ex)
            {
                messageBox.Show("Erreur", $"Une erreur est survenue lors de la mise à jour du médecin.\n{ex.Message}", System.Windows.MessageBoxButton.OK);
            }
        }
        public void RefreshCanExecuteCommands()
        {
            CreateNewMedecinCommand.NotifyCanExecuteChanged();
            ClearFieldsCommand.NotifyCanExecuteChanged();
            DeleteMedecinCommand.NotifyCanExecuteChanged();
            UpdateMedecinCommand.NotifyCanExecuteChanged();
        }

        // CanExecute Methods
        private bool CanCreateNewMedecin()
        {
            return !string.IsNullOrWhiteSpace(MedecinFirstName) &&
                   !string.IsNullOrWhiteSpace(MedecinLastName) &&
                   !string.IsNullOrWhiteSpace(Speciality);
        }
        private bool CanClearFields()
        {
            return !string.IsNullOrWhiteSpace(MedecinFirstName) ||
                   !string.IsNullOrWhiteSpace(MedecinLastName) ||
                   !string.IsNullOrWhiteSpace(Speciality) ||
                   !string.IsNullOrWhiteSpace(Address) ||
                   !string.IsNullOrWhiteSpace(PhoneNumberOne) ||
                   !string.IsNullOrWhiteSpace(PhoneNumberTwo) ||
                   StartWork != DateTime.Now ||
                   EndWork != null ||
                   !string.IsNullOrWhiteSpace(Email);
        }
        private bool CanDeleteMedecin()
        {
            return SelectedMedecin != null;
        }
        private bool CanUpdateMedecin()
        {
            return SelectedMedecin != null &&
                   !string.IsNullOrWhiteSpace(MedecinFirstName) &&
                   !string.IsNullOrWhiteSpace(MedecinLastName) &&
                   !string.IsNullOrWhiteSpace(Speciality);
        }

        // Methods
        private void updateSelectedMedecinFields()
        {
            if (SelectedMedecin != null)
            {
                MedecinId = SelectedMedecin.MedecinId;
                MedecinFirstName = SelectedMedecin.MedecinFirstName;
                MedecinLastName = SelectedMedecin.MedecinLastName;
                Speciality = SelectedMedecin.Speciality;
                Address = SelectedMedecin.Address;
                PhoneNumberOne = SelectedMedecin.PhoneNumberOne;
                PhoneNumberTwo = SelectedMedecin.PhoneNumberTwo;
                StartWork = SelectedMedecin.StartWork;
                EndWork = SelectedMedecin.EndWork;
                Email = SelectedMedecin.Email;
            }
        }
    }




}
