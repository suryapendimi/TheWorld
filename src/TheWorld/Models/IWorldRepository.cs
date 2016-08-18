using System.Collections.Generic;
using System.Threading.Tasks;

namespace TheWorld.Models
{
    public interface IWorldRepository
    {
        IEnumerable<Trip> GetAllTrips();
        IEnumerable<Trip> GetUserTripsWithStops(string name);

        void AddTrip(Trip trip);
        //Task<bool> SaveChangesAsync();
        bool SaveAll();

        Trip GetTripByName(string tripName);              
        void AddStop(string tripName, string username, Stop newStop);
        Trip GetTripByName(string tripName, string username);
        
      
    }
}