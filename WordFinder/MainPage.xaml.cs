namespace WordFinder;

public partial class MainPage : ContentPage
{
	private MainPageViewModel _viewModel;
	public MainPage(MainPageViewModel viewModel)
	{
		InitializeComponent();
		_viewModel = viewModel;
		BindingContext = viewModel;
	}

	private async void OnButtonClicked(object sender, EventArgs args)
	{
		await (sender as Button).AnimateScale();
		_viewModel.PlayGameCommmand.Execute(null);
	}
}

