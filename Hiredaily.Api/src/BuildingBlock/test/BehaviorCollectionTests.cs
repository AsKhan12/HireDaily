// using Hiredaily.BuildingBlock.Application.Mediator.Pipeline;
// using Hiredaily.BuildingBlock.Application.Mediator.Requests;
// using Hiredaily.BuildingBlock.Application.Mediator.src.Results;
// using NSubstitute;

// namespace Mediator.Tests;

// public class BehaviorCollectionTests
// {
//     [Fact]
//     public void Add_WhenFirstBehaviorIsAdded_SetsFirstBehavior()
//     {
//         var behavior = Substitute.For<IPipelineBehavior>();
//         var collection = new BehaviorCollection();

//         collection.Add(behavior);

//         Assert.Same(behavior, collection.First);
//     }

//     [Fact]
//     public void Add_WhenMultipleBehaviorsAreAdded_ChainsBehaviorsInOrder()
//     {
//         var firstBehavior = Substitute.For<IPipelineBehavior<TestRequest, TestResponse>>();
//         var secondBehavior = Substitute.For<IPipelineBehavior<TestRequest, TestResponse>>();
//         var thirdBehavior = Substitute.For<IPipelineBehavior<TestRequest, TestResponse>>();
//         var collection = new BehaviorCollection<TestRequest, TestResponse>();

//         collection.Add(firstBehavior);
//         collection.Add(secondBehavior);
//         collection.Add(thirdBehavior);

//         Assert.Same(firstBehavior, collection.First);
//         Assert.Same(secondBehavior, firstBehavior.Next);
//         Assert.Same(thirdBehavior, secondBehavior.Next);
//     }

//     public sealed class TestRequest : IRequest<IResult<TestResponse>>
//     {
//         public Guid RequestId { get; set; } = Guid.NewGuid();
//         public DateTime RequestedAt { get; set; } = DateTime.UtcNow;
//         public string RequestedBy { get; set; } = string.Empty;
//     }

//     public sealed class TestResponse;
// }
