using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using GreenDonut;
using api.Data;
using api.Data.Entities;

namespace api.DataLoader
{
    public class DivisionByIdDataLoader : BatchDataLoader<int, Division>
    {
        private readonly IDbContextFactory<AppDbContext> _dbContextFactory;

        public DivisionByIdDataLoader(
            IBatchScheduler batchScheduler, 
            IDbContextFactory<AppDbContext> dbContextFactory)
            : base(batchScheduler)
        {
            _dbContextFactory = dbContextFactory ?? 
                throw new ArgumentNullException(nameof(dbContextFactory));
        }

        protected override async Task<IReadOnlyDictionary<int, Division>> LoadBatchAsync(
            IReadOnlyList<int> keys, 
            CancellationToken cancellationToken)
        {
            await using AppDbContext dbContext = 
                _dbContextFactory.CreateDbContext();
            
            return await dbContext.Divisions
                .Where(s => keys.Contains(s.Id))
                .ToDictionaryAsync(t => t.Id, cancellationToken);
        }
    }
}
