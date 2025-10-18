using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace invoice.Utilities
{
    public class TemplateSelector: DataTemplateSelector
    {
        public required DataTemplate CrudListTemplate { get; set; }
        public required DataTemplate CrudCreateTemplate { get; set; }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            if (item is string str && str == "crudList")
                return CrudListTemplate;
            else if (item is string str2 && str2 == "crudAdd")
                return CrudCreateTemplate;

            return CrudCreateTemplate;
        }
    }
}
