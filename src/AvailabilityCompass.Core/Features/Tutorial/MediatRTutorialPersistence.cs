using System.Text.Json;
using AvailabilityCompass.Core.Features.ManageSettings.Commands.SaveSetting;
using AvailabilityCompass.Core.Features.ManageSettings.Queries.GetSetting;
using Guidely.Core.Abstractions;
using MediatR;

namespace AvailabilityCompass.Core.Features.Tutorial;

/// <summary>
/// Persistence implementation for tutorial state using MediatR and settings storage.
/// </summary>
// ReSharper disable once ClassNeverInstantiated.Global
public class MediatRTutorialPersistence : ITutorialPersistence
{
    private const string StateKey = "Tutorial_State";
    private readonly IMediator _mediator;

    public MediatRTutorialPersistence(IMediator mediator)
    {
        _mediator = mediator;
    }

    public async Task<TutorialState?> LoadStateAsync(CancellationToken ct = default)
    {
        var response = await _mediator.Send(new GetSettingQuery(StateKey), ct);

        if (string.IsNullOrEmpty(response.Value))
        {
            return null;
        }

        try
        {
            return JsonSerializer.Deserialize<TutorialState>(response.Value);
        }
        catch
        {
            return null;
        }
    }

    public async Task SaveStateAsync(TutorialState state, CancellationToken ct = default)
    {
        var json = JsonSerializer.Serialize(state);
        await _mediator.Send(new SaveSettingRequest(StateKey, json), ct);
    }
}