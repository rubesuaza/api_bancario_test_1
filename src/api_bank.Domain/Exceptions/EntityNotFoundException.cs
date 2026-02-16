namespace api_bank.Domain.Exceptions;

public class EntityNotFoundException : Exception
{
    public EntityNotFoundException(string message) : base(message)
    {
    }

    public EntityNotFoundException(string entityName, Guid id) 
        : base($"La entidad {entityName} con ID {id} no fue encontrada.")
    {
    }
}
