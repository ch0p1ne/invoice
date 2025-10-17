using invoice.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;

namespace invoice.Services
{
    public interface ISessionService
    {
        User? User { get; set; }
    }
}
