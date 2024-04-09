namespace WordFinder;

public class GameLetter : BindableObject
{
    public GameLetter() { }
    public GameLetter(string letter) => Letter = letter;

    public static readonly BindableProperty LetterProperty =
        BindableProperty.Create(nameof(Letter), typeof(string), typeof(GameLetter), default(string));

    public string Letter
    {
        get { return (string)GetValue(LetterProperty); }
        set { SetValue(LetterProperty, value); }
    }

    public static readonly BindableProperty IsCheckedProperty =
        BindableProperty.Create(nameof(IsChecked), typeof(bool), typeof(GameLetter), default(bool));

    public bool IsChecked
    {
        get { return (bool)GetValue(IsCheckedProperty); }
        set { SetValue(IsCheckedProperty, value); }
    }

    public static readonly BindableProperty IsMainLetterProperty =
        BindableProperty.Create(nameof(IsMainLetter), typeof(bool), typeof(GameLetter), default(bool));

    public bool IsMainLetter
    {
        get { return (bool)GetValue(IsMainLetterProperty); }
        set { SetValue(IsMainLetterProperty, value); }
    }

    public override string ToString() => Letter ?? string.Empty;
}