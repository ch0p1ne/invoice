using invoice.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;

namespace invoice.Services
{
    public class SessionService : ISessionService
    {
        public User? User { get; set; }
    }
}
