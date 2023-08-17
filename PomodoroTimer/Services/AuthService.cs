using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using PomodoroTimer.Common.Account;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace PomodoroTimer.Services
{
    public class AuthService : IAuthService
    {
        private readonly HttpClient _httpClient;
        private readonly AuthenticationStateProvider _authenticationStateProvider;
        private readonly ILocalStorageService _localStorage;

        public AuthService(
            HttpClient httpClient,
            AuthenticationStateProvider authenticationStateProvider,
            ILocalStorageService localStorage)
        {
            _httpClient = httpClient;
            _authenticationStateProvider = authenticationStateProvider;
            _localStorage = localStorage;
        }

        public async Task<LoginResult?> Login(LoginModel loginModel)
        {
            var response = await _httpClient.PostAsJsonAsync("account/login", loginModel);
            var loginResult = await response.Content.ReadFromJsonAsync<LoginResult>();

            if (!response.IsSuccessStatusCode || loginResult is null || !loginResult.Succeeded)
                return loginResult;

            await _localStorage.SetItemAsync("authToken", loginResult.AccessToken);
            ((PomodoroAuthStateProvider)_authenticationStateProvider).MarkUserAsAuthenticated(loginModel.Email ?? string.Empty);
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", loginResult.AccessToken);

            return loginResult;
        }

        public async Task<LogoutResult?> Logout()
        {
            await _localStorage.RemoveItemAsync("authToken");
            var response = await _httpClient.PostAsync("account/logout", null);
            return await response.Content.ReadFromJsonAsync<LogoutResult>();
        }

        public async Task<RegisterResult?> Register(RegisterModel registerModel)
        {
            var response = await _httpClient.PostAsJsonAsync("account/register", registerModel);
            return await response.Content.ReadFromJsonAsync<RegisterResult>();
        }
    }
}
