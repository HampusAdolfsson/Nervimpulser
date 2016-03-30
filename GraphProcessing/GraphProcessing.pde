import g4p_controls.*;
import java.io.FileOutputStream;
import processing.serial.*;

final static int SHUAICONSTANT = 9600; //baud rate
final static int UPDATE_INTERVAL = 5; //intervall mellan mätningar i ms
final static int PIXELS_PER_POINT = 2;
final static int PANEL_HEIGHT = 50;

Serial port;
PrintWriter output;

int windowSize = 10;
Filter filter = new MeanFilter(windowSize);
int offset = 0;
short values[];

void setup() {
  size(1000, 650, JAVA2D);
  createGUI();
  values = new short[width/PIXELS_PER_POINT];
  
  // initiera outputfiler
  try {
    File file = new File(sketchPath("output.csv"));
    if (!file.exists() && !file.createNewFile()) System.exit(1);
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
  try {
  // avläs och spara värde
    String inString = port.readStringUntil('\n');
    if (inString != null && !inString.equals("")) {
      inString = trim(inString);
      short inShort = Short.parseShort(inString);
      writeToFile(inShort);
      if (inShort < 1024) {
        short s = filter.getNext(inShort);
        values[offset] = (short) Math.round(map(s, 0, 1023, height - PANEL_HEIGHT, 0));
        //updateServo();    
        if (++offset == values.length) offset = 0;
      }
    }
  } catch (NumberFormatException e){ println(e.toString());}
}

// töm buffern innan programmet avslutas
void dispose() {
  output.close();
}

void writeToFile(short toWrite) {
  output.println(toWrite + ',');
}