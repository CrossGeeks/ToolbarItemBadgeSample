using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Xamarin.Forms;
using Xamarin.Forms.Platform.UWP;
using Grid = Windows.UI.Xaml.Controls.Grid;
using Image = Windows.UI.Xaml.Controls.Image;

namespace ToolbarItemBadgeSample.UWP.Utils
{
    public static class AppBarButtonExtension
    {
        private const string BadgeValueOverflow = "*"; // text to display if the number is greater than 99

        public static void AddBadge(this AppBarButton appBarButton, ToolbarItem item, string value, Color backgroundColor, Color textColor)
        {
            if (item.IconImageSource == null) return; // if icon is not specified in toolbar item return

            var img = new Xamarin.Forms.Image { Source = item.IconImageSource }; // create new image with image source specified in toolbar item
            var imagePlatform = Platform.CreateRenderer(img) as ImageRenderer; // get platform renderer for image

            var grid = new Grid(); // create new UWP grid

            var image = new Image { Source = imagePlatform?.Control.Source, Stretch = Stretch.Fill, HorizontalAlignment = HorizontalAlignment.Stretch, VerticalAlignment = VerticalAlignment.Stretch}; // create new UWP Image, and set the source to toolbar item's image source
            Grid.SetColumn(image, 0); // get row for image control in grid
            Grid.SetRow(image, 0); // get column for image control in grid
            grid.Children.Add(image); // add image to grid

            if (!string.IsNullOrWhiteSpace(value) && value != "0") // check if value is set and value not equal to 0
            {
                var border = new Border // create new UWP border
                {
                    Margin = new Windows.UI.Xaml.Thickness(10, 0, 0, 10), // add margins
                    Background = new SolidColorBrush(backgroundColor.ToWindowsColor()), // set background color
                    CornerRadius = new Windows.UI.Xaml.CornerRadius(13), // make border as circle
                    Child = new TextBlock // create a UWP TextBlock
                    {
                        Text = value.Length > 2 ? BadgeValueOverflow : value, // set text, if value length is greater than 2, replace value with *
                        HorizontalAlignment = HorizontalAlignment.Center, // align control center horizontally
                        VerticalAlignment = VerticalAlignment.Center, // align control center Vertically
                        HorizontalTextAlignment = Windows.UI.Xaml.TextAlignment.Center, // text alignment to center
                        Foreground = new SolidColorBrush(textColor.ToWindowsColor()) // set fore color
                    }
                };
                Grid.SetColumn(border, 0); // get row for border control in grid
                Grid.SetRow(border, 0); // get column for border control in grid
                grid.Children.Add(border); // add image to grid
            }

            appBarButton.Content = grid; // replace UWP toolbar item content with newly created grid
        }
    }
}
