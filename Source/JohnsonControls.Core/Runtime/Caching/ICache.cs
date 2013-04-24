/*----------------------------------------------------------------------------

  (C) Copyright 2012 Johnson Controls, Inc.
      Use or Copying of all or any part of this program, except as
      permitted by License Agreement, is prohibited.

------------------------------------------------------------------------------*/
using System;

namespace JohnsonControls.Runtime.Caching
{
    public interface ICache
    {
        T RetrieveOrAdd<T>(string key, Func<T> value, DateTimeOffset expirationTime) where T : class;

        void AddOrUpdate<T>(string key, T value, DateTimeOffset expirationTime) where T : class;

        void Invalidate(string key);
    }
}
