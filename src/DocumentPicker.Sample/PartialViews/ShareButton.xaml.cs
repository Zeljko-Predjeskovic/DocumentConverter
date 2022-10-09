using DocumentPicker.Samples.NotifyVisibility;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace DocumentPicker.Samples.PartialViews
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ShareButton : ContentView, IObserver
    {


        public Func<bool> OnCLickedShare { get; set; }

        public bool IsVisible
        {
            get => shareButton.IsVisible;
            set
            {
                shareButton.IsVisible = value;
                OnPropertyChanged("shareButton");
            }
        }

        public ShareButton()
        {
            InitializeComponent();
            SharedVisibles.Visibles.Attach(this);
            shareButton.Clicked += (sender, args) => { OnCLickedShare.Invoke(); };
        }

        public void Update(ISubject subject)
        { 
            IsVisible = SharedVisibles.Visibles.IsVisible;
        }
    }
}