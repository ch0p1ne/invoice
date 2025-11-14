using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace invoice.Utilities
{
    public enum InvoiceType
    {
        Consultation,
        Examen,
        Autre
    }
    public enum StatusType
    {
        Payer,
        Non_payer,
        Partiellement,
        autre
    }
    public enum PaymentMethod
    {
        Especes,
        Cheque,
        MobileMoney
    }
    public enum MedicalConsultationType
    {
        MedecineDuTravail,
        Dental,
        Veterinary,
        Other
    }
}
