// using Hiredaily.BuildingBlock.Application.Mediator.Handlers;
// using Hiredaily.BuildingBlock.Application.Mediator.Pipeline.ValidationBehaviour;
// using Hiredaily.BuildingBlock.Application.Mediator.Requests;
// using Hiredaily.BuildingBlock.Application.Mediator.src.Results;
// using Microsoft.Extensions.DependencyInjection;
// using NSubstitute;

// namespace Mediator.Tests;

// public class MediatrTests
// {
//     [Fact]
//     public async Task Send_WhenPipelineIsValid_CallsHandlerAndReturnsResponse()
//     {
//         var request = new TestRequest();
//         var response = new TestResponse();
//         var handler = Substitute.For<IRequestHandler<TestRequest, TestResponse>>();
//         var behavior = Substitute.For<IPipelineBehavior<TestRequest, TestResponse>>();
//         var collection = Substitute.For<IBehaviorCollection<TestRequest, TestResponse>>();
//         handler.Handle(request).Returns(response);
//         behavior.Start(request).Returns(ValidationResult.Valid());
//         collection.First.Returns(behavior);
//         var mediatr = CreateMediatr(collection, handler);

//         var result = await mediatr.Send<TestRequest, TestResponse>(request);

//         Assert.True(result.IsSuccess);
//         Assert.Same(response, result.Response);
//         Assert.Null(result.Error);
//         Assert.True(result.ValidationResult.IsValid);
//         await handler.Received(1).Handle(request);
//     }

//     [Fact]
//     public async Task Send_WhenPipelineIsInvalid_ReturnsFailureAndDoesNotCallHandler()
//     {
//         var request = new TestRequest();
//         var handler = Substitute.For<IRequestHandler<TestRequest, TestResponse>>();
//         var behavior = Substitute.For<IPipelineBehavior<TestRequest, TestResponse>>();
//         var collection = Substitute.For<IBehaviorCollection<TestRequest, TestResponse>>();
//         var validationErrors = new[]
//         {
//             new ValidationError
//             {
//                 PropertyName = nameof(TestRequest.RequestedBy),
//                 ErrorMessage = "RequestedBy is required."
//             }
//         };
//         behavior.Start(request).Returns(ValidationResult.InValid(validationErrors));
//         collection.First.Returns(behavior);
//         var mediatr = CreateMediatr(collection, handler);

//         var result = await mediatr.Send<TestRequest, TestResponse>(request);

//         Assert.False(result.IsSuccess);
//         Assert.Null(result.Response);
//         Assert.Equal("validation error!", result.Error);
//         Assert.False(result.ValidationResult.IsValid);
//         Assert.Same(validationErrors, result.ValidationResult.Errors);
//         await handler.DidNotReceive().Handle(Arg.Any<TestRequest>());
//     }

//     private static Hiredaily.BuildingBlock.Application.Mediator.Mediatr CreateMediatr(
//         IBehaviorCollection<TestRequest, TestResponse> collection,
//         IRequestHandler<TestRequest, TestResponse> handler)
//     {
//         var services = new ServiceCollection();
//         services.AddSingleton(collection);
//         services.AddSingleton(new PipelineStartup<TestRequest, TestResponse>(collection));
//         services.AddSingleton(handler);
//         return new Hiredaily.BuildingBlock.Application.Mediator.Mediatr(services.BuildServiceProvider());
//     }

//     public sealed class TestRequest : IRequest<IResult<TestResponse>>
//     {
//         public Guid RequestId { get; set; } = Guid.NewGuid();
//         public DateTime RequestedAt { get; set; } = DateTime.UtcNow;
//         public string RequestedBy { get; set; } = string.Empty;
//     }

//     public sealed class TestResponse;
// }
