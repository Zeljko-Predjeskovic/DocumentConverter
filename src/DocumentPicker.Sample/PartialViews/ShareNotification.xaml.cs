using DocumentPicker.Samples.NotifyVisibility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace DocumentPicker.Samples.PartialViews
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ShareNotification : ContentView, IObserver
    {


        public bool IsVisible
        {
            get => Layout.IsVisible;
            set
            {
                Layout.IsVisible = value;
                OnPropertyChanged("Layout");
            }
        }

        public ShareNotification()
        {
            InitializeComponent();
            SharedVisibles.Visibles.Attach(this);
        }

        public void Update(ISubject subject)
        { 
            IsVisible = SharedVisibles.Visibles.IsVisible;
        }
    }
}