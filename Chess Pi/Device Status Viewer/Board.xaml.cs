using Device_Interface.GPIO;
using System.Linq;
using Windows.UI.Xaml.Controls;

namespace Device_Status_Viewer
{
    public sealed partial class Board : UserControl
    {
        GPIOPoller poller;

        public Board()
        {
            this.InitializeComponent();

            poller = new GPIOPoller();
            poller.PollCompleted += Poller_PollCompleted;
        }

        private void Poller_PollCompleted(object sender, bool[] e)
        {
            int i = 0;
            foreach (var pinView in BoardGrid.Children.Cast<Square>())
            {
                pinView.PinStatus = e[i++];
            }
        }
    }
}
