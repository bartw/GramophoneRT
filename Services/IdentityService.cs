using System.Linq;
using Windows.Security.Credentials;

namespace Services
{
    public class IdentityService
    {
        private readonly string _resourceKey = "GramophoneKey";
        private readonly string _resourceSecret = "GramophoneSecret";

        public string TokenKey { get; private set; }
        public string TokenSecret { get; private set; }
        public string Username { get; private set; }

        public bool IsLoggedIn
        {
            get
            {
                return !string.IsNullOrEmpty(TokenKey) && !string.IsNullOrEmpty(TokenSecret) && !string.IsNullOrEmpty(Username);
            }
        }

        public IdentityService()
        {
            var vault = new PasswordVault();
            var credentials = vault.RetrieveAll();

            var keyCredential = (from credential in credentials
                                 where credential.Resource == _resourceKey && !string.IsNullOrEmpty(credential.UserName)
                                 select credential).FirstOrDefault();

            if (keyCredential != null)
            {
                var secretCredential = (from credential in credentials
                                        where credential.Resource == _resourceSecret && credential.UserName == keyCredential.UserName
                                        select credential).FirstOrDefault();

                if (secretCredential != null)
                {
                    keyCredential.RetrievePassword();
                    secretCredential.RetrievePassword();

                    Username = keyCredential.UserName;
                    TokenKey = keyCredential.Password;
                    TokenSecret = secretCredential.Password;
                }
            }
        }

        public void Authorize(string tokenKey, string tokenSecret)
        {
            TokenKey = tokenKey;
            TokenSecret = tokenSecret;
        }

        public void Identify(string username)
        {
            ClearVault();

            Username = username;

            var vault = new PasswordVault();

            vault.Add(new PasswordCredential(_resourceKey, Username, TokenKey));
            vault.Add(new PasswordCredential(_resourceSecret, Username, TokenSecret));
        }

        public void Logout()
        {
            ClearVault();

            TokenKey = null;
            TokenSecret = null;
            Username = null;
        }

        private void ClearVault()
        {
            var vault = new PasswordVault();

            var toRemove = vault.RetrieveAll();

            foreach (var item in toRemove)
            {
                vault.Remove(item);
            }
        }
    }
}
