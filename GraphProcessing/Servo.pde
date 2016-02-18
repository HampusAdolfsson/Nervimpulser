static final int THRESHOLD = 100;

void updateServo() {
    byte degrees = 0;
    if (values[offset] >= THRESHOLD) {
      degrees = (byte) Math.round(values[offset] * 180 / 1023);
    }
    port.write(degrees + '\n');
    
}