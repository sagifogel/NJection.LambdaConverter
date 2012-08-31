using System;

namespace NJection.LambdaConverter.Synchronization
{
    public interface ILockerFinalizer : IDisposable, ILockIndicator
    {
        bool IsLockHeld { get; }
    }
}