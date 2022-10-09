using System;
using System.Collections.Generic;
using System.Text;

namespace DocumentPicker.Samples.NotifyVisibility
{
    public interface IObserver
    {
        void Update(ISubject subject);
         }
}
