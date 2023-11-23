namespace LionFire.UI.Entities
{
    // OLD
    ///// <summary>
    ///// Navigate to the context
    ///// </summary>
    ///// <param name="instantiation">Examples: A URI string for web browsers.  For WPF, it could be a custom UserControl.  If MVVM support is available, it can be a custom ViewModel type</param>
    ///// <param name="viewName"></param>
    ///// <param name="options"></param>
    //IUIObject Show(IUIParent parent, UIInstantiation instantiation);

    public interface IFactory<in TInput, out TOutput>
    {
        TOutput Create(TInput instantiation);
    }
}

#region OLD - startup task
//Func<Task> startup; // StartAsync waits for ctor tasks to finish
//startup = () => Task.Run(async () =>
//{
//    WindowSettings = await windowSettings.GetNonDefaultValue().ConfigureAwait(false);
//    // ENH Make this a Participant that contributes to CanStartShell?
//});

//await startup().ConfigureAwait(false);
//startup = null;
#endregion
