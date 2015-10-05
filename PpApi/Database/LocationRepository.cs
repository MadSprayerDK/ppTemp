using System;
using System.Collections.Generic;
using System.Linq;
using PpApi.Models;

namespace PpApi.Database
{
    public class LocationRepository : BaseRepository<Location, PpDbContext>
    {
        public LocationRepository(PpDbContext dbContext)
            : base(x => x.Locations, dbContext)
        { }

        public Location CreateLocation(Location location)
        {
            return CreateDbObj(location);
        }

        public IEnumerable<Location> GetLocationss(Func<Location, bool> predicate)
        {
            return GetDbObjs(predicate);
        }

        public IQueryable<Location> GetLocationQueryable()
        {
            return GetDbObjQueryable();
        }

        public void UpdateLocation()
        {
            UpdateDbObjects();
        }

        public void DeleteLocation(Location location)
        {
            DeleteDbObj(location);
        }
    }
}