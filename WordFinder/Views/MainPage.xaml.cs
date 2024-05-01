using WordFinder.Models;
using WordFinder.ViewModels;

namespace WordFinder.Views;

public partial class MainPage : ContentPage
{
	private MainPageViewModel _viewModel;
	private GameDatabase _db;
	public MainPage(MainPageViewModel viewModel, GameDatabase db)
	{
		InitializeComponent();
		_viewModel = viewModel;
		_db = db;
		BindingContext = viewModel;
	}

	private async void OnPlayGameClicked(object sender, EventArgs args)
	{
		await (sender as Button).AnimateScale();
		_viewModel.PlayGameCommmand.Execute(null);
	}

	private async void OnBestScoreClicked(object sender, EventArgs args)
	{
		await (sender as Button).AnimateScale();
		_viewModel.BestScoreCommmand.Execute(null);
	}
	private async void OnSettingsClicked(object sender, EventArgs args)
	{
		await (sender as Button).AnimateScale();
		_viewModel.GameSettingsCommmand.Execute(null);
	}

    protected override async void OnNavigatedTo(NavigatedToEventArgs args)
    {
        base.OnNavigatedTo(args);
		await _db.Init();
    }
}

