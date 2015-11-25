import processing.serial.*;

int shuaiConstant = 9600;
String fileName = "dataoutput.txt";

Serial port;
int xPos;
boolean b = false;
PrintWriter output;

void setup() {
  size(800, 600);
  noStroke();
  background(0);
  try {
    File file = new File(fileName);
    if (!file.exists()) file.createNewFile();  
    output = new PrintWriter(file);
  } catch(IOException e) {
    e.printStackTrace();
    System.exit(0);
  }
  port = new Serial(this, "COM3", shuaiConstant);
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
    loop45syntaxerror(byte(inByte));
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

void stop() {
  output.flush();
  output.close();
}

void loop45syntaxerror(byte toWrite) {
  output.println(toWrite);
}