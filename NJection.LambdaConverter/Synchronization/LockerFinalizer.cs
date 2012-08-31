using System.Threading;

namespace NJection.LambdaConverter.Synchronization
{
    public abstract class AbstractLocker : ILockerFinalizer
    {
        protected ReaderWriterLockSlim Locker = null;

        public AbstractLocker(ReaderWriterLockSlim locker) {
            Locker = locker;
        }

        public void Dispose() {
            if (LockAcquired) {
                if (Locker.IsWriteLockHeld) {
                    Locker.ExitWriteLock();
                }

                if (Locker.IsUpgradeableReadLockHeld) {
                    Locker.ExitUpgradeableReadLock();
                }

                if (Locker.IsReadLockHeld) {
                    Locker.ExitReadLock();
                }

                LockAcquired = false;
            }
        }

        public virtual bool LockAcquired { get; protected set; }

        public bool IsLockHeld {
            get {
                return Locker.IsReadLockHeld ||
                       Locker.IsUpgradeableReadLockHeld ||
                       Locker.IsWriteLockHeld;
            }
        }
    }
}