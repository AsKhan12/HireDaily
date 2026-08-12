using Hiredaily.Modules.Feed.Application.Common;

namespace Hiredaily.Modules.Feed.Application.Write;

public record JobTitleUpdatedMessage(string Title, JobId JobId);
