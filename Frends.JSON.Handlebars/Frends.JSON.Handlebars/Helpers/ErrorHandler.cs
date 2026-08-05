using System;
using Frends.JSON.Handlebars.Definitions;

namespace Frends.JSON.Handlebars.Helpers;

internal static class ErrorHandler
{
    internal static Result Handle(this Exception exception, Options options, bool throwCanceled = true)
    {
        if (throwCanceled && exception is OperationCanceledException) throw exception;

        if (options.ThrowErrorOnFailure)
        {
            if (string.IsNullOrEmpty(options.ErrorMessageOnFailure))
                throw new Exception(exception.Message, exception);

            throw new Exception(options.ErrorMessageOnFailure, exception);
        }

        var errorMessage = string.IsNullOrEmpty(options.ErrorMessageOnFailure)
            ? exception.Message
            : $"{options.ErrorMessageOnFailure}: {exception.Message}";

        return new Result
        {
            Success = false,
            Error = new Error
            {
                Message = errorMessage,
                AdditionalInfo = exception,
            },
        };
    }
}
