
int value;
int pin = 0;

void setup()
{
  value = 0;
  pinMode(pin, INPUT);
  Serial.begin(9600);
}

void loop()
{
  value = analogRead(pin);

  Serial.println(value);
  delay(10);
}

