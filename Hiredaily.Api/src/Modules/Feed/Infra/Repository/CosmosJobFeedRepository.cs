
using Hiredaily.Modules.Feed.Application.Common;
using Microsoft.Azure.Cosmos;
using Newtonsoft.Json;

namespace Hiredaily.Modules.Feed.Infra.Repository;

public partial class CosmosJobFeedRepository(Container container) : IJobFeedRepository
{
    public async Task<IReadOnlyList<JobFeed>> GetJobFeed(
    Location? location,
    List<string>? skillNames,
    CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(location);
        ArgumentNullException.ThrowIfNull(skillNames);

        var bucketsToQuery =
            PartionKeyHashGenerator.GetNearbyBuckets(location);

        if (skillNames.Count == 0)
        {
            return [];
        }

        var sql =
    """
    SELECT *
    FROM c
    WHERE
    """ +
    string.Join(
        " OR ",
        skillNames.Select((_, index) =>
            $" EXISTS(SELECT VALUE 1 FROM skill IN c.requiredSkills WHERE skill.name = @skill{index})"));

        var query = new QueryDefinition(sql);

        for (var i = 0; i < skillNames.Count; i++)
        {
            query.WithParameter($"@skill{i}", skillNames[i]);
        }

        var results = new List<JobFeed>();

        foreach (var bucket in bucketsToQuery)
        {
            var iterator =
                container.GetItemQueryIterator<JobFeedDocument>(
                    query,
                    requestOptions: new QueryRequestOptions
                    {
                        PartitionKey =
                            new PartitionKey(bucket)
                    });

            while (iterator.HasMoreResults)
            {
                var response =
                    await iterator.ReadNextAsync(cancellationToken);

                results.AddRange(response.Select(ToJobFeed));

            }
        }

        return results;
    }

    public async Task Insert(JobFeed feed, CancellationToken cancellationToken = default)
    {
        var document = JobFeedDocument.From(feed);
        await container.UpsertItemAsync(document, new PartitionKey(document.LocationBucket), cancellationToken: cancellationToken);
    }

    public async Task UpdateHourlyRate(JobId jobId, DateTime timestamp, Money hourlyRate, CancellationToken cancellationToken = default)
    {
        var document = await GetByJobId(jobId, cancellationToken);
        document.HourlyRate = hourlyRate;
        document.JobLastUpdatedAt = timestamp;

        await Save(document, document.LocationBucket, cancellationToken);
    }

    public async Task UpdateJobSite(JobId jobId, DateTime timestamp, JobSite jobSite, CancellationToken cancellationToken = default)
    {
        var document = await GetByJobId(jobId, cancellationToken);
        var previousPartitionKey = document.LocationBucket;

        document.JobSite = jobSite;
        document.GeoHash = PartionKeyHashGenerator.CreateFrom(jobSite.Location);
        document.LocationBucket = PartionKeyHashGenerator.CreatePartitionKey(jobSite.Location);
        document.JobLastUpdatedAt = timestamp;

        await Save(document, previousPartitionKey);
    }

    public async Task UpdateRequiredSkills(JobId jobId, DateTime timestamp, IReadOnlyList<Skill> requiredSkills, CancellationToken cancellationToken = default)
    {
        var document = await GetByJobId(jobId, cancellationToken);
        var previousPartitionKey = document.LocationBucket;

        document.RequiredSkills = requiredSkills;
        document.LocationBucket = PartionKeyHashGenerator.CreatePartitionKey(document.JobSite.Location);
        document.JobLastUpdatedAt = timestamp;

        await Save(document, previousPartitionKey, cancellationToken);
    }

    private static JobFeed ToJobFeed(JobFeedDocument document)
    {
        return JobFeed.Create(
            document.Timestamp,
            new JobId { Value = Guid.Parse(document.JobId) },
            document.Title,
            document.JobCreatedAt,
            document.JobLastUpdatedAt,
            new OrganizationId { Value = Guid.Parse(document.OrganizationId) },
            document.HourlyRate,
            document.JobSite,
            document.RequiredSkills);
    }

    private async Task<JobFeedDocument> GetByJobId(JobId jobId, CancellationToken cancellationToken = default)
    {
        var query = new QueryDefinition("SELECT * FROM c WHERE c.jobId = @jobId")
            .WithParameter("@jobId", jobId.Value.ToString());

        using var iterator = container.GetItemQueryIterator<JobFeedDocument>(query);

        while (iterator.HasMoreResults)
        {
            var response = await iterator.ReadNextAsync(cancellationToken);
            var document = response.FirstOrDefault();

            if (document is not null)
                return document;
        }

        throw new InvalidDataException($"Job feed document was not found for job id '{jobId.Value}'.");
    }

    private async Task Save(JobFeedDocument document, string previousPartitionKey, CancellationToken cancellationToken = default)
    {
        await container.UpsertItemAsync(document, new PartitionKey(document.LocationBucket), cancellationToken: cancellationToken);

        if (previousPartitionKey == document.LocationBucket)
            return;

        await container.DeleteItemAsync<JobFeedDocument>(document.Id, new PartitionKey(previousPartitionKey), cancellationToken: cancellationToken);
    }

    public async Task UpdateTitle(JobId jobId, string title, DateTime timestamp, CancellationToken cancellationToken = default)
    {
        var document = await GetByJobId(jobId, cancellationToken);
        document.Title = title;
        document.JobLastUpdatedAt = timestamp;

        await Save(document, document.LocationBucket, cancellationToken);
    }

    private sealed class JobFeedDocument
    {
        [JsonProperty("id")]
        public string Id { get; set; } = string.Empty;

        [JsonProperty("title")]
        public string Title { get; set; } = string.Empty;

        [JsonProperty("aggregateId")]
        public string AggregateId { get; set; } = string.Empty;

        [JsonProperty("timestamp")]
        public DateTime Timestamp { get; set; }

        [JsonProperty("jobId")]
        public string JobId { get; set; } = string.Empty;

        [JsonProperty("organizationId")]
        public string OrganizationId { get; set; } = string.Empty;

        [JsonProperty("geoHash")]
        public string GeoHash { get; set; } = string.Empty;

        [JsonProperty("locationBucket")]
        public string LocationBucket { get; set; } = string.Empty;

        [JsonProperty("createdAt")]
        public DateTime CreatedAt { get; set; }

        [JsonProperty("jobCreatedAt")]
        public DateTime JobCreatedAt { get; set; }

        [JsonProperty("jobLastUpdatedAt")]
        public DateTime? JobLastUpdatedAt { get; set; }

        [JsonProperty("hourlyRate")]
        public Money HourlyRate { get; set; } = default!;

        [JsonProperty("jobSite")]
        public JobSite JobSite { get; set; } = default!;

        [JsonProperty("requiredSkills")]
        public IReadOnlyList<Skill> RequiredSkills { get; set; } = [];

        [JsonProperty("isActive")]
        public bool IsActive { get; set; }

        public static JobFeedDocument From(JobFeed feed)
        {
            return new JobFeedDocument
            {
                Id = feed.JobId.Value.ToString(),
                Timestamp = feed.Timestamp,
                JobId = feed.JobId.Value.ToString(),
                OrganizationId = feed.OrganizationId.Value.ToString(),
                GeoHash = PartionKeyHashGenerator.CreateFrom(feed.JobSite.Location),
                LocationBucket = PartionKeyHashGenerator.CreatePartitionKey(feed.JobSite.Location),
                CreatedAt = feed.JobCreatedAt,
                JobCreatedAt = feed.JobCreatedAt,
                JobLastUpdatedAt = feed.JobLastUpdatedAt,
                HourlyRate = feed.HourlyRate,
                JobSite = feed.JobSite,
                RequiredSkills = feed.RequiredSkills,
                IsActive = feed.IsActive,
                Title = feed.Title
            };
        }
    }
}
