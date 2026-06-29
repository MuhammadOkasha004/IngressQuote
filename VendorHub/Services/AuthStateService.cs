namespace VendorHub.Services
{
    public class AuthStateService
    {
        public string UserId { get; private set; } = "";
        public string UserName { get; private set; } = "";
        public string UserRole { get; private set; } = "";
        public string UserEmail { get; private set; } = "";
        public int CompanyId { get; private set; } = 1;
        public int VendorId { get; private set; }

        public bool IsLoggedIn => !string.IsNullOrEmpty(UserRole);

        public event Action? OnAuthStateChanged;

        public void SetUser(string userId, string userName, string userRole, string userEmail, int companyId = 1, int vendorId = 0)
        {
            UserId = userId;
            UserName = userName;
            UserRole = userRole;
            UserEmail = userEmail;
            CompanyId = companyId;
            VendorId = vendorId;
            NotifyChanged();
        }

        public void Clear()
        {
            UserId = "";
            UserName = "";
            UserRole = "";
            UserEmail = "";
            CompanyId = 1;
            VendorId = 0;
            NotifyChanged();
        }

        private void NotifyChanged()
        {
            OnAuthStateChanged?.Invoke();
        }
    }
}
