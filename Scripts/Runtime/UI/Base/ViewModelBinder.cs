using System;
using R3;

namespace Content.Scripts.UI
{
    public class ViewModelBinder<TValue> : ViewModelBinder
    {
        protected TValue _value;
        protected ReactiveProperty<TValue> _binderTriggered = new();
        private CompositeDisposable _disposable = new();
        
        public ViewModelBinder(string id) : base(id)
        {
        }

        public TValue Value
        {
            get => _value;
            set
            {
                _value = value;
                _binderTriggered.Value = value;
            }
        }
        
        public void SubscribeParse(Action<TValue> action)
        {
            _binderTriggered.Subscribe(value => action?.Invoke(value)).AddTo(_disposable);
        }

        public void DisposeParse()
        {
            _disposable.Dispose();
        }
    }
    
    public class RefTypeViewModelBinder<TValue> : ViewModelBinder<TValue> where TValue : class, new()
    {
        public RefTypeViewModelBinder(string id) : base(id)
        {
            _value = new ();
            _binderTriggered.Value = _value;
        }
    }

    public class ViewModelBinder
    {
        public string Id;
        public ViewModelBinder(string id)
        {
            Id = id;
        }
    }
}