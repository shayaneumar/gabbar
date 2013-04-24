/*----------------------------------------------------------------------------

  (C) Copyright 2012 Johnson Controls, Inc.
      Use or Copying of all or any part of this program, except as
      permitted by License Agreement, is prohibited.

------------------------------------------------------------------------------*/
using System;
using System.Linq;
using System.Runtime.Caching;

namespace JohnsonControls.Runtime.Caching
{
    public class Cache : ICache
    {
        private readonly ObjectCache _backingCache;
        private readonly string _nameSpace;
        private const char NamespaceDelimiter = '♣';
        public Cache(ObjectCache backingCache,string nameSpace)
        {
            _backingCache = backingCache;
            _nameSpace = nameSpace;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <remarks>keys may not contain the '♣' character</remarks>
        /// <returns></returns>
        public T RetrieveOrAdd<T>(string key, Func<T> value, DateTimeOffset expirationTime) where T : class
        {
            if (key.Contains(NamespaceDelimiter))//ensure collisions are not possible
            {
                throw new ArgumentException();
            }
            var fqKey = _nameSpace + NamespaceDelimiter + key;
            var cachedValue = _backingCache.Get(fqKey) as T;
            if(cachedValue == null)
            {
                var result = value();
                _backingCache.Add(fqKey,result,expirationTime);
                return result;
            }
            return cachedValue;
        }

        public void AddOrUpdate<T>(string key, T value, DateTimeOffset expirationTime) where T : class
        {
            if (key.Contains(NamespaceDelimiter))
            {
                throw new ArgumentException();
            }
            var fqKey = _nameSpace + NamespaceDelimiter + key;
            _backingCache.Set(fqKey, value, expirationTime);
        }

        public void Invalidate(string key)
        {
            if (key.Contains(NamespaceDelimiter))
            {
                throw new ArgumentException();
            }
            var fqKey = _nameSpace + NamespaceDelimiter + key;
            _backingCache.Remove(fqKey);
        }
    }
}
