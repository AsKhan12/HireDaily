using System.Globalization;
using Hiredaily.BuildingBlock.Application.Mediator.Pipeline.ValidationBehaviour;

namespace Hiredaily.Modules.Feed.Application.Read;

public class JobFeedRequestValidator : IValidator<JobFeedRequest>
{
    public Task<ValidationResult> ValidateAsync(JobFeedRequest request, CancellationToken cancellationToken)
    {
        var errors = new List<ValidationError>();
        if (request.Location is null && request.Skills is null)
        {
            errors.Add(
                new ValidationError
                {
                    ErrorMessage = "Either Location or Skills must be provided",
                    PropertyName=$"{nameof(JobFeedRequest.Location)}, {nameof(JobFeedRequest.Skills)}"
                });
        }
        if(request.Location is not null && !ValidLocation(request.Location.Lat, request.Location.Long))
        {
            errors.Add(
                new ValidationError
                {
                    ErrorMessage = "Invalid Location",
                    PropertyName = nameof(request.Location)
                });
        }
        return errors.Count == 0 
                ? Task.FromResult(ValidationResult.Valid()) 
                : Task.FromResult(ValidationResult.InValid(errors.AsReadOnly()));
    }

    private static bool ValidLocation(string lat, string longitude)
    {
        if (!TryParseCoordinate(lat, -90, 90, out _))            return false;

        if (!TryParseCoordinate(longitude, -180, 180, out _))
            return false;

        return true;
    }

    private static bool TryParseCoordinate(string value, double minimum, double maximum, out double coordinate)
    {
        return double.TryParse(value, NumberStyles.Float, CultureInfo.InvariantCulture, out coordinate)
            && coordinate >= minimum
            && coordinate <= maximum;
    }
}