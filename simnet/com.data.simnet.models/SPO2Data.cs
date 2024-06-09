using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace simnet.com.data.simnet.models
{
    internal class SPO2Data
    {
   

        public int ID { get; set; }
        public string PatientID { get; set; }
        public DateTime MeasureTime { get; set; }
        public int SPO2Value { get; set; }
        public int PrValue { get; set; }
        public string Clasificacion_spo { get; set; }
        public string Clasificacion_pres { get; set; }

        public SPO2Data()
        {
        }

        /// <summary>
        /// constructor 
        /// </summary>
        /// <param name="patientID"></param>
        /// <param name="measureTime"></param>
        /// <param name="spo2Value"></param>
        /// <param name="prValue"></param>
        /// <param name="clasificacion_spo"></param>
        /// <param name="clasificacion_pres"></param>
        public SPO2Data(string patientID, DateTime measureTime, int spo2Value , int prValue , string clasificacion_spo , string clasificacion_pres)
        {

            PatientID = patientID;
            MeasureTime = measureTime;
            SPO2Value = spo2Value;
            PrValue = prValue;
            Clasificacion_spo = clasificacion_spo;
            Clasificacion_pres = clasificacion_pres;
        }

    

    }
}
