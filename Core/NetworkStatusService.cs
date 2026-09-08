using System;
using System.Threading.Tasks;
using Microsoft.Maui.Networking;

namespace VRCGalleryManager.Core
{
    public class NetworkStatusService : IDisposable
    {
        private bool _isOnline;
        public bool IsOnline => _isOnline;

        public event Action<bool>? OnNetworkStatusChanged;

        public NetworkStatusService()
        {
            _isOnline = Connectivity.Current.NetworkAccess == NetworkAccess.Internet;
            Connectivity.Current.ConnectivityChanged += HandleConnectivityChanged;
        }

        private void HandleConnectivityChanged(object? sender, ConnectivityChangedEventArgs e)
        {
            bool newStatus = e.NetworkAccess == NetworkAccess.Internet;
            if (_isOnline != newStatus)
            {
                _isOnline = newStatus;
                OnNetworkStatusChanged?.Invoke(_isOnline);
            }
        }

        public async Task<bool> CheckConnectionAsync()
        {
            bool status = Connectivity.Current.NetworkAccess == NetworkAccess.Internet;
            if (_isOnline != status)
            {
                _isOnline = status;
                OnNetworkStatusChanged?.Invoke(_isOnline);
            }
            return status;
        }

        public void Dispose()
        {
            Connectivity.Current.ConnectivityChanged -= HandleConnectivityChanged;
        }
    }
}
