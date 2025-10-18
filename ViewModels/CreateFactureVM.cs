using invoice.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace invoice.ViewModels
{
    public class CreateFactureVM : VMBase
    {
        private readonly string _title = "Facture";
        public string Title
        {
            get => _title;
        }
    }
}
