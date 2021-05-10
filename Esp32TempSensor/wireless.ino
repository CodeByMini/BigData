void initWifi(char* ssid, char* pass) {
  WiFi.begin(ssid, pass);

  Serial.println("Connecting WiFi...");
  while(WiFi.status() != WL_CONNECTED) {
    delay(1000);
    Serial.print(".");
  }
  Serial.println("Connected!");
  Serial.print("\nIP Address: ");
  Serial.println(WiFi.localIP());
}
