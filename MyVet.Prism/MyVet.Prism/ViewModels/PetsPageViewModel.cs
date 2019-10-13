using Prism.Commands;
using Newtonsoft.Json;
using Prism.Navigation;
using System.Collections.ObjectModel;
using System.Linq;
using MyVet.Common.Models;
using MyVet.Common.Helpers;

namespace MyVet.Prism.ViewModels
{
    public class PetsPageViewModel : ViewModelBase
    {
        private readonly INavigationService _navigationService;
        private OwnerResponse _owner;
        private ObservableCollection<PetItemViewModel> _pets;
        private DelegateCommand _addPetCommand;

        public PetsPageViewModel(INavigationService navigationService) : base(navigationService)
        {
            _navigationService = navigationService;
            Title = "Mascotas";
            LoadOwner();
        }

        public DelegateCommand AddPetCommand => _addPetCommand ?? (_addPetCommand = new DelegateCommand(AddPet));

        public ObservableCollection<PetItemViewModel> Pets
        {
            get => _pets;
            set => SetProperty(ref _pets, value);
        }


        private void LoadOwner()
        {
            _owner = JsonConvert.DeserializeObject<OwnerResponse>(Settings.Owner);
            Title = $"Pets of: {_owner.FullName}";
            Pets = new ObservableCollection<PetItemViewModel>(_owner.Pets.Select(p => new PetItemViewModel(_navigationService)
            {
                Born = p.Born,
                Histories = p.Histories,
                Id = p.Id,
                ImageUrl = p.ImageUrl,
                Name = p.Name,
                PetType = p.PetType,
                Race = p.Race,
                Remarks = p.Remarks
            }).ToList());
        }

        private async void AddPet()
        {
            await _navigationService.NavigateAsync("EditPet");
        }
    }
}
