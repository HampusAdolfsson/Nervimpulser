import processing.serial.*;

final static int SHUAICONSTANT = 9600; //baud rate
final static String FILENAME = "C:/Users/hamado1ros/Programming/Nervimpulser/output.txt";
final static int UPDATE_INTERVAL = 100;

Serial port;
int xPos;
boolean redraw = false;
PrintWriter output;

void setup() {
  size(800, 600);
  noStroke();
  stroke(127, 34, 255);
  background(0);
  frameRate(1000 / UPDATE_INTERVAL);
  try {
    File file = new File(FILENAME);
    if (!file.exists()) file.createNewFile();  
    output = new PrintWriter(file);
  } 
  catch(IOException e) {
    e.printStackTrace();
    System.exit(1);
  }
  registerMethod("dispose", this);
  port = new Serial(this, "COM3", SHUAICONSTANT);
  port.bufferUntil('\n');
}

void draw() {
  if (redraw) {
    background(0);
    redraw = false;
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
    loop45syntaxerror(inString);
    if (inByte < 1024) {
      inByte = map(inByte, 0, 1023, 0, height);

      if (xPos >= width) {
        xPos = 0;
        redraw = true;
      } else xPos++;
    }
  }
}

void dispose() {
  output.flush();
  output.close();
}

void loop45syntaxerror(String toWrite) {
  output.println(toWrite);
}