using MyVet.Common.Models;
using MyVet.Common.Services;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MyVet.Prism.ViewModels
{
    public class LoginPageViewModel : ViewModelBase
    {
        private readonly INavigationService _navigationService;
        private readonly IApiService _apiService;
        private string _password;
        private bool _isRunning;
        private bool _IsEnabled;
        private DelegateCommand _loginCommand;

        public LoginPageViewModel(INavigationService navigationService, IApiService apiService) : base(navigationService)
        {
            Title = "Login";
            IsEnabled = true;
            _apiService = apiService;
            _navigationService = navigationService;
        }

        public DelegateCommand LoginCommand => _loginCommand ?? (_loginCommand = new DelegateCommand(Login));

        

        public string Email { get; set; }
        public string Password
        {
            get => _password;
            set => SetProperty(ref _password, value);
        }
        public bool IsRunning
        {
            get => _isRunning;
            set => SetProperty(ref _isRunning, value);
        }
        public bool IsEnabled
        {
            get => _IsEnabled;
            set => SetProperty(ref _IsEnabled, value);
        }

        private async void Login()
        {
            if (string.IsNullOrEmpty(Email))
            {
                await App.Current.MainPage.DisplayAlert("Error","Debe Ingresar un correo","Aceptar");
                return;
            }
            if (string.IsNullOrEmpty(Password))
            {
                await App.Current.MainPage.DisplayAlert("Error", "Debe ingresar su contraseña", "Aceptar");
                return;
            }
            IsRunning = true;
            IsEnabled = false;

            var request = new TokenRequest
            {
                Password = Password,
                Username = Email
            };

            var url = App.Current.Resources["UrlAPI"].ToString();
            var response = await _apiService.GetTokenAsync(url, "Account", "/CreateToken", request);
 
            

            if (!response.IsSuccess)
            {
                IsRunning = false;
                IsEnabled = true;
                await App.Current.MainPage.DisplayAlert("Error", "Correo o contraseña incorrecta", "Aceptar");
                Password = string.Empty;
                return;
            }

            var token = (TokenResponse)response.Result;
            var response2 = await _apiService.GetOwnerByEmailAsync(url,"api","/Owners/GetOwnerByEmail","bearer",token.Token,Email);

            if (!response2.IsSuccess)
            {
                IsRunning = false;
                IsEnabled = true;
                await App.Current.MainPage.DisplayAlert("Error", "Este usuario no existe", "Aceptar");
                return;
            }

            var owner = (OwnerResponse)response2.Result;
            var parameters = new NavigationParameters
            {
                {"owner", owner }
            };

            IsRunning = false;
            IsEnabled = true;
                                    
            await _navigationService.NavigateAsync("PetsPage", parameters);
            Password = string.Empty;
        }
    }
}
