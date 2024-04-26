using CommunityToolkit.Maui;
using Microsoft.Extensions.Logging;
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
		builder
			.UseMauiApp<App>()
			.UseMauiCommunityToolkit()
			.ConfigureFonts(fonts =>
			{
				fonts.AddFont("Roboto-Regular.ttf", "MainFont");
				fonts.AddFont("Roboto-Medium.ttf", "MainFontMedium");
				fonts.AddFont("Roboto-Light.ttf", "MainFontLight");
				fonts.AddFont("FontAwesome.oft", "FontAwesome");
			});

		builder.Services.AddSingleton<GameModel>();
		builder.Services.AddSingleton<WordFitter>();
		builder.Services.AddSingleton<TableService>();
		builder.Services.AddSingleton<GameDatabase>();
		builder.Services.AddSingleton<GameTimer>();

		builder.Services.AddSingleton<MainPage>();
		builder.Services.AddSingleton<GamePage>();
		builder.Services.AddSingleton<GameOver>();
		builder.Services.AddSingleton<AwaitableMessageService>();

		builder.Services.AddSingleton<MainPageViewModel>();
		builder.Services.AddSingleton<GamePageViewModel>();
		builder.Services.AddSingleton<GameOverViewModel>();

		builder.Services.AddTransientPopup<GameModePopup, GameModePopupViewModel>();
		builder.Services.AddTransientPopup<ExitGamePopup, ExitGamePopupViewModel>();

#if DEBUG
		builder.Logging.AddDebug();
#endif

		return builder.Build();
	}
}
