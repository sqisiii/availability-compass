using MediatR;

namespace AvailabilityCompass.Core.Features.ManageCalendars.Commands.AddCalendarRequest;

/// <summary>
/// MediatR request to add a new calendar to the database.
/// </summary>
public class AddCalendarToDbRequest : IRequest<AddCalendarToDbResponse>
{
    public AddCalendarToDbRequest(string name, bool isOnly)
    {
        Name = name;
        IsOnly = isOnly;
    }

    public string Name { get; }

    public bool IsOnly { get; }
}