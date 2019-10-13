﻿using MyVet.Common.Models;
using MyVet.Common.Helpers;
using Prism.Commands;
using Prism.Navigation;
using Newtonsoft.Json;

namespace MyVet.Prism.ViewModels
{
    public class PetItemViewModel : PetResponse
    {
        private readonly INavigationService _navigationService;
        private DelegateCommand _selectPetCommand;

        public PetItemViewModel(INavigationService navigationService)
        {
            _navigationService = navigationService;
        }

        public DelegateCommand SelectPetCommand => _selectPetCommand ?? (_selectPetCommand = new DelegateCommand(SelectPet));

        private async void SelectPet()
        {
            Settings.Pet = JsonConvert.SerializeObject(this);

            await _navigationService.NavigateAsync("PetTabbedPage");

        }
    }
}
