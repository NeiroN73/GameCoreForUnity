namespace Content.Scripts.UI.Base
{
    public abstract class Screen<TViewModel> : View<TViewModel>
        where TViewModel : ViewModel, new()
    {
    }
}