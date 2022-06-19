using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using ConferencePlanner.Api.DataLoader;
using HotChocolate;
using HotChocolate.Types;
using HotChocolate.Types.Relay;

namespace ConferencePlanner.Api.Speakers
{
    [ExtendObjectType(OperationTypeNames.Query)]
    public class SpeakerQueries
    {
        
        [UsePaging]
        public IQueryable<Speaker> GetSpeakers(
             ApplicationDbContext context) 
            => context.Speakers.OrderBy(t => t.Name);

        public Task<Speaker> GetSpeakerByIdAsync(
            [ID(nameof(Speaker))] int id,
            SpeakerByIdDataLoader dataLoader,
            CancellationToken cancellationToken) 
            => dataLoader.LoadAsync(id, cancellationToken);

        public async Task<IEnumerable<Speaker>> GetSpeakersByIdAsync(
            [ID(nameof(Speaker))] int[] ids,
            SpeakerByIdDataLoader dataLoader,
            CancellationToken cancellationToken) 
            => await dataLoader.LoadAsync(ids, cancellationToken);
    }
}
