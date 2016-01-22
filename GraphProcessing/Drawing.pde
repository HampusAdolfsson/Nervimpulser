void draw() {
  background(0);
  
  // axlar
  line(0, height * 2/3, width, height * 2/3);
  line(0, height * 1/3, width, height * 1/3);
  line(width - (2*offset % (width/2)), 0, width - (2*offset % (width/2)), height);
  line(width/2 - (2*offset % (width/2)), 0, width/2 - (2*offset % (width/2)), height);
  
  for(int i = offset; i % values.length != offset - 1; i++) {  
      if (i > 0) line(PIXELS_PER_POINT*(i - 1 - offset), filter.processValues(values, (i - 1) % values.length), PIXELS_PER_POINT*(i - offset), values[i % values.length]);
  }
}