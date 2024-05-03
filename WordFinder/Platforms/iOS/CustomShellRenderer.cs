using Microsoft.Maui.Controls.Handlers.Compatibility;
using Microsoft.Maui.Controls.Platform.Compatibility;

namespace WordFinder;

public class CustomShellRenderer : ShellRenderer
{
    public CustomShellRenderer()
    {
    }
    
    protected override IShellSectionRenderer CreateShellSectionRenderer(ShellSection shellSection)
    {
        return new CustomSectionRenderer(this);
    }
}
