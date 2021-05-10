using IoTHubTrigger = Microsoft.Azure.WebJobs.EventHubTriggerAttribute;

using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Azure.EventHubs;
using System.Text;
using System.Net.Http;
using Microsoft.Extensions.Logging;
using System.Data.SqlClient;
using System;
using System.Globalization;
using Newtonsoft.Json;

namespace FromIotHubToSql
{
    public static class FromIotHubToSql
    {
        private static HttpClient client = new HttpClient();

        [FunctionName("FromIotHubToSql")]
        public static void Run([IoTHubTrigger("messages/events", Connection = "iothub", ConsumerGroup = "bigdata")] EventData message, ILogger log)
        {
            log.LogInformation($"C# IoT Hub trigger function processed a message: {Encoding.UTF8.GetString(message.Body.Array)}");

            using (var connection = new SqlConnection(Environment.GetEnvironmentVariable("connection")))
            {
                var data = JsonConvert.DeserializeObject<Models.Data>(Encoding.UTF8.GetString(message.Body.Array));

                float temp = data.temperature;
                float humidity = data.humidity;
                long timeStamp = data.logtime;
                string tempAlert;
                if (data.tempAlert)
                {
                    tempAlert = "TEMP WARNING!";
                }
                else
                {
                    tempAlert = "TEMP OK";
                }
                
                string sensorType = message.Properties["SensorType"].ToString();
                string deviceModel = message.Properties["DeviceModel"].ToString();
                string vendor = message.Properties["Vendor"].ToString();
                
                float latitude = float.Parse(message.Properties["Latitude"].ToString(), CultureInfo.InvariantCulture);
                float longitude = float.Parse(message.Properties["Longitude"].ToString(), CultureInfo.InvariantCulture);
                
                string deviceId = message.Properties["DeviceId"].ToString();

                long _timeId;
                long _locationId;
                string _deviceId;
                long _sensorTypeId;
                long _vendorId;
                long _deviceModelId;
                long _tempAlertId;

                connection.Open();

                var qTimeTable = "IF NOT EXISTS (SELECT TimeStamp FROM TimeTable WHERE TimeStamp = @timeStamp) " +
                                 "INSERT INTO TimeTable OUTPUT inserted.TimeStamp VALUES (@timeStamp)" +
                                 "ELSE SELECT TimeStamp FROM TimeTable WHERE TimeStamp = @timeStamp";

                using (var command = new SqlCommand(qTimeTable, connection))
                {
                    command.Parameters.AddWithValue("@timeStamp", timeStamp);
                    _timeId = Convert.ToInt32(command.ExecuteScalar());
                }


                var qLocationTable = "IF NOT EXISTS (SELECT Id FROM LocationTable WHERE Longitude = @longitude AND Latitude = @latitude) " +
                                     "INSERT INTO LocationTable OUTPUT inserted.Id VALUES (@longitude, @latitude)" +
                                     "ELSE SELECT Id FROM LocationTable WHERE Longitude = @longitude AND Latitude = @latitude";

                using (var command = new SqlCommand(qLocationTable, connection))
                {
                    command.Parameters.AddWithValue("@longitude", longitude);
                    command.Parameters.AddWithValue("@latitude", latitude);
                    _locationId = Convert.ToInt32(command.ExecuteScalar());
                }


                var qSensorTypeTable = "IF NOT EXISTS (SELECT Id FROM SensorTypeTable WHERE SensorType = @sensorType) " +
                                       "INSERT INTO SensorTypeTable OUTPUT inserted.Id VALUES (@sensorType)" +
                                       "ELSE SELECT Id FROM SensorTypeTable WHERE SensorType = @sensorType";

                using (var command = new SqlCommand(qSensorTypeTable, connection))
                {
                    command.Parameters.AddWithValue("@sensorType", sensorType);
                    _sensorTypeId = Convert.ToInt32(command.ExecuteScalar());
                }


                var qVendorTable = "IF NOT EXISTS (SELECT Id FROM VendorTable WHERE Vendor = @vendor) " +
                                   "INSERT INTO VendorTable OUTPUT inserted.Id VALUES (@vendor)" +
                                   "ELSE SELECT Id FROM VendorTable WHERE Vendor = @vendor";

                using (var command = new SqlCommand(qVendorTable, connection))
                {
                    command.Parameters.AddWithValue("@vendor", vendor);
                    _vendorId = Convert.ToInt32(command.ExecuteScalar());
                }


                var qDeviceModelTable = "IF NOT EXISTS (SELECT Id FROM DeviceModelTable WHERE DeviceModel = @deviceModel) " +
                                        "INSERT INTO DeviceModelTable OUTPUT inserted.Id VALUES (@deviceModel)" +
                                        "ELSE SELECT Id FROM DeviceModelTable WHERE DeviceModel = @deviceModel";

                using (var command = new SqlCommand(qDeviceModelTable, connection))
                {
                    command.Parameters.AddWithValue("@deviceModel", deviceModel);
                    _deviceModelId = Convert.ToInt32(command.ExecuteScalar());
                }


                var qDeviceTable = "IF NOT EXISTS(SELECT Id FROM DeviceTable WHERE SensorTypeId = @sensorTypeId AND VendorId = @vendorId AND DeviceModelId = @deviceModelId) " +
                                   "INSERT INTO DeviceTable OUTPUT inserted.Id VALUES(@deviceId, @sensorTypeId, @vendorId, @deviceModelId) " +
                                   "ELSE SELECT Id FROM DeviceTable WHERE SensorTypeId = @sensorTypeId AND VendorId = @vendorId AND DeviceModelId = @deviceModelId";

                using (var command = new SqlCommand(qDeviceTable, connection))
                {
                    command.Parameters.AddWithValue("@deviceId", deviceId);
                    command.Parameters.AddWithValue("@sensorTypeId", _sensorTypeId);
                    command.Parameters.AddWithValue("@vendorId", _vendorId);
                    command.Parameters.AddWithValue("@deviceModelId", _deviceModelId);
                    _deviceId = Convert.ToString(command.ExecuteScalar());
                }


                var qtempAlert = "IF NOT EXISTS(SELECT Id FROM TempAlertTable WHERE TempAlert = @tempAlert) " +
                                   "INSERT INTO TempAlertTable OUTPUT inserted.Id VALUES(@tempAlert) " +
                                   "ELSE SELECT Id FROM TempAlertTable WHERE TempAlert = @tempAlert";

                using (var command = new SqlCommand(qtempAlert, connection))
                {
                    command.Parameters.AddWithValue("@tempAlert", tempAlert);
                    _tempAlertId = Convert.ToInt32(command.ExecuteScalar());
                }


                var qMeasurmentTable = "INSERT INTO MeasurmentTable VALUES (@timeId, @deviceId, @locationId, @tempAlertId, @temp, @humidity)";

                using (var command = new SqlCommand(qMeasurmentTable, connection))
                {
                    command.Parameters.AddWithValue("@timeId", _timeId);
                    command.Parameters.AddWithValue("@deviceId", _deviceId);
                    command.Parameters.AddWithValue("@locationId", _locationId);
                    command.Parameters.AddWithValue("@tempAlertId", _tempAlertId);
                    command.Parameters.AddWithValue("@temp", temp);
                    command.Parameters.AddWithValue("@humidity", humidity);
                    command.ExecuteNonQuery();
                }
            }
        }
    }
}