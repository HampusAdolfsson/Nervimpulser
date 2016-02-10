#define PIN 2
#define HIGHRES true

void setup()
{
  #if HIGHRES
    analogReference(INTERNAL);
  #endif
  Serial.begin(9600);
}

void loop()
{
  Serial.println(analogRead(PIN));
  delay(5);
}
