using System;
using System.Collections.Generic;

namespace VRCGalleryManager.Core
{
    public class MediaCacheService
    {
        private readonly VRCAuth _auth;

        public MediaCacheService(VRCAuth auth)
        {
            _auth = auth;
            _auth.OnAuthStateChanged += HandleAuthStateChanged;
        }

        private void HandleAuthStateChanged()
        {
            if (!_auth.LoggedIn)
            {
                ClearAll();
            }
        }

        public Dictionary<string, object> Caches { get; } = new();

        public void Set<T>(string key, T value)
        {
            Caches[key] = value;
        }

        public T Get<T>(string key)
        {
            if (Caches.TryGetValue(key, out var value) && value is T typedValue)
            {
                return typedValue;
            }
            return default;
        }

        public void ClearAll()
        {
            Caches.Clear();
        }
    }
}
