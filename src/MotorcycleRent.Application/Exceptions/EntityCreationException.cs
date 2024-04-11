namespace MotorcycleRent.Application.Exceptions;

public sealed class EntityCreationException : Exception
{
    public EntityCreationException(Type entityType, string? entityData) : base($"An error occurred when trying to create an instance of {entityType.Name} with the following main data: {entityData}") { }
}
