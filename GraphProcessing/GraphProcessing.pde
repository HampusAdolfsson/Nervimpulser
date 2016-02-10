import g4p_controls.*;
import java.io.FileOutputStream;
import processing.serial.*;

final static int SHUAICONSTANT = 9600; //baud rate
final static int UPDATE_INTERVAL = 5; //intervall mellan mätningar i ms
final static int PIXELS_PER_POINT = 2;
final static int PANEL_HEIGHT = 50;

Serial port;
PrintWriter output;
FileOutputStream binOutput;

int windowSize = 10;
Filter filter = new MeanFilter(windowSize);
int offset = 0;
short values[];

void setup() {
  size(1000, 650, JAVA2D);
  //frameRate(1000 / UPDATE_INTERVAL);
  noLoop();
  createGUI();
  values = new short[width/PIXELS_PER_POINT];
  
  // initiera outputfiler
  try {
    File file = new File(sketchPath("output.csv"));
    if (!file.exists() && !file.createNewFile()) System.exit(1);
    output = new PrintWriter(file);
    
    File file2 = new File(sketchPath("WFDB/signal.dat"));
    if (!file2.exists() && !file2.createNewFile()) System.exit(1);
    binOutput = new FileOutputStream(file2, true);
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
  if (filter == null || output == null || binOutput == null || values == null) return;
  // avläs och spara värde
  String inString = port.readStringUntil('\n');
  if (inString != null && !inString.equals("")) {
    inString = trim(inString);
    short inShort = Short.parseShort(inString);
    loop45syntaxerror(inShort);
    if (inShort < 1024) {
      short s = filter.getNext(inShort);
      values[offset] = (short) Math.round(map(s, 0, 1023, height - PANEL_HEIGHT, 0));
      if (++offset == values.length) offset = 0;
    }
    redraw();
  }
  } catch (NumberFormatException e){ println(e.toString());}
}

// töm buffern innan programmet avslutas
void dispose() {
  try {
    output.close();
    binOutput.close();
  } catch (IOException e) {}
}

void loop45syntaxerror(short toWrite) {
  output.println(toWrite + ',');
  byte[] bytes = new byte[2];
  bytes[0] = (byte) (toWrite & 0xFF);
  bytes[1] = (byte) (toWrite >> 8 & 0xFF);
  try {
    binOutput.write(bytes);
  } catch(IOException e) {}
}