using Prism.Commands;
using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MyVet.Prism.ViewModels
{
    public class MapPageViewModel : ViewModelBase
    {
        
        public MapPageViewModel(INavigationService navigationService):base(navigationService)
        {
            Title = "Mapa";
        }
    }
}
