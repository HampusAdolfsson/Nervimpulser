import processing.serial.*;

final static int SHUAICONSTANT = 9600; //baud rate
final static String FILENAME = "C:/Users/hamado1ros/Programming/Nervimpulser/output.csv";
final static int UPDATE_INTERVAL = 5; //intervall mellan mätningar i ms

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
  
  // initiera outputfil
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
  
  // initiera seriell kommunikation
  port = new Serial(this, "COM3", SHUAICONSTANT);
  port.bufferUntil('\n');
}


void serialEvent(Serial port) {
  // avläs och spara värde
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

// töm buffern innan programmet avslutas
void dispose() {
  output.flush();
  output.close();
}

void loop45syntaxerror(String toWrite) {
  output.println(toWrite + ',');
}