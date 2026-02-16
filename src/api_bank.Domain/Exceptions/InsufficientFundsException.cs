namespace api_bank.Domain.Exceptions;

public class InsufficientFundsException : Exception
{
    public InsufficientFundsException(string message) : base(message)
    {
    }

    public InsufficientFundsException(decimal saldoActual, decimal montoSolicitado) 
        : base($"Saldo insuficiente. Saldo actual: {saldoActual}, Monto solicitado: {montoSolicitado}")
    {
    }
}
