using System;
using System.Collections.Concurrent;
using NJection.LambdaConverter.Synchronization;

namespace NJection.LambdaConverter.DynamicProxies
{
    internal static class DynamicProxyRepository
    {
        private static ConcurrentDictionary<Type, DynamicProxy> _cache = new ConcurrentDictionary<Type, DynamicProxy>();

        internal static DynamicProxy Get(Type type) {
            DynamicProxy proxy;

            using (var readLock = ReadWriteLocker.AcquireReadLock()) {
                _cache.TryGetValue(type, out proxy);
            }

            return proxy;
        }

        internal static DynamicProxy GetOrAdd(Type type) {
            DynamicProxy proxy = null;

            using (var readLock = ReadWriteLocker.AcquireUpgradeableReadLock()) {
                if (!_cache.TryGetValue(type, out proxy)) {
                    using (var writeLock = readLock.UpgradeToWriterLock()) {
                        proxy = type.CreateProxy();
                        _cache.GetOrAdd(type, proxy);
                    }
                }

                return proxy;
            }
        }

        internal static DynamicProxy Cache(Type type) {
            return _cache.GetOrAdd(type, DynamicProxy.Wrap(type));
        }
    }
}