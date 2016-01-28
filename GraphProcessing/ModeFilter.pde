// beräknar medelvärdet av ett antal mediantal
public class ModeFilter extends Filter {
  short sortedBuffer[];
  int offset;
  
  public ModeFilter(int values, short[] startdata, int offs) {
    super(values, startdata, offs);
    sortedBuffer = new short[num_values];
  }
  
  public short getNext(short next) {
    int n = num_values - 1;
    boolean move = false;
    while (next < sortedBuffer[n] && n >= 0) {
      if (move) sortedBuffer[n+1] = buffer[n];
      else if (sortedBuffer[n] == buffer[offset]) move = true;
      n--;
    }
    sortedBuffer[n + 1] = next;
    if (++offset == num_values) offset = 0;
    buffer[offset] = next;
    
    int sum = 0;
    if (num_values % 2 == 1) {
      for (int i = (num_values - 1)/ 2 - 2; i <= (num_values - 1)/ 2 + 2; i++) {
        sum += sortedBuffer[i];  
      }
      return (short) Math.round(num_values / 5.0);
    }
    for (int i = num_values/2 - 2; i < num_values/2 + 2; i++) {
        sum += sortedBuffer[i];
    }
    return (short) Math.round(sum / 4.0);
    
  }
  
}