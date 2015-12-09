void draw() {
  background(0);
  line(0, height * 2/3, width, height * 2/3);
  line(0, height * 1/3, width, height * 1/3);
  for(short value: values) {
    point(width - (offset % width), value);
  }
  line();
  //xPos++;
}