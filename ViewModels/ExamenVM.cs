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
    class ExamenVM : VMBase
    {
        private readonly string _title = "Examen";
        public string Title
        {
            get => _title;
        }

        public ObservableCollection<Examen> Examens { get; set; } = new ObservableCollection<Examen>();

        public ExamenVM()
        {
            // Correction : Appeler explicitement la méthode asynchrone sans lambda incorrecte
            LoadExamensasync().ConfigureAwait(false);
        }


        public async Task LoadExamensasync()
        {
            using var context = new ClimaDbContext();
            var examensList = await context.Examens.ToListAsync();

            // Mettre à jour la collection existante pour que les bindings voient les changements
            // Si ce code peut s'exécuter hors du thread UI, utiliser le Dispatcher.
            var dispatcher = System.Windows.Application.Current?.Dispatcher;
            if (dispatcher != null && !dispatcher.CheckAccess())
            {
                dispatcher.Invoke(() =>
                {
                    Examens.Clear();
                    foreach (var e in examensList) Examens.Add(e);
                });
            }
            else
            {
                Examens.Clear();
                foreach (var e in examensList) Examens.Add(e);
            }
        }
    }
}
