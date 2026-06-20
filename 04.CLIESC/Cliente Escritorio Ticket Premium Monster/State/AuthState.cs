using System;

namespace ec.edu.monster.TicketClient.State
{
    public class AuthState
    {
        public bool IsLoggedIn { get; private set; }
        public string Token { get; private set; } = string.Empty;
        public string Cedula { get; private set; } = string.Empty;
        public string Nombre { get; private set; } = string.Empty;
        public string Rol { get; private set; } = string.Empty;

        public bool IsAdmin => IsLoggedIn && Rol == "ADMIN";

        public event Action? OnChange;

        public void Login(string token, string cedula, string nombre, string rol)
        {
            IsLoggedIn = true;
            Token = token;
            Cedula = cedula;
            Nombre = nombre;
            Rol = rol;
            NotifyStateChanged();
        }

        public void Logout()
        {
            IsLoggedIn = false;
            Token = string.Empty;
            Cedula = string.Empty;
            Nombre = string.Empty;
            Rol = string.Empty;
            NotifyStateChanged();
        }

        private void NotifyStateChanged() => OnChange?.Invoke();
    }
}
