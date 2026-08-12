namespace Hiredaily.Modules.Jobs.Infra.Persistence.SQL;

public class JobsDbSettings
{
    public const string Sectionname = "ConnectionStrings";
    public required string JobsDbConnection { get; set; }
}