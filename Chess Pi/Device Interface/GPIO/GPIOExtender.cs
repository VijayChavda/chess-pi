using System;
using System.Collections;
using System.Linq;
using Windows.Devices.I2c;

namespace Device_Interface.GPIO
{
    public class GPIOExtender
    {
        public readonly int Address;
        I2cConnectionSettings Settings;
        I2cController Controller;
        I2cDevice Device;

        public GPIOExtender(int Address)
        {
            this.Address = Address;
            Initialize();
        }

        async void Initialize()
        {
            //Initialize I2c device
            Settings = new I2cConnectionSettings(Address) { BusSpeed = I2cBusSpeed.FastMode };
            Controller = await I2cController.GetDefaultAsync();
            Device = Controller.GetDevice(Settings);

            //Activate internal pull-up resistors
            WriteRegister(Registers.GPPUA, 0xFF);
            WriteRegister(Registers.GPPUB, 0xFF);

            //Invert GPIO polarity
            WriteRegister(Registers.IPOLA, 0xFF);
            WriteRegister(Registers.IPOLB, 0xFF);
        }

        byte ReadRegister(Registers Register)
        {
            byte[] buffer = new byte[1];
            Device.WriteRead(new byte[] { (byte)Register }, buffer);
            return buffer[0];
        }

        void WriteRegister(Registers Register, byte Data)
        {
            var buffer = new byte[] { (byte)Register, Data };
            Device.Write(buffer);
        }

        public bool[] ReadGPIOStatus()
        {
            //byte[] buffer = new byte[2];
            //Device.WriteRead(new byte[] { (byte)Registers.GPIOA, (byte)Registers.GPIOB }, buffer);
            //return new BitArray(buffer).Cast<bool>().ToArray();

            var a = ReadRegister(Registers.GPIOA);
            var b = ReadRegister(Registers.GPIOB);

            return new BitArray(new byte[] { a, b }).Cast<bool>().ToArray();
        }

        public enum Registers : byte
        {
            IODIRA = 0x00,
            IODIRB = 0x01,

            IPOLA = 0X02,
            IPOLB = 0X03,

            GPPUA = 0x0C,
            GPPUB = 0x0D,

            GPIOA = 0x12,
            GPIOB = 0x13,
        }
    }
}
