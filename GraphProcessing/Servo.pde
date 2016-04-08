void updateServo() {
    byte degrees = 0;
    if (values[offset] >= threshold) {
      degrees = (byte) Math.round(values[offset] * 180 / 1023);
    }
    port.write(degrees + '\n');
    
}