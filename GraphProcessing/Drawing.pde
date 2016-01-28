void draw() {
  background(0);
  
  // axlar
  line(0, height * 2/3, width, height * 2/3);
  line(0, height * 1/3, width, height * 1/3);
  line(width - (PIXELS_PER_POINT*offset % (width/2)), 0, width - (PIXELS_PER_POINT*offset % (width/2)), height);
  line(width/2 - (PIXELS_PER_POINT*offset % (width/2)), 0, width/2 - (PIXELS_PER_POINT*offset % (width/2)), height);
  for(int i = offset; i % values.length != offset - 1; i++) {  
    if (i == offset) println(i - 1 - offset + ", " + (i-1) + ", " + offset);
    else if (i == offset+1) println(i - 1 - offset + ", " + (i-1) + "; " + offset);
      line(PIXELS_PER_POINT*(i - offset), values[i % values.length],
            PIXELS_PER_POINT*(i - offset + 1), values[i + 1]);
  }
}