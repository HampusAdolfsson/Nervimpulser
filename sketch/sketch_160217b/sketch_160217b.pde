import interfascia.*;

GUIController c;
IFRadioController controller;
IFRadioButton b1, b2;

void setup() {
  size(400, 300);
  background(200);
  
  c = new GUIController(this);
  controller = new IFRadioController();
  b1 = new IFRadioButton("btn1", 25, 25, controller);
  b2 = new IFRadioButton("btn2", 25, 70, controller);
  
  c.add(b1);
  c.add(b2);
  
}

void draw() {
  
}