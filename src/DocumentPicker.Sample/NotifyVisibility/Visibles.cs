using System;
using System.Collections.Generic;
using System.Text;

namespace DocumentPicker.Samples.NotifyVisibility
{
    public class Visibles : ISubject
    {

        private List<IObserver> _observers;

        private bool _isVisible;

        public bool IsVisible
        {
            get => _isVisible;
            set
            {
                _isVisible = value;
                Notify();
            }
        }
        public void Attach(IObserver observer)
        {
            if(_observers == null)
                _observers = new List<IObserver>();

            _observers.Add(observer);
        }

        public void Notify()
        {
            _observers.ForEach(o => o.Update(this));
        }
    }
}
