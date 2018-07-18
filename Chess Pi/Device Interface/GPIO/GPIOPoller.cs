using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Windows.UI.Xaml;

namespace Device_Interface.GPIO
{
    public class GPIOPoller
    {
        RpiGPIO RpiGPIO;
        GPIOExtender Extender1, Extender2, Extender3;

        const int Frequency = 200;
        DispatcherTimer Timer;

        public GPIOPoller()
        {
            RpiGPIO = new RpiGPIO();
            Extender1 = new GPIOExtender(0x20);
            Extender2 = new GPIOExtender(0x21);
            Extender3 = new GPIOExtender(0x24);
            Timer = new DispatcherTimer() { Interval = TimeSpan.FromMilliseconds(Frequency) };
            Timer.Tick += (sender, e) => Poll();
            Timer.Start();
        }

        void Poll()
        {
            var status16 = RpiGPIO.ReadGPIOStatus();
            var status32 = Extender1.ReadGPIOStatus();
            var status48 = Extender2.ReadGPIOStatus();
            var status64 = Extender3.ReadGPIOStatus();
            var status = status16.Concat(status32).Concat(status48).Concat(status64).ToArray();
            PollCompleted.Invoke(this, status);
        }

        public event EventHandler<bool[]> PollCompleted;
    }
}
