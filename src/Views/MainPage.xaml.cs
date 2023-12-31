﻿// https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x804 上介绍了“空白页”项模板


namespace LiveCompass.Views;

/// <summary>
/// 可用于自身或导航至 Frame 内部的空白页。
/// </summary>
public sealed partial class MainPage : Page
{
    public MainViewModel ViewModel { get; } = new();

    public MainPage()
    {
        this.InitializeComponent();

        if (EnvironmentHelper.IsWindowsMobile)
        {
            TitleBarTextBlock.Visibility = Visibility.Collapsed;
        }
        ViewModel.CompassValueUpdated += OnCompassValueUpdated;
    }

    private void OnCompassValueUpdated()
    {
        double rawAngle = ViewModel.HeadingNorthAngle;
        AngleAnimation.To = rawAngle == 360 ? 0 : rawAngle;
        AngleStoryboard.Begin();
    }
}
