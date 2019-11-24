using System;
using System.Globalization;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Simple.Wpf.Composition.Workspaces.Dilbert
{
    public sealed class DilbertFilePathConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                if (value == null) return null;

                var src = new BitmapImage();
                var dilbertImage = new Image();

                src.BeginInit();
                src.UriSource = new Uri(value.ToString(), UriKind.Absolute);
                src.EndInit();
                dilbertImage.Source = src;
                dilbertImage.Stretch = Stretch.Uniform;

                return dilbertImage;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Binding.DoNothing;
        }
    }
}