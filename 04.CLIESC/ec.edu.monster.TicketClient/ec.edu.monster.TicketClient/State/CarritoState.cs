namespace ec.edu.monster.TicketClient.State;

public class CarritoState
{
    // Lista de códigos de asiento seleccionados (Ej: "AS-L009-02")
    public List<string> AsientosSeleccionados { get; private set; } = new List<string>();

    // Evento para notificar a la interfaz gráfica que el carrito cambió (para actualizar el ícono de número de items)
    public event Action? OnChange;

    public void AgregarAsiento(string codigoAsiento)
    {
        if (!AsientosSeleccionados.Contains(codigoAsiento))
        {
            AsientosSeleccionados.Add(codigoAsiento);
            NotifyStateChanged();
        }
    }

    public void RemoverAsiento(string codigoAsiento)
    {
        if (AsientosSeleccionados.Contains(codigoAsiento))
        {
            AsientosSeleccionados.Remove(codigoAsiento);
            NotifyStateChanged();
        }
    }

    public void VaciarCarrito()
    {
        AsientosSeleccionados.Clear();
        NotifyStateChanged();
    }

    private void NotifyStateChanged() => OnChange?.Invoke();
}