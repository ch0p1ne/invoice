using invoice.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace invoice.Services
{
    public interface INavigationService
    {
        void NavigateTo<TViewModel>() where TViewModel : VMBase;
        void CloseWindow<TViewModel>() where TViewModel : VMBase;

    }
}
