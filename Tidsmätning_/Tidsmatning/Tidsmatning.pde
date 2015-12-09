import processing.serial.*;
Serial port;

final static String fileName = "C:/Users/hamado1ros/Programming/Nervimpulser/Tidsmätning_/output.txt";
PrintWriter output;

void setup(){
  size(300, 200);
  background(0);
  try {
    File file = new File(fileName);
    System.out.println(file.getAbsolutePath());
    if (!file.exists()) file.createNewFile();
    output = new PrintWriter(file);
  } 
  catch(IOException e) {
    e.printStackTrace();
    System.exit(1);
  }
  registerMethod("dispose", this);
  port = new Serial(this, "COM3", 9600);
  port.bufferUntil('\n');
}


void draw() {
}

void serialEvent(Serial port) {
  String inString = port.readStringUntil('\n');
  if (inString != null) {
    inString = trim(inString);
    int inInt = int(inString);
    loop45syntaxerror(inInt);
  }
}

public void dispose()
{      
  output.flush();
  output.close();
}

void loop45syntaxerror(int toWrite) {
  System.out.println(toWrite);
  output.println(toWrite);
}