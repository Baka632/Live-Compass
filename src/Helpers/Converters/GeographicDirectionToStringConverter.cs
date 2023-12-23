namespace LiveCompass.Helpers.Converters;

public sealed class GeographicDirectionToStringConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        if (value is GeographicDirection direction)
        {
            return direction switch
            {
                GeographicDirection.North => ReswHelper.GetReswString("North"),
                GeographicDirection.South => ReswHelper.GetReswString("South"),
                GeographicDirection.West => ReswHelper.GetReswString("West"),
                GeographicDirection.East => ReswHelper.GetReswString("East"),
                GeographicDirection.NorthWest => ReswHelper.GetReswString("NorthWest"),
                GeographicDirection.NorthEast => ReswHelper.GetReswString("NorthEast"),
                GeographicDirection.SouthWest => ReswHelper.GetReswString("SouthWest"),
                GeographicDirection.SouthEast => ReswHelper.GetReswString("SouthEast"),
                _ => DependencyProperty.UnsetValue
            };
        }
        else
        {
            return DependencyProperty.UnsetValue;
        }
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
        throw new NotImplementedException();
    }
}
