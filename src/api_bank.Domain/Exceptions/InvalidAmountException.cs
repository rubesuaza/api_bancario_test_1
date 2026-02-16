namespace api_bank.Domain.Exceptions;

public class InvalidAmountException : Exception
{
    public InvalidAmountException(string message) : base(message)
    {
    }

    public InvalidAmountException(decimal monto) 
        : base($"El monto debe ser mayor a cero. Monto proporcionado: {monto}")
    {
    }
}
