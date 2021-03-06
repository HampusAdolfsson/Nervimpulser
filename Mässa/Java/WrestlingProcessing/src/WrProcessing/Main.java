package WrProcessing;

import filter.*;
import g4p_controls.*;
import processing.core.PApplet;
import processing.serial.Serial;
import signal.Signal;
import signal.SignalCollection;

import java.io.File;
import java.io.IOException;
import java.nio.file.Files;


public class Main extends PApplet {

    private final static int SHUAICONSTANT = 128000; //baud rate
    private final static int PIXELS_PER_POINT = 2;
    private final static int PANEL_HEIGHT = 50;

    private Serial port;

    private int lastOffset = 0;
    private Float standing = 0.5f;

    private SignalCollection signals;

    public void setup() {
        surface.setResizable(true);
        clearAndDrawAxes();
        createGUI();

        signals = new SignalCollection(2, width / PIXELS_PER_POINT);
        try {
            File file = new File("init_values.conf");
            String init = new String(Files.readAllBytes(file.toPath()));
            // initiera seriell kommunikation
            port = new Serial(this, init, SHUAICONSTANT);
            port.bufferUntil('\n');
            Thread.sleep(1000);
            DataClient.start(standing);
        } catch (IOException|InterruptedException e) {
            print(e.getMessage());
        }
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
                signals.addValues((short) a1, (short) a2);
                //updateServo();
            }

        } catch (Exception e) {
            e.printStackTrace();
        }///System.err.println(e.getMessage());}
    }

    private void clearAndDrawAxes() {
        background(0);
        // axlar
        stroke(0, 0x80, 0x1A);
        line(0, (height - PANEL_HEIGHT) * 2 / 3, width, (height - PANEL_HEIGHT) * 2 / 3);
        line(0, (height - PANEL_HEIGHT) * 1 / 3, width, (height - PANEL_HEIGHT) * 1 / 3);
        line(0, height - PANEL_HEIGHT, width, height - PANEL_HEIGHT);
        line(width / 3, 0, width / 3, height - PANEL_HEIGHT);
        line(width * 2 / 3, 0, width * 2 / 3, height - PANEL_HEIGHT);
    }

    private int lastWidth;
    public void draw() {
        if (width != lastWidth) {
            clearAndDrawAxes();
            lastOffset = 0;
            signals.setSignalBufferSize(width / PIXELS_PER_POINT);

            repositionGUI();
            lastWidth = width;
        }
        else if (lastOffset > signals.offset) {
            lastOffset = 0;
            clearAndDrawAxes();
        }
        while (lastOffset < signals.offset) {
            if (lastOffset > 0){
                stroke(0, 0xBB, 0xFF);
                int vertOffs = signals.getVerticalOffset(0);
                line(PIXELS_PER_POINT * (lastOffset - 1), Math.round(map(signals.getValues(0)[lastOffset - 1], 0, 1023, height - PANEL_HEIGHT, 0) + vertOffs),
                        PIXELS_PER_POINT * (lastOffset), Math.round(map(signals.getValues(0)[lastOffset], 0, 1023, height - PANEL_HEIGHT, 0)) + vertOffs);

                vertOffs = signals.getVerticalOffset(1);
                stroke(0xFC, 0x00, 0x82);
                line(PIXELS_PER_POINT * (lastOffset - 1), Math.round(map(signals.getValues(1)[lastOffset - 1], 0, 1023, height - PANEL_HEIGHT, 0) + vertOffs),
                        PIXELS_PER_POINT * (lastOffset), Math.round(map(signals.getValues(1)[lastOffset], 0, 1023, height - PANEL_HEIGHT, 0)) + vertOffs);
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

    public void keyPressed() {
        switch (key) {
            case 'u':
                signals.incrementVerticalOffset(0, (short) 10);
                break;
            case 'j':
                signals.incrementVerticalOffset(0, (short) -10);
                break;
            case 'i':
                signals.incrementVerticalOffset(1, (short) 10);
                break;
            case 'k':
                signals.incrementVerticalOffset(1, (short) -10);
                break;
        }
    }

    static final int THRESHOLD = 100;

    public void updateServo() {
    /*byte degrees = 0;
    if (values1[offset] >= THRESHOLD) {
      degrees = (byte) Math.round(values1[offset] * 180 / 1023);
    }
    port.write(degrees + '\n');*/
        port.write(0 + "\n");

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
        signals.setFilterType(Filter.FilterType.Mean);
    } //_CODE_:option_mean:495889:

    public void option_median_clicked(GOption source, GEvent event) { //_CODE_:option_median:750489:
        signals.setFilterType(Filter.FilterType.Median);
    } //_CODE_:option_median:750489:

    public void option_mode_clicked(GOption source, GEvent event) { //_CODE_:option_mode:559635:
        signals.setFilterType(Filter.FilterType.Mode);
    } //_CODE_:option_mode:559635:

    public void option_empty_clicked(GOption source, GEvent event) { //_CODE_:option_empty:776717:
        signals.setFilterType(Filter.FilterType.Empty);
    } //_CODE_:option_empty:776717:

    public void option_weigthm_clicked(GOption source, GEvent event) { //_CODE_:option_weightm:808971:
        signals.setFilterType(Filter.FilterType.WeigthedMean);
    } //_CODE_:option_weightm:808971:

    public void textfield_window_change(GTextField source, GEvent event) { //_CODE_:textfield_window:342111:
        if (event != GEvent.CHANGED) return;
        try {
            int size = Integer.parseInt(source.getText());
            signals.setFilterSize(size);
        } catch (Exception e) {
            if (e instanceof NumberFormatException) System.err.println("Invalid number");
            else e.printStackTrace();
        }
    } //_CODE_:textfield_window:342111:

    public void button_calibrate_clicked(GButton source, GEvent event) {
        //
    }

    // Create all the GUI controls.
// autogenerated do not edit
    public void createGUI() {
        G4P.messagesEnabled(false);
        G4P.setGlobalColorScheme(GCScheme.BLUE_SCHEME);
        G4P.setCursor(ARROW);
        surface.setTitle("Nervimpulser");
        togGroup1 = new GToggleGroup();
        option_mean = new GOption(this, 270, height - 30, 120, 20);
        option_mean.setTextAlign(GAlign.LEFT, GAlign.MIDDLE);
        option_mean.setText("Mean Filter");
        option_mean.setTextBold();
        option_mean.setOpaque(false);
        option_mean.addEventHandler(this, "option_mean_clicked");
        option_median = new GOption(this, 140, height - 30, 120, 20);
        option_median.setTextAlign(GAlign.LEFT, GAlign.MIDDLE);
        option_median.setText("Median Filter");
        option_median.setTextBold();
        option_median.setOpaque(false);
        option_median.addEventHandler(this, "option_median_clicked");
        option_mode = new GOption(this, 10, height - 30, 120, 20);
        option_mode.setTextAlign(GAlign.LEFT, GAlign.MIDDLE);
        option_mode.setText("MM Filter");
        option_mode.setTextBold();
        option_mode.setOpaque(false);
        option_mode.addEventHandler(this, "option_mode_clicked");
        option_empty = new GOption(this, 530, height - 30, 120, 20);
        option_empty.setTextAlign(GAlign.LEFT, GAlign.MIDDLE);
        option_empty.setText("#Nofilter");
        option_empty.setTextBold();
        option_empty.setOpaque(false);
        option_empty.addEventHandler(this, "option_empty_clicked");
        option_weightm = new GOption(this, 400, height - 30, 120, 20);
        option_weightm.setTextAlign(GAlign.LEFT, GAlign.MIDDLE);
        option_weightm.setText("Weighted Mean");
        option_weightm.setTextBold();
        option_weightm.setOpaque(false);
        option_weightm.addEventHandler(this, "option_weigthm_clicked");
        togGroup1.addControl(option_mean);
        option_mean.setSelected(true);
        togGroup1.addControl(option_median);
        togGroup1.addControl(option_mode);
        togGroup1.addControl(option_empty);
        togGroup1.addControl(option_weightm);
        label_window = new GLabel(this, 670, height - 30, 80, 20);
        label_window.setText("Window size:");
        label_window.setTextBold();
        label_window.setOpaque(false);
        textfield_window = new GTextField(this, 750, height - 30, 60, 20, G4P.SCROLLBARS_NONE);
        textfield_window.setText(""+ Signal.DEFAULT_WINDOW_SIZE);
        textfield_window.setOpaque(true);
        textfield_window.addEventHandler(this, "textfield_window_change");
        button_calibrate = new GButton(this, width - 85, height - 35, 80, 30);
        button_calibrate.setText("Calibrate");
        button_calibrate.addEventHandler(this, "button_calibrate_clicked");
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
    GButton button_calibrate;

    private void repositionGUI() {
        option_mean.moveTo(270, height - 30);
        option_median.moveTo(140, height - 30);
        option_mode.moveTo(10, height - 30);
        option_empty.moveTo(530, height - 30);
        option_weightm.moveTo(400, height - 30);
        label_window.moveTo(670, height - 30);
        textfield_window.moveTo(750, height - 30);
        button_calibrate.moveTo(width - 85, height - 35);
    }

    public void settings() {
        size(1000, 650, JAVA2D);
    }

    static public void main(String[] passedArgs) {
        String[] appletArgs = new String[]{"WrProcessing.Main"};
        if (passedArgs != null) {
            PApplet.main(concat(appletArgs, passedArgs));
        } else {
            PApplet.main(appletArgs);
        }
    }
}
