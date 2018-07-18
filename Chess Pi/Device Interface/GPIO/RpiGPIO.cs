using System;
using Windows.Devices.Gpio;

namespace Device_Interface.GPIO
{
    public class RpiGPIO
    {
        int[] PinNumbers = new int[] { 4, 5, 6, 12, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25, 26, 27 };
        GpioController GpioController;
        GpioPin[] GpioPins;

        public RpiGPIO()
        {
            Initialize();
        }

        public bool[] ReadGPIOStatus()
        {
            bool[] status = new bool[16];
            for (int pin = 0; pin < 16; pin++)
            {
                status[pin] = GpioPins[pin].Read() == GpioPinValue.Low;
            }
            return status;
        }

        void Initialize()
        {
            #region Initialize Controller

            GpioController = GpioController.GetDefault();
            if (GpioController == null)
            {
                throw new NotSupportedException("No GPIO controller found.");
            }

            #endregion

            #region Initialize Pins

            GpioPins = new GpioPin[16];
            for (int pin = 0; pin < 16; pin++)
            {
                GpioPins[pin] = GpioController.OpenPin(PinNumbers[pin]);
                GpioPins[pin].SetDriveMode(GpioPinDriveMode.InputPullUp);
            }

            #endregion
        }
    }
}
