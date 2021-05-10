void SerializeData(char *outgoing, float temperature, float humidity) {
    DynamicJsonDocument doc(MAX_MESSAGE_SIZE);
    doc["temperature"] = temperature;
     doc["humidity"] = humidity;
     doc["logtime"] = getTime();
     if(temperature >= 27){
       doc["tempAlert"] = true;
     }else{
       doc["tempAlert"] = false;
     }
    char tempBuffer[MAX_MESSAGE_SIZE];
    serializeJson(doc, tempBuffer);
    strcpy(outgoing, tempBuffer);
    delay(10);
}

unsigned long getTime(){
  time_t now;
  struct tm timeinfo;
  if (!getLocalTime(&timeinfo)) {
    return(0);
  }
  time(&now);
  return now;
}