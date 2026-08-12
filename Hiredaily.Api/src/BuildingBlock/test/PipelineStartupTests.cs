// using Hiredaily.BuildingBlock.Application.Mediator.Pipeline.ValidationBehaviour;
// using Hiredaily.BuildingBlock.Application.Mediator.Requests;
// using Hiredaily.BuildingBlock.Application.Mediator.src.Results;
// using NSubstitute;

// namespace Mediator.Tests;

// public class PipelineStartupTests
// {
//     [Fact]
//     public async Task Run_WhenCollectionHasNoBehavior_ReturnsValidResult()
//     {
//         var collection = new BehaviorCollection<TestRequest, TestResponse>();
//         var startup = new PipelineStartup<TestRequest, TestResponse>(collection);

//         var result = await startup.Run(new TestRequest());

//         Assert.True(result.IsValid);
//     }

//     [Fact]
//     public async Task Run_WhenCollectionHasFirstBehavior_StartsFirstBehavior()
//     {
//         var request = new TestRequest();
//         var expectedResult = ValidationResult.Valid();
//         var behavior = Substitute.For<IPipelineBehavior<TestRequest, TestResponse>>();
//         var collection = Substitute.For<IBehaviorCollection<TestRequest, TestResponse>>();
//         behavior.Start(request).Returns(expectedResult);
//         collection.First.Returns(behavior);
//         var startup = new PipelineStartup<TestRequest, TestResponse>(collection);

//         var result = await startup.Run(request);

//         Assert.Same(expectedResult, result);
//         await behavior.Received(1).Start(request);
//     }

//     public sealed class TestRequest : IRequest<IResult<TestResponse>>
//     {
//         public Guid RequestId { get; set; } = Guid.NewGuid();
//         public DateTime RequestedAt { get; set; } = DateTime.UtcNow;
//         public string RequestedBy { get; set; } = string.Empty;
//     }

//     public sealed class TestResponse;
// }
