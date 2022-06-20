using System.Collections.Generic;

using HotChocolate.Types.Relay;

namespace ConferencePlanner.Api.Sessions
{
    public record AddSessionInput(
        string Title,
        string? Abstract,
        [property: ID(nameof(Speaker))]
        IReadOnlyList<int> SpeakerIds);
}