import processing.serial.*;

final static int SHUAICONSTANT = 9600; //baud rate
final static String FILENAME = "C:/Users/hamado1ros/Programming/Nervimpulser/output.txt";
final static int UPDATE_INTERVAL = 100;

Serial port;
PrintWriter output;

int offset;
short values[];

void setup() {
  size(800, 600);
  noStroke();
  stroke(127, 34, 255);
  background(0);
  frameRate(1000 / UPDATE_INTERVAL);
  
  values = new short[width];
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
void serialEvent(Serial port) {
  String inString = port.readStringUntil('\n');
  if (inString != null) {
    inString = trim(inString);
    float inByte = float(inString);
    loop45syntaxerror(inString);
    if (inByte < 1024) {
      values[offset] = (short) Math.round(map(inByte, 0, 1023, height, 0));  
      if (offset == values.length - 1) offset = 0;
      else offset++;
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