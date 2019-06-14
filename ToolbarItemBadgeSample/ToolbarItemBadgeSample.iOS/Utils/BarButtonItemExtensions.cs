using CoreAnimation;
using CoreGraphics;
using Foundation;
using ObjCRuntime;
using System;
using System.Linq;
using System.Runtime.InteropServices;
using UIKit;

namespace ToolbarItemBadgeSample.iOS.Utils
{
    public static class BarButtonItemExtensions
    {
        enum AssociationPolicy
        {
            ASSIGN = 0,
            RETAIN_NONATOMIC = 1,
            COPY_NONATOMIC = 3,
            RETAIN = 01401,
            COPY = 01403,
        }

        static NSString BadgeKey = new NSString(@"BadgeKey");

        [DllImport(Constants.ObjectiveCLibrary)]
        static extern void objc_setAssociatedObject(IntPtr obj, IntPtr key, IntPtr value, AssociationPolicy policy);


        [DllImport(Constants.ObjectiveCLibrary)]
        static extern IntPtr objc_getAssociatedObject(IntPtr obj, IntPtr key);

        static CAShapeLayer GetBadgeLayer(UIBarButtonItem barButtonItem)
        {
            var handle = objc_getAssociatedObject(barButtonItem.Handle, BadgeKey.Handle);

            if (handle != IntPtr.Zero)
            {
                var value = ObjCRuntime.Runtime.GetNSObject(handle);
                if (value != null)
                    return value as CAShapeLayer;
                else
                    return null;
            }
            return null;
        }

        static void DrawRoundedRect(CAShapeLayer layer, CGRect rect, float radius, UIColor color, bool filled)
        {
            layer.FillColor = filled ? color.CGColor : UIColor.White.CGColor;
            layer.StrokeColor = color.CGColor;
            layer.Path = UIBezierPath.FromRoundedRect(rect, radius).CGPath;
        }

        public static void AddBadge(this UIBarButtonItem barButtonItem, string text, UIColor backgroundColor, UIColor textColor, bool filled = true, float fontSize = 11.0f)
        {

            if (string.IsNullOrEmpty(text))
            {
                return;
            }

           CGPoint offset = CGPoint.Empty;

            if (backgroundColor == null)
                backgroundColor = UIColor.Red;

            var font = UIFont.SystemFontOfSize(fontSize);

            if (UIDevice.CurrentDevice.CheckSystemVersion(9, 0))
            {
                font = UIFont.MonospacedDigitSystemFontOfSize(fontSize, UIFontWeight.Regular);
            }

            var view = barButtonItem.ValueForKey(new NSString(@"view")) as UIView;
            var bLayer = GetBadgeLayer(barButtonItem);
            bLayer?.RemoveFromSuperLayer();


            var badgeSize = text.StringSize(font);


            var height = badgeSize.Height;
            var width = badgeSize.Width + 2; /* padding */

            //make sure we have at least a circle
            if (width < height)
            {
                width = height;
            }

            //x position is offset from right-hand side
            var x = view.Frame.Width - width + offset.X;


            var badgeFrame = new CGRect(new CGPoint(x: x, y: offset.Y), size: new CGSize(width: width, height: height));

            bLayer = new CAShapeLayer();
            DrawRoundedRect(bLayer, badgeFrame, 7.0f, backgroundColor, filled);
            view.Layer.AddSublayer(bLayer);

            // Initialiaze Badge's label
            var label = new CATextLayer();
            label.String = text;
            label.AlignmentMode = CATextLayer.AlignmentCenter;
            label.SetFont(CGFont.CreateWithFontName(font.Name));
            label.FontSize = font.PointSize;
            label.Frame = badgeFrame;
            label.ForegroundColor = filled ? textColor.CGColor : UIColor.White.CGColor;
            label.BackgroundColor = UIColor.Clear.CGColor;
            label.ContentsScale = UIScreen.MainScreen.Scale;
            bLayer.AddSublayer(label);

            // Save Badge as UIBarButtonItem property
            objc_setAssociatedObject(barButtonItem.Handle, BadgeKey.Handle, bLayer.Handle, AssociationPolicy.RETAIN_NONATOMIC);

        }
        public static void UpdateBadge(this UIBarButtonItem barButtonItem, string text, UIColor backgroundColor, UIColor textColor)
        {
            var bLayer = GetBadgeLayer(barButtonItem);

            if (bLayer != null)
            {
                bLayer.Hidden = string.IsNullOrEmpty(text) || text == "0";
                if (bLayer.Hidden)
                {
                    return;
                }
            }

            var textLayer = bLayer?.Sublayers?.First(p => p is CATextLayer) as CATextLayer;
            if (textLayer != null && textLayer.String != "0")
            {
                    textLayer.String = text;
            }
            else
            {
                barButtonItem.AddBadge(text, backgroundColor, textColor);
            }
        }

    }
}
