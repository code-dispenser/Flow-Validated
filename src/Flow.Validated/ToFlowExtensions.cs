using Flow.Core.Areas.Returns;
using Validated.Core.Common.Constants;
using Validated.Core.Types;
using FlowModels = Flow.Core.Common.Models;
/*
    * Namespace where Flow extensions are so they merge. 
*/
namespace Flow.Core.Areas.Extensions;

/// <summary>
/// Extension methods for converting a Validated to a Flow.
/// </summary>
public static class ToFlowExtensions
{
    /// <summary>
    /// Converts a Validated instance to a Flow instance.
    /// If the validation is successful, returns a successful Flow with the value.
    /// If the validation failed, returns a failed Flow with an InvalidEntryFailure containing all validation errors.
    /// </summary>
    /// <typeparam name="T">The type of the validated value.</typeparam>
    /// <param name="validated">The validated instance to convert.</param>
    /// <returns>A Flow instance representing the validation result.</returns>
    public static Flow<T> ToFlow<T>(this Validated<T> validated) where T : notnull
    {
        if (validated.IsValid) return Flow<T>.Success(validated.GetValueOr(default!));

        var hasError = validated.Failures.Any(i => i.Cause != CauseType.Validation);
       
        return Flow<T>.Failed(new Failure.InvalidEntryFailure([.. validated.Failures.Select(v => new FlowModels.InvalidEntry(v.FailureMessage, v.Path, v.PropertyName, v.DisplayName, v.Cause.ToString()))],
                                                                    $"Validation failed with {validated.Failures.Count} error(s)", null, 0, !hasError));
    }

    // <summary>
    /// Converts a Task of Validated to a Task of Flow.
    /// </summary>
    /// <typeparam name="T">The type of the validated value.</typeparam>
    /// <param name="validatedTask">The task returning a validated instance.</param>
    /// <returns>A task returning a Flow instance.</returns>
    public static async Task<Flow<T>> ToFlow<T>(this Task<Validated<T>> validated) where T : notnull

        => (await validated.ConfigureAwait(false)).ToFlow();
     
    
}
