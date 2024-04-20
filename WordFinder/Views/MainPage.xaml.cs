using WordFinder.Models;
using WordFinder.ViewModels;

namespace WordFinder.Views;

public partial class MainPage : ContentPage
{
	private MainPageViewModel _viewModel;
	private WordsDatabase _db;
	public MainPage(MainPageViewModel viewModel, WordsDatabase db)
	{
		InitializeComponent();
		_viewModel = viewModel;
		_db = db;
		BindingContext = viewModel;
	}

	private async void OnButtonClicked(object sender, EventArgs args)
	{
		await (sender as Button).AnimateScale();
		_viewModel.PlayGameCommmand.Execute(null);
	}

    protected override async void OnNavigatedTo(NavigatedToEventArgs args)
    {
        base.OnNavigatedTo(args);
		await _db.Init();
    }
}

