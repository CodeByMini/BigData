#define DIFF 0.5
#define DHT_PIN 4
#define DHT_TYPE DHT11
#define SECONDSBETWEEN 3
#define MAX_MESSAGE_SIZE 256

#include "include.h"

DHT dht(DHT_PIN, DHT_TYPE);

char* ssid = "";
char* pass = "";

char* conn = "";
bool  isConnectedToIotHub = false;

const char* ntpServer = "pool.ntp.org";

float longitude;
float latitude;
unsigned long epochTime;

float lastreading = 0;
float difference = 1;

void setup(){
Serial.begin(115200);
delay(900);
Serial.println("This is your ESP32TempSensor speaking to Azure");
initWifi(ssid, pass);
initIotDevice(conn);
dht.begin();
GetLocation();
configTime(0, 0, ntpServer);
}

void loop() {
    float temperature = dht.readTemperature();
    float humidity = dht.readHumidity();

    if(!isnan(temperature) && !isnan(humidity)) {
      if(temperature >= (lastreading+difference) || temperature <= (lastreading-difference)){
        lastreading = temperature;
        char payload[MAX_MESSAGE_SIZE];
        SerializeData(payload, temperature, humidity);
        SendIotMessage(payload);
        Serial.println("................................");
      }
    }
  delay(SECONDSBETWEEN * 1000);  
} 



