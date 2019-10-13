using MyVet.Common.Models;
using MyVet.Common.Services;
using MyVet.Common.Helpers;
using Newtonsoft.Json;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using System;


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
        private DelegateCommand _registerCommand;
        private DelegateCommand _forgotPasswordCommand;

        
        public LoginPageViewModel(INavigationService navigationService, IApiService apiService) : base(navigationService)
        {
            Title = "VETERINARIA ZULU";
            IsEnabled = true;
            IsRemember = true;
            _apiService = apiService;
            _navigationService = navigationService;
        }

        public DelegateCommand ForgotPasswordCommand => _forgotPasswordCommand ?? (_forgotPasswordCommand = new DelegateCommand(ForgotPassword));

        public DelegateCommand LoginCommand => _loginCommand ?? (_loginCommand = new DelegateCommand(Login));

        public DelegateCommand RegisterCommand => _registerCommand ?? (_registerCommand = new DelegateCommand(Register));

        public bool IsRemember { get; set; }

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

            var url = App.Current.Resources["UrlAPI"].ToString();
            var connection = await _apiService.CheckConnection(url);

            if (!connection)
            {
                IsEnabled = true;
                IsRunning = false;
                
                await App.Current.MainPage.DisplayAlert("Error de Conexion", "No se pudo conectar a la Web","Aceptar");
                return;
            }

            var request = new TokenRequest
            {
                Password = Password,
                Username = Email
            };

            
            var response = await _apiService.GetTokenAsync(url, "Account", "/CreateToken", request);
 
            

            if (!response.IsSuccess)
            {
                IsRunning = false;
                IsEnabled = true;
                await App.Current.MainPage.DisplayAlert("Error", "Correo o contraseña incorrecta", "Aceptar");
                Password = string.Empty;
                return;
            }

            var token = response.Result;
            var response2 = await _apiService.GetOwnerByEmailAsync(url,"api","/Owners/GetOwnerByEmail","bearer",token.Token,Email);

            if (!response2.IsSuccess)
            {
                IsRunning = false;
                IsEnabled = true;
                await App.Current.MainPage.DisplayAlert("Error", "Este usuario no existe", "Aceptar");
                return;
            }

            var owner = response2.Result;

            Settings.Owner = JsonConvert.SerializeObject(owner);
            Settings.Token = JsonConvert.SerializeObject(token);
            Settings.IsRemembered = IsRemember;

            IsRunning = false;
            IsEnabled = true;
                                    
            await _navigationService.NavigateAsync("/VeterinaryMasterDetailPage/NavigationPage/PetsPage");
            Password = string.Empty;
        }

        private async void Register()
        {
            await _navigationService.NavigateAsync("RegisterPage");
        }

        private async void ForgotPassword()
        {
            await _navigationService.NavigateAsync("RememberPasswordPage");
        }
    }
}
