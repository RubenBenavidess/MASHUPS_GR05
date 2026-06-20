namespace TicketPremium.Console.Utils;

public static class CedulaValidator
{
    public static bool EsValida(string cedula, out string mensajeError)
    {
        mensajeError = string.Empty;

        if (string.IsNullOrWhiteSpace(cedula))
        {
            mensajeError = "La cedula no puede estar vacia.";
            return false;
        }

        var trimmed = cedula.Trim();

        if (trimmed.Length != 10)
        {
            mensajeError = "La cedula debe tener exactamente 10 digitos.";
            return false;
        }

        if (!trimmed.All(char.IsDigit))
        {
            mensajeError = "La cedula debe contener solo numeros.";
            return false;
        }

        var provincia = int.Parse(trimmed.Substring(0, 2));

        if ((provincia < 1 || provincia > 24) && provincia != 30)
        {
            mensajeError = $"El codigo de provincia ({provincia:D2}) no es valido. Debe ser 01-24 o 30 (extranjeros).";
            return false;
        }

        var digitos = trimmed.Select(c => int.Parse(c.ToString())).ToArray();
        var verificador = digitos[9];

        var suma = 0;
        for (int i = 0; i < 9; i++)
        {
            int valor = digitos[i] * (2 - i % 2);
            if (valor >= 10)
                valor -= 9;
            suma += valor;
        }

        var digitoCalculado = (10 - suma % 10) % 10;

        if (digitoCalculado != verificador)
        {
            mensajeError = "La cedula no es valida. El digito verificador no coincide.";
            return false;
        }

        return true;
    }
}
