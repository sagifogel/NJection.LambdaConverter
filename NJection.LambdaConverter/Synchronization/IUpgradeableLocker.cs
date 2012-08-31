using System;

namespace NJection.LambdaConverter.Synchronization
{
    public interface IUpgradeableLocker : ILockIndicator, IDisposable
    {
        IDowngradeableLocker UpgradeToWriterLock();
    }
}