using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace invoice.Models
{
    public class FactureExamen
    {
        public int FactureId;
        public int ExamenId;
        
        [Range(0, 100)]
        public int Qte { get; set; }
    }
}
