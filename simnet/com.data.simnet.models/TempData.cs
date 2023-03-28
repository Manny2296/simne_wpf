using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace simnet.com.data.simnet.models
{
    internal class TempData
    {
        private DateTime now;
        private float v;

        public int ID { get; set; }
        public string PatientID { get; set; }
        public DateTime MeasureTime { get; set; }
        public float TempValue { get; set; }
        public string Clasificacion_tem { get; set; }

        public TempData()
        {
        }

        public TempData(string patientID, DateTime measureTime, float tempValue, string clasificacion_tem)
        {

            PatientID = patientID;
            MeasureTime = measureTime;
            TempValue = tempValue;
            Clasificacion_tem = clasificacion_tem;
        }

        public TempData(string patientID, DateTime now, float v)
        {
            PatientID = patientID;
            this.now = now;
            this.v = v;
        }
    }
}
