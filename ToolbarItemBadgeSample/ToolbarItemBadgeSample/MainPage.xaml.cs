using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ToolbarItemBadgeSample.Services;
using Xamarin.Forms;

namespace ToolbarItemBadgeSample
{
	public partial class MainPage : ContentPage
	{
		public MainPage()
		{
			InitializeComponent();
		}

        protected override void OnAppearing()
        {
            base.OnAppearing();
        }

        protected override void OnDisappearing()
        {
            base.OnAppearing();
        }

        private void Stepper_ValueChanged(object sender, ValueChangedEventArgs e)
        {
            if(ToolbarItems.Count > 0)
            {
                foreach(var t in ToolbarItems)
                {
                    DependencyService.Get<IToolbarItemBadgeService>().SetBadge(this, t, $"{e.NewValue}",Color.Red,Color.White);
                }
            }
              
            
        }
    }
}
