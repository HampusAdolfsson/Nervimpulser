import processing.core.*; 
import processing.data.*; 
import processing.event.*; 
import processing.opengl.*; 

import g4p_controls.*; 
import java.io.FileOutputStream; 
import processing.serial.*; 
import java.util.Random; 

import java.util.HashMap; 
import java.util.ArrayList; 
import java.io.File; 
import java.io.BufferedReader; 
import java.io.PrintWriter; 
import java.io.InputStream; 
import java.io.OutputStream; 
import java.io.IOException; 

public class WrestlingProcessing extends PApplet {






final static int SHUAICONSTANT = 9600; //baud rate
final static int UPDATE_INTERVAL = 5; //intervall mellan m\u00e4tningar i ms
final static int PIXELS_PER_POINT = 2;
final static int PANEL_HEIGHT = 50;

Serial port;

int windowSize = 10;
Filter filter1 = new MeanFilter(windowSize);
Filter filter2 = new MeanFilter(windowSize);
int offset = 0;
short values1[];
short values2[];

public void setup() {
  
  createGUI();
  values1 = new short[width/PIXELS_PER_POINT];
  values2 = new short[width/PIXELS_PER_POINT];
  
  // initiera seriell kommunikation
  port = new Serial(this, "COM3", SHUAICONSTANT);
  port.bufferUntil('\n');
}


public void serialEvent(Serial port) {
  try {
  // avl\u00e4s och spara v\u00e4rde
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

Random random = new Random();
public void draw() {
  background(0);
  
  // axlar
  stroke(0, 0x80, 0x1A);
  line(0, (height- PANEL_HEIGHT) * 2/3, width, (height- PANEL_HEIGHT) * 2/3);
  line(0, (height- PANEL_HEIGHT) * 1/3, width, (height- PANEL_HEIGHT) * 1/3);
  line(0, height - PANEL_HEIGHT, width, height - PANEL_HEIGHT);
  line(width - (PIXELS_PER_POINT*offset % (width/2)), 0, width - (PIXELS_PER_POINT*offset % (width/2)), height - PANEL_HEIGHT);
  line(width/2 - (PIXELS_PER_POINT*offset % (width/2)), 0, width/2 - (PIXELS_PER_POINT*offset % (width/2)), height - PANEL_HEIGHT);
  stroke(0, 0xBB, 0xFF);
  for(int i = offset; i - values1.length != offset - 1; i++) {
      line(PIXELS_PER_POINT*(i - offset), values1[i % values1.length],
            PIXELS_PER_POINT*(i - offset + 1), values1[(i + 1) % values1.length]);
  }
  
  stroke(0xFC, 0x00, 0x82);
  for(int i = offset; i - values2.length != offset - 1; i++) {
      line(PIXELS_PER_POINT*(i - offset), values2[i % values2.length],
            PIXELS_PER_POINT*(i - offset + 1), values2[(i + 1) % values2.length]);
  }
  
}
class EmptyFilter extends Filter {
  
  EmptyFilter(){
    super(1);  
  }
  
  public @Override
  short getNext(short next) {
    return next;
  }
}
abstract class Filter {
  short[] buffer;
  
  Filter(int num_values) {
    buffer = new short[num_values];
  }
  
  // r\u00e4kna ut n\u00e4sta v\u00e4rde baserat p\u00e5 next och v\u00e4rdena i buffern
  public abstract short getNext(short next);
}
class MeanFilter extends Filter {
  int sum = 0;
  int offset = 0;
  
  MeanFilter(int num_values) {
    super(num_values);
  }
  
  public @Override
  short getNext(short next) {
    sum += next;
    sum -= buffer[offset];
    buffer[offset] = next;
    if (++offset == buffer.length) offset = 0;
    return (short) Math.round((float) sum / buffer.length);
  }
}
class MedianFilter extends Filter {
  short sortedBuffer[];
  int offset;
  
  MedianFilter(int num_values) {
    super(num_values);  
    sortedBuffer = new short[num_values];
  }
  
  public @Override
  short getNext(short next) {
    // ta bort det \u00e4ldsta v\u00e4rdet, s\u00e4tt in next i den sorterade arrayen
    boolean move = false;
    if (next < buffer[offset]) {
        int n = sortedBuffer.length - 1;
        while (n >= 0 && next < sortedBuffer[n]) {
            if (move) sortedBuffer[n+1] = sortedBuffer[n];
            else if (sortedBuffer[n] == buffer[offset]) move = true;
            n--;
        }
        sortedBuffer[n+1] = next;
    } else if (next > buffer[offset]) {
        int n = 0;
        while (n < sortedBuffer.length && next > sortedBuffer[n]) {
            if (move) sortedBuffer[n-1] = sortedBuffer[n];
            else if (sortedBuffer[n] == buffer[offset]) move = true;
            n++;
        }
        sortedBuffer[n - 1] = next;
    }
    buffer[offset] = next;
    if (++offset == sortedBuffer.length) offset = 0;
    if (buffer.length % 2 == 1) return buffer[(buffer.length - 1)/2];
    return (short)((buffer[buffer.length / 2] + buffer[buffer.length/2-1]) / 2);
  }
}
// ber\u00e4knar medelv\u00e4rdet av ett antal mediantal
class ModeFilter extends Filter {
  short sortedBuffer[];
  int offset;
  
  ModeFilter(int num_values) {
    super(num_values);
    sortedBuffer = new short[buffer.length];
  }
  
  public short getNext(short next) {
    if (buffer.length < 5) return next;
    // ta bort det \u00e4ldsta v\u00e4rdet, s\u00e4tt in next i den sorterade arrayen
    boolean move = false;
    if (next < buffer[offset]){
        int n = sortedBuffer.length - 1;
        while (n >= 0 && next < sortedBuffer[n]) {
            if (move) sortedBuffer[n+1] = sortedBuffer[n];
            else if (sortedBuffer[n] == buffer[offset]) move = true;
            n--;
        }
        sortedBuffer[n+1] = next;
    } else if (next > buffer[offset]){
        int n = 0;
        while (n < sortedBuffer.length && next > sortedBuffer[n]){
            if (move) sortedBuffer[n-1] = sortedBuffer[n];
            else if (sortedBuffer[n] == buffer[offset]) move = true;
            n++;
        }
        sortedBuffer[n - 1] = next;
    };
    buffer[offset] = next;
    if (++offset == sortedBuffer.length) offset = 0;
    
    // ta medelv\u00e4rdet av de mittersta sorterade v\u00e4rdena
    int sum = 0;
    if (buffer.length % 2 == 1) {
      for (int i = (buffer.length - 1)/ 2 - 2; i <= (buffer.length - 1)/ 2 + 2; i++) {
        sum += sortedBuffer[i];  
      }
      return (short) Math.round(buffer.length / 5.0f);
    }
    for (int i = buffer.length/2 - 2; i < buffer.length/2 + 2; i++) {
        sum += sortedBuffer[i];
    }
    return (short) Math.round(sum / 4.0f);
    
  }
  
}
static final int THRESHOLD = 100;

public void updateServo() {
    /*byte degrees = 0;
    if (values1[offset] >= THRESHOLD) {
      degrees = (byte) Math.round(values1[offset] * 180 / 1023);
    }
    port.write(degrees + '\n');*/
    port.write(0 + '\n');  
    
}
// https://en.wikipedia.org/wiki/Moving_average#Weighted_moving_average
class WeightedMeanFilter extends Filter {
  int sum, numerator;
  int denominator;
  
  WeightedMeanFilter(int num_values) {
    super(num_values);
    denominator = num_values * (num_values - 1) / 2; //n + (n - 1) + ... + 2 + 1
  }
  
  public short getNext(short next) {
    numerator += (next * buffer.length) - sum;
    
    sum += next - buffer[offset];
    buffer[offset] = next;
    if (++offset == buffer.length) offset = 0;
    
    return (short) Math.round((float)numerator / denominator);
  }
  
  
}
/* =========================================================
 * ====                   WARNING                        ===
 * =========================================================
 * The code in this tab has been generated from the GUI form
 * designer and care should be taken when editing this file.
 * Only add/edit code inside the event handlers i.e. only
 * use lines between the matching comment tags. e.g.

 void myBtnEvents(GButton button) { //_CODE_:button1:12356:
     // It is safe to enter your event code here  
 } //_CODE_:button1:12356:
 
 * Do not rename this tab!
 * =========================================================
 */

public void option_mean_clicked(GOption source, GEvent event) { //_CODE_:option_mean:495889:
  filter1 = new MeanFilter(windowSize);
  filter2 = new MeanFilter(windowSize);
} //_CODE_:option_mean:495889:

public void option_median_clicked(GOption source, GEvent event) { //_CODE_:option_median:750489:
  filter1 = new MedianFilter(windowSize);
  filter2 = new MedianFilter(windowSize);
} //_CODE_:option_median:750489:

public void option_mode_clicked(GOption source, GEvent event) { //_CODE_:option_mode:559635:
  filter1 = new ModeFilter(windowSize);
  filter2 = new ModeFilter(windowSize);
} //_CODE_:option_mode:559635:

public void option_empty_clicked(GOption source, GEvent event) { //_CODE_:option_empty:776717:
  filter1 = new EmptyFilter();
  filter2 = new EmptyFilter();
} //_CODE_:option_empty:776717:

public void option_weigthm_clicked(GOption source, GEvent event) { //_CODE_:option_weightm:808971:
  filter1 = new WeightedMeanFilter(windowSize);
  filter2 = new WeightedMeanFilter(windowSize);
} //_CODE_:option_weightm:808971:

public void textfield_window_change(GTextField source, GEvent event) { //_CODE_:textfield_window:342111:
  if (event != GEvent.CHANGED) return;
  try {
    windowSize = Integer.parseInt(source.getText());
    filter1 = (Filter) filter1.getClass().getConstructors()[0].newInstance(this, windowSize);
    filter2 = (Filter) filter2.getClass().getConstructors()[0].newInstance(this, windowSize);
  } catch (Exception e) {
    if (e instanceof NumberFormatException) System.err.println("Invalid number");
    else e.printStackTrace();  
  }
} //_CODE_:textfield_window:342111:



// Create all the GUI controls. 
// autogenerated do not edit
public void createGUI(){
  G4P.messagesEnabled(false);
  G4P.setGlobalColorScheme(GCScheme.BLUE_SCHEME);
  G4P.setCursor(ARROW);
  surface.setTitle("Nervimpulser");
  togGroup1 = new GToggleGroup();
  option_mean = new GOption(this, 270, 620, 120, 20);
  option_mean.setTextAlign(GAlign.LEFT, GAlign.MIDDLE);
  option_mean.setText("Mean Filter");
  option_mean.setTextBold();
  option_mean.setOpaque(false);
  option_mean.addEventHandler(this, "option_mean_clicked");
  option_median = new GOption(this, 140, 620, 120, 20);
  option_median.setTextAlign(GAlign.LEFT, GAlign.MIDDLE);
  option_median.setText("Median Filter");
  option_median.setTextBold();
  option_median.setOpaque(false);
  option_median.addEventHandler(this, "option_median_clicked");
  option_mode = new GOption(this, 10, 620, 120, 20);
  option_mode.setTextAlign(GAlign.LEFT, GAlign.MIDDLE);
  option_mode.setText("Mode Filter");
  option_mode.setTextBold();
  option_mode.setOpaque(false);
  option_mode.addEventHandler(this, "option_mode_clicked");
  option_empty = new GOption(this, 530, 620, 120, 20);
  option_empty.setTextAlign(GAlign.LEFT, GAlign.MIDDLE);
  option_empty.setText("#Nofilter");
  option_empty.setTextBold();
  option_empty.setOpaque(false);
  option_empty.addEventHandler(this, "option_empty_clicked");
  option_weightm = new GOption(this, 400, 620, 120, 20);
  option_weightm.setTextAlign(GAlign.LEFT, GAlign.MIDDLE);
  option_weightm.setText("Weighted Mean");
  option_weightm.setTextBold();
  option_weightm.setLocalColorScheme(GCScheme.BLUE_SCHEME);
  option_weightm.setOpaque(false);
  option_weightm.addEventHandler(this, "option_weigthm_clicked");
  togGroup1.addControl(option_mean);
  option_mean.setSelected(true);
  togGroup1.addControl(option_median);
  togGroup1.addControl(option_mode);
  togGroup1.addControl(option_empty);
  togGroup1.addControl(option_weightm);
  label_window = new GLabel(this, 670, 620, 80, 20);
  label_window.setText("Window size:");
  label_window.setTextBold();
  label_window.setOpaque(false);
  textfield_window = new GTextField(this, 750, 620, 60, 20, G4P.SCROLLBARS_NONE);
  textfield_window.setText(""+windowSize);
  textfield_window.setOpaque(true);
  textfield_window.addEventHandler(this, "textfield_window_change");
}

// Variable declarations 
// autogenerated do not edit
GToggleGroup togGroup1; 
GOption option_mean; 
GOption option_median; 
GOption option_mode; 
GOption option_empty; 
GOption option_weightm; 
GLabel label_window; 
GTextField textfield_window; 
  public void settings() {  size(1000, 650, JAVA2D); }
  static public void main(String[] passedArgs) {
    String[] appletArgs = new String[] { "WrestlingProcessing" };
    if (passedArgs != null) {
      PApplet.main(concat(appletArgs, passedArgs));
    } else {
      PApplet.main(appletArgs);
    }
  }
}
