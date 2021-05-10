#include <WifiLocation.h>

const char* googleApiKey = "";

WifiLocation location(googleApiKey);

void GetLocation() {
    location_t loc = location.getGeoFromWiFi();

    Serial.println("Location request data");
    Serial.println(location.getSurroundingWiFiJson());
    Serial.println("Latitude: " + String(loc.lat, 7));
    Serial.println("Longitude: " + String(loc.lon, 7));
    Serial.println("Accuracy: " + String(loc.accuracy));
    longitude = loc.lon; 
    latitude = loc.lat;
}
