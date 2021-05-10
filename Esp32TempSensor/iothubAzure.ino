void initIotDevice(char* connectionString) {
  if(!Esp32MQTTClient_Init((const uint8_t *) connectionString)) {
    isConnectedToIotHub = false;
    return;
  }
  isConnectedToIotHub = true;
}

void SendIotMessage(char* MessageToSend){
  EVENT_INSTANCE * message = Esp32MQTTClient_Event_Generate(MessageToSend, MESSAGE);
  Esp32MQTTClient_Event_AddProp(message, "SensorType", "temperature");
  Esp32MQTTClient_Event_AddProp(message, "DeviceModel", "ESP32S");
  Esp32MQTTClient_Event_AddProp(message, "Vendor", "unknown");
  Esp32MQTTClient_Event_AddProp(message, "Longitude", String(longitude).c_str());
  Esp32MQTTClient_Event_AddProp(message, "Latitude", String(latitude).c_str());
  Esp32MQTTClient_Event_AddProp(message, "DeviceId", WiFi.macAddress().c_str());
  Esp32MQTTClient_SendEventInstance(message);
    Serial.print("Payload sent: ");
    Serial.println(MessageToSend);
}
