using System;
using System.Collections.Generic;
using System.Text;

namespace DocumentPicker.Samples.NotifyVisibility
{
    public interface ISubject
    {
        void Attach(IObserver observer);

        void Notify();
    }
}
