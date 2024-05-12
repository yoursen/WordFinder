using CommunityToolkit.Maui;
using Microsoft.Extensions.Logging;
using WordFinder.Interfaces;
using WordFinder.Models;
using WordFinder.Services;
using WordFinder.ViewModels;
using WordFinder.Views;

namespace WordFinder;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		var builder = MauiApp.CreateBuilder();

		// app
		builder.UseMauiApp<App>().ConfigureMauiHandlers((handlers) =>
		{
#if IOS
			handlers.AddHandler(typeof(Shell), typeof(CustomShellRenderer));
#endif
		});

		builder.UseMauiCommunityToolkit();
		builder.ConfigureFonts(fonts =>
		{
			fonts.AddFont("Roboto-Regular.ttf", "MainFont");
			fonts.AddFont("Roboto-Medium.ttf", "MainFontMedium");
			fonts.AddFont("FontAwesome.oft", "FontAwesome");
		});

		// models
		builder.Services.AddSingleton<GameModel>();
		builder.Services.AddSingleton<GameHintsModel>();
		builder.Services.AddSingleton<WordFitter>();
		builder.Services.AddSingleton<TableService>();
		builder.Services.AddSingleton<GameDatabase>();
		builder.Services.AddSingleton<GameTimer>();
		builder.Services.AddSingleton<GameSettings>();

		// pages
		builder.Services.AddSingleton<MainPage>();
		builder.Services.AddSingleton<GamePage>();
		builder.Services.AddSingleton<GameOverPage>();
		builder.Services.AddSingleton<GameBestScorePage>();
		builder.Services.AddSingleton<GameSettingsPage>();

		// services
		builder.Services.AddSingleton<AwaitableMessageService>();
		builder.Services.AddSingleton<TouchFeedbackService>();
		builder.Services.AddSingleton<LicenseService>();
		builder.Services.AddSingleton<ISound, Sound>();

		// view models
		builder.Services.AddSingleton<MainPageViewModel>();
		builder.Services.AddSingleton<GamePageViewModel>();
		builder.Services.AddSingleton<GameOverViewModel>();
		builder.Services.AddSingleton<GameSettingsViewModel>();
		builder.Services.AddSingleton<GameBestScoreViewModel>();

		// popups
		builder.Services.AddTransientPopup<GameModePopup, GameModePopupViewModel>();
		builder.Services.AddTransientPopup<ExitGamePopup, ExitGamePopupViewModel>();
		builder.Services.AddTransientPopup<NoWordsPopup, NoWordsPopupViewModel>();

#if DEBUG
		builder.Logging.AddDebug();
#endif

		return builder.Build();
	}
}
