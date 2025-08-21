using System;
using R3;

namespace Content.Scripts.Utils
{
    public static class R3Extensions
    {
        public static void Execute(this ReactiveCommand<Unit> command)
        {
            command.Execute(Unit.Default);
        }
        
        public static IDisposable Subscribe(this ReactiveCommand<Unit> command, Action action)
        {
            return command.Subscribe(_ => action?.Invoke());
        }
    }
}