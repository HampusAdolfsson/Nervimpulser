import filter.*;
import g4p_controls.*;
import processing.core.PApplet;
import processing.serial.Serial;

import javax.xml.crypto.Data;
import java.io.File;
import java.io.IOException;
import java.nio.file.Files;


public class Main extends PApplet {

    final static int SHUAICONSTANT = 128000; //baud rate
    final static int UPDATE_INTERVAL = 5; //intervall mellan m\u00e4tningar i ms
    final static int PIXELS_PER_POINT = 2;
    final static int PANEL_HEIGHT = 50;

    Serial port;

    int windowSize = 10;
    Filter filter1 = new MeanFilter(windowSize);
    Filter filter2 = new MeanFilter(windowSize);


    int offset = 0;
    int lastOffset = 0;
    short values1[];
    short values2[];
    Float standing = 0.5f;

    public void setup() {
        clearAndDrawAxes();
        createGUI();
        values1 = new short[width / PIXELS_PER_POINT];
        values2 = new short[width / PIXELS_PER_POINT];

        try {
            DataClient.start(standing);
            File file = new File("init_values.conf");
            String init = new String(Files.readAllBytes(file.toPath()));
            // initiera seriell kommunikation
            port = new Serial(this, init, SHUAICONSTANT);
            port.bufferUntil('\n');
            //port.buffer(4);
        } catch (IOException e) {
            e.printStackTrace();
        }
        double c = 1 * Math.pow(10, -6);
        double x = 80600;
        print(1/(2* Math.PI*x*c));
    }


    public void serialEvent(Serial port) {
        try {
            String string = port.readStringUntil('\n');
            int in = Integer.parseInt(trim(string));

            byte[] bytes = new byte[4];
            bytes[0] = (byte) (in & 0xFF);
            bytes[1] = (byte) ((in >> 8) & 0xFF);
            bytes[2] = (byte) ((in >> 16) & 0xFF);
            bytes[3] = (byte) ((in >> 24) & 0xFF);

            int a1 = bytes[0] & 0xFF | (bytes[1] & 0xFF) << 8;
            int a2 = bytes[2] & 0xFF | (bytes[3] & 0xFF) << 8;
            if (a1 < 1024 && a2 < 1024) {
                a1 = filter1.getNext((short) a1);
                a2 = filter2.getNext((short) a2);
                values1[offset] = (short) Math.round(map(a1, 0, 1023, height - PANEL_HEIGHT, 0));
                values2[offset] = (short) Math.round(map(a2, 0, 1023, height - PANEL_HEIGHT, 0));
                DataClient.send(a1 + "." + a2);
                updateServo();
                if (++offset == values1.length) {
                    offset = 0;
                }
            }

        } catch (Exception e) {
            e.printStackTrace();
        }///System.err.println(e.getMessage());}
    }

    public void clearAndDrawAxes() {
        background(0);
        // axlar
        stroke(0, 0x80, 0x1A);
        line(0, (height - PANEL_HEIGHT) * 2 / 3, width, (height - PANEL_HEIGHT) * 2 / 3);
        line(0, (height - PANEL_HEIGHT) * 1 / 3, width, (height - PANEL_HEIGHT) * 1 / 3);
        line(0, height - PANEL_HEIGHT, width, height - PANEL_HEIGHT);
        line(width / 2, 0, width / 2, height - PANEL_HEIGHT);
    }

    public void draw() {
        if (lastOffset > offset) {
            lastOffset = 0;
            clearAndDrawAxes();
        }
        while (lastOffset < offset) {
            if (lastOffset > 0){
                stroke(0, 0xBB, 0xFF);
                line(PIXELS_PER_POINT * (lastOffset - 1), values1[(lastOffset - 1)],
                        PIXELS_PER_POINT * (lastOffset), values1[(lastOffset)]);

                stroke(0xFC, 0x00, 0x82);
                line(PIXELS_PER_POINT * (lastOffset - 1), values2[(lastOffset - 1)],
                        PIXELS_PER_POINT * (lastOffset), values2[(lastOffset)]);
            }
            //println(lastOffset + " " + offset);
            //line(lastOffset, values1[lastOffset], lastOffset, 0);
            lastOffset++;
        }

        /*for (int i = 0; i < offset - lastOffset; i++) {
            line(PIXELS_PER_POINT * (lastOffset + i), values2[(lastOffset + i) % values2.length],
                    PIXELS_PER_POINT * (lastOffset + i + 1), values2[(lastOffset + i + 1) % values2.length]);
        }*/

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
    public void createGUI() {
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
        textfield_window.setText("" + windowSize);
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

    public void settings() {
        size(1000, 650, JAVA2D);
    }

    static public void main(String[] passedArgs) {
        String[] appletArgs = new String[]{"Main"};
        if (passedArgs != null) {
            PApplet.main(concat(appletArgs, passedArgs));
        } else {
            PApplet.main(appletArgs);
        }
    }
}
