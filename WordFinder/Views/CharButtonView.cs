namespace WordFinder.Views;
public class CharButtonView : ContentView
{
    public static readonly BindableProperty TitleProperty =
        BindableProperty.Create(nameof(Title), typeof(string), typeof(CharButtonView), "A");
    public string Title
    {
        get => (string)GetValue(TitleProperty);
        set => SetValue(TitleProperty, value);
    }

    public static readonly BindableProperty IsCheckedProperty =
        BindableProperty.Create(nameof(IsChecked), typeof(bool), typeof(CharButtonView), default(bool));

    public bool IsChecked
    {
        get { return (bool)GetValue(IsCheckedProperty); }
        set { SetValue(IsCheckedProperty, value); }
    }

    public static readonly BindableProperty IsFixedProperty =
        BindableProperty.Create(nameof(IsFixed), typeof(bool), typeof(CharButtonView), default(bool));

    public bool IsFixed
    {
        get { return (bool)GetValue(IsFixedProperty); }
        set { SetValue(IsFixedProperty, value); }
    }
}