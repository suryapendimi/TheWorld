using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace TheWorld.Models
{
    public class WorldRepository: IWorldRepository
    {
        private WorldContext _context;
        private ILogger<WorldRepository> _logger;

        public WorldRepository(WorldContext context,ILogger<WorldRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public IEnumerable<Trip> GetAllTrips()
        {
            _logger.LogInformation("Getting all Trips information from database");
            return _context.Trips.ToList();
        }

        public void  AddTrip(Trip trip)
        {
            _context.Add(trip);
        }        

        public async Task<bool> SaveChangesAsync()
        {
           return (await _context.SaveChangesAsync())  >0;
        }

        public Trip GetTripByName(string tripName)
        {
            return _context.Trips
                .Include(t => t.Stops)
                .Where(t => t.Name == tripName)
                .FirstOrDefault();
            
        }

        public void AddStop(string tripName,string username, Stop newStop)
        {
            var trip = GetTripByName(tripName);
            if(trip!=null)
            {
                trip.Stops.Add(newStop);//it's not enough for EF //Foreign key will be set
                _context.Stops.Add(newStop);
            }
        }

        public IEnumerable<Trip> GetUserTripsWithStops(string name)
        {
            try
            {
                return _context.Trips
                .Include(t => t.Stops)
                .OrderBy(t => t.Name)
                .Where(t => t.Name == name)
                .ToList();
            }
            catch(Exception ex)
            {
                _logger.LogError("Could not get trips with stops from database");
                    return null;

            }
        }

        public Trip GetTripByName(string tripName, string username)
        {
            return _context.Trips.Include(t => t.Stops)
                 .Where(t => t.Name == tripName && t.UserName == username)
                 .FirstOrDefault();
        }
    }
}
