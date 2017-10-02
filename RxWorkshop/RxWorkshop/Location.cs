using System;

namespace RxWorkshop
{
    public class UserLocation : IEquatable<UserLocation>
    {
        public UserLocation(int id, double latitude, double longtitude)
        {
            Id = id;
            Latitude = latitude;
            Longtitude = longtitude;
        }

        public int Id { get; private set; }
        public double Latitude { get; private set; }
        public double Longtitude { get; private set; }

        public bool Equals(UserLocation other)
        {
            if (other == null)
            {
                return false;
            }

            return Id == other.Id &&
                   Latitude.Equals(other.Latitude) &&
                   Longtitude.Equals(other.Longtitude);
        }

        public override string ToString()
        {
            return $"Id: {Id}, Latitude: {Latitude}, Longtitude: {Longtitude}";
        }
    }
}
