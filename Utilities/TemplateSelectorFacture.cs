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
        public required DataTemplate DefinePatientTemplate { get; set; }
        public required DataTemplate CreateExamenTemplate { get; set; }
        public required DataTemplate CreateConsultationTemplate { get; set; }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            if (item is string str && str == "definePatient")
                return DefinePatientTemplate;
            else if (item is string str2 && str2 == "createExamen")
                return CreateExamenTemplate;
            else if (item is string str3 && str3 == "createConsultation")
                return CreateConsultationTemplate;

            return DefinePatientTemplate;
        }
    }
}
