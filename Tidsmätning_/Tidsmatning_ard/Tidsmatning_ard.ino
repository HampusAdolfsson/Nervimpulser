void setup() {
  // put your setup code here, to run once:
  Serial.begin(9600);
}
static byte separator = -1;
int value;
void loop() {
  // put your main code here, to run repeatedly:
  value = analogRead(0);
  int time = millis();
  //String s = value + "" + separator + time;
  Serial.println(time);
  delay(10);
}
