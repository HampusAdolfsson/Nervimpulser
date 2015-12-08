void setup() {
  // put your setup code here, to run once:
  Serial.begin(9600);
}
int value;
void loop() {
  // put your main code here, to run repeatedly:
  value = analogRead(0);
  String s = value + "," + millis();
  Serial.println(millis());
  delay(100);
}
