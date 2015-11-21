import processing.serial.*;

int bps = 9600;

Serial port;
int xPos;
boolean b = false;

void setup() {
  size(800, 600);
  noStroke();
  background(0);
  port = new Serial(this, "COM4", bps);
  port.bufferUntil('\n');
}

void draw() {
  if (b) {
    background(0);
    b = false;
  }
  line(xPos, height, xPos, inByte);
  //xPos++;
}
float inByte =0 ;
void serialEvent(Serial port) {
  String inString = port.readStringUntil('\n');
  if (inString != null) {
    inString = trim(inString);
    inByte = float(inString);
    if (inByte < 1024) {
      inByte = map(inByte, 0, 1023, 0, height);
      stroke(127, 34, 255);

      if (xPos >= width) {
        xPos = 0;
        b = true;
      } else xPos++;
    }
  }
}