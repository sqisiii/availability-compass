using AvailabilityCompass.Core.Features.ManageCalendars.Dialogs;

namespace AvailabilityCompass.Core.Features.ManageCalendars.Commands.DeleteRecurringDateRequest;

public record DeleteRecurringDateFromDbResponse(bool IsSuccess) : IProcessResult;