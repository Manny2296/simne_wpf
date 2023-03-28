using simnet.com.data.simnet.models;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Diagnostics;
using System.Net;
using System.Reflection;
using System.Windows.Controls;
using System.Windows.Media.Media3D;
using System.Xml.Linq;

namespace simnet.com.data.simnet
{
    class DBConnection
    {
        public SQLiteConnection sQLite { get; set; }
        public SQLiteCommand sQLiteCmd { get; set; }
        public string IdentityCode { get; set; }
        public string Name { get; set; }

        public DBConnection(SQLiteConnection sQLite)
        {
            this.sQLite = sQLite;

        }



        public void InsertPatient(Patient p)
        {
            string insertQUery = "INSERT INTO Patient(PatientID,DeviceSN,IdInDevice,Name,Gender,Height,Weight,Mobile,Doctor,IdentityType,IdentityCode,SocietyCode,PatientType,Age,Address,ReadFlag,LastUpdateTime) VALUES(@PatientID,@DeviceSN,@IdInDevice,@Name,@Gender,@Height,@Weight,@Mobile,@Doctor,@IdentityType,@Documento,@SocietyCode,@PatientType,@Age,@Address,@ReadFlag,@LastUpdateTime)";

           // string updateQuery = "Update Patient SET name=@Name,Age=@Age,Gender=@Gender,Height=@Height,Weight=@Weight,Mobile=@Mobile,Address=@Address WHERE IdentityCode = @Documento";

            sQLite.Open();

            using (SQLiteCommand command = new SQLiteCommand(insertQUery, sQLite))
            {
                command.Parameters.AddWithValue("@PatientID", p.PatientID);
                command.Parameters.AddWithValue("@DeviceSN", p.DeviceSN);
                command.Parameters.AddWithValue("@IdInDevice", p.IdInDevice);
                command.Parameters.AddWithValue("@Name", p.Name);
                command.Parameters.AddWithValue("@Gender", p.Gender);
                command.Parameters.AddWithValue("@Height", p.Height);
                command.Parameters.AddWithValue("@Weight", p.Weight);
                command.Parameters.AddWithValue("@Mobile", p.Mobile);
                command.Parameters.AddWithValue("@Doctor", p.Doctor);
                command.Parameters.AddWithValue("@IdentityType", p.IdentityType);
                command.Parameters.AddWithValue("@Documento",p.IdentityCode);
                command.Parameters.AddWithValue("@SocietyCode", p.SocietyCode);
                command.Parameters.AddWithValue("@PatientType", p.PatientType);
                command.Parameters.AddWithValue("@Age", p.Age);
                command.Parameters.AddWithValue("@Address", p.Address);
                command.Parameters.AddWithValue("@ReadFlag", p.ReadFlag);
                command.Parameters.AddWithValue("@LastUpdateTime", DateTime.Now);
                
            
                using (SQLiteDataReader reader = command.ExecuteReader())
                {
                    Debug.WriteLine("Inserted : " + reader.RecordsAffected);
                }
            }
            sQLite.Close();
        }

        /// <summary>
        /// Traer historico de temperaturas x id de paciente
        /// </summary>

        public List<TempData> GetTempDatasbyPatientID(string patientId)
        {
            string selectQuery = "SELECT MeasureTime, TempValue, Clasificacion_tem from TEMPData where PatientID = @PatientID order by MeasureTime DESC LIMIT 5 ";
            List<TempData> tempDatas = new List<TempData>();
           

            try
            {
             sQLite.Open();
             using (SQLiteCommand command = new SQLiteCommand(selectQuery, sQLite))
            {
                command.Parameters.AddWithValue("@PatientID", patientId);


                using (SQLiteDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        tempDatas.Add(new TempData("",reader.GetDateTime(0), reader.GetFloat(1),reader.GetString(2)));
                      //  Console.WriteLine($" |  {reader.GetString(0)} - {sQLiteDataReader.GetInt32(2)} - {sQLiteDataReader.GetString(3)}");
                    }
                }

            }
            sQLite.Close();

            }
            catch (Exception ex)
            {
                Debug.WriteLine("Exception :" + ex.Message);
               // Debug.WriteLine(ex.Message);
            }

            return tempDatas;
        }




        /// <summary>
        /// Traer historico de NIBPmetria x id de paciente 
        /// </summary>
        /// <param name="patientId"></param>
        /// <returns></returns>
        public List<NIBPData> GetNibpbyPatientID(string patientId)
        {
            string selectQuery = "SELECT MeasureTime, SysValue, DiaValue, MeanValue, PrValue,CLasificacion_pre from NIBPData where PatientID = @PatientID order by MeasureTime DESC LIMIT 5 ";
            List<NIBPData> nibpDatas = new List<NIBPData>();


            try
            {
                sQLite.Open();
                using (SQLiteCommand command = new SQLiteCommand(selectQuery, sQLite))
                {
                    command.Parameters.AddWithValue("@PatientID", patientId);


                    using (SQLiteDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                           nibpDatas.Add(new NIBPData("", reader.GetDateTime(0), reader.GetInt32(1), reader.GetInt32(2), reader.GetInt32(3), reader.GetInt32(4), reader.GetString(5)));
                            //  Console.WriteLine($" |  {reader.GetString(0)} - {sQLiteDataReader.GetInt32(2)} - {sQLiteDataReader.GetString(3)}");
                        }
                    }

                }
                sQLite.Close();

            }
            catch (Exception ex)
            {
                Debug.WriteLine("Exception :" + ex.Message);
                // Debug.WriteLine(ex.Message);
            }

            return nibpDatas;
        }











        /// <summary>
        /// Traer historico de oxymetria x id de paciente 
        /// </summary>
        /// <param name="patientId"></param>
        /// <returns></returns>
        public List<SPO2Data> GetOxybyPatientID(string patientId)
        {
            string selectQuery = "SELECT MeasureTime, SPO2Value, PrValue,CLasificacion_spo,Clasificacion_pres from SPO2Data where PatientID = @PatientID order by MeasureTime DESC LIMIT 5 ";
            List<SPO2Data> oxyDatas = new List<SPO2Data>();


            try
            {
                sQLite.Open();
                using (SQLiteCommand command = new SQLiteCommand(selectQuery, sQLite))
                {
                    command.Parameters.AddWithValue("@PatientID", patientId);


                    using (SQLiteDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            oxyDatas.Add(new SPO2Data("", reader.GetDateTime(0),reader.GetInt32(1),reader.GetInt32(2),reader.GetString(3),reader.GetString(4)));
                            //  Console.WriteLine($" |  {reader.GetString(0)} - {sQLiteDataReader.GetInt32(2)} - {sQLiteDataReader.GetString(3)}");
                        }
                    }

                }
                sQLite.Close();

            }
            catch (Exception ex)
            {
                Debug.WriteLine("Exception :" + ex.Message);
                // Debug.WriteLine(ex.Message);
            }

            return oxyDatas;
        }




        public void InsertOxyData(SPO2Data spo2data)
        {
            string insertQUery = "INSERT INTO SPO2Data(PatientID,MeasureTime,SPO2Value,PrValue,Clasificacion_spo,Clasificacion_pres) VALUES(@PatientID,@MeasureTime,@SPO2Value,@PrValue,@Clasificacion_spo,@Clasificacion_pres)";

            // string updateQuery = "Update Patient SET name=@Name,Age=@Age,Gender=@Gender,Height=@Height,Weight=@Weight,Mobile=@Mobile,Address=@Address WHERE IdentityCode = @Documento";

            sQLite.Open();

            using (SQLiteCommand command = new SQLiteCommand(insertQUery, sQLite))
            {
                command.Parameters.AddWithValue("@PatientID", spo2data.PatientID);
                command.Parameters.AddWithValue("@MeasureTime", spo2data.MeasureTime);
                command.Parameters.AddWithValue("@SPO2Value", spo2data.SPO2Value);
                command.Parameters.AddWithValue("@PrValue", spo2data.PrValue);
                command.Parameters.AddWithValue("@Clasificacion_spo", spo2data.Clasificacion_spo);
                command.Parameters.AddWithValue("@Clasificacion_pres", spo2data.Clasificacion_pres);




                using (SQLiteDataReader reader = command.ExecuteReader())
                {
                    Debug.WriteLine("Inserted Oxy: " + reader.RecordsAffected);
                }
            }
            sQLite.Close();
        }

        public void InsertNibpData(NIBPData nibpdata)
        {
            string insertQUery = "INSERT INTO NIBPData(PatientID,MeasureTime,SysValue,DiaValue,MeanValue,PrValue,Clasificacion_pre) VALUES(@PatientID,@MeasureTime,@SysValue,@DiaValue,@MeanValue,@PrValue,@Clasificacion_pre)";

            // string updateQuery = "Update Patient SET name=@Name,Age=@Age,Gender=@Gender,Height=@Height,Weight=@Weight,Mobile=@Mobile,Address=@Address WHERE IdentityCode = @Documento";

            sQLite.Open();

            using (SQLiteCommand command = new SQLiteCommand(insertQUery, sQLite))
            {
                command.Parameters.AddWithValue("@PatientID", nibpdata.PatientID);
                command.Parameters.AddWithValue("@MeasureTime", nibpdata.MeasureTime);
                command.Parameters.AddWithValue("@SysValue", nibpdata.SysValue);
                command.Parameters.AddWithValue("@DiaValue", nibpdata.DiaValue);
                command.Parameters.AddWithValue("@MeanValue", nibpdata.MeanValue);
                command.Parameters.AddWithValue("@PrValue", nibpdata.PrValue);
                command.Parameters.AddWithValue("@Clasificacion_pre", nibpdata.Clasificacion_pre);

                using (SQLiteDataReader reader = command.ExecuteReader())
                {
                    Debug.WriteLine("Inserted Nibp: " + reader.RecordsAffected);
                }
            }
            sQLite.Close();
        }

        public void InsertTempData(TempData temp)
        {
            string insertQUery = "INSERT INTO TEMPData(PatientID,MeasureTime,TempValue,Clasificacion_tem) VALUES(@PatientID,@MeasureTime,@TempValue,@Clasificacion_tem)";

            // string updateQuery = "Update Patient SET name=@Name,Age=@Age,Gender=@Gender,Height=@Height,Weight=@Weight,Mobile=@Mobile,Address=@Address WHERE IdentityCode = @Documento";

            sQLite.Open();

            using (SQLiteCommand command = new SQLiteCommand(insertQUery, sQLite))
            {
                command.Parameters.AddWithValue("@PatientID", temp.PatientID);
                command.Parameters.AddWithValue("@MeasureTime", temp.MeasureTime);
                command.Parameters.AddWithValue("@TempValue", temp.TempValue);
                command.Parameters.AddWithValue("@Clasificacion_tem", temp.Clasificacion_tem);

                

                using (SQLiteDataReader reader = command.ExecuteReader())
                {
                    Debug.WriteLine("Inserted Temp: " + reader.RecordsAffected);
                }
            }
            sQLite.Close();
        }

        public Patient UpdatePatient(string documento, Patient patient)
        {
            string updateQuery = "Update Patient SET name=@Name,Age=@Age,Gender=@Gender,Height=@Height,Weight=@Weight,Mobile=@Mobile,Address=@Address,LastUpdateTime=@LastUpdateTime WHERE IdentityCode = @Documento";

            sQLite.Open();

            using (SQLiteCommand command = new SQLiteCommand(updateQuery, sQLite))
            {
                command.Parameters.AddWithValue("@Documento", documento);
                command.Parameters.AddWithValue("@Name", patient.Name);
                command.Parameters.AddWithValue("@Age", patient.Age);
                command.Parameters.AddWithValue("@Gender", patient.Gender);
                command.Parameters.AddWithValue("@Height", patient.Height);
                command.Parameters.AddWithValue("@Weight", patient.Weight);
                command.Parameters.AddWithValue("@Mobile", patient.Mobile);
                command.Parameters.AddWithValue("@Address", patient.Address);
                command.Parameters.AddWithValue("@LastUpdateTime", DateTime.Now);
                using (SQLiteDataReader reader = command.ExecuteReader())
                {
                    Debug.WriteLine("Inserted : " + reader.RecordsAffected);
                }

            }
            sQLite.Close();
           return patient;
        }
        public Patient GetPatientById(string documento)
        {
            string selectQuery = "SELECT * FROM Patient WHERE IdentityCode = @Documento";

            sQLite.Open();

            using (SQLiteCommand command = new SQLiteCommand(selectQuery, sQLite))
            {
                command.Parameters.AddWithValue("@Documento", documento);

                using (SQLiteDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        Patient patient = new Patient
                        {
                            PatientID = reader.GetString(0),
                            DeviceSN = reader.GetString(1),
                            IdInDevice = reader.GetInt32(2),
                            Name = reader.GetString(3),
                            Gender = reader.GetInt32(4),
                            Height = reader.GetFloat(5),
                            Weight = reader.GetFloat(6),
                            Mobile = reader.GetString(7),
                            Doctor = reader.GetString(8),
                            IdentityType = reader.GetInt32(9),
                            IdentityCode = reader.GetString(10),
                            SocietyCode = reader.GetString(11),
                            PatientType = reader.GetInt32(12),
                            Age = reader.GetInt32(13),
                            Address = reader.GetString(14),
                           // ReadFlag = reader.GetString(15),
                            LastUpdateTime = reader.GetDateTime(16),


                            // Add more property assignments as needed
                        };
                        sQLite.Close();
                        return patient;
                    }
                }

            }
            sQLite.Close();
            return null; // If no patient is found with the given ID
        }
        








        //public Patient getPatienById(string documento)
        //{
        //    string selectAll = "SELECT * FROM Patient where IdentityCode =" + documento;
        //    sQLite.Open();
        //    SQLiteCommand sQLiteCmd = new SQLiteCommand(selectAll, sQLite);
        //    using SQLiteDataReader sQLiteDataReader = sQLiteCmd.ExecuteReader();
        //    while (sQLiteDataReader.Read())
        //    {

        //        Console.WriteLine($" |  {sQLiteDataReader.GetString(0)} - {sQLiteDataReader.GetString(5)}");
        //    }
        //    patient.nam


        //    Console.WriteLine("Encontré a " + patient.Name + " Patient ID" + patient.PatientID);

        //    return null;

        //}
        public SQLiteDataReader getPatients()
        {
            try
            {

                Console.WriteLine("Conectado con sucesso");
                string selectAll = "SELECT * FROM Patient";
                sQLite.Open();
                SQLiteCommand sQLiteCmd = new SQLiteCommand(selectAll, sQLite);
                using SQLiteDataReader sQLiteDataReader = sQLiteCmd.ExecuteReader();
                Console.WriteLine("------------- PATIENTS --------------------");
                // Console.WriteLine("  |  Device SN |  IdInDevice | Name  |");
                while (sQLiteDataReader.Read())
                {

                    Console.WriteLine($" |  {sQLiteDataReader.GetString(1)} - {sQLiteDataReader.GetInt32(2)} - {sQLiteDataReader.GetString(3)}");
                }
                return sQLiteDataReader;

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;

            }

        }

    }

}
