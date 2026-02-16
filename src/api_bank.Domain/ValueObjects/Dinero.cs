using api_bank.Domain.Exceptions;

namespace api_bank.Domain.ValueObjects;

public record Dinero
{
    public decimal Cantidad { get; init; }
    public string Divisa { get; init; }

    public Dinero(decimal cantidad, string divisa)
    {
        if (cantidad <= 0)
        {
            throw new InvalidAmountException(cantidad);
        }

        Cantidad = cantidad;
        Divisa = divisa ?? throw new ArgumentNullException(nameof(divisa));
    }

    public bool EsSuficiente(Dinero monto)
    {
        ValidarMismaDivisa(monto);
        return Cantidad >= monto.Cantidad;
    }

    public Dinero Sumar(Dinero otro)
    {
        ValidarMismaDivisa(otro);
        return new Dinero(Cantidad + otro.Cantidad, Divisa);
    }

    public Dinero Restar(Dinero otro)
    {
        ValidarMismaDivisa(otro);
        
        var resultado = Cantidad - otro.Cantidad;
        if (resultado < 0)
        {
            throw new InvalidAmountException(resultado);
        }

        // Permitir cero para operaciones de débito
        if (resultado == 0)
        {
            return new Dinero(0m, Divisa, permitirCero: true);
        }

        return new Dinero(resultado, Divisa);
    }

    private Dinero(decimal cantidad, string divisa, bool permitirCero)
    {
        if (!permitirCero && cantidad <= 0)
        {
            throw new InvalidAmountException(cantidad);
        }

        if (cantidad < 0)
        {
            throw new InvalidAmountException(cantidad);
        }

        Cantidad = cantidad;
        Divisa = divisa ?? throw new ArgumentNullException(nameof(divisa));
    }

    private void ValidarMismaDivisa(Dinero otro)
    {
        if (Divisa != otro.Divisa)
        {
            throw new ArgumentException($"Las operaciones solo pueden realizarse con la misma divisa. Divisa actual: {Divisa}, Divisa proporcionada: {otro.Divisa}");
        }
    }
}
