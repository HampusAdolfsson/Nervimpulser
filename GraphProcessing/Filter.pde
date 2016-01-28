public abstract class Filter {
  int num_values;
  short[] buffer;
  
  public Filter(int num_values, short[] startdata, int offs) {
    this.num_values = num_values;
    buffer = new short[num_values];
    for (int i = offs; i > offs - num_values; i--) {
      buffer[offs - i] = startdata[i < 0 ? i + startdata.length - 1 : i];
    }
  }
  
  // räkna ut nästa värde baserat på next och värdena i buffern
  public abstract short getNext(short next);
}