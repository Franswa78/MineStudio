using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.IO.Ports;
using System.Threading;
using MineStudio.Properties;
using System.Net;
using System.Net.Sockets;

namespace MineStudio
{
    public class CardCmds
    {
        private SerialPort _currentPort = new SerialPort();
        private string _currentPortName = "COM1";
        private int _currentBaudrate = 9600;
        private int _maxBaud = 115200;
        private int _timeOut = 2000;
        private int CardType = 0;
        public const int BULL_START = 0x260;
        public const int BULL_END = 0x4140;
        char[] Hex = { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', 'A', 'B', 'C', 'D', 'E', 'F' };

        //#define UPDC16(ch, crc) (crc_16_tab[((crc) ^ (ch)) & 0xff] ^ ((crc) >> 8))
        /* CRC polynomial 0xA001 */
        private UInt16[] crc_16_tab =
        {
            0x0000, 0xC0C1, 0xC181, 0x0140, 0xC301, 0x03C0, 0x0280, 0xC241,
            0xC601, 0x06C0, 0x0780, 0xC741, 0x0500, 0xC5C1, 0xC481, 0x0440,
            0xCC01, 0x0CC0, 0x0D80, 0xCD41, 0x0F00, 0xCFC1, 0xCE81, 0x0E40,
            0x0A00, 0xCAC1, 0xCB81, 0x0B40, 0xC901, 0x09C0, 0x0880, 0xC841,
            0xD801, 0x18C0, 0x1980, 0xD941, 0x1B00, 0xDBC1, 0xDA81, 0x1A40,
            0x1E00, 0xDEC1, 0xDF81, 0x1F40, 0xDD01, 0x1DC0, 0x1C80, 0xDC41,
            0x1400, 0xD4C1, 0xD581, 0x1540, 0xD701, 0x17C0, 0x1680, 0xD641,
            0xD201, 0x12C0, 0x1380, 0xD341, 0x1100, 0xD1C1, 0xD081, 0x1040,
            0xF001, 0x30C0, 0x3180, 0xF141, 0x3300, 0xF3C1, 0xF281, 0x3240,
            0x3600, 0xF6C1, 0xF781, 0x3740, 0xF501, 0x35C0, 0x3480, 0xF441,
            0x3C00, 0xFCC1, 0xFD81, 0x3D40, 0xFF01, 0x3FC0, 0x3E80, 0xFE41,
            0xFA01, 0x3AC0, 0x3B80, 0xFB41, 0x3900, 0xF9C1, 0xF881, 0x3840,
            0x2800, 0xE8C1, 0xE981, 0x2940, 0xEB01, 0x2BC0, 0x2A80, 0xEA41,
            0xEE01, 0x2EC0, 0x2F80, 0xEF41, 0x2D00, 0xEDC1, 0xEC81, 0x2C40,
            0xE401, 0x24C0, 0x2580, 0xE541, 0x2700, 0xE7C1, 0xE681, 0x2640,
            0x2200, 0xE2C1, 0xE381, 0x2340, 0xE101, 0x21C0, 0x2080, 0xE041,
            0xA001, 0x60C0, 0x6180, 0xA141, 0x6300, 0xA3C1, 0xA281, 0x6240,
            0x6600, 0xA6C1, 0xA781, 0x6740, 0xA501, 0x65C0, 0x6480, 0xA441,
            0x6C00, 0xACC1, 0xAD81, 0x6D40, 0xAF01, 0x6FC0, 0x6E80, 0xAE41,
            0xAA01, 0x6AC0, 0x6B80, 0xAB41, 0x6900, 0xA9C1, 0xA881, 0x6840,
            0x7800, 0xB8C1, 0xB981, 0x7940, 0xBB01, 0x7BC0, 0x7A80, 0xBA41,
            0xBE01, 0x7EC0, 0x7F80, 0xBF41, 0x7D00, 0xBDC1, 0xBC81, 0x7C40,
            0xB401, 0x74C0, 0x7580, 0xB541, 0x7700, 0xB7C1, 0xB681, 0x7640,
            0x7200, 0xB2C1, 0xB381, 0x7340, 0xB101, 0x71C0, 0x7080, 0xB041,
            0x5000, 0x90C1, 0x9181, 0x5140, 0x9301, 0x53C0, 0x5280, 0x9241,
            0x9601, 0x56C0, 0x5780, 0x9741, 0x5500, 0x95C1, 0x9481, 0x5440,
            0x9C01, 0x5CC0, 0x5D80, 0x9D41, 0x5F00, 0x9FC1, 0x9E81, 0x5E40,
            0x5A00, 0x9AC1, 0x9B81, 0x5B40, 0x9901, 0x59C0, 0x5880, 0x9841,
            0x8801, 0x48C0, 0x4980, 0x8941, 0x4B00, 0x8BC1, 0x8A81, 0x4A40,
            0x4E00, 0x8EC1, 0x8F81, 0x4F40, 0x8D01, 0x4DC0, 0x4C80, 0x8C41,
            0x4400, 0x84C1, 0x8581, 0x4540, 0x8701, 0x47C0, 0x4680, 0x8641,
            0x8201, 0x42C0, 0x4380, 0x8341, 0x4100, 0x81C1, 0x8081, 0x4040,
        };

        private Thread readCardThread;

        public CardCmds()
        {
            this._currentPortName = Settings.Default.cardReaderPort;
            if (int.TryParse(Settings.Default.cardReaderBaud, out this._currentBaudrate) == false)
            {
                this._currentBaudrate = 9600;
            }

        }

        public string PortName
        {
            get { return this._currentPortName; }
            set
            {
                this._currentPortName = value;
                if (this._currentPortName != Settings.Default.cardReaderPort)
                {
                    Settings.Default.cardReaderPort = this._currentPortName;
                    Settings.Default.Save();
                }

            }
        }
        public int Baudrate
        {
            get { return this._currentBaudrate; }
            set
            {
                this._currentBaudrate = value;
                if (this._currentBaudrate.ToString() != Settings.Default.cardReaderBaud)
                {
                    Settings.Default.cardReaderBaud = this._currentBaudrate.ToString();
                    Settings.Default.Save();
                }

            }
        }
        public int TimeOut
        {
            get { return this._timeOut; }
            set { this._timeOut = value; }
        }

        public ErrState openPort()
        {
            try
            {
                if (this._currentPort.IsOpen) this._currentPort.Close();
                this._currentPort.PortName = this._currentPortName;
                this._currentPort.BaudRate = this._currentBaudrate;
                this._currentPort.DataBits = 8;
                this._currentPort.Parity = Parity.None;
                this._currentPort.StopBits = StopBits.One;
                this._currentPort.Open();
                return ErrState.ERR_NO_ERROR;
            }
            catch (Exception e)
            {
                return ErrState.ERR_COMPORT_OPEN;
            }
        }
        public ErrState closePort()
        {
            try
            {
                if (this._currentPort.IsOpen) this._currentPort.Close();
                return ErrState.ERR_NO_ERROR;
            }
            catch (Exception)
            {
                return ErrState.ERR_COMPORT_OPEN;
            }
        }
        public ErrState sendMsg(byte[] txStr, byte noToSend, byte[] rxStr, ref int rxLen)
        {
            byte[] buf = new byte[256];
            byte[] outBuf = new byte[256];
            int i = 0, x, n, crc;
            DateTime startTime;
            TimeSpan tspan = new TimeSpan();
            int rxTempCount = 0;
            int bytesRead = 0;
            int msgLen = 0;

            crc = noToSend;
            outBuf[0] = 0xF0;
            outBuf[1] = noToSend;
            for (i = 0, x = 2; i < noToSend; i++, x++)
            {
                outBuf[x] = txStr[i];
                crc = crc ^ txStr[i];
            }
            outBuf[x++] = (byte)crc;
            outBuf[x++] = 0xF2;

            try
            {
                if (this._currentPort.IsOpen == false) return ErrState.ERR_COMPORT_OPEN;
                this._currentPort.Write(outBuf, 0, x);
                startTime = DateTime.Now;
                do
                {
                    rxTempCount = this._currentPort.BytesToRead;
                    if ((rxTempCount > 0) && (rxTempCount < 256))
                    {
                        this._currentPort.Read(buf, bytesRead, rxTempCount);
                        bytesRead += rxTempCount;

                        if (bytesRead > 2)
                        {
                            msgLen = buf[1] + 4;
                            if (bytesRead >= msgLen) break;
                        }

                    }
                    tspan = DateTime.Now - startTime;
                }
                while (tspan.TotalMilliseconds <= this._timeOut);

                if (tspan.TotalMilliseconds > this._timeOut) return ErrState.ERR_TIMEOUT;

                if (bytesRead >= 252) return ErrState.ERR_MSGLEN_OVERFLOW;

                crc = buf[1];
                for (x = 2; x < (buf[1] + 3); x++) { crc = crc ^ buf[x]; }
                if (crc == 0)
                {
                    for (x = 2; x < (buf[1] + 2); x++) rxStr[x - 2] = buf[x];
                    rxLen = buf[1];
                    return ErrState.ERR_NO_ERROR;
                }
                else return ErrState.ERR_CRC;

            }
            catch (Exception e)
            {
                return ErrState.MAX_ERRORS;
            }

        }
        public int setBaud(int speed)
        {
            byte[] str = new byte[8];
            byte[] resp = new byte[256];
            int ns;
            int rxLen = 0;

            // <0xc1><'A'><Speed>   Speed 0 = 9600, 1=19200,38400,57600,115200
            str[0] = 0xc1;
            str[1] = (byte)'A';
            ns = speed;

            switch (speed)
            {
                case 9600:
                    { str[2] = 0; break; }
                case 19200:
                    { str[2] = 1; break; }
                case 38400:
                    { str[2] = 2; break; }
                case 57600:
                    { str[2] = 3; break; }
                case 115200:
                    { str[2] = 4; break; }
                default:
                    { str[2] = 0; ns = 9600; break; }
            }
            this._timeOut = 1000;
            ErrState state = sendMsg(str, 3, resp, ref rxLen);
            if (state != ErrState.ERR_NO_ERROR) return 9600;

            try
            {
                this._currentPort.Close();
                Thread.Sleep(20);
                Baudrate = ns;
                this._currentPort.Open();
                Array.Clear(resp, 0, resp.Length);
                state = sendMsg(str, 3, resp, ref rxLen);
                if (state != ErrState.ERR_NO_ERROR) return 9600;
                if ((rxLen > 0) && (resp[0] == 0xC1) && (resp[1] == 'A') && (resp[2] == str[2]))
                {
                    return ns;
                }
                else
                {
                    Baudrate = 9600;
                    return 9600;
                }
            }
            catch (Exception)
            {
                return 9600;
            }
        }
        public int buzzer(int onOff)
        {
            byte[] buf = new byte[256];
            int rxLen = 0;

            if (onOff > 0) buf[0] = 0x81;  // Buzzer on
            else buf[0] = 0x80;           // Buzzer off
            ErrState state = sendMsg(buf, 1, buf, ref rxLen);
            if ((state != ErrState.ERR_NO_ERROR) || (rxLen == 0)) { return 1; }
            return 0;
        }
        public ErrState getCardType(byte[] snr, ref byte type)
        {
            byte[] buf = new byte[256];
            int rxLen = 0;

            buf[0] = 0xFF;
            buf[1] = (byte)'N';   // read button type
            ErrState state = sendMsg(buf, 2, buf, ref rxLen);
            if ((state == ErrState.ERR_NO_ERROR) && (rxLen > 0))
            {
                if ((buf[0] == 0xFF) && (buf[1] == 'N') && (buf[2] > 0))
                {
#if true
                    snr[0] = buf[8];
                    snr[1] = buf[7];
                    snr[2] = buf[6];
                    snr[3] = buf[5];
                    snr[4] = buf[4];
                    snr[5] = buf[3];
#else
                        snr[5] = buf[8];
                        snr[4] = buf[7];
                        snr[3] = buf[6];
                        snr[2] = buf[5];
                        snr[1] = buf[4];
                        snr[0] = buf[3];

#endif
                    type = buf[2];
                    return ErrState.ERR_NO_ERROR;
                }
            }
            type = 0;
            return state;
        }
        public ErrState readCardData(uint pos, ushort size, byte[] ptr, ref int numBytesRead)
        {
            uint start, end, t;
            int nrBytes;
            ushort len = 0, index = 0;
            byte[] txbuf = new byte[256];
            byte[] rxbuf = new byte[256];
            int rxLen = 0;
            ErrState state;

            /* first turn the card reader on */
            txbuf[0] = 0x01;
            state = sendMsg(txbuf, 1, rxbuf, ref rxLen);
            if (state != ErrState.ERR_NO_ERROR) { numBytesRead = 0; return state; }
            if ((rxLen == 0) || (rxbuf[0] == 0x10)) return ErrState.MAX_ERRORS;   /* if timeout or no response return */


#if true
            /* calculate the start position to read from */
            start = (pos & 0xfffc);                /* bull works in multiples of 4 */
            //  index = start - pos;
            end = pos + size;                      /* start and end are on 4 byte boundries */
            for (; (end & 0x03) > 0; end++) ;
            nrBytes = (int)(end - start);
            /*if nrBytes is more than 64, read in blocks of data 64 bytes at a time*/
            for (; nrBytes > 0;)
            {
                txbuf[1] = (byte)(((start * 2) + BULL_START) / 256);
                txbuf[2] = (byte)(((start * 2) + BULL_START) % 256);
                if (nrBytes > 64)
                {
                    txbuf[0] = 0x10 | ((64 / 4) - 1);
                    nrBytes = nrBytes - 64;
                    start = start + 64;
                }
                else
                {
                    txbuf[0] = (byte)(0x10 | ((nrBytes / 4) - 1));
                    nrBytes = 0;
                }

                state = sendMsg(txbuf, 3, rxbuf, ref rxLen);
                if (state != ErrState.ERR_NO_ERROR) { numBytesRead = 0; return state; }
                len = (ushort)rxLen;

                if ((rxbuf[0] < 4) && (rxLen > 0))      /* bytes in buf[3] onwards */    // &&&& 2>0
                {
                    //for(t=0;(t<(buf[1]-1))&&(index<size);t++)
                    for (t = 0; t < (len - 1); t++)
                    {
                        if (index >= 0)                        /* only store if index >= 0 */
                        {
                            ptr[index] = rxbuf[t + 1];                               // &&&& 3>1
                        }
                        index++;
                    }
                }
                else { numBytesRead = 0; return ErrState.MAX_ERRORS; }
            } // end for nrBytes


#else
            

#endif
            txbuf[0] = 0x0;           // turn off card reader
            state = sendMsg(txbuf, 1, rxbuf, ref rxLen);
            numBytesRead = index;
            return ErrState.ERR_NO_ERROR;
        }
        public ErrState readDalPage(UInt16 page, byte[] ptr, ref int numBytesRead)
        {
            byte[] txbuf = new byte[256];
            byte[] rxbuf = new byte[256];
            int rxLen = 0;
            ErrState state;
            UInt32 start, end, t;
            int nrBytes;
            UInt16 index = 0;
            int retryCnt = 0;

            /* first turn the card reader on */
            txbuf[0] = 0x01;
            state = sendMsg(txbuf, 1, rxbuf, ref rxLen);
            if (state != ErrState.ERR_NO_ERROR) { numBytesRead = 0; return state; }
            if ((rxLen == 0) || (rxbuf[0] == 0x10)) { numBytesRead = 0; return ErrState.MAX_ERRORS; }  /* if timeout or no response return */

            // new read commands
            start = (UInt32)(page * 32);            /* calculate the start position to read from */
            nrBytes = 32;
            index = 0;

            for (; nrBytes > 0;)
            {
                txbuf[0] = 0xFF;
                txbuf[1] = (byte)'R';
                Array.Copy(BitConverter.GetBytes(start), 0, txbuf, 2, 4);
                if (nrBytes > 128)
                {
                    txbuf[6] = 128;
                    txbuf[7] = 0;
                    nrBytes = nrBytes - 128;
                    start = start + 128;
                }
                else
                {
                    txbuf[6] = (byte)nrBytes;
                    txbuf[7] = 0x00;
                    nrBytes = 0;
                }


                for (retryCnt = 0; retryCnt < 3; retryCnt++)
                {
                    state = sendMsg(txbuf, 8, rxbuf, ref rxLen);
                    if ((state == ErrState.ERR_NO_ERROR) && (rxLen > 0) && (rxbuf[0] == 0xFF) && (rxbuf[1] == 'R'))
                    {
                        for (t = 0; (t < (rxLen - 2)); t++)
                        {
                            ptr[index] = rxbuf[t + 2];
                            index++;
                        }
                        break;
                    }

                }//end for retryCnt

                if (retryCnt >= 3) { numBytesRead = 0; return ErrState.MAX_ERRORS; }
            } // end for nrBytes

            txbuf[0] = 0x0;           // turn off card reader
            state = sendMsg(txbuf, 1, rxbuf, ref rxLen);
            numBytesRead = index;
            return ErrState.ERR_NO_ERROR;

        }
        private UInt16 UPDC16(byte ch, UInt16 crc)
        {
            return (UInt16)(crc_16_tab[((crc) ^ (ch)) & 0xff] ^ ((crc) >> 8));
        }
        private UInt16 dalCRC16Page(byte page, byte[] str, UInt16 length)
        {
            UInt16 crc16;
            UInt16 i;

            crc16 = page;

            for (i = 0; i < length; i++)
            {
                crc16 = UPDC16(str[i], crc16);
            }
            return crc16;
        }
        public ErrState writeDalPage(byte page, byte[] ptr)
        {
            UInt16 crc, index;
            UInt32 start, end, t;
            int nrBytes;
            byte[] txbuf = new byte[256];
            byte[] rxbuf = new byte[256];
            ErrState state;
            int rxLen = 0;
            int retryCnt = 0;

            crc = dalCRC16Page(page, ptr, 30);

            /* first turn the card reader on */
            txbuf[0] = 0x01;
            state = sendMsg(txbuf, 1, rxbuf, ref rxLen);
            if (state != ErrState.ERR_NO_ERROR) { return state; }
            if ((rxLen == 0) || (rxbuf[0] == 0x10)) return ErrState.MAX_ERRORS;   /* if timeout or no response return */

            // new write commands

            start = (UInt32)(page * 32);            /* calculate the start position to read from */
            nrBytes = 32;                           // crc takes 2 bytes!!!
            index = 0;

            for (retryCnt = 0; retryCnt < 3; retryCnt++)
            {
                txbuf[0] = 0xFF;
                txbuf[1] = (byte)'W';
                Array.Copy(BitConverter.GetBytes(start), 0, txbuf, 2, 4);
                txbuf[6] = 32;
                txbuf[7] = 0x00;
                Array.Copy(ptr, 0, txbuf, 8, 30);
                txbuf[38] = (byte)(crc / 256);
                txbuf[39] = (byte)(crc % 256);

                state = sendMsg(txbuf, 40, rxbuf, ref rxLen);
                if (state == ErrState.ERR_NO_ERROR)
                {
                    if ((rxLen > 0) && (rxbuf[0] == 0xFF) && (rxbuf[1] == 'W'))
                    {
                        break;
                    }
                }

            }

            txbuf[0] = 0x0;           // turn off card reader
            state = sendMsg(txbuf, 1, rxbuf, ref rxLen);

            if (retryCnt >= 3) return ErrState.MAX_ERRORS;
            else return ErrState.ERR_NO_ERROR;
        }
        public ErrState power(bool onOff, byte[] ptr) // off=false, on=true
        {
            byte[] buf = new byte[10];
            byte[] rxbuf = new byte[256];
            int rxLen = 0;
            ErrState state;

            if (onOff) buf[0] = 0x01;  // Card Power on
            else buf[0] = 0x00;        // off
            state = sendMsg(buf, 1, rxbuf, ref rxLen);
            if ((state != ErrState.ERR_NO_ERROR) || (rxLen == 0)) return ErrState.ERR_TIMEOUT;
            if (onOff)
            {
                if (ptr != null) { Array.Copy(rxbuf, 0, ptr, 0, 6); }
                if (buf[3] == 0x63) CardType = 0;
                else CardType = 1;
            }
            return ErrState.ERR_NO_ERROR;
        }
        public ErrState readAllData(byte[] dataBuf, ref int rxlen)
        {

            int len = 0;
            uint max;
            byte[] buf = new byte[256];
            ErrState state;

            if (power(true, null) == ErrState.ERR_NO_ERROR)
            {
                state = readCardData(0, 200, buf, ref len);
                max = (uint)(buf[184] * 256) + buf[185];
                if (max == 0xFFFF) max = 0;
                if (max > 0)
                {
                    state = readCardData(200, (ushort)max, dataBuf, ref len);
                    if (state == ErrState.ERR_NO_ERROR) { rxlen = len; return ErrState.ERR_NO_ERROR; }
                    else { rxlen = 0; return state; }
                }
                else
                {
                    rxlen = 0;
                    return ErrState.ERR_NO_ERROR;
                }
            }
            else
            {
                rxlen = 0;
                return ErrState.ERR_CARD_POWER;
            }
        }
        public int getCardFileNo()
        {
            int cardFileNo = 0;
            if (int.TryParse(Settings.Default.cardFileNo, out cardFileNo))
            {
                Settings.Default.cardFileNo = (cardFileNo + 1).ToString();
                Settings.Default.Save();

            }
            return cardFileNo;
        }
        public ErrState cardErase()
        {
            ErrState state;

            state = power(true, null);
            if (state != ErrState.ERR_NO_ERROR) { power(false, null); return state; }

            // clear the data area
            state = absErase(BULL_START + 400, BULL_START + 800); //card uses 2 bytes per byte!
            if (state != ErrState.ERR_NO_ERROR) { power(false, null); return state; }

            // clear data and cmd ptr positions
            state = absErase(0 + BULL_START + (184 * 2), BULL_START + (188 * 2));
            if (state != ErrState.ERR_NO_ERROR) { power(false, null); return state; }

            return ErrState.ERR_NO_ERROR;
        }
        private ErrState absErase(Int16 start, Int16 end)
        {
            byte[] buf = new byte[5];
            byte[] rxbuf = new byte[256];
            int rxlen = 0;
            ErrState state;
            // first submit the PIK command before erasing the card
            state = checkPIK();
            if (state == ErrState.ERR_NO_ERROR)   // only erase if card response to PIK is OK
            {
                buf[0] = 0x30;           // erase bytes command
                buf[1] = (byte)(start / 256);
                buf[2] = (byte)(start % 256);
                buf[3] = (byte)(end / 256);
                buf[4] = (byte)(end % 256);
                state = sendMsg(buf, 5, rxbuf, ref rxlen);
                if (state != ErrState.ERR_NO_ERROR) return ErrState.ERR_CARD_ERASE;
                if (rxlen == 0) return ErrState.ERR_CARD_TIMEOUT;
                if (rxbuf[0] > 3) { return ErrState.ERR_CARD_ERASE; }    // ignore PIN errors
                else { return ErrState.ERR_NO_ERROR; }                  // erase successful
            }
            else
            {
                return ErrState.ERR_CARD_ERASE;
            }
        }
        //----------------------------------------------------------------------------
        // returns 0 on success
        // returns -3 if data addr error
        // returns -4 if timeout
        private int absWrite(int pos, byte[] ptr, int noBytes)
        {
            int len, i, ret;                 // len is the amount of bytes to write
            byte error = 0;                                 // sum of all the errors
            byte index;                                    // index into ptr array
            int address;                                        // address on card
            byte[] buf = new byte[256];
            byte[] rxbuf = new byte[256];
            int rxLen = 0;
            ErrState state;

            /* first turn the card reader on */
            buf[0] = 0x01;
            state = sendMsg(buf, 1, rxbuf, ref rxLen);
            if ((rxLen == 0) || (rxbuf[0] == 0x10)) return -4;   /* if timeout or no response return */

            address = pos & 0xfff8;
            for (len = noBytes; (len & 0x03) > 0; len++) ;   // must write a multiple of 4 bytes
            if ((address < BULL_START) || ((address + len) > BULL_END)) return -3;

            for (index = 0; (len > 0) && (error == 0);)
            {
                buf[1] = (byte)(address / 256);
                buf[2] = (byte)(address % 256);
                if (len > 64)
                {
                    address = address + 64 * 2;   // each byte takes 2 places on card!
                    buf[0] = 0x20 + (64 / 4) - 1;   // write bytes command
                    for (i = 0; i < 64; i++)
                    {
                        buf[3 + i] = ptr[index++];
                    }
                    len = len - 64;
                }
                else
                {
                    buf[0] = (byte)(0x20 + (len / 4) - 1);
                    for (i = 0; i < len; i++)
                    {
                        buf[3 + i] = ptr[index++];
                    }
                    len = 0;                    // all the bytes have been sent
                } // end else
                state = sendMsg(buf, (byte)(i + 3), rxbuf, ref rxLen);
                if ((rxLen > 0) && (state == ErrState.ERR_NO_ERROR))
                {
                    error = (byte)(error | rxbuf[0]);    // set the error to error returned
                }
                else return -3;    // timeout
            } // end for

            if (error > 0) return -4;
            else return 0;
        }
        private ErrState checkPIK()
        {
            byte[] tx1buf = new byte[9];
            byte[] tx2buf = new byte[5];
            byte[] rxbuf = new byte[256];
            int rxLen = 0;
            ErrState state;
            int retry = 0;

            for (retry = 0; retry < 3; retry++)
            {
                tx1buf[0] = 0x01;
                state = sendMsg(tx1buf, 1, rxbuf, ref rxLen);
                if (state != ErrState.ERR_NO_ERROR) return state;
                CardType = 1;

                tx1buf[0] = 0x57;  // PIK command
                tx1buf[1] = 0x11;
                tx1buf[2] = 0x11;
                tx1buf[3] = 0x11;
                tx1buf[4] = 0x11;
                tx1buf[5] = 0x11;
                tx1buf[6] = 0x11;
                tx1buf[7] = 0x11;
                tx1buf[8] = 0x11;
                tx2buf[0] = 0x43; // PIN command
                tx2buf[1] = 0x00;
                tx2buf[2] = 0x00;
                tx2buf[3] = 0x12;
                tx2buf[4] = 0x34;

                rxLen = 0;
                state = ErrState.ERR_NO_ERROR;
                Array.Clear(rxbuf, 0, rxbuf.Length);

                if (CardType == 0) { state = sendMsg(tx1buf, 9, rxbuf, ref rxLen); }
                else { state = sendMsg(tx2buf, 5, rxbuf, ref rxLen); }

                if (state != ErrState.ERR_NO_ERROR) return state;

                if ((rxLen > 0) && (rxbuf[0] < 4))                                 // ignore PIN errors
                {
                    return ErrState.ERR_NO_ERROR;
                }

            }

            return ErrState.ERR_CARD_ERASE;
        }
        public ErrState readCardHeader(TcpClient tcpclient)
        {
            byte[] str = new byte[256];
            int rxLen = 0;
            Socket soc = tcpclient.Client;
            int fIndex, line = 0, readSize = 37, z, fSize = 200;
            byte[] rawBuf = new byte[80];
            byte crc;
            byte[] txBuf = new byte[256];

            ErrState state = readCardData(0, 200, str, ref rxLen);
            if (state != ErrState.ERR_NO_ERROR) return state;
            if (rxLen != 200) { return ErrState.ERR_NO_CARDREADER; }

            soc.Send(Encoding.ASCII.GetBytes("OK CARDDATA READHEADER 200"), SocketFlags.None);

            for (fIndex = 0; fIndex < fSize; line++)
            {
                string sline = string.Format("{0:X4}", line);
                Array.Copy(Encoding.ASCII.GetBytes(sline), 0, txBuf, 0, 4);
                if ((fIndex + readSize) > fSize) readSize = fSize - fIndex;
                Array.Copy(str, line * 37, rawBuf, 0, readSize);
                fIndex += readSize;

                for (z = 0; z < readSize; z++)
                {
                    txBuf[4 + (z * 2) + 0] = (byte)Hex[rawBuf[z] / 16];
                    txBuf[4 + (z * 2) + 1] = (byte)Hex[rawBuf[z] % 16];
                } // end for z

                // now calc the CRC
                crc = 0;
                for (z = 0; z < ((readSize * 2) + 4); z++) crc ^= txBuf[z];
                txBuf[z++] = (byte)Hex[crc / 16];
                txBuf[z++] = (byte)Hex[crc % 16];
                txBuf[z++] = 0x0d;

                int tRet = 0;

                for (; tRet != z;)
                {
                    tRet = soc.Send(txBuf, 0, z, SocketFlags.None);
                    if (tRet == 0) Thread.Sleep(10);
                }

            }
            byte[] r = { (byte)'\r' };
            soc.Send(r, SocketFlags.None);
            return ErrState.ERR_NO_ERROR;
        }
        //-----------------------------------------------------------------------------

        // returns 0 on success
        //         -1 if power could not be turned on
        //         -2 if could not erase
        //         -3 if could not write
        //         -4 if error writing to card
        public int writeCardHeader(byte[] buf, int noBytes)
        {
            int ret = 0;

            if (power(true, null) == ErrState.ERR_NO_ERROR)
            {
                if (absErase((short)BULL_START, (short)(BULL_START + (noBytes * 2))) == ErrState.ERR_NO_ERROR)
                {
                    int retVal = absWrite(0 + BULL_START, buf, noBytes);
                    power(false, null);
                    return retVal;
                }
                else { ret = -2; }
            }
            else ret = -1;
            power(false, null);
            return ret;
        }

    }
}
