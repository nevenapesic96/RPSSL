using FluentValidation;

namespace Api.Configuration;

public static class ValidationFilter
{
    public static RouteHandlerBuilder RequireValidation<T>(this RouteHandlerBuilder endpoint) where T : class
    {
        endpoint.AddEndpointFilter(async (context, next) =>
        {
            if (context.Arguments.FirstOrDefault(a => a is T) is not T arg)
                return Results.BadRequest("Invalid input");

            var validator = context.HttpContext.RequestServices.GetService<IValidator<T>>();
            if (validator == null)
                return Results.Problem("Validator not registered");
            var validationResult = await validator.ValidateAsync(arg);

            if (!validationResult.IsValid)
                return Results.ValidationProblem(validationResult.ToDictionary());

            return await next(context);
        });

        return endpoint;
    }
}