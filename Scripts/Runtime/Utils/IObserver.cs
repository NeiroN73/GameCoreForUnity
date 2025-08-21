using System;
using R3;

namespace Game.Scripts.Utils
{
    public interface IObserver : IObserver<Unit>
    {
        void OnNext();
    }
}