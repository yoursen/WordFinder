namespace WordFinder;
public static class VisualElementExtensions{
    public static async Task AnimateScale(this VisualElement visualElement){
        await visualElement.ScaleTo(1.05, 250, Easing.SinOut);
        await visualElement.ScaleTo(1, 250, Easing.SinIn);
        visualElement.Scale = 1;
    }
}