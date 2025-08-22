namespace GameCore.UI
{
    public abstract class Screen<TViewModel> : View<TViewModel>
        where TViewModel : ViewModel, new()
    {
    }
}