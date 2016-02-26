static final int THRESHOLD = 100;

void updateServo() {
    /*byte degrees = 0;
    if (values1[offset] >= THRESHOLD) {
      degrees = (byte) Math.round(values1[offset] * 180 / 1023);
    }
    port.write(degrees + '\n');*/
    port.write(0 + '\n');  
    
}