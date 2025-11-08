using CommunityToolkit.Mvvm.ComponentModel;
using invoice.Utilities;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace invoice.Models
{
    public class Categorie : ObservableObject
    {
        private string _categorie = string.Empty;
        public int CategorieId { get; set; }

        [Column(TypeName ="nvarchar(15)")]
        public string CategorieName 
        { get => _categorie;
            set => SetProperty(ref _categorie, InputValidator.ToUpperString(value) ?? value);
            };

        public string CategorieDescription { get; set; } = string.Empty;

        public ICollection<PrixHomologue> PrixHomologues { get; set; } = [];
        public override string ToString() => CategorieName;
    }
}
