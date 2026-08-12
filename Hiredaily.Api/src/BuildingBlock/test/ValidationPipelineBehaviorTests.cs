// using Hiredaily.BuildingBlock.Application.Mediator.Pipeline.ValidationBehaviour;
// using Hiredaily.BuildingBlock.Application.Mediator.Requests;
// using Hiredaily.BuildingBlock.Application.Mediator.src.Results;
// using NSubstitute;

// namespace Mediator.Tests;

// public class ValidationPipelineBehaviorTests
// {
//     [Fact]
//     public async Task Start_WhenValidationFails_ReturnsInvalidResultAndDoesNotCallNext()
//     {
//         var request = new TestRequest();
//         var validator = Substitute.For<IValidator<TestRequest>>();
//         var next = Substitute.For<IPipelineBehavior<TestRequest, TestResponse>>();
//         var expectedResult = ValidationResult.InValid(
//         [
//             new ValidationError
//             {
//                 PropertyName = nameof(TestRequest.RequestedBy),
//                 ErrorMessage = "RequestedBy is required."
//             }
//         ]);
//         validator.ValidateAsync(request).Returns(expectedResult);
//         var behavior = new ValidationPipelineBehavior<TestRequest, TestResponse>(validator)
//         {
//             Next = next
//         };

//         var result = await behavior.Start(request);

//         Assert.Same(expectedResult, result);
//         await next.DidNotReceive().Start(Arg.Any<TestRequest>());
//     }

//     [Fact]
//     public async Task Start_WhenValidationSucceedsAndNextExists_ReturnsNextResult()
//     {
//         var request = new TestRequest();
//         var validator = Substitute.For<IValidator<TestRequest>>();
//         var next = Substitute.For<IPipelineBehavior<TestRequest, TestResponse>>();
//         var expectedResult = ValidationResult.Valid();
//         validator.ValidateAsync(request).Returns(ValidationResult.Valid());
//         next.Start(request).Returns(expectedResult);
//         var behavior = new ValidationPipelineBehavior<TestRequest, TestResponse>(validator)
//         {
//             Next = next
//         };

//         var result = await behavior.Start(request);

//         Assert.Same(expectedResult, result);
//         await next.Received(1).Start(request);
//     }

//     [Fact]
//     public async Task Start_WhenValidationSucceedsAndNoNextExists_ReturnsValidResult()
//     {
//         var request = new TestRequest();
//         var validator = Substitute.For<IValidator<TestRequest>>();
//         validator.ValidateAsync(request).Returns(ValidationResult.Valid());
//         var behavior = new ValidationPipelineBehavior<TestRequest, TestResponse>(validator);

//         var result = await behavior.Start(request);

//         Assert.True(result.IsValid);
//     }

//     public sealed class TestRequest : IRequest<IResult<TestResponse>>
//     {
//         public Guid RequestId { get; set; } = Guid.NewGuid();
//         public DateTime RequestedAt { get; set; } = DateTime.UtcNow;
//         public string RequestedBy { get; set; } = string.Empty;
//     }

//     public sealed class TestResponse;
// }
