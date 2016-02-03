
int value;
int pin = 2;

void setup()
{
  value = 0;
  //pinMode(pin, INPUT);
  analogReference(INTERNAL);
  Serial.begin(9600);
}

void loop()
{
  //value = analogRead(pin);

  Serial.println(analogRead(pin));
  delay(5);
}
