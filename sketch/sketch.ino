#include <Servo.h>

#define PIN 2
#define SERVO_PIN 9
#define HIGHRES false
#define SERVO false 

#if SERVO
  Servo servo;
#endif

void setup()
{
  #if HIGHRES
    analogReference(INTERNAL);
  #endif
  #if SERVO
    servo.attach(SERVO_PIN);
  #endif
  Serial.begin(9600);
}

void loop()
{
  Serial.println(analogRead(PIN));
  #if SERVO
    String response = Serial.readStringUntil('\n');
    if (response.length() > 0) {
      response.trim();
      servo.write(response.toInt());
    }
    delay(2);
  #else
    delay(5);
  #endif
}
