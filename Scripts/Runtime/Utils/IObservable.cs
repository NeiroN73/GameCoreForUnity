using System;
using R3;

namespace GameCore.Utils
{
    public interface IObservable : IObservable<Unit>
    {
        IDisposable Subscribe(IObserver observer);
        IDisposable Subscribe(Action action);
    }
}