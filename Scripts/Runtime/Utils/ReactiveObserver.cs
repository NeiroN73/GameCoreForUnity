using R3;
using System;

namespace GameCore.ReactiveObservers
{
    public abstract class ReactiveObserverBase : IDisposable
    {
        protected readonly CompositeDisposable _compositeDisposable = new();

        public void Unsubscribe(IDisposable disposable)
        {
            disposable?.Dispose();
        }

        public virtual void Dispose()
        {
            _compositeDisposable.Dispose();
        }

        protected IDisposable AddToDisposable(IDisposable disposable)
        {
            return disposable.AddTo(_compositeDisposable);
        }
    }

    public interface IReadOnlyReactiveObserver
    {
        IDisposable Subscribe(Action callback);
    }

    public interface IReadOnlyReactiveObserver<T>
    {
        IDisposable Subscribe(Action<T> callback);
        T Value { get; }
    }

    public interface IReadOnlyReactiveObserver<T1, T2>
    {
        IDisposable Subscribe(Action<T1, T2> callback);
        T1 Value1 { get; }
        T2 Value2 { get; }
    }

    public interface IReadOnlyReactiveObserver<T1, T2, T3>
    {
        IDisposable Subscribe(Action<T1, T2, T3> callback);
        T1 Value1 { get; }
        T2 Value2 { get; }
        T3 Value3 { get; }
    }
    
    public class ReactiveObserver : ReactiveObserverBase, IReadOnlyReactiveObserver
    {
        private readonly Subject<Unit> _subject = new();

        public void Execute()
        {
            _subject.OnNext(Unit.Default);
        }

        public IDisposable Subscribe(Action callback)
        {
            return AddToDisposable(_subject.Subscribe(_ => callback?.Invoke()));
        }

        public override void Dispose()
        {
            _subject.Dispose();
            base.Dispose();
        }
    }

    public class ReactiveObserver<T> : ReactiveObserverBase, IReadOnlyReactiveObserver<T>
    {
        private readonly Subject<T> _subject = new();
        private T _value;

        public void Execute(T value)
        {
            _value = value;
            _subject.OnNext(value);
        }

        public IDisposable Subscribe(Action<T> callback)
        {
            return AddToDisposable(_subject.Subscribe(callback));
        }

        public T Value 
        { 
            get => _value; 
            set => Execute(value);
        }

        public override void Dispose()
        {
            _subject.Dispose();
            base.Dispose();
        }
    }

    public class ReactiveObserver<T1, T2> : ReactiveObserverBase, IReadOnlyReactiveObserver<T1, T2>
    {
        private readonly Subject<(T1, T2)> _subject = new();
        private T1 _value1;
        private T2 _value2;

        public void Execute(T1 value1, T2 value2)
        {
            _value1 = value1;
            _value2 = value2;
            _subject.OnNext((value1, value2));
        }

        public IDisposable Subscribe(Action<T1, T2> callback)
        {
            return AddToDisposable(_subject.Subscribe(tuple => callback(tuple.Item1, tuple.Item2)));
        }

        public T1 Value1 
        { 
            get => _value1; 
            set => Execute(value, _value2);
        }

        public T2 Value2 
        { 
            get => _value2; 
            set => Execute(_value1, value);
        }

        public override void Dispose()
        {
            _subject.Dispose();
            base.Dispose();
        }
    }

    public class ReactiveObserver<T1, T2, T3> : ReactiveObserverBase, IReadOnlyReactiveObserver<T1, T2, T3>
    {
        private readonly Subject<(T1, T2, T3)> _subject = new();
        private T1 _value1;
        private T2 _value2;
        private T3 _value3;

        public void Execute(T1 value1, T2 value2, T3 value3)
        {
            _value1 = value1;
            _value2 = value2;
            _value3 = value3;
            _subject.OnNext((value1, value2, value3));
        }

        public IDisposable Subscribe(Action<T1, T2, T3> callback)
        {
            return AddToDisposable(_subject.Subscribe(tuple => callback(tuple.Item1, tuple.Item2, tuple.Item3)));
        }

        public T1 Value1 
        { 
            get => _value1; 
            set => Execute(value, _value2, _value3);
        }

        public T2 Value2 
        { 
            get => _value2; 
            set => Execute(_value1, value, _value3);
        }

        public T3 Value3 
        { 
            get => _value3; 
            set => Execute(_value1, _value2, value);
        }

        public override void Dispose()
        {
            _subject.Dispose();
            base.Dispose();
        }
    }
}