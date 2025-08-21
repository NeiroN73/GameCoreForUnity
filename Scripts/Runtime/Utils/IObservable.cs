using System;
using R3;

namespace Game.Scripts.Utils
{
    public interface IObservable : IObservable<Unit>
    {
        IDisposable Subscribe(IObserver observer);
        IDisposable Subscribe(Action action);
    }
}