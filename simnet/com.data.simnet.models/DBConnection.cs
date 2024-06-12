using log4net;
using log4net.Config;
using simnet.com.data.simnet.models;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Reflection;

namespace simnet.com.data.simnet
{
    class DBConnection
    {
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        static DBConnection()
        {
            XmlConfigurator.Configure();
        }

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
            log.Info("Inserting patient...");

            string insertQUery = "INSERT INTO Patient(PatientID,DeviceSN,IdInDevice,Name,Gender,Height,Weight,Mobile,Doctor,IdentityType,IdentityCode,SocietyCode,PatientType,Age,Address,ReadFlag,LastUpdateTime) VALUES(@PatientID,@DeviceSN,@IdInDevice,@Name,@Gender,@Height,@Weight,@Mobile,@Doctor,@IdentityType,@Documento,@SocietyCode,@PatientType,@Age,@Address,@ReadFlag,@LastUpdateTime)";

            try
            {
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
                    command.Parameters.AddWithValue("@Documento", p.IdentityCode);
                    command.Parameters.AddWithValue("@SocietyCode", p.SocietyCode);
                    command.Parameters.AddWithValue("@PatientType", p.PatientType);
                    command.Parameters.AddWithValue("@Age", p.Age);
                    command.Parameters.AddWithValue("@Address", p.Address);
                    command.Parameters.AddWithValue("@ReadFlag", p.ReadFlag);
                    command.Parameters.AddWithValue("@LastUpdateTime", DateTime.Now);

                    using (SQLiteDataReader reader = command.ExecuteReader())
                    {
                        log.Info("Inserted patient with records affected: " + reader.RecordsAffected);
                    }
                }
            }
            catch (Exception ex)
            {
                log.Error("Exception during patient insertion", ex);
            }
            finally
            {
                sQLite.Close();
            }
        }

        public List<TempData> GetTempDatasbyPatientID(string patientId)
        {
            log.Info("Retrieving temperature data for patient ID: " + patientId);

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
                            tempDatas.Add(new TempData("", reader.GetDateTime(0), reader.GetFloat(1), reader.GetString(2)));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                log.Error("Exception during temperature data retrieval", ex);
            }
            finally
            {
                sQLite.Close();
            }

            return tempDatas;
        }

        public List<NIBPData> GetNibpbyPatientID(string patientId)
        {
            log.Info("Retrieving NIBP data for patient ID: " + patientId);

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
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                log.Error("Exception during NIBP data retrieval", ex);
            }
            finally
            {
                sQLite.Close();
            }

            return nibpDatas;
        }

        public List<SPO2Data> GetOxybyPatientID(string patientId)
        {
            log.Info("Retrieving SPO2 data for patient ID: " + patientId);

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
                            oxyDatas.Add(new SPO2Data("", reader.GetDateTime(0), reader.GetInt32(1), reader.GetInt32(2), reader.GetString(3), reader.GetString(4)));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                log.Error("Exception during SPO2 data retrieval", ex);
            }
            finally
            {
                sQLite.Close();
            }

            return oxyDatas;
        }

        public void InsertOxyData(SPO2Data spo2data)
        {
            log.Info("Inserting SPO2 data...");

            string insertQUery = "INSERT INTO SPO2Data(PatientID,MeasureTime,SPO2Value,PrValue,Clasificacion_spo,Clasificacion_pres) VALUES(@PatientID,@MeasureTime,@SPO2Value,@PrValue,@Clasificacion_spo,@Clasificacion_pres)";

            try
            {
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
                        log.Info("Inserted SPO2 data with records affected: " + reader.RecordsAffected);
                    }
                }
            }
            catch (Exception ex)
            {
                log.Error("Exception during SPO2 data insertion", ex);
            }
            finally
            {
                sQLite.Close();
            }
        }

        public void InsertNibpData(NIBPData nibpdata)
        {
            log.Info("Inserting NIBP data...");

            string insertQUery = "INSERT INTO NIBPData(PatientID,MeasureTime,SysValue,DiaValue,MeanValue,PrValue,Clasificacion_pre) VALUES(@PatientID,@MeasureTime,@SysValue,@DiaValue,@MeanValue,@PrValue,@Clasificacion_pre)";

            try
            {
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
                        log.Info("Inserted NIBP data with records affected: " + reader.RecordsAffected);
                    }
                }
            }
            catch (Exception ex)
            {
                log.Error("Exception during NIBP data insertion", ex);
            }
            finally
            {
                sQLite.Close();
            }
        }

        public void InsertTempData(TempData temp)
        {
            log.Info("Inserting temperature data...");

            string insertQUery = "INSERT INTO TEMPData(PatientID,MeasureTime,TempValue,Clasificacion_tem) VALUES(@PatientID,@MeasureTime,@TempValue,@Clasificacion_tem)";

            try
            {
                sQLite.Open();
                using (SQLiteCommand command = new SQLiteCommand(insertQUery, sQLite))
                {
                    command.Parameters.AddWithValue("@PatientID", temp.PatientID);
                    command.Parameters.AddWithValue("@MeasureTime", temp.MeasureTime);
                    command.Parameters.AddWithValue("@TempValue", temp.TempValue);
                    command.Parameters.AddWithValue("@Clasificacion_tem", temp.Clasificacion_tem);

                    using (SQLiteDataReader reader = command.ExecuteReader())
                    {
                        log.Info("Inserted temperature data with records affected: " + reader.RecordsAffected);
                    }
                }
            }
            catch (Exception ex)
            {
                log.Error("Exception during temperature data insertion", ex);
            }
            finally
            {
                sQLite.Close();
            }
        }

        public Patient UpdatePatient(string documento, Patient patient)
        {
            log.Info("Updating patient with document ID: " + documento);

            string updateQuery = "Update Patient SET name=@Name,Age=@Age,Gender=@Gender,Height=@Height,Weight=@Weight,Mobile=@Mobile,Address=@Address,LastUpdateTime=@LastUpdateTime WHERE IdentityCode = @Documento";

            try
            {
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
                        log.Info("Updated patient with records affected: " + reader.RecordsAffected);
                    }
                }
            }
            catch (Exception ex)
            {
                log.Error("Exception during patient update", ex);
            }
            finally
            {
                sQLite.Close();
            }

            return patient;
        }

        public Patient GetPatientById(string documento)
        {
            log.Info("Retrieving patient by document ID: " + documento);

            string selectQuery = "SELECT * FROM Patient WHERE IdentityCode = @Documento";

            try
            {
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
                                LastUpdateTime = reader.GetDateTime(16)
                            };

                            log.Info("Retrieved patient: " + patient.Name);
                            return patient;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                log.Error("Exception during patient retrieval", ex);
            }
            finally
            {
                sQLite.Close();
            }

            return null; // If no patient is found with the given ID
        }

        public SQLiteDataReader getPatients()
        {
            log.Info("Retrieving all patients...");

            string selectAll = "SELECT * FROM Patient";

            try
            {
                sQLite.Open();
                SQLiteCommand sQLiteCmd = new SQLiteCommand(selectAll, sQLite);
                SQLiteDataReader sQLiteDataReader = sQLiteCmd.ExecuteReader();

                log.Info("Retrieved all patients");
                return sQLiteDataReader;
            }
            catch (Exception ex)
            {
                log.Error("Exception during patient retrieval", ex);
                return null;
            }
        }
    }
}