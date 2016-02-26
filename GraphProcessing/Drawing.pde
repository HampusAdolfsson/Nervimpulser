void draw() {
  background(0);
  
  // axlar
  stroke(0, 0x80, 0x1A);
  line(0, (height- PANEL_HEIGHT) * 2/3, width, (height- PANEL_HEIGHT) * 2/3);
  line(0, (height- PANEL_HEIGHT) * 1/3, width, (height- PANEL_HEIGHT) * 1/3);
  line(0, height - PANEL_HEIGHT, width, height - PANEL_HEIGHT);
  line(width - (PIXELS_PER_POINT*offset % (width/2)), 0, width - (PIXELS_PER_POINT*offset % (width/2)), height - PANEL_HEIGHT);
  line(width/2 - (PIXELS_PER_POINT*offset % (width/2)), 0, width/2 - (PIXELS_PER_POINT*offset % (width/2)), height - PANEL_HEIGHT);
  stroke(0, 0xBB, 0xFF);
  for(int i = offset; i - values.length != offset - 1; i++) {
      line(PIXELS_PER_POINT*(i - offset), values[i % values.length],
            PIXELS_PER_POINT*(i - offset + 1), values[(i + 1) % values.length]);
  }
}