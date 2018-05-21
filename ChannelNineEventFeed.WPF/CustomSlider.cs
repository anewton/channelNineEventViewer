using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;

namespace ChannelNineEventFeed.WPF
{
    public class CustomSlider : Slider
    {
        public static readonly DependencyProperty ThumbToolTipPlacementProperty =
            DependencyProperty.Register("ThumbToolTipPlacement", typeof(PlacementMode), typeof(CustomSlider), new UIPropertyMetadata(PlacementMode.Top, ThumbToolTipPlacementChanged));

        private static void ThumbToolTipPlacementChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            CustomSlider slider = sender as CustomSlider;
            if (slider.Thumb != null)
            {
                slider.UpdateToolTip();
            }
        }

        public static readonly DependencyProperty ThumbToolTipTextProperty =
            DependencyProperty.Register("ThumbToolTipText", typeof(string), typeof(CustomSlider), new UIPropertyMetadata(null, ThumbToolTipPlacementChanged));

        public Thumb Thumb
        {
            get
            {
                return GetThumb(this) as Thumb;
            }
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            FrameworkElement left = GetTemplateChild("LeftTrack") as FrameworkElement;
            FrameworkElement right = GetTemplateChild("RightTrack") as FrameworkElement;
            if (left != null)
            {
                left.MouseLeftButtonDown += new MouseButtonEventHandler(OnMoveThumbToMouse);
            }

            if (right != null)
            {
                right.MouseLeftButtonDown += new MouseButtonEventHandler(OnMoveThumbToMouse);
            }
        }

        public PlacementMode ThumbToolTipPlacement
        {
            get { return (PlacementMode)GetValue(ThumbToolTipPlacementProperty); }
            set { SetValue(ThumbToolTipPlacementProperty, value); }
        }

        public string ThumbToolTipText
        {
            get { return (string)GetValue(ThumbToolTipTextProperty); }
            set { SetValue(ThumbToolTipTextProperty, value); }
        }

        public CustomSlider()
        {
            Loaded += new RoutedEventHandler(CustomSlider_Loaded);
        }

        private DependencyObject GetThumb(DependencyObject root)
        {
            if (root is Thumb)
            {
                return root;
            }

            DependencyObject thumb = null;
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(root); i++)
            {
                thumb = GetThumb(VisualTreeHelper.GetChild(root, i));
                if (thumb is Thumb)
                {
                    return thumb;
                }
            }
            return thumb;
        }

        private void CustomSlider_Loaded(object sender, RoutedEventArgs e)
        {
            UpdateToolTip();
        }

        private void UpdateToolTip()
        {
            if (Thumb.ToolTip == null)
            {
                Thumb.ToolTip = new ToolTip();
            }

            ToolTip tip = Thumb.ToolTip as ToolTip;
            if (tip != null)
            {
                tip.Content = ThumbToolTipText;
                ToolTipService.SetPlacement(Thumb.ToolTip as ToolTip, ThumbToolTipPlacement);
            }
        }

        private void OnMoveThumbToMouse(object sender, MouseButtonEventArgs e)
        {
            Point p = e.GetPosition(this);
            Value = (p.X - (Thumb.ActualWidth / 2)) / (ActualWidth - Thumb.ActualWidth) * Maximum;
        }
    }
}