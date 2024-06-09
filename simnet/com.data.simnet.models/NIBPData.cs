using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace simnet.com.data.simnet.models
{
    internal class NIBPData
    {
        private DateTime now;
        private float v;

        public int ID { get; set; }
        public string PatientID { get; set; }
        public DateTime MeasureTime { get; set; }
        public int SysValue { get; set; }
        public int DiaValue { get; set; }
        public int MeanValue { get; set; }
        public int PrValue { get; set; }
        public string Clasificacion_pre { get; set; }

        public NIBPData()
        {
        }

        public NIBPData(string patientID, DateTime measureTime, int sysValue, int diaValue, int meanValue, int prValue, string clasificacion_pre)
        {

            PatientID = patientID;
            MeasureTime = measureTime;
            SysValue = sysValue;
            DiaValue = diaValue;
            MeanValue = meanValue;
            PrValue = prValue;
            Clasificacion_pre = clasificacion_pre;
        }

        public NIBPData(string patientID, DateTime now, float v)
        {
            PatientID = patientID;
            this.now = now;
            this.v = v;
        }

    }
}
