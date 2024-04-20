using WordFinder.Views;

namespace WordFinder;

public partial class AppShell : Shell
{
	public AppShell()
	{
		InitializeComponent();

		Routing.RegisterRoute("GamePage", typeof(GamePage));
		Routing.RegisterRoute("GameOver", typeof(GameOver));
	}
}
