namespace api_bank.Domain.Exceptions;

public class SelfTransferException : Exception
{
    public SelfTransferException(string message) : base(message)
    {
    }

    public SelfTransferException(Guid cuentaId) 
        : base($"No se permite realizar transferencias a la misma cuenta. Cuenta ID: {cuentaId}")
    {
    }
}
