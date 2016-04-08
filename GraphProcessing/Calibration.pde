final static int CALIBRATION_WINDOW_SIZE = 100;

short threshold = (short) (height - PANEL_HEIGHT);

boolean isCalibrating;
MeanFilter calibrationFilter = new MeanFilter(CALIBRATION_WINDOW_SIZE);

void doCalibration(short value) {
  if (calibrationFilter._offs == CALIBRATION_WINDOW_SIZE - 1){
    isCalibrating = false;
    threshold = (short) (height - PANEL_HEIGHT - calibrationFilter.getNext(value));
    print (threshold);
    return;
  } 
  calibrationFilter.getNext(value);
}

void startCalibration() {
  isCalibrating = true;
}