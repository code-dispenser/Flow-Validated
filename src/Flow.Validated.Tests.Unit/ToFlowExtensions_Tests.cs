using Flow.Core.Areas.Extensions;
using Flow.Core.Areas.Returns;
using Flow.Validated.Tests.Unit.Common.Models;
using FluentAssertions;
using FluentAssertions.Execution;
using Validated.Core.Common.Constants;
using Validated.Core.Extensions;
using Validated.Core.Types;
using Xunit.Sdk;

namespace Flow.Validated.Tests.Unit;

public class ToFlowExtensions_Tests
{
    [Fact]
    public void To_flow_should_transfer_a_valid_validated_to_a_successful_flow()
    {
        var dateTime  = new DateTime(2000, 6, 15);
        var validated = Validated<Contractor>.Valid(new Contractor("Jim Bob", 42, dateTime));

        var flowResult  = validated.ToFlow();
        var flowContent = flowResult.Finally(f => throw new XunitException("Should not be a failure"), success => success);

        using(new AssertionScope())
        {
            flowResult.Should().Match<Flow<Contractor>>(r => r.IsSuccess == true);

            flowContent.Should().Match<Contractor>(s => s.Name == "Jim Bob" && s.Age == 42 && s.StartDate == dateTime);
        }
    }

    [Fact]
    public async Task To_flow_should_transfer_a_valid_validated_task_to_a_successful_flow()
    {
        var dateTime  = new DateTime(2000, 6, 15);
        var validated = Task.FromResult(Validated<Contractor>.Valid(new Contractor("Jim Bob", 42, dateTime)));

        var flowResult  = await validated.ToFlow();
        var flowContent = flowResult.Finally(f => throw new XunitException("Should not be a failure"), success => success);

        using (new AssertionScope())
        {
            flowResult.Should().Match<Flow<Contractor>>(r => r.IsSuccess == true);

            flowContent.Should().Match<Contractor>(s => s.Name == "Jim Bob" && s.Age == 42 && s.StartDate == dateTime);
        }
    }

    [Fact]
    public void To_flow_should_transfer_a_invalid_validated_to_a_failed_flow_using_the_invalid_entry_failure_type_with_a_retry_of_true()
    {
        var failureMessage = "Incorrect start date";
        var path           = String.Join(nameof(Contractor),".",nameof(Contractor.StartDate));
        var validated      = Validated<Contractor>.Invalid(new InvalidEntry(failureMessage, path, nameof(Contractor.StartDate), nameof(Contractor.StartDate), CauseType.Validation));

        var flowResult  = validated.ToFlow();
        var flowContent = flowResult.Finally(failure => (Failure.InvalidEntryFailure)failure , success => throw new XunitException("Should not be a success"));

        using (new AssertionScope())
        {
            flowResult.Should().Match<Flow<Contractor>>(r => r.IsSuccess == false);
            flowContent.Should().Match<Failure.InvalidEntryFailure>(f => f.InvalidEntries.Count == 1 && f.CanRetry == true
                                                                    && f.Exception == null && f.SubTypeID == 0 && f.Reason == "Validation failed with 1 error(s)");
            
            flowContent.OccurredAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));

            flowContent.Details.Should().BeEmpty();
        }
    }
    [Fact]
    public async Task To_flow_should_transfer_a_invalid_validated_task_to_a_failed_flow_using_the_invalid_entry_failure_type_with_a_retry_of_true()
    {
        var failureMessage = "Incorrect start date";
        var path           = String.Join(nameof(Contractor), ".", nameof(Contractor.StartDate));
        var validated      = Task.FromResult(Validated<Contractor>.Invalid(new InvalidEntry(failureMessage, path, nameof(Contractor.StartDate), nameof(Contractor.StartDate), CauseType.Validation)));
    
        var flowResult  = await validated.ToFlow();
        var flowContent = flowResult.Finally(failure => (Failure.InvalidEntryFailure)failure, success => throw new XunitException("Should not be a success"));

        using (new AssertionScope())
        {
            flowResult.Should().Match<Flow<Contractor>>(r => r.IsSuccess == false);
            flowContent.Should().Match<Failure.InvalidEntryFailure>(f => f.InvalidEntries.Count == 1 && f.CanRetry == true
                                                                    && f.Exception == null && f.SubTypeID == 0 && f.Reason == "Validation failed with 1 error(s)");

            flowContent.OccurredAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));

            flowContent.Details.Should().BeEmpty();
        }
    }
    [Fact]
    public void To_flow_should_transfer_a_invalid_validated_to_a_failed_flow_using_the_invalid_entry_failure_type_with_a_retry_of_false()
    {
        var failureMessage = "date";
        var path           = nameof(Contractor);
        var validated      = Validated<Contractor>.Invalid
                        ([
                            new InvalidEntry(failureMessage, path, nameof(Contractor.Age), nameof(Contractor.Age), CauseType.SystemError),
                            new InvalidEntry(failureMessage, path, nameof(Contractor.StartDate), nameof(Contractor.StartDate), CauseType.Validation)
                        ]);

        var flowResult  = validated.ToFlow();
        var flowContent = flowResult.Finally(failure => (Failure.InvalidEntryFailure)failure, success => throw new XunitException("Should not be a success"));

        using (new AssertionScope())
        {
            flowResult.Should().Match<Flow<Contractor>>(r => r.IsSuccess == false);
            flowContent.Should().Match<Failure.InvalidEntryFailure>(f => f.InvalidEntries.Count == 2 && f.CanRetry == false
                                                                    && f.Exception == null && f.SubTypeID == 0 && f.Reason == "Validation failed with 2 error(s)");

            flowContent.OccurredAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
            flowContent.Details.Should().BeEmpty();
        }
    }

    [Fact]
    public async Task To_flow_should_transfer_a_invalid_validated_task_to_a_failed_flow_using_the_invalid_entry_failure_type_with_a_retry_of_false()
    {
        var failureMessage = "date";
        var path           = nameof(Contractor);
        var validated      = Task.FromResult(Validated<Contractor>.Invalid
                           ([
                               new InvalidEntry(failureMessage, path, nameof(Contractor.Age), nameof(Contractor.Age), CauseType.SystemError),
                               new InvalidEntry(failureMessage, path, nameof(Contractor.StartDate), nameof(Contractor.StartDate), CauseType.Validation)
                           ]));

        var flowResult  = await validated.ToFlow();
        var flowContent = flowResult.Finally(failure => (Failure.InvalidEntryFailure)failure, success => throw new XunitException("Should not be a success"));

        using (new AssertionScope())
        {
            flowResult.Should().Match<Flow<Contractor>>(r => r.IsSuccess == false);
            flowContent.Should().Match<Failure.InvalidEntryFailure>(f => f.InvalidEntries.Count == 2 && f.CanRetry == false
                                                                    && f.Exception == null && f.SubTypeID == 0 && f.Reason == "Validation failed with 2 error(s)");

            flowContent.OccurredAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
            flowContent.Details.Should().BeEmpty();
        }
    }
}
