using Windows.Devices.Sensors;

namespace LiveCompass.ViewModels;

/// <summary>
/// <see cref="MainPage"/> 的视图模型
/// </summary>
public sealed partial class MainViewModel : ObservableObject
{
    /// <summary>
    /// 设备罗盘的实例。如果罗盘不可用，则此属性的值为 <see langword="null"/>
    /// </summary>
    public Compass CurrentCompass { get; } = Compass.GetDefault();

    [ObservableProperty]
    private double headingNorthAngel;
    [ObservableProperty]
    private GeographicDirection currentDirection;
    [ObservableProperty]
    private MagnetometerAccuracy currentCompassAccuracy;

    public MainViewModel()
    {
        CurrentCompass.ReadingChanged += OnCompassReadingChanged;
    }

    private async void OnCompassReadingChanged(Compass sender, CompassReadingChangedEventArgs args)
    {
        CompassReading reading = args.Reading;
        // 如果地北极的偏角值不可用，则使用磁北极角度
        double angle = reading.HeadingTrueNorth ?? reading.HeadingMagneticNorth;
        GeographicDirection direction = GetDirectionByAngle(angle);

        await UIThreadHelper.RunOnUIThread(() =>
        {
            HeadingNorthAngel = angle;
            CurrentDirection = direction;
        });
    }

    private static GeographicDirection GetDirectionByAngle(double angle)
    {
        GeographicDirection direction = angle switch
        {
            0 or 360 => GeographicDirection.North,
            90 => GeographicDirection.East,
            180 => GeographicDirection.South,
            270 => GeographicDirection.West,
            > 0 and < 90 => GeographicDirection.NorthEast,
            > 90 and < 180 => GeographicDirection.SouthEast,
            > 180 and < 270 => GeographicDirection.SouthWest,
            > 270 and < 360 => GeographicDirection.NorthWest,
            _ => throw new NotImplementedException($"What's wrong with the angle???\nCurrent angle value:{angle}")
        };

        return direction;
    }
}