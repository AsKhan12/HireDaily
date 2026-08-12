// using Azure.Messaging.ServiceBus;
// using Hiredaily.Modules.Feed.Application;
// using Microsoft.Azure.Functions.Worker;
// using Microsoft.Extensions.DependencyInjection;
// using Microsoft.Extensions.Logging;

// namespace Hiredaily.Host.Function;

// public class JobFeedGenaratorFunction(
//     ILogger<JobFeedGenaratorFunction> _logger,
//     IServiceProvider serviceProvider)
// {

//     // [Function(nameof(JobFeedGenaratorFunction))]
//     public async Task Run(
//         [ServiceBusTrigger("hiredaily-jobs-sb", "mysubscription", Connection = "")]
//         ServiceBusReceivedMessage message,
//         ServiceBusMessageActions messageActions)
//     {
//         _logger.LogInformation("Handling job feed event {eventType} with message id {messageId}.", message.Subject, message.MessageId);

//         var handler = serviceProvider.GetRequiredKeyedService<IIntegrationEventHandler>(message.Subject);
//         await handler.HandleAsync(message.Body.ToString());
//             // Complete the message
//         await messageActions.CompleteMessageAsync(message);
//     }
// }
