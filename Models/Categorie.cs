using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace invoice.Models
{
    public class Categorie : ObservableObject
    {
        public int CategorieId { get; set; }

        [Column(TypeName ="nvarchar(15)")]
        public string CategorieName { get; set; } = string.Empty;

        public string CategorieDescription { get; set; } = string.Empty;

        public ICollection<PrixHomologue> PrixHomologues { get; set; } = [];
        public override string ToString() => CategorieName;
    }
}
