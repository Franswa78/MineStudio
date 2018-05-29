using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using System.Windows.Forms;
using static MineStudio.frmMain;

namespace MineStudio
{
    public class RS232_Comms
    {
        public List<SerialPort> connectedVMUs = new List<SerialPort>();
        public List<SerialPort> connectedDallas = new List<SerialPort>();
        public byte[] RxBuf = new byte[256];
        public byte[] dump = new byte[256];
        public int RxLen = 0;
        COMMS_STATUS status;

        public RS232_Comms()
        {

        }

        public int searchForVMUs()
        {
            byte[] msg = new byte[1];
            string[] availablePorts = SerialPort.GetPortNames();
            List<SerialPort> ports = new List<SerialPort>();
            //COMMS_STATUS status;

            closePorts();

            if (availablePorts.Length == 0)
            {
                return -1;
            }

            msg[0] = (byte)'x';     // Read the version string from the VMU

            foreach (string name in availablePorts)
            {
                try
                {
                    SerialPort p = new SerialPort(name);
                    p.BaudRate = 9600;
                    p.DataBits = 8;
                    p.StopBits = StopBits.One;
                    p.Parity = Parity.None;
                    ports.Add(p);
                }
                catch (Exception)
                {

                }
            }

            foreach (SerialPort port in ports)
            {
                try
                {
                    if (!port.IsOpen) port.Open();

                    for (int retry = 0; retry < 2; retry++)
                    {
                        status = sendAndReceiveMsg(port, msg, 1, this.RxBuf, ref this.RxLen);
                        if (status == COMMS_STATUS.OKAY) { break; }
                        if (status == COMMS_STATUS.PORT_ERROR) { break; }
                        Thread.Sleep(15);
                    }

                    if (status == COMMS_STATUS.OKAY)
                    {
                        if ((RxLen > 5) && (RxBuf[0] == 0xF0) && (RxBuf[2] == 'x')) { this.connectedVMUs.Add(port); break; }
                    }
                    
                    port.Close();
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.Message);
                }
            }
            
            if (this.connectedVMUs.Count > 0) { return 0; }
            else return -1;
        }//end of searchForVMUs

        public SerialPort getConnectedVMUPort(int portListIndex)
        {
            if ((portListIndex < 0) && (portListIndex >= this.connectedVMUs.Count)) return null;
            else
            {
                return this.connectedVMUs[portListIndex];
            }
        }

        public void closePorts()
        {
            if (this.connectedVMUs.Count > 0)
            {
                foreach (SerialPort p in this.connectedVMUs)
                {
                    try
                    {
                        p.Close();
                        p.Dispose();
                        Thread.Sleep(100);
                    }
                    catch (Exception)
                    {

                    }
                }
            }
            this.connectedVMUs.Clear();
        }
        public void decode_Trip_Data(byte[] data, int startIndex, ref TripDataStruct tripStruct)
        {
            tripStruct.header = data[startIndex];
            tripStruct.maxSpeed = data[startIndex + 1];
            tripStruct.serialNumber = BitConverter.ToUInt16(data, startIndex + 2);
            tripStruct.driverID = Encoding.ASCII.GetString(data, startIndex + 4, 13);
            tripStruct.vehicleID = Encoding.ASCII.GetString(data, startIndex + 17, 11);
            tripStruct.jobNumber = Encoding.ASCII.GetString(data, startIndex + 28, 12);
            tripStruct.startDate.year = (int)(BitConverter.ToInt16(data, startIndex + 40));
            tripStruct.startDate.month = data[startIndex + 42];
            tripStruct.startDate.day = data[startIndex + 43];
            tripStruct.endDate.year = (int)(BitConverter.ToInt16(data, startIndex + 44));
            tripStruct.endDate.month = data[startIndex + 46];
            tripStruct.endDate.day = data[startIndex + 47];
            tripStruct.startTime.hour = data[startIndex + 48];
            tripStruct.startTime.minute = data[startIndex + 49];
            tripStruct.startTime.second = data[startIndex + 50];
            tripStruct.endTime.hour = data[startIndex + 51];
            tripStruct.endTime.minute = data[startIndex + 52];
            tripStruct.endTime.second = data[startIndex + 53];
            tripStruct.startOdo = BitConverter.ToUInt32(data, startIndex + 54);
            tripStruct.endOdo = BitConverter.ToUInt32(data, startIndex + 58);
            tripStruct.maxRPM = BitConverter.ToUInt16(data, startIndex + 62);
            tripStruct.idleTime.hour = data[startIndex + 64];
            tripStruct.idleTime.minute = data[startIndex + 65];
            tripStruct.idleTime.second = data[startIndex + 66];
            tripStruct.greenTime.hour = data[startIndex + 67];
            tripStruct.greenTime.minute = data[startIndex + 68];
            tripStruct.greenTime.second = data[startIndex + 69];
            tripStruct.speedingTime.hour = data[startIndex + 70];
            tripStruct.speedingTime.minute = data[startIndex + 71];
            tripStruct.speedingTime.second = data[startIndex + 72];
            tripStruct.overRPMTime.hour = data[startIndex + 73];
            tripStruct.overRPMTime.minute = data[startIndex + 74];
            tripStruct.overRPMTime.second = data[startIndex + 75];
            tripStruct.fuel = BitConverter.ToUInt16(data, startIndex + 76);
            tripStruct.fuelPrice = BitConverter.ToUInt32(data, startIndex + 78);
            tripStruct.timerCnt = BitConverter.ToUInt16(data, startIndex + 82);
            tripStruct.eventCnt = BitConverter.ToUInt16(data, startIndex + 84);
            tripStruct.harshAccel = data[startIndex + 86];
            tripStruct.harshBrake = data[startIndex + 87];
            tripStruct.gpsLat = (((UInt64)(BitConverter.ToUInt32(data, startIndex + 88) * 90)) / 0x7FFFFFFF);
            tripStruct.gpsLong = (((UInt64)(BitConverter.ToUInt32(data, startIndex + 92) * 180)) / 0x7FFFFFFF);
            tripStruct.mode = data[startIndex + 96];
            Array.Copy(data, startIndex + 97, tripStruct.spare, 0, 3);
            tripStruct.crc = BitConverter.ToUInt16(data, startIndex + 100);
            tripStruct.status = data[startIndex + 102];
            tripStruct.endOfRec = data[startIndex + 103];
        }
        public void decode_BrakeEvent_Data(byte[] data, int startIndex, ref brakeEventStruct brakeStruct)
        {
            brakeStruct.header = data[startIndex];
            brakeStruct.time.hour = data[startIndex + 1];
            brakeStruct.time.minute = data[startIndex + 2];
            brakeStruct.time.second = data[startIndex + 3];
            brakeStruct.serialNum = BitConverter.ToUInt16(data, startIndex + 4);
            Array.Copy(data, startIndex + 6, brakeStruct.speed, 0, frmMain.MAX_POINTS);
            int i = startIndex + 6 + frmMain.MAX_POINTS;
            brakeStruct.odo = BitConverter.ToUInt32(data, i);
            brakeStruct.date.year = BitConverter.ToInt16(data, i + 4);
            brakeStruct.date.month = data[i + 6];
            brakeStruct.date.day = data[i + 7];
            Array.Copy(data, i + 8, brakeStruct.spare, 0, 2);
            brakeStruct.crc = BitConverter.ToUInt16(data, i + 10);
            brakeStruct.status = data[i + 12];
            brakeStruct.endOfRec = data[i + 13];
        }
        public void decode_accelEvent_Data(byte[] data, int startIndex, ref accelEventStruct accelStruct)
        {
            accelStruct.header = data[startIndex];
            accelStruct.time.hour = data[startIndex + 1];
            accelStruct.time.minute = data[startIndex + 2];
            accelStruct.time.second = data[startIndex + 3];
            accelStruct.serialNum = BitConverter.ToUInt16(data, startIndex + 4);
            Array.Copy(data, startIndex + 6, accelStruct.speed, 0, frmMain.MAX_POINTS);
            int i = startIndex + 6 + frmMain.MAX_POINTS;
            accelStruct.odo = BitConverter.ToUInt32(data, i);
            accelStruct.date.year = BitConverter.ToInt16(data, i + 4);
            accelStruct.date.month = data[i + 6];
            accelStruct.date.day = data[i + 7];
            Array.Copy(data, i + 8, accelStruct.spare, 0, 2);
            accelStruct.crc = BitConverter.ToUInt16(data, i + 10);
            accelStruct.status = data[i + 12];
            accelStruct.endOfRec = data[i + 13];
        }
        public void decode_fuelEvent_Data(byte[] data, int startIndex, ref fuelEventStruct fuelStruct)
        {
            fuelStruct.header = data[startIndex];
            fuelStruct.time.hour = data[startIndex + 1];
            fuelStruct.time.minute = data[startIndex + 2];
            fuelStruct.time.second = data[startIndex + 3];
            fuelStruct.vehicleID = Encoding.ASCII.GetString(data, startIndex + 4, frmMain.VEHICLE_ID_SIZE);
            fuelStruct.driverID = Encoding.ASCII.GetString(data, startIndex + 4 + frmMain.VEHICLE_ID_SIZE, frmMain.DRIVER_ID_SIZE);
            int i = 4 + startIndex + frmMain.VEHICLE_ID_SIZE + frmMain.DRIVER_ID_SIZE;
            fuelStruct.pumpID = BitConverter.ToUInt16(data, i);
            fuelStruct.date.year = BitConverter.ToUInt16(data, i + 2);
            fuelStruct.date.month = data[i + 4];
            fuelStruct.date.day = data[i + 5];
            fuelStruct.fuel = BitConverter.ToUInt16(data, i + 6);
            fuelStruct.fuelPrice = BitConverter.ToUInt32(data, i + 8);
            fuelStruct.index = BitConverter.ToUInt16(data, i + 12);
            Array.Copy(data, i + 14, fuelStruct.spare, 0, 2);
            fuelStruct.crc = BitConverter.ToUInt16(data, i + 16);
            fuelStruct.status = data[i + 18];
            fuelStruct.endOfRec = data[i + 19];
        }
        public void decode_inputEvent_Data(byte[] data, int startIndex, ref inputEventStruct inputStruct)
        {
            inputStruct.header = data[startIndex];
            inputStruct.type = data[startIndex + 1];
            inputStruct.onDate.year = BitConverter.ToUInt16(data, startIndex + 2);
            inputStruct.onDate.month = data[startIndex + 4];
            inputStruct.onDate.day = data[startIndex + 5];
            inputStruct.onTime.hour = data[startIndex + 6];
            inputStruct.onTime.minute = data[startIndex + 7];
            inputStruct.onTime.second = data[startIndex + 8];
            inputStruct.offTime.hour = data[startIndex + 9];
            inputStruct.offTime.minute = data[startIndex + 10];
            inputStruct.offTime.second = data[startIndex + 11];
            inputStruct.id = Encoding.ASCII.GetString(data, startIndex + 12, frmMain.VEHICLE_ID_SIZE);
            inputStruct.driver = Encoding.ASCII.GetString(data, startIndex + 12 + frmMain.VEHICLE_ID_SIZE, frmMain.DRIVER_ID_SIZE);
            int i = 12 + startIndex + frmMain.VEHICLE_ID_SIZE + frmMain.DRIVER_ID_SIZE;
            inputStruct.distance = BitConverter.ToUInt16(data, i);
            inputStruct.odo = BitConverter.ToUInt32(data, i + 2);
            Array.Copy(data, i + 6, inputStruct.spare, 0, 2);
            inputStruct.crc = BitConverter.ToUInt16(data, i + 8);
            inputStruct.status = data[i + 10];
            inputStruct.endOfRec = data[i + 11];
        }
        public void decode_userEvent_Data(byte[] data, int startIndex, ref userEventStruct userStruct)
        {
            userStruct.header = data[startIndex];
            userStruct.time.hour = data[startIndex + 1];
            userStruct.time.minute = data[startIndex + 2];
            userStruct.time.second = data[startIndex + 3];
            userStruct.date.year = BitConverter.ToUInt16(data, startIndex + 4);
            userStruct.date.month = data[startIndex + 6];
            userStruct.date.day = data[startIndex + 7];
            userStruct.tripNo = BitConverter.ToUInt16(data, startIndex + 8);
            userStruct.odometer = BitConverter.ToUInt32(data, startIndex + 10);
            userStruct.id = Encoding.ASCII.GetString(data, startIndex + 14, frmMain.VEHICLE_ID_SIZE);
            userStruct.driver = Encoding.ASCII.GetString(data, startIndex + 14 + frmMain.VEHICLE_ID_SIZE, frmMain.DRIVER_ID_SIZE);
            int i = 14 + startIndex + frmMain.VEHICLE_ID_SIZE + frmMain.DRIVER_ID_SIZE;
            userStruct.input = Encoding.ASCII.GetString(data, i, 16);
            userStruct.type = data[i + 16];
            userStruct.spare[0] = data[i + 17];
            userStruct.crc = BitConverter.ToUInt16(data, i + 18);
            userStruct.status = data[i + 20];
            userStruct.endOfRec = data[i + 21];
        }
        public void decode_idleEvent_Data(byte[] data, int startIndex, ref idleEventStruct idleStruct)
        {
            idleStruct.header = data[startIndex];
            idleStruct.time.hour = data[startIndex + 1];
            idleStruct.time.minute = data[startIndex + 2];
            idleStruct.time.second = data[startIndex + 3];
            idleStruct.date.year = BitConverter.ToUInt16(data, startIndex + 4);
            idleStruct.date.month = data[startIndex + 6];
            idleStruct.date.day = data[startIndex + 7];
            idleStruct.tripNum = BitConverter.ToUInt16(data, startIndex + 8);
            idleStruct.odoMeter = BitConverter.ToUInt32(data, startIndex + 10);
            idleStruct.duration = BitConverter.ToUInt16(data, startIndex + 14);
            idleStruct.id = Encoding.ASCII.GetString(data, startIndex + 16, frmMain.VEHICLE_ID_SIZE);
            idleStruct.driver = Encoding.ASCII.GetString(data, startIndex + 16 + frmMain.VEHICLE_ID_SIZE, frmMain.DRIVER_ID_SIZE);
            int i = 16 + startIndex + frmMain.VEHICLE_ID_SIZE + frmMain.DRIVER_ID_SIZE;
            idleStruct.crc = BitConverter.ToUInt16(data, i);
            idleStruct.status = data[i + 2];
            idleStruct.endOfRec = data[i + 3];

        }
        public void decode_powerEvent_Data(byte[] data, int startIndex, ref powerEventStruct powerStruct)
        {
            powerStruct.header = data[startIndex];
            powerStruct.time.hour = data[startIndex + 1];
            powerStruct.time.minute = data[startIndex + 2];
            powerStruct.time.second = data[startIndex + 3];
            powerStruct.date.year = BitConverter.ToUInt16(data, startIndex + 4);
            powerStruct.date.month = data[startIndex + 6];
            powerStruct.date.day = data[startIndex + 7];
            powerStruct.tripNo = BitConverter.ToUInt16(data, startIndex + 8);
            powerStruct.odoMeter = BitConverter.ToUInt32(data, startIndex + 10);
            powerStruct.spare = BitConverter.ToUInt16(data, startIndex + 14);
            powerStruct.id = Encoding.ASCII.GetString(data, startIndex + 16, frmMain.VEHICLE_ID_SIZE);
            powerStruct.driver = Encoding.ASCII.GetString(data, startIndex + 16 + frmMain.VEHICLE_ID_SIZE, frmMain.DRIVER_ID_SIZE);
            int i = 16 + startIndex + frmMain.VEHICLE_ID_SIZE + frmMain.DRIVER_ID_SIZE;
            powerStruct.crc = BitConverter.ToUInt16(data, i);
            powerStruct.status = data[i + 2];
            powerStruct.endOfRec = data[i + 3];

        }
        public void decode_overSpeedEvent_Data(byte[] data, int startIndex, ref speedEventStruct speedStruct)
        {
            speedStruct.header = data[startIndex];
            speedStruct.time.hour = data[startIndex + 1];
            speedStruct.time.minute = data[startIndex + 2];
            speedStruct.time.second = data[startIndex + 3];
            speedStruct.date.year = BitConverter.ToUInt16(data, startIndex + 4);
            speedStruct.date.month = data[startIndex + 6];
            speedStruct.date.day = data[startIndex + 7];
            speedStruct.tripNo = BitConverter.ToUInt16(data, startIndex + 8);
            speedStruct.odometer = BitConverter.ToUInt32(data, startIndex + 10);
            speedStruct.id = Encoding.ASCII.GetString(data, startIndex + 14, frmMain.VEHICLE_ID_SIZE);
            speedStruct.driver = Encoding.ASCII.GetString(data, startIndex + 14 + frmMain.VEHICLE_ID_SIZE, frmMain.DRIVER_ID_SIZE);
            int i = 14 + startIndex + frmMain.VEHICLE_ID_SIZE + frmMain.DRIVER_ID_SIZE;
            speedStruct.limit = data[i];
            speedStruct.spdAvg = data[i + 1];
            speedStruct.tolTotal = BitConverter.ToUInt16(data, i + 2);
            speedStruct.tol10 = BitConverter.ToUInt16(data, i + 4);
            speedStruct.tol20 = BitConverter.ToUInt16(data, i + 6);
            speedStruct.tol30 = BitConverter.ToUInt16(data, i + 8);
            speedStruct.crc = BitConverter.ToUInt16(data, i + 10);
            speedStruct.status = data[i + 12];
            speedStruct.endOfRec = data[i + 13];
        }

        public COMMS_STATUS sendAndReceiveMsg(SerialPort port, byte[] msg, int txLen, byte[] rxBuf, ref int rxLen)
        {
            List<byte> rxList = new List<byte>();
            int rxMsgLen = 0;
            DateTime time;
            bool timedOut = false;
            bool frameError = false;
            COMMS_STATUS status;
            int b, rxCRC;
            byte temp;

            try
            {
                port.DiscardInBuffer();
                this.status = sendMsg(port, msg, txLen);            // BytesToRead == 13 NADAT dit na sendMsg gegaan het
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "COMMS ERROR");
            }

            if (this.status == COMMS_STATUS.OKAY)
            {
                try
                {
                    b = 0;
                    rxMsgLen = 0;
                    rxList.Clear();
                    time = DateTime.Now;
                    while (true)
                    {
                        if (port.BytesToRead > 0) { rxList.Add((byte)port.ReadByte()); b++; }   //  rxList == 1     != 0
                        if ((b == 2) && (rxList[0] == 0xF0)) { rxMsgLen = rxList[1]; }
                        if (b >= rxMsgLen + 4) { timedOut = false; break; }
                        TimeSpan period = DateTime.Now.Subtract(time);
                        //if (period.Seconds >= 5) { timedOut = true; break; }
                        if (period.Seconds >= 0.1) { timedOut = true; break; }
                    }

                    while (port.BytesToRead > 0) { port.ReadByte(); }                           // BytesToRead word minder van 13 na 0

                    if (timedOut) { return COMMS_STATUS.TIMED_OUT; }

                    rxCRC = rxList[1];
                    for (int y = 2; y < rxList.Count - 1; y++)
                    {
                        rxCRC = rxCRC ^ rxList[y];
                    }
                    if (rxCRC == 0)
                    {
                        rxList.CopyTo(0, rxBuf, 0, rxList.Count);
                        rxLen = rxList.Count;
                        return COMMS_STATUS.OKAY;
                    }
                    else
                    {
                        return COMMS_STATUS.CRC_ERROR;
                    }
                }
                catch (Exception)
                {
                    return COMMS_STATUS.PORT_ERROR;
                }
            }
            else return this.status;
        }
        public COMMS_STATUS sendMsg(SerialPort port, byte[] msg, int len)
        {
            byte[] txBuf = new byte[len + 4];
            int i, x, crc = len;

            txBuf[0] = 0xF0;
            txBuf[1] = (byte)len;
            for (i = 0, x = 2; i < len; i++, x++)
            {
                txBuf[x] = msg[i];
                crc ^= msg[i];
            }
            txBuf[x++] = (byte)crc;
            txBuf[x++] = 0xF2;

            try
            {
                port.Write(txBuf, 0, x);
                return COMMS_STATUS.OKAY;           // txBuf 5 values met getalle
            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.Message);
                return COMMS_STATUS.PORT_ERROR;
            }
        }
        public COMMS_STATUS setVMUTime(SerialPort port)
        {
            byte[] msg = new byte[7];
            DateTime pcTime = DateTime.Now;

            msg[0] = (byte)'T';
            msg[1] = (byte)pcTime.Hour;
            msg[2] = (byte)pcTime.Minute;
            msg[3] = (byte)pcTime.Second;
            msg[4] = (byte)(pcTime.Year - 2000);
            msg[5] = (byte)pcTime.Month;
            msg[6] = (byte)pcTime.Day;

            this.status = sendMsg(port, msg, 7);
            Thread.Sleep(25);
            return this.status;
        }
        public COMMS_STATUS setVMUFlashConfig(SerialPort port, blockConfigStruct[] newConfig, int numBlocks)
        {
            int index;
            byte[] msg = new byte[(numBlocks * 2) + 3];
            msg[0] = 0xB1;
            msg[1] = (byte)'F';
            msg[2] = (byte)numBlocks;

            for (int i = 0; i < numBlocks; i++)
            {
                index = (2 * i) + 3;
                msg[index] = (byte)newConfig[i].type;
                msg[index + 1] = (byte)newConfig[i].recordSize;
            }

            for (int retry = 0; retry < 3; retry++)
            {
                this.status = sendAndReceiveMsg(port, msg, (numBlocks * 2) + 3, this.RxBuf, ref this.RxLen);
                if (this.status == COMMS_STATUS.OKAY) { break; }
                if (this.status == COMMS_STATUS.PORT_ERROR) { break; }
                Thread.Sleep(15);
            }
            if (this.status == COMMS_STATUS.OKAY)
            {
                if ((this.RxBuf[2] == 0xB1) && (this.RxBuf[3] == 'F'))
                {

                }
                else
                {
                    this.status = COMMS_STATUS.COMMAND_ERROR;
                }
            }

            return status;
        }
        public COMMS_STATUS setVMUBaudrate(SerialPort port, int baudrate)
        {
            List<byte> rxList = new List<byte>();
            int rxMsgLen = 0;
            DateTime time;
            bool timedOut = false;
            bool frameError = false;
            COMMS_STATUS status;
            int b, rxCRC;
            byte temp;
            byte baudIndex = 0;
            byte[] msg = new byte[3];

            switch (baudrate)
            {
                case 9600: { baudIndex = 0; break; }
                case 19200: { baudIndex = 1; break; }
                case 38400: { baudIndex = 2; break; }
                case 56000: { baudIndex = 3; break; }
                default: { baudIndex = 0; break; }
            }
            msg[0] = 0xC1;
            msg[1] = (byte)'A';
            msg[2] = baudIndex;

            this.status = sendMsg(port, msg, 3);
            Thread.Sleep(100);

            if (this.status == COMMS_STATUS.OKAY)
            {
                port.Close();
                port.BaudRate = baudrate;
                port.Open();

                DateStruct d = new DateStruct(0);
                TimeStruct t = new TimeStruct(0);

                this.status = this.getVMUTime(port, ref d, ref t);
                if (this.status == COMMS_STATUS.OKAY) { }
                else
                {
                    port.Close();
                    port.BaudRate = 9600;
                    port.Open();
                }
            }

            return this.status;
        }
        public COMMS_STATUS eraseFlash(SerialPort port)
        {
            byte[] msg = new byte[1];
            msg[0] = (byte)'J';

            for (int retry = 0; retry < 3; retry++)
            {
                this.status = sendAndReceiveMsg(port, msg, 1, this.RxBuf, ref this.RxLen);
                if (this.status == COMMS_STATUS.OKAY) { break; }
                if (this.status == COMMS_STATUS.PORT_ERROR) { break; }
                Thread.Sleep(15);
            }
            if (this.status == COMMS_STATUS.OKAY)
            {
                if (this.RxBuf[3] == 'J')
                {

                }
                else
                {
                    this.status = COMMS_STATUS.COMMAND_ERROR;
                }
            }

            return this.status;
        }

        public COMMS_STATUS getVMUVersion(SerialPort port, ref VMUConstStruct vmuStruct)
        {
            byte[] msg = new byte[1];
            msg[0] = (byte)'x';


            for (int retry = 0; retry < 3; retry++)
            {
                status = sendAndReceiveMsg(port, msg, 1, this.RxBuf, ref this.RxLen);
                if (status == COMMS_STATUS.OKAY) { break; }
                if (status == COMMS_STATUS.PORT_ERROR) { break; }
                Thread.Sleep(15);
            }

            if (status == COMMS_STATUS.OKAY)
            {
                if (this.RxBuf[2] == 'x')
                {
                    vmuStruct.version = Encoding.ASCII.GetString(this.RxBuf, 3, 8);     // reads VMU Version "2.5O"
                }
            }

            return status;
        }
        public COMMS_STATUS getVMUSerialNum(SerialPort port, ref VMUConstStruct vmuStruct)
        {
            byte[] msg = new byte[1];
            msg[0] = (byte)'w';
            //COMMS_STATUS status;

            for (int retry = 0; retry < 3; retry++)
            {
                status = sendAndReceiveMsg(port, msg, 1, this.RxBuf, ref this.RxLen);
                if (status == COMMS_STATUS.OKAY) { break; }
                if (status == COMMS_STATUS.PORT_ERROR) { break; }
                Thread.Sleep(15);
            }

            if (status == COMMS_STATUS.OKAY)
            {
                if (this.RxBuf[2] == 'w')
                {
                    vmuStruct.serialNumber = Encoding.ASCII.GetString(this.RxBuf, 4, this.RxBuf[3]);
                }
            }

            return status;
        }
        public COMMS_STATUS getVMUCalibration(SerialPort port, ref VMUConstStruct vmuStruct)
        {
            byte[] msg = new byte[1];
            msg[0] = (byte)'D';
            //COMMS_STATUS status;

            for (int retry = 0; retry < 3; retry++)
            {
                status = sendAndReceiveMsg(port, msg, 1, this.RxBuf, ref this.RxLen);
                if (status == COMMS_STATUS.OKAY) { break; }
                if (status == COMMS_STATUS.PORT_ERROR) { break; }
                Thread.Sleep(15);
            }

            if (status == COMMS_STATUS.OKAY)
            {
                if (this.RxBuf[2] == 'D')
                {
                    vmuStruct.calRPM = (Int16)((this.RxBuf[3] * 256) + this.RxBuf[4]);
                    vmuStruct.calDistance = (Int16)((this.RxBuf[5] * 256) + this.RxBuf[6]);
                }
            }

            return status;
        }
        public COMMS_STATUS getVMULimits(SerialPort port, ref VMUConstStruct vmuStruct)
        {
            byte[] msg = new byte[1];
            msg[0] = (byte)'G';
            //COMMS_STATUS status;

            for (int retry = 0; retry < 3; retry++)
            {
                status = sendAndReceiveMsg(port, msg, 1, this.RxBuf, ref this.RxLen);
                if (status == COMMS_STATUS.OKAY) { break; }
                if (status == COMMS_STATUS.PORT_ERROR) { break; }
                Thread.Sleep(15);
            }
            if (status == COMMS_STATUS.OKAY)
            {
                if (this.RxBuf[2] == 'G')
                {
                    vmuStruct.maxRPM = (Int16)(this.RxBuf[3] * 256 + this.RxBuf[4]);
                    vmuStruct.maxSpeed = (Int16)(this.RxBuf[5] * 256 + this.RxBuf[6]);
                    vmuStruct.maxBrake = (Int16)(this.RxBuf[7] * 256 + this.RxBuf[8]);
                    vmuStruct.maxAccel = (Int16)(this.RxBuf[9] * 256 + this.RxBuf[10]);
                    vmuStruct.bell = this.RxBuf[11];
                }
            }

            return status;
        }
        public COMMS_STATUS getVMUTime(SerialPort port, ref DateStruct vmuDate, ref TimeStruct vmuTime)
        {
            byte[] msg = new byte[1];
            msg[0] = (byte)'R';
            //COMMS_STATUS status;

            for (int retry = 0; retry < 3; retry++)
            {
                status = sendAndReceiveMsg(port, msg, 1, this.RxBuf, ref this.RxLen);
                if (status == COMMS_STATUS.OKAY) { break; }
                if (status == COMMS_STATUS.PORT_ERROR) { break; }
                Thread.Sleep(15);
            }

            if (status == COMMS_STATUS.OKAY)
            {
                if (this.RxBuf[2] == 'R')
                {
                    vmuTime.hour = this.RxBuf[3];
                    vmuTime.minute = this.RxBuf[4];
                    vmuTime.second = this.RxBuf[5];
                    vmuDate.year = this.RxBuf[6] + 2000;
                    vmuDate.month = this.RxBuf[7];
                    vmuDate.day = this.RxBuf[8];
                    vmuDate.dow = this.RxBuf[9];
                }
            }

            return status;
        }
        public COMMS_STATUS getVMUCountData(SerialPort port, ref CountsStruct countsStruct)
        {
            byte[] msg = new byte[1];
            msg[0] = (byte)'E';
            //COMMS_STATUS status;

            for (int retry = 0; retry < 3; retry++)
            {
                status = sendAndReceiveMsg(port, msg, 1, this.RxBuf, ref this.RxLen);
                if (status == COMMS_STATUS.OKAY) { break; }
                if (status == COMMS_STATUS.PORT_ERROR) { break; }
                Thread.Sleep(15);
            }
            if (status == COMMS_STATUS.OKAY)
            {
                if (this.RxBuf[2] == 'E')
                {
                    countsStruct.currentRPM = (UInt16)(this.RxBuf[3] * 256 + this.RxBuf[4]);
                    countsStruct.currentSpeed = (UInt16)(this.RxBuf[5] * 256 + this.RxBuf[6]);
                    countsStruct.currentDistance = (UInt16)(this.RxBuf[7] * 256 + this.RxBuf[8]);
                    countsStruct.RPMCount = (UInt16)(this.RxBuf[9] * 256 + this.RxBuf[10]);
                    countsStruct.speedCount = (UInt16)(this.RxBuf[11] * 256 + this.RxBuf[12]);
                    countsStruct.totalTime = (UInt16)(this.RxBuf[13] * 256 + this.RxBuf[14]);
                }
            }

            return status;
        }
        public COMMS_STATUS getVMUCurrentTripStruct(SerialPort port, ref TripDataStruct tripStruct)
        {
            byte[] msg = new byte[1];
            msg[0] = (byte)'K';
            //COMMS_STATUS status;

            for (int retry = 0; retry < 3; retry++)
            {
                status = sendAndReceiveMsg(port, msg, 1, this.RxBuf, ref this.RxLen);
                if (status == COMMS_STATUS.OKAY) { break; }
                if (status == COMMS_STATUS.PORT_ERROR) { break; }
                Thread.Sleep(15);
            }
            if (status == COMMS_STATUS.OKAY)
            {
                if (this.RxBuf[2] == 'K')
                {
                    decode_Trip_Data(this.RxBuf, 3, ref tripStruct);
                    /*
                    tripStruct.header = this.RxBuf[3];
                    tripStruct.maxSpeed = this.RxBuf[4];
                    tripStruct.serialNumber = BitConverter.ToUInt16(this.RxBuf, 5);
                    tripStruct.driverID = Encoding.ASCII.GetString(this.RxBuf, 7, 13);
                    tripStruct.vehicleID = Encoding.ASCII.GetString(this.RxBuf, 20, 11);
                    tripStruct.jobNumber = Encoding.ASCII.GetString(this.RxBuf, 31, 12);
                    tripStruct.startDate.year = (int)(BitConverter.ToInt16(this.RxBuf, 43));
                    tripStruct.startDate.month = this.RxBuf[45];
                    tripStruct.startDate.day = this.RxBuf[46];
                    tripStruct.endDate.year = (int)(BitConverter.ToInt16(this.RxBuf, 47));
                    tripStruct.endDate.month = this.RxBuf[49];
                    tripStruct.endDate.day = this.RxBuf[50];
                    tripStruct.startTime.hour = this.RxBuf[51];
                    tripStruct.startTime.minute = this.RxBuf[52];
                    tripStruct.startTime.second = this.RxBuf[53];
                    tripStruct.endTime.hour = this.RxBuf[54];
                    tripStruct.endTime.minute = this.RxBuf[55];
                    tripStruct.endTime.second = this.RxBuf[56];
                    tripStruct.startOdo = BitConverter.ToUInt32(this.RxBuf, 57);
                    tripStruct.endOdo = BitConverter.ToUInt32(this.RxBuf, 61);
                    tripStruct.maxRPM = BitConverter.ToUInt16(this.RxBuf, 65);
                    tripStruct.idleTime.hour = this.RxBuf[67];
                    tripStruct.idleTime.minute = this.RxBuf[68];
                    tripStruct.idleTime.second = this.RxBuf[69];
                    tripStruct.greenTime.hour = this.RxBuf[70];
                    tripStruct.greenTime.minute = this.RxBuf[71];
                    tripStruct.greenTime.second = this.RxBuf[72];
                    tripStruct.speedingTime.hour = this.RxBuf[73];
                    tripStruct.speedingTime.minute = this.RxBuf[74];
                    tripStruct.speedingTime.second = this.RxBuf[75];
                    tripStruct.overRPMTime.hour = this.RxBuf[76];
                    tripStruct.overRPMTime.minute = this.RxBuf[77];
                    tripStruct.overRPMTime.second = this.RxBuf[78];
                    tripStruct.fuel = BitConverter.ToUInt16(this.RxBuf, 79);
                    tripStruct.fuelPrice = BitConverter.ToUInt32(this.RxBuf, 81);
                    tripStruct.timerCnt = BitConverter.ToUInt16(this.RxBuf, 85);
                    tripStruct.eventCnt = BitConverter.ToUInt16(this.RxBuf, 87);
                    tripStruct.harshAccel = this.RxBuf[89];
                    tripStruct.harshBrake = this.RxBuf[90];
                    tripStruct.gpsLat = (((UInt64)(BitConverter.ToUInt32(this.RxBuf, 91)*90))/0x7FFFFFFF);
                    tripStruct.gpsLong = (((UInt64)(BitConverter.ToUInt32(this.RxBuf, 95)*180))/0x7FFFFFFF);
                    tripStruct.mode = this.RxBuf[99];
                    Array.Copy(this.RxBuf, 100, tripStruct.spare, 0, 3);
                    tripStruct.crc = BitConverter.ToUInt16(this.RxBuf, 103);
                    tripStruct.status = this.RxBuf[105];
                    tripStruct.endOfRec = this.RxBuf[106];
                    */
                }
            }

            return status;
        }
        public COMMS_STATUS getVMU_RSTDO_data(SerialPort port, ref RSTDO_Struct rstdo)
        {
            byte[] msg = new byte[1];
            msg[0] = (byte)'O';
            //COMMS_STATUS status;

            for (int retry = 0; retry < 3; retry++)
            {
                status = sendAndReceiveMsg(port, msg, 1, this.RxBuf, ref this.RxLen);
                if (status == COMMS_STATUS.OKAY) { break; }
                if (status == COMMS_STATUS.PORT_ERROR) { break; }
                Thread.Sleep(15);
            }
            if (status == COMMS_STATUS.OKAY)
            {
                if (this.RxBuf[2] == 'O')
                {
                    rstdo.currentRPM = (UInt16)(this.RxBuf[3] * 256 + this.RxBuf[4]);
                    rstdo.currentSpeed = (UInt16)(this.RxBuf[5] * 256 + this.RxBuf[6]);
                    rstdo.currentDistance = (UInt16)(this.RxBuf[7] * 256 + this.RxBuf[8]);
                    rstdo.oddoPlusCurDist = (UInt32)((this.RxBuf[9] << 24) | (this.RxBuf[10] << 16) | (this.RxBuf[11] << 8) | this.RxBuf[12]);
                    rstdo.totalTime = (UInt16)(this.RxBuf[13] * 256 + this.RxBuf[14]);
                    rstdo.currentDate.year = this.RxBuf[15] + 2000;
                    rstdo.currentDate.month = this.RxBuf[16];
                    rstdo.currentDate.day = this.RxBuf[17];
                    rstdo.currentTime.hour = this.RxBuf[18];
                    rstdo.currentTime.minute = this.RxBuf[19];
                    rstdo.currentTime.second = this.RxBuf[20];
                }
            }

            return status;
        }
        public COMMS_STATUS getVMUAltSpeed(SerialPort port, byte[] rxBuf, ref int rxLen)
        {
            byte[] msg = new byte[1];
            msg[0] = (byte)'g';

            return sendAndReceiveMsg(port, msg, 1, rxBuf, ref rxLen);
        }
        public COMMS_STATUS getVMUPrompt(int promptNumber, SerialPort port, byte[] rxBuf, ref int rxLen)
        {
            byte[] msg = new byte[2];
            msg[0] = (byte)'u';
            msg[1] = (byte)promptNumber;

            return sendAndReceiveMsg(port, msg, 2, rxBuf, ref rxLen);
        }
        public COMMS_STATUS getVMUFlashConfig(SerialPort port, int blockNum, ref flashHeaderStruct block)
        {
            byte[] msg = new byte[3];
            msg[0] = 0xB1;
            msg[1] = (byte)'C';
            msg[2] = (byte)blockNum;

            for (int retry = 0; retry < 3; retry++)
            {
                this.status = sendAndReceiveMsg(port, msg, 3, this.RxBuf, ref this.RxLen);
                if (this.status == COMMS_STATUS.OKAY) { break; }
                if (this.status == COMMS_STATUS.PORT_ERROR) { break; }
                Thread.Sleep(100);
            }
            if (this.status == COMMS_STATUS.OKAY)
            {
                if ((this.RxBuf[2] == 0xB1) && (this.RxBuf[3] == 'C') && (this.RxBuf[4] == blockNum))
                {
                    block.blockNum = blockNum;
                    block.blockType = this.RxBuf[5];
                    block.headSize = this.RxBuf[6];
                    block.eraseCnt = BitConverter.ToUInt32(this.RxBuf, 7);
                    block.recordSize = BitConverter.ToInt16(this.RxBuf, 11);
                    block.seqNo = BitConverter.ToUInt32(this.RxBuf, 13);
                    if (block.seqNo == 0xFFFFFFFF) block.seqNo = 0;
                    block.lastEraseDate.year = BitConverter.ToUInt16(this.RxBuf, 17);
                    block.lastEraseDate.month = this.RxBuf[19];
                    block.lastEraseDate.day = this.RxBuf[20];
                    block.used = BitConverter.ToUInt16(this.RxBuf, 21);
                    block.saved = BitConverter.ToUInt16(this.RxBuf, 23);
                    block.lastEraseTime.hour = this.RxBuf[25];
                    block.lastEraseTime.minute = this.RxBuf[26];
                    block.lastEraseTime.second = this.RxBuf[27];
                    //Array.Copy(this.RxBuf, 28, block.spare, 0, 9);
                }
            }

            return this.status;
        }
        public COMMS_STATUS getVMUFlashData(SerialPort port, int blockNum, int address, int numToRead, byte[] rxData)
        {
            byte[] msg = new byte[6];
            msg[0] = 0xB1;
            msg[1] = (byte)'R';
            msg[2] = (byte)blockNum;
            msg[3] = (byte)(address & 0xFF);
            msg[4] = (byte)((address >> 8) & 0xFF);
            msg[5] = (byte)numToRead;

            for (int retry = 0; retry < 3; retry++)
            {
                this.status = sendAndReceiveMsg(port, msg, 6, this.RxBuf, ref this.RxLen);
                if (this.status == COMMS_STATUS.OKAY) { break; }
                if (this.status == COMMS_STATUS.PORT_ERROR) { break; }
                Thread.Sleep(15);
            }
            if (this.status == COMMS_STATUS.OKAY)
            {
                if ((this.RxBuf[2] == 0xB1) && (this.RxBuf[3] == 'R'))
                {
                    Array.Copy(this.RxBuf, 4, rxData, 0, numToRead);
                }
            }

            return status;
        }
        public COMMS_STATUS getGPSConfig(SerialPort port, ref gpsConfig gc)
        {
            byte[] msg = new byte[2];
            msg[0] = 0xd1;
            msg[1] = (byte)'R';
            //COMMS_STATUS status;

            for (int retry = 0; retry < 3; retry++)
            {
                status = sendAndReceiveMsg(port, msg, 2, this.RxBuf, ref this.RxLen);
                if (status == COMMS_STATUS.OKAY) { break; }
                if (status == COMMS_STATUS.PORT_ERROR) { break; }
                Thread.Sleep(15);
            }

            if (status == COMMS_STATUS.OKAY)
            {
                if ((this.RxBuf[2] == 0xd1) && (this.RxBuf[3] == 'R'))
                {
                    gc.logInterval = ((this.RxBuf[4] << 8) | this.RxBuf[5]);
                    gc.timeDiff = this.RxBuf[6];
                    gc.gpsOptions = this.RxBuf[7];
                }
            }

            return status;
        }
        public COMMS_STATUS turnONOutputs(SerialPort port)
        {
            byte[] msg = new byte[3];
            msg[0] = 0xFF;
            msg[1] = (byte)'O';
            msg[2] = 0x00;

            for (int retry = 0; retry < 3; retry++)
            {
                status = sendAndReceiveMsg(port, msg, 3, this.RxBuf, ref this.RxLen);
                if (status == COMMS_STATUS.OKAY) { break; }
                if (status == COMMS_STATUS.PORT_ERROR) { break; }
                Thread.Sleep(15);
            }
            if (status == COMMS_STATUS.OKAY)
            {

            }

            return status;
        }
        public COMMS_STATUS turnOFFOutputs(SerialPort port)
        {
            byte[] msg = new byte[3];
            msg[0] = 0xFF;
            msg[1] = (byte)'O';
            msg[2] = 0x0F;

            for (int retry = 0; retry < 3; retry++)
            {
                status = sendAndReceiveMsg(port, msg, 3, this.RxBuf, ref this.RxLen);
                if (status == COMMS_STATUS.OKAY) { break; }
                if (status == COMMS_STATUS.PORT_ERROR) { break; }
                Thread.Sleep(15);
            }
            if (status == COMMS_STATUS.OKAY)
            {

            }
            return status;
        }
        public COMMS_STATUS setGPSConfig(SerialPort port, gpsConfig gc)
        {
            byte[] msg = new byte[6];
            msg[0] = 0xd1;
            msg[1] = (byte)'W';
            msg[2] = (byte)(gc.logInterval / 256);
            msg[3] = (byte)(gc.logInterval % 256);
            msg[4] = (byte)(gc.timeDiff);
            msg[5] = 0x01;          // select zero speed log B0=1

            for (int retry = 0; retry < 3; retry++)
            {
                status = sendAndReceiveMsg(port, msg, 6, this.RxBuf, ref this.RxLen);
                if (status == COMMS_STATUS.OKAY) { break; }
                if (status == COMMS_STATUS.PORT_ERROR) { break; }
                Thread.Sleep(15);
            }

            if (status == COMMS_STATUS.OKAY)
            {
                if ((this.RxBuf[2] == 0xd1) && (this.RxBuf[3] == 'W'))
                {
                    return COMMS_STATUS.OKAY;
                }
                else
                {
                    return COMMS_STATUS.COMMAND_ERROR;
                }
            }

            return status;
        }
        public COMMS_STATUS setVMUCalibration(SerialPort port, profileStruct profile)
        {
            byte[] msg = new byte[5];
            msg[0] = (byte)'C';
            msg[1] = (byte)(profile.calRPM / 256);
            msg[2] = (byte)(profile.calRPM % 256);
            msg[3] = (byte)(profile.calDist / 256);
            msg[4] = (byte)(profile.calDist % 256);

            this.status = sendMsg(port, msg, 5);
            Thread.Sleep(25);
            return this.status;
        }
        public COMMS_STATUS setVMULimits(SerialPort port, profileStruct profile)
        {
            byte[] msg = new byte[10];
            msg[0] = (byte)'F';
            msg[1] = (byte)(profile.maxRPM / 256);
            msg[2] = (byte)(profile.maxRPM % 256);
            msg[3] = (byte)(profile.maxSpeed / 256);
            msg[4] = (byte)(profile.maxSpeed % 256);
            msg[5] = (byte)(profile.maxBrake / 256);
            msg[6] = (byte)(profile.maxBrake % 256);
            msg[7] = (byte)(profile.maxAccel / 256);
            msg[8] = (byte)(profile.maxAccel % 256);
            msg[9] = profile.bell;

            this.status = sendMsg(port, msg, 10);
            Thread.Sleep(25);
            return this.status;
        }
        public COMMS_STATUS getVMUConstants(SerialPort port, ref VMUConstStruct vmuStruct)
        {
            byte[] msg = new byte[1];
            msg[0] = (byte)'I';
            //COMMS_STATUS status;

            for (int retry = 0; retry < 3; retry++)
            {
                status = sendAndReceiveMsg(port, msg, 1, this.RxBuf, ref this.RxLen);
                if (status == COMMS_STATUS.OKAY) { break; }
                if (status == COMMS_STATUS.PORT_ERROR) { break; }
                Thread.Sleep(15);
            }

            if (status == COMMS_STATUS.OKAY)
            {
                if (this.RxBuf[2] == 'I')
                {
                    vmuStruct.ID = Encoding.ASCII.GetString(this.RxBuf, 3, 11);
                    vmuStruct.oddo = (UInt32)((this.RxBuf[14] << 24) | (this.RxBuf[15] << 16) | (this.RxBuf[16] << 8) | this.RxBuf[17]);
                    vmuStruct.type = (UInt16)(this.RxBuf[18] * 256 + this.RxBuf[19]);
                    vmuStruct.tripNumber = (UInt16)(this.RxBuf[20] * 256 + this.RxBuf[21]);
                }
            }

            return status;
        }
        public COMMS_STATUS setVMUIdleLimit(SerialPort port, profileStruct profile)
        {
            byte[] msg = new byte[2];
            msg[0] = (byte)'V';
            msg[1] = profile.IdleLimit;

            this.status = sendAndReceiveMsg(port, msg, 2, this.RxBuf, ref this.RxLen);
            if (status == COMMS_STATUS.OKAY)
            {
                if (this.RxBuf[2] != 'V') this.status = COMMS_STATUS.COMMAND_ERROR;
            }
            return this.status;
        }
        public COMMS_STATUS getVMUIdleTimeLimit(SerialPort port, ref VMUConstStruct vmuStruct)
        {
            byte[] msg = new byte[1];
            msg[0] = (byte)'v';
            //COMMS_STATUS status;

            for (int retry = 0; retry < 3; retry++)
            {
                status = sendAndReceiveMsg(port, msg, 1, this.RxBuf, ref this.RxLen);
                if (status == COMMS_STATUS.OKAY) { break; }
                if (status == COMMS_STATUS.PORT_ERROR) { break; }
                Thread.Sleep(15);
            }

            if (status == COMMS_STATUS.OKAY)
            {
                if (this.RxBuf[2] == 'v')
                {
                    vmuStruct.idleTimeLimit = this.RxBuf[3];
                }
            }

            return status;
        }

        public COMMS_STATUS setVMUConstatnts(SerialPort port, profileStruct profile)
        {
            byte[] temp = Encoding.ASCII.GetBytes(profile.vehicleID);
            UInt32 odo = (UInt32)(profile.odometer * 10);

            byte[] msg = new byte[20];
            msg[0] = (byte)'H';
            Array.Copy(temp, 0, msg, 1, (temp.Length > 11) ? 11 : temp.Length);
            msg[12] = (byte)((odo >> 24) & 0xFF);
            msg[13] = (byte)((odo >> 16) & 0xFF);
            msg[14] = (byte)((odo >> 8) & 0xFF);
            msg[15] = (byte)(odo & 0xFF);
            msg[16] = (byte)(profile.type / 256);
            msg[17] = (byte)(profile.type % 256);
            msg[18] = (byte)(profile.tripNumber / 256);
            msg[19] = (byte)(profile.tripNumber % 256);

            this.status = sendMsg(port, msg, 20);
            Thread.Sleep(25);
            return this.status;
        }
        public COMMS_STATUS setVMUOptionalParms(SerialPort port, profileStruct profile)
        {
            byte[] msg = new byte[8];
            msg[0] = (byte)'q';
            msg[1] = (byte)(profile.greenBandUpper / 256);
            msg[2] = (byte)(profile.greenBandUpper % 256);
            msg[3] = (byte)(profile.greenBandLower / 256);
            msg[4] = (byte)(profile.greenBandLower % 256);
            msg[5] = profile.tripStartDist;
            msg[6] = profile.inputOptions;
            msg[7] = profile.promptOptions;

            this.status = sendMsg(port, msg, 8);
            Thread.Sleep(25);
            return this.status;
        }
        public COMMS_STATUS setVMUExtendedGroups(SerialPort port, profileStruct profile)
        {
            byte[] msg = new byte[6];
            msg[0] = (byte)'A';
            msg[1] = (byte)(profile.type2 / 256);
            msg[2] = (byte)(profile.type2 % 256);
            msg[3] = (byte)(profile.type3 / 256);
            msg[4] = (byte)(profile.type3 % 256);
            msg[5] = profile.options;

            this.status = sendMsg(port, msg, 6);
            Thread.Sleep(25);
            return this.status;
        }

        public COMMS_STATUS getVMUExtendedGroups(SerialPort port, ref VMUConstStruct vmuStruct)
        {
            byte[] msg = new byte[1];
            msg[0] = (byte)'a';
            //COMMS_STATUS status;

            for (int retry = 0; retry < 3; retry++)
            {
                status = sendAndReceiveMsg(port, msg, 1, this.RxBuf, ref this.RxLen);
                if (status == COMMS_STATUS.OKAY) { break; }
                if (status == COMMS_STATUS.PORT_ERROR) { break; }
                Thread.Sleep(15);
            }

            if (status == COMMS_STATUS.OKAY)
            {
                if (this.RxBuf[2] == 'a')
                {
                    vmuStruct.type2 = (UInt16)(this.RxBuf[3] * 256 + this.RxBuf[4]);
                    vmuStruct.type3 = (UInt16)(this.RxBuf[5] * 256 + this.RxBuf[6]);
                    if ((this.RxBuf[7] & 0x01) == 0x01) vmuStruct.options |= 0x01;
                    else vmuStruct.options &= 0xFE;
                }
            }

            return status;
        }
        public COMMS_STATUS getVMUOptionalParms(SerialPort port, ref VMUConstStruct vmuStruct)
        {
            byte[] msg = new byte[1];
            msg[0] = (byte)'Q';
            //COMMS_STATUS status;

            for (int retry = 0; retry < 3; retry++)
            {
                status = sendAndReceiveMsg(port, msg, 1, this.RxBuf, ref this.RxLen);
                if (status == COMMS_STATUS.OKAY) { break; }
                if (status == COMMS_STATUS.PORT_ERROR) { break; }
                Thread.Sleep(15);
            }
            if (status == COMMS_STATUS.OKAY)
            {
                if (this.RxBuf[2] == 'Q')
                {
                    vmuStruct.greenBandUpper = (Int16)(this.RxBuf[3] * 256 + this.RxBuf[4]);
                    vmuStruct.greenBandLower = (Int16)(this.RxBuf[5] * 256 + this.RxBuf[6]);
                    vmuStruct.tripStartDistance = this.RxBuf[7];
                    vmuStruct.inputOptions = this.RxBuf[8];
                    vmuStruct.promptOptions = this.RxBuf[9];
                }
            }

            return status;
        }
        public COMMS_STATUS getVMUBypassStatus(SerialPort port, ref VMUConstStruct vmuStruct)
        {
            byte[] msg = new byte[1];
            msg[0] = (byte)'b';
            //COMMS_STATUS status;

            for (int retry = 0; retry < 3; retry++)
            {
                status = sendAndReceiveMsg(port, msg, 1, this.RxBuf, ref this.RxLen);
                if (status == COMMS_STATUS.OKAY) { break; }
                if (status == COMMS_STATUS.PORT_ERROR) { break; }
                Thread.Sleep(15);
            }
            if (status == COMMS_STATUS.OKAY)
            {
                if (this.RxBuf[2] == 'b')
                {
                    vmuStruct.bypass = this.RxBuf[3];
                }
            }

            return status;
        }
        public byte getVMUFirstRecord(SerialPort port, byte[] buf)
        {
            byte[] msg = new byte[1];
            msg[0] = (byte)'L';

            this.status = sendAndReceiveMsg(port, msg, 1, this.RxBuf, ref this.RxLen);
            if (this.status == COMMS_STATUS.OKAY)
            {
                if (this.RxBuf[2] == 'L')
                {
                    Array.Copy(this.RxBuf, 0, buf, 0, this.RxLen);
                    return this.RxBuf[3];
                }
                else { return 0; }
            }

            return 0xFF;
        }
        public byte getVMUCurrentRecord(SerialPort port, byte[] buf)
        {
            byte[] msg = new byte[1];
            msg[0] = (byte)'K';

            this.status = sendAndReceiveMsg(port, msg, 1, this.RxBuf, ref this.RxLen);
            if (this.status == COMMS_STATUS.OKAY)
            {
                if (this.RxBuf[2] == 'K')
                {
                    Array.Copy(this.RxBuf, 0, buf, 0, this.RxLen);
                    return this.RxBuf[3];
                }
                else { return 0; }
            }

            return 0xFF;
        }
        public byte getVMUNextRecord(SerialPort port, byte[] buf)
        {
            byte[] msg = new byte[1];
            msg[0] = (byte)'M';

            this.status = sendAndReceiveMsg(port, msg, 1, this.RxBuf, ref this.RxLen);
            if (this.status == COMMS_STATUS.OKAY)
            {
                if (this.RxBuf[2] == 'L')
                {
                    Array.Copy(this.RxBuf, 0, buf, 0, this.RxLen);
                    return this.RxBuf[3];
                }
                else { return 0; }
            }

            return 0xFF;
        }
    }
}
