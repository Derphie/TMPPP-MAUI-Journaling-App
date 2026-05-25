using CommunityToolkit.Maui;
using Microcharts.Maui;
using Reflecta.Patterns.Behavioral;
using Reflecta.Patterns.Creational;
using Reflecta.Patterns.Structural;
using Reflecta.Repositories;
using Reflecta.Services;
using Reflecta.ViewModels;
using Reflecta.Views;

namespace Reflecta;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();

        builder
            .UseMauiApp<App>()
            .UseMauiCommunityToolkit()
            .UseMicrocharts()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf",  "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });

        var services = builder.Services;

        // ── Database path ─────────────────────────────────────────────────
        var dbPath = System.IO.Path.Combine(FileSystem.AppDataDirectory, "reflecta.db");

        // ── Repositories (singletons share the same DB file) ──────────────
        services.AddSingleton<IJournalRepository>(_ => new SQLiteJournalRepository(dbPath));
        services.AddSingleton<IChatRepository>(_ =>    new SQLiteChatRepository(dbPath));

        // ── AI service ────────────────────────────────────────────────────
        // MockAiService is always registered so HttpAiService can use it as a fallback.
        services.AddSingleton<MockAiService>();

        if (AppConfig.UseRemoteAi)
        {
            // 70 s timeout: Render free tier can take ~50 s to wake from idle
            services.AddSingleton<IAiService>(sp =>
            {
                var http = new HttpClient
                {
                    BaseAddress = new Uri(AppConfig.AiBaseUrl),
                    Timeout     = TimeSpan.FromSeconds(70)
                };
                return new HttpAiService(http, sp.GetRequiredService<MockAiService>());
            });
        }
        else
        {
            services.AddSingleton<IAiService>(sp => sp.GetRequiredService<MockAiService>());
        }

        // ── Pattern 3: Abstract Factory — platform-specific service family ─
        services.AddSingleton<IServiceAbstractFactory>(_ => ServiceAbstractFactoryResolver.Resolve());
        services.AddSingleton<INotificationService>(sp =>
            sp.GetRequiredService<IServiceAbstractFactory>().CreateNotificationService());
        services.AddSingleton<IShareService>(sp =>
            sp.GetRequiredService<IServiceAbstractFactory>().CreateShareService());

        // ── Export service ────────────────────────────────────────────────
        services.AddSingleton<IExportService, ExportService>();

        // ── Pattern 9: Observer — mood subject + concrete observers ───────
        services.AddSingleton<MoodSubject>();
        services.AddSingleton<SummaryChartObserver>(sp =>
        {
            var obs     = new SummaryChartObserver();
            var subject = sp.GetRequiredService<MoodSubject>();
            subject.Subscribe(obs);

            var alertObs = new MoodAlertObserver();
            alertObs.AlertTriggered += msg => MainThread.BeginInvokeOnMainThread(async () =>
                await Shell.Current.DisplayAlert("Reflecta 💜", msg, "Thanks"));
            subject.Subscribe(alertObs);

            return obs;
        });

        // ── Pattern 10: Strategy — AIWeightedStrategy routes through HttpAiService
        //    when UseRemoteAi is true; otherwise SimpleAnalysisStrategy runs offline.
        services.AddSingleton<IMoodAnalysisStrategy>(sp =>
            AppConfig.UseRemoteAi
                ? (IMoodAnalysisStrategy) new AIWeightedStrategy(sp.GetRequiredService<IAiService>())
                : new SimpleAnalysisStrategy());

        // ── Pattern 6: Facade ─────────────────────────────────────────────
        services.AddSingleton<ReflectaFacade>();

        // ── Seed data ─────────────────────────────────────────────────────
        services.AddSingleton<SeedDataService>();

        // ── ViewModels ────────────────────────────────────────────────────
        services.AddSingleton<OnboardingViewModel>();
        services.AddSingleton<ChatViewModel>();
        services.AddSingleton<JournalViewModel>();
        services.AddSingleton<SummaryViewModel>();
        services.AddSingleton<ProfileViewModel>();

        // ── Pages ─────────────────────────────────────────────────────────
        services.AddSingleton<OnboardingPage>();
        services.AddSingleton<ChatPage>();
        services.AddSingleton<JournalPage>();
        services.AddSingleton<SummaryPage>();
        services.AddSingleton<ProfilePage>();

        // ── App ───────────────────────────────────────────────────────────
        services.AddSingleton<App>();

        return builder.Build();
    }
}
