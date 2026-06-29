using System.Security.Claims;
using Microsoft.AspNetCore.Components.Authorization;

namespace VendorHub.Services
{
    public class AppAuthenticationStateProvider : AuthenticationStateProvider
    {
        private readonly AuthStateService _authState;

        public AppAuthenticationStateProvider(AuthStateService authState)
        {
            _authState = authState;
            _authState.OnAuthStateChanged += NotifyStateChanged;
        }

        public override Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            if (!_authState.IsLoggedIn)
            {
                return Task.FromResult(new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity())));
            }

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, _authState.UserName),
                new Claim(ClaimTypes.Role, _authState.UserRole),
                new Claim(ClaimTypes.NameIdentifier, _authState.UserId),
                new Claim("CompanyId", _authState.CompanyId.ToString()),
                new Claim("VendorId", _authState.VendorId.ToString())
            };

            var identity = new ClaimsIdentity(claims, "InviHubAuth");
            return Task.FromResult(new AuthenticationState(new ClaimsPrincipal(identity)));
        }

        private void NotifyStateChanged()
        {
            NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
        }
    }
}
