using simnet.com.data.simnet.models;
using System.Collections.Generic;
using System.Configuration;

namespace simnet.com.data.simnet.services
{
    internal class PatientService
    {
        private DBConnection conn;
        public PatientService()
        {
            string connectionString = ConfigurationManager.ConnectionStrings["SQLiteConnectionString"].ConnectionString;
            conn = new DBConnection(new System.Data.SQLite.SQLiteConnection(connectionString));

        }

        public Patient GetPatientById(string identity_code)
        {
            return conn.GetPatientById(identity_code);
        }

        public Patient UpdatePatient(Patient p)
        {
           return conn.UpdatePatient(p.IdentityCode, p);
        }
        public void InsertPatient(Patient p)
        {
            conn.InsertPatient(p);
        }

        public void InsertTempData(TempData t)
        {
            conn.InsertTempData(t);
        }
        public void InsertOxyData(SPO2Data sPO2Data)
        {
            conn.InsertOxyData(sPO2Data);
        }
        public void InsertNibpData(NIBPData nIBPData)
        {
            conn.InsertNibpData(nIBPData);
        }

        public List<TempData> GetTempDatasbyPatientID(string patientId)
        {
            return conn.GetTempDatasbyPatientID(patientId);
        }
        public List<SPO2Data> GetOxybyPatientID(string patientId)
        {
            return conn.GetOxybyPatientID(patientId);
        }
        public List<NIBPData> GetNibpbyPatientID(string patientId)
        {
            return conn.GetNibpbyPatientID(patientId);
        }
    }
}
