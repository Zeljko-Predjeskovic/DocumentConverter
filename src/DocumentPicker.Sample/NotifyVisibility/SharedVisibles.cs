using System;
using System.Collections.Generic;
using System.Text;

namespace DocumentPicker.Samples.NotifyVisibility
{
    public static class SharedVisibles
    {
        private static Visibles _visibles;
        public static Visibles Visibles
        {
            get => _visibles ?? (_visibles = new Visibles());
        }

        public static void Init()
        {
            Visibles.IsVisible = false;
        }

        public static void ShowShareViews()
        {
            Visibles.IsVisible = true;
        }
        public static void HideShareViews()
        {
            Visibles.IsVisible = false;
        }
    }
}
