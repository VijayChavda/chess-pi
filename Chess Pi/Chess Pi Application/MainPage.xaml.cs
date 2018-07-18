using Device_Interface.GPIO;
using Windows.UI.Xaml.Controls;

namespace Chess_Pi_Application
{
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();

            GPIOPoller poller = new GPIOPoller();
            poller.PollCompleted += Poller_PollCompleted;
        }

        private void Poller_PollCompleted(object sender, bool[] e)
        {
            GameView.StateChanged(e);
        }

        private void Simulator_StatusChanged(object sender, bool[] e)
        {
            //GameView.StateChanged(e);
        }

        private void UpdateStatusClicked(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            //GameView.StateChanged(Simulator.Status);
        }
    }
}
