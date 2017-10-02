using System;

namespace RxWorkshop
{
    public class Monitor
    {
        public event EventHandler<UserLocation> LocationUpdated;

        public void RaiseEvent(UserLocation userLocation)
        {
            LocationUpdated?.Invoke(this, userLocation);
        }
    }
}
