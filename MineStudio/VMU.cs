using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static MineStudio.frmMain;

namespace MineStudio
{
    public class VMU
    {
        public string version;
        public const int MAX_LOG_RECORDS = 9357;
        public const int MAX_GPS_RECORDS = 3639;
        public const int FLASH_BLOCKS = 32;
        public TimeStruct currentTime = new TimeStruct(0);
        public DateStruct currentDate = new DateStruct(0);
        public VMUConstStruct vmuConst = new VMUConstStruct(0);
        public RSTDO_Struct rstdoData = new RSTDO_Struct(0);
        public CountsStruct countsData = new CountsStruct(0);
        public TripDataStruct currentTrip = new TripDataStruct(0);
        public flashHeaderStruct[] flashConfig = new flashHeaderStruct[FLASH_BLOCKS];
        public blockConfigStruct[] newFlashConfig = new blockConfigStruct[FLASH_BLOCKS];
        public brakeEventStruct brakeEvent = new brakeEventStruct(0);
        public accelEventStruct accelEvent = new accelEventStruct(0);
        public fuelEventStruct fuelEvent = new fuelEventStruct(0);
        public inputEventStruct inputEvent = new inputEventStruct(0);
        public userEventStruct userEvent = new userEventStruct(0);
        public idleEventStruct idleEvent = new idleEventStruct(0);
        public powerEventStruct powerEvent = new powerEventStruct(0);
        public speedEventStruct speedEvent = new speedEventStruct(0);
        public logStruct logData = new logStruct(0);
        public gpsDataStruct gpsData = new gpsDataStruct(0);
        public gpsConfig gpsconfig = new gpsConfig(0);

        public VMU()
        {

        }
    }
}
