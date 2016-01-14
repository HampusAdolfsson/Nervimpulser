void draw() {
  background(0);
  line(0, height * 2/3, width, height * 2/3);
  line(0, height * 1/3, width, height * 1/3);
  
  for(int i = offset; i % values.length != offset - 1; i++) {
    if (i % 100 == 0){println("w:" + width + ", i:" + i + ", offs:" + offset + ", value:" + (i - offset) + ", mod:" + (i % values.length) + ", target:" + (offset - 1));}  
      if (i > 0) line((i - 1 - offset), values[(i - 1) % values.length], (i - offset), values[i % values.length]);
  }
}