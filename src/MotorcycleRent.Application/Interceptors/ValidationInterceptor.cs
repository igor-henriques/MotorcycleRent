namespace MotorcycleRent.Application.Interceptors;

/// <summary>
/// Dinamically validates DTOs from the services layers.
/// </summary>
public class ValidationInterceptor : DispatchProxy
{
    private object? _target = null;
    private IServiceProvider? _serviceProvider = null;

    protected override object? Invoke(MethodInfo? targetMethod, object?[]? args)
    {
        ArgumentValidator.ThrowIfNullOrDefault(_serviceProvider, nameof(_serviceProvider));
        ArgumentValidator.ThrowIfNullOrDefault(_target, nameof(_target));
        ArgumentValidator.ThrowIfNullOrDefault(targetMethod, nameof(targetMethod));
        ArgumentValidator.ThrowIfNullOrDefault(args, nameof(args));

        var dto = args!.FirstOrDefault(arg => arg is IDto);
        if (dto == null)
        {
            return targetMethod!.Invoke(_target, args);
        }

        var validatorType = typeof(IValidator<>).MakeGenericType(dto!.GetType());

        if (_serviceProvider!.GetService(validatorType) is IValidator validator)
        {
            var context = new ValidationContext<object>(dto);
            var validationResult = validator.Validate(context);
            if (!validationResult.IsValid)
            {
                throw new ValidationException(validationResult.Errors);
            }
        }
        
        return targetMethod!.Invoke(_target, args);
    }

    public static TService Create<TService>(TService target, IServiceProvider serviceProvider)
    {
        object? proxy = Create<TService, ValidationInterceptor>() 
            ?? throw new InvalidOperationException("An error occurred while creating a service proxy");

        ((ValidationInterceptor)proxy)._target = target!;
        ((ValidationInterceptor)proxy)._serviceProvider = serviceProvider;

        return (TService)proxy;
    }
}
