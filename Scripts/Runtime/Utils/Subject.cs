using System;
using R3;

namespace Game.Scripts.Utils
{
    public class Subject : ISubject, IDisposable
    {
        private readonly CompositeDisposable disposable = new();

        private readonly Subject<object> subject = new();

        public void OnNext()
        {
            subject.OnNext(null);
        }

        public IDisposable Subscribe(IObserver observer)
        {
            return subject
                .Subscribe(_ => observer.OnNext())
                .AddTo(disposable);
        }

        public IDisposable Subscribe(Action action)
        {
            return subject
                .Subscribe(_ => action.Invoke())
                .AddTo(disposable);
        }

        public IDisposable Subscribe(IObserver<object> observer)
        {
            return subject
                .Subscribe(_ => observer.OnNext(Unit.Default))
                .AddTo(disposable);
        }

        public void OnCompleted()
        {
            
        }

        public void OnError(Exception error)
        {
        }

        public void OnNext(Unit value)
        {
        }

        public IDisposable Subscribe(IObserver<Unit> observer)
        {
            return subject
                .Subscribe(_ => observer.OnNext(Unit.Default))
                .AddTo(disposable);
        }
        
        public void Dispose()
        {
            disposable.Dispose();
            subject.Dispose();
        }
    }
}