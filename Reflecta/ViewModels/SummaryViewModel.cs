using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microcharts;
using Reflecta.Models;
using Reflecta.Patterns.Behavioral;
using Reflecta.Patterns.Structural;
using Reflecta.Services;
using SkiaSharp;

namespace Reflecta.ViewModels;

public partial class SummaryViewModel : BaseViewModel
{
    private readonly ReflectaFacade       _facade;
    private readonly SummaryChartObserver _chartObserver;
    private readonly IShareService        _share;

    [ObservableProperty] private string _insightText   = "Loading your weekly insight...";
    [ObservableProperty] private Chart? _moodChart;
    [ObservableProperty] private int    _periodIndex   = 1; // 0=Daily, 1=Weekly, 2=Monthly

    public ObservableCollection<ThemeItem> ThemeItems { get; } = new();

    public bool HasNoThemes => ThemeItems.Count == 0;

    public SummaryViewModel(ReflectaFacade facade, SummaryChartObserver chartObserver, IShareService share)
    {
        _facade        = facade;
        _chartObserver = chartObserver;
        _share         = share;
        Title          = "Summary";

        ThemeItems.CollectionChanged += (_, _) => OnPropertyChanged(nameof(HasNoThemes));
        _chartObserver.ChartDataChanged += async () =>
            await MainThread.InvokeOnMainThreadAsync(LoadAsync);
    }

    public async Task InitializeAsync() => await LoadAsync();

    [RelayCommand]
    private async Task LoadAsync()
    {
        await RunSafeAsync(async () =>
        {
            WeeklySummary summary = PeriodIndex == 0
                ? await _facade.GetDailySummaryAsync()
                : await _facade.GetWeeklySummaryAsync();

            InsightText = summary.InsightText;
            BuildChart(summary.MoodTrend);

            ThemeItems.Clear();
            foreach (var kv in summary.ThemeCounts.OrderByDescending(x => x.Value))
                ThemeItems.Add(new ThemeItem { Tag = kv.Key, Count = kv.Value });
        });
    }

    partial void OnPeriodIndexChanged(int value) => _ = LoadAsync();

    [RelayCommand]
    private async Task ExportAsync()
    {
        var cmd = new ExportCommand(_facade);
        await cmd.ExecuteAsync();
        await _share.ShareTextAsync("My Reflecta Journal", cmd.ExportedText);
    }

    private void BuildChart(List<DayMoodPoint> trend)
    {
        var purple = SKColor.Parse("#A855F7");
        var white  = SKColor.Parse("#F5F5F7");

        var entries = trend.Select(p => new ChartEntry(p.Score)
        {
            Label           = p.Day,
            ValueLabel      = p.Score.ToString("F0"),
            Color           = purple,
            TextColor       = white,
            ValueLabelColor = white,
        }).ToList();

        MoodChart = new LineChart
        {
            Entries            = entries,
            LineMode           = LineMode.Spline,
            LineSize           = 4,
            PointMode          = PointMode.Circle,
            PointSize          = 8,
            BackgroundColor    = SKColor.Parse("#1A1825"),
            LabelColor         = white,
            LabelTextSize      = 28,
            ValueLabelTextSize = 26,
            AnimationDuration  = TimeSpan.FromMilliseconds(500),
            MinValue           = 0,
            MaxValue           = 10,
        };
    }
}
