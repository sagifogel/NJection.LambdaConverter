using System.Threading;

namespace NJection.LambdaConverter.Synchronization
{
    public static class ReadWriteLocker
    {
        private static readonly ReaderWriterLockSlim _locker = new ReaderWriterLockSlim(LockRecursionPolicy.NoRecursion);

        public static bool LockAcquired {
            get {
                return _locker.IsReadLockHeld ||
                       _locker.IsUpgradeableReadLockHeld ||
                       _locker.IsWriteLockHeld;
            }
        }

        public static ILockerFinalizer AcquireReadLock() {
            return new ReadLock(_locker);
        }

        public static IDowngradeableLocker AcquireWriterLock() {
            return new DowngradeableLocker(_locker);
        }

        public static IUpgradeableLocker AcquireUpgradeableReadLock() {
            return new UpgradeableLocker(_locker);
        }
    }
}