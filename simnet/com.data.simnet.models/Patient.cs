using System;

namespace simnet.com.data.simnet.models
{
    public class Patient
    {
        public string PatientID { get; set; }
        public string DeviceSN { get; set; }
        public int IdInDevice { get; set; }
        public string Name { get; set; }
        public int Gender { get; set; }
        public float Height { get; set; }
        public float Weight { get; set; }
        public string Mobile { get; set; }
        public string Doctor { get; set; }
        public int IdentityType { get; set; }
        public string IdentityCode { get; set; }
        public string SocietyCode { get; set; }
        public int PatientType { get; set; }
        public int Age { get; set; }
        public string Address { get; set; }
        public string ReadFlag { get; set; }
        public DateTime LastUpdateTime { get; set; }



        public Patient()
        {

        }

        public Patient(string patientID, string deviceSN, int idInDevice, string name, int gender, float height, float weight, string mobile, string doctor, int identityType, string identityCode, string societyCode, int patientType, int age, string address, string readFlag, DateTime lastUpdateTime)
        {
            PatientID = patientID;
            DeviceSN = deviceSN;
            IdInDevice = idInDevice;
            Name = name;
            Gender = gender;
            Height = height;
            Weight = weight;
            Mobile = mobile;
            Doctor = doctor;
            IdentityType = identityType;
            IdentityCode = identityCode;
            SocietyCode = societyCode;
            PatientType = patientType;
            Age = age;
            Address = address;
            ReadFlag = readFlag;
            LastUpdateTime = lastUpdateTime;
        }
    }
}
