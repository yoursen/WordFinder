using CommunityToolkit.Maui;
using Microsoft.Extensions.Logging;

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
		builder.Services.AddSingleton<WordsDatabase>();
		builder.Services.AddSingleton<GameTimer>();

		builder.Services.AddSingleton<MainPage>();
		builder.Services.AddSingleton<GamePage>();
		builder.Services.AddSingleton<GameOver>();

		builder.Services.AddSingleton<MainPageViewModel>();
		builder.Services.AddSingleton<GamePageViewModel>();
		builder.Services.AddSingleton<GameOverViewModel>();

#if DEBUG
		builder.Logging.AddDebug();
#endif

		return builder.Build();
	}
}
