namespace WordFinder;
public static class VisualElementExtensions
{
    public static async Task AnimateScale(this VisualElement visualElement, double scaleFactor = 1.05)
    {
        await visualElement.ScaleTo(scaleFactor, 250, Easing.SinOut);
        await visualElement.ScaleTo(1, 250, Easing.SinIn);
        visualElement.Scale = 1;
    }

    public static void AnimateShake(this VisualElement element)
    {
        // todo: make awaitable animation

        double shakeTranslation = 10;
        var shakeXAnimation = new Animation
            {
                { 0, 0.2, new Animation(v => element.TranslationX = v, 0, shakeTranslation) },
                { 0.2, 0.4, new Animation(v => element.TranslationX = v, shakeTranslation, -shakeTranslation) },
                { 0.4, 0.6, new Animation(v => element.TranslationX = v, -shakeTranslation, shakeTranslation) },
                { 0.6, 0.8, new Animation(v => element.TranslationX = v, shakeTranslation, -shakeTranslation) },
                { 0.8, 1, new Animation(v => element.TranslationX = v, -shakeTranslation, 0) }
            };

        var shakeAnimation = new Animation();
        shakeAnimation.Add(0, 1, shakeXAnimation);
        shakeAnimation.Commit(element, "ShakeAnimation");
    }
}