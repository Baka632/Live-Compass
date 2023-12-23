using Windows.Devices.Sensors;

namespace LiveCompass.ViewModels;

/// <summary>
/// <see cref="MainPage"/> 的视图模型
/// </summary>
public sealed partial class MainViewModel : ObservableObject
{
    public event Action CompassValueUpdated;

    /// <summary>
    /// 设备罗盘的实例。如果罗盘不可用，则此属性的值为 <see langword="null"/>
    /// </summary>
    public Compass CurrentCompass { get; } = Compass.GetDefault();
    public string HeadingNorthAngleString => HeadingNorthAngle.ToString("f0");

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HeadingNorthAngleString))]
    private double headingNorthAngle;
    [ObservableProperty]
    private GeographicDirection currentDirection;
    [ObservableProperty]
    private MagnetometerAccuracy currentCompassAccuracy;

    public MainViewModel()
    {
        if (CurrentCompass is not null)
        {
            uint minInterval = 700;
            CurrentCompass.ReportInterval = minInterval > CurrentCompass.MinimumReportInterval
                ? minInterval
                : CurrentCompass.MinimumReportInterval + minInterval;
            CurrentCompass.ReadingChanged += OnCompassReadingChanged;
        }
    }

    private async void OnCompassReadingChanged(Compass sender, CompassReadingChangedEventArgs args)
    {
        CompassReading reading = args.Reading;
        // 如果地北极的偏角值不可用，则使用磁北极角度
        double angle = reading.HeadingTrueNorth ?? reading.HeadingMagneticNorth;
        GeographicDirection direction = GetDirectionByAngle(angle);

        await UIThreadHelper.RunOnUIThread(() =>
        {
            HeadingNorthAngle = angle;
            CurrentDirection = direction;
            CurrentCompassAccuracy = reading.HeadingAccuracy;
            CompassValueUpdated?.Invoke();
        });
    }

    private static GeographicDirection GetDirectionByAngle(double angle)
    {
        GeographicDirection direction = angle switch
        {
            // 在正东/正南/正西/正北的角度，容许 20 度的误差
            (>= 0d and <= 20d) or (>= 340d and <= 360d) => GeographicDirection.North, // 精确值：0 或 360
            >= 70d and <= 110d => GeographicDirection.East, // 精确值：90
            >= 160d and <= 200d => GeographicDirection.South, // 精确值：180
            >= 250d and <= 290d => GeographicDirection.West, // 精确值：270
            > 20d and < 70d => GeographicDirection.NorthEast,
            > 110d and < 160d => GeographicDirection.SouthEast,
            > 200d and < 250d => GeographicDirection.SouthWest,
            > 290d and < 340d => GeographicDirection.NorthWest,
            _ => throw new NotImplementedException($"What's wrong with the angle???\nCurrent angle value:{angle}")
        };

        return direction;
    }
}