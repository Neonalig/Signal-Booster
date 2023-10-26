namespace SignalBooster; 

/// <summary> Interaction logic for MainWindow.xaml </summary>
public partial class MainWindow {
    public MainWindow() {
        InitializeComponent();
    }

    /// <inheritdoc cref="SignalBooster.ViewModels.MainWindowViewModel.Back"/>
    public void Back( bool Release = false ) => ViewModel.Back(Release);
}