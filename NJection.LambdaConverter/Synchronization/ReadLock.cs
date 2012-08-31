using System;
using System.Threading;

namespace NJection.LambdaConverter.Synchronization
{
    public class ReadLock : AbstractLocker, IDisposable
    {
        public ReadLock(ReaderWriterLockSlim locker)
            : base(locker) {
            if (!IsLockHeld) {
                while (!LockAcquired) {
                    LockAcquired = Locker.TryEnterReadLock(0);
                }
            }
        }
    }
}