using System;
using R3;

namespace GameCore.Utils
{
    public interface IObserver : IObserver<Unit>
    {
        void OnNext();
    }
}