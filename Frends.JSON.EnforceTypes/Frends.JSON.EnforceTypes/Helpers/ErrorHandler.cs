using System;
using Frends.JSON.EnforceTypes.Definitions;

namespace Frends.JSON.EnforceTypes.Helpers;

internal static class ErrorHandler
{
    /// <summary>
    /// Handles an exception and returns a Result or rethrows depending on options.
    /// </summary>
    /// <param name="exception">The exception to handle.</param>
    /// <param name="options">Task options that control whether failures are returned as a Result object or thrown.</param>
    /// <param name="throwCanceled">
    /// When true, an OperationCanceledException is rethrown immediately.
    /// When false, cancellation is handled like any other failure.
    /// </param>
    internal static Result Handle(this Exception exception, Options options, bool throwCanceled = true)
    {
        ThrowIfCanceled(exception, throwCanceled);
        if (options.ThrowErrorOnFailure) ThrowBaseException(exception, options.ErrorMessageOnFailure);

        return ReturnResult(exception, options.ErrorMessageOnFailure);
    }

    private static void ThrowIfCanceled(Exception exception, bool throwCanceled = true)
    {
        if (throwCanceled && exception is OperationCanceledException) throw exception;
    }

    private static void ThrowBaseException(Exception exception, string customMessage = null)
    {
        if (string.IsNullOrEmpty(customMessage))
            throw new Exception(exception.Message, exception);

        throw new Exception(customMessage, exception);
    }

    private static Result ReturnResult(Exception exception, string customMessage = null)
    {
        var errorMessage = string.IsNullOrEmpty(customMessage)
            ? exception.Message
            : $"{customMessage}: {exception.Message}";

        return new Result(new Error
        {
            Message = errorMessage,
            AdditionalInfo = exception,
        });
    }
}
