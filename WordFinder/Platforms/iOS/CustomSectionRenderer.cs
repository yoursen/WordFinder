using Microsoft.Maui.Controls.Platform.Compatibility;

namespace WordFinder;

public class CustomSectionRenderer : ShellSectionRenderer
{
    public CustomSectionRenderer(IShellContext context) : base(context)
    {
    }
    
    public override void ViewDidLoad()
    {
        base.ViewDidLoad();
        InteractivePopGestureRecognizer.Enabled = false;
    }
}