using Hiredaily.Modules.Feed.Application.Common;

namespace Hiredaily.Modules.Feed.API.InputRequestModels;

public class JobFeedRequestInput
{
    public Location? Location { get; init; }
    public List<string>? Skills { get; init;}
}