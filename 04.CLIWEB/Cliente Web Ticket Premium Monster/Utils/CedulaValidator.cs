namespace ec.edu.monster.TicketClient.Utils;

public static class CedulaValidator
{
    public static bool EsValida(string cedula, out string mensajeError)
    {
        mensajeError = string.Empty;

        if (string.IsNullOrWhiteSpace(cedula))
        {
            mensajeError = "La cédula no puede estar vacía.";
            return false;
        }

        var trimmed = cedula.Trim();

        if (trimmed.Length != 10)
        {
            mensajeError = "La cédula debe tener exactamente 10 dígitos.";
            return false;
        }

        if (!trimmed.All(char.IsDigit))
        {
            mensajeError = "La cédula debe contener solo números.";
            return false;
        }

        var provincia = int.Parse(trimmed.Substring(0, 2));

        if ((provincia < 1 || provincia > 24) && provincia != 30)
        {
            mensajeError = $"El código de provincia ({provincia:D2}) no es válido. Debe ser 01-24 o 30 (extranjeros).";
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
            mensajeError = "La cédula no es válida. El dígito verificador no coincide.";
            return false;
        }

        return true;
    }
}
