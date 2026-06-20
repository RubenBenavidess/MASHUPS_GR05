using ec.edu.monster.TicketClient.Modelos;

namespace ec.edu.monster.TicketClient.State;

public class CarritoState
{
    public List<Seat> AsientosSeleccionados { get; private set; } = new();

    public int Count => AsientosSeleccionados.Count;

    public event Action? OnChange;

    public void AgregarAsiento(Seat seat)
    {
        if (!AsientosSeleccionados.Any(s => s.Id == seat.Id))
        {
            AsientosSeleccionados.Add(seat);
            NotifyStateChanged();
        }
    }

    public void RemoverAsiento(string codigoAsiento)
    {
        var removed = AsientosSeleccionados.RemoveAll(s => s.Id == codigoAsiento);
        if (removed > 0)
            NotifyStateChanged();
    }

    public void VaciarCarrito()
    {
        AsientosSeleccionados.Clear();
        NotifyStateChanged();
    }

    public decimal GetTotal() => AsientosSeleccionados.Sum(s => s.Price);

    public string[] GetCodigosAsientos() => AsientosSeleccionados.Select(s => s.Id).ToArray();

    private void NotifyStateChanged() => OnChange?.Invoke();
}
