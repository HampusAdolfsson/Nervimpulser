abstract class Filter {
  short[] buffer;
  
  Filter(int num_values) {
    buffer = new short[num_values];
  }
  
  // räkna ut nästa värde baserat på next och värdena i buffern
  abstract short getNext(short next);
}