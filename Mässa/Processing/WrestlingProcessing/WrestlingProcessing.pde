import g4p_controls.*;
import java.io.FileOutputStream;
import processing.serial.*;
import java.util.Random;

final static int SHUAICONSTANT = 9600; //baud rate
final static int UPDATE_INTERVAL = 5; //intervall mellan mätningar i ms
final static int PIXELS_PER_POINT = 2;
final static int PANEL_HEIGHT = 50;

Serial port;

int windowSize = 10;
Filter filter1 = new MeanFilter(windowSize);
Filter filter2 = new MeanFilter(windowSize);
int offset = 0;
short values1[];
short values2[];

void setup() {
  size(1000, 650, JAVA2D);
  createGUI();
  values1 = new short[width/PIXELS_PER_POINT];
  values2 = new short[width/PIXELS_PER_POINT];
  
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
      String[] parts = inString.split(".");
      short short1 = Short.parseShort(parts[0]);
      short short2 = Short.parseShort(parts[1]);
      if (short1 < 1024 && short2 < 1024) {
        short1 = filter1.getNext(short1);
        short2 = filter1.getNext(short2);
        values1[offset] = (short) Math.round(map(short1, 0, 1023, height - PANEL_HEIGHT, 0));
        values2[offset] = (short) Math.round(map(short2, 0, 1023, height - PANEL_HEIGHT, 0));
        println(values1[offset] + "." + values2[offset]);
        //updateServo();    
        if (++offset == values1.length) offset = 0;
      }
    }
  } catch (NumberFormatException e){ System.err.println(e.getMessage());}
}