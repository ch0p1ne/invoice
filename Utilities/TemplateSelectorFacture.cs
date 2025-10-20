using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace invoice.Utilities
{
    public class TemplateSelectorFacture : DataTemplateSelector
    {
        public required DataTemplate CrudOneTemplate { get; set; }
        public required DataTemplate CrudTwoTemplate { get; set; }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            if (item is string str && str == "crudCreateOne")
                return CrudOneTemplate;
            else if (item is string str2 && str2 == "crudCreateTwo")
                return CrudTwoTemplate;

            return CrudTwoTemplate;
        }
    }
}
