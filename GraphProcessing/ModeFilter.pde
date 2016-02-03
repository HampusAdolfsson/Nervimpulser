// beräknar medelvärdet av ett antal mediantal
public class ModeFilter extends Filter {
  short sortedBuffer[];
  int offset;
  
  public ModeFilter(int values) {
    super(values);
    sortedBuffer = new short[buffer.length];
  }
  
  public short getNext(short next) {
    boolean move = false;
    if (next <= buffer[offset]){
        int n = sortedBuffer.length - 1;
        while (n >= 0 && next < sortedBuffer[n]) {
            if (move) sortedBuffer[n+1] = sortedBuffer[n];
            else if (sortedBuffer[n] == buffer[offset]) move = true;
            n--;
        }
        sortedBuffer[n+1] = next;
    } else {
        int n = 0;
        while (n < sortedBuffer.length && next > sortedBuffer[n]){
            if (move) sortedBuffer[n-1] = sortedBuffer[n];
            else if (sortedBuffer[n] == buffer[offset]) move = true;
            n++;
        }
        sortedBuffer[n - 1] = next;
    };
    buffer[offset] = next;
    if (++offset == sortedBuffer.length) offset = 0;
    
    int sum = 0;
    if (buffer.length % 2 == 1) {
      for (int i = (buffer.length - 1)/ 2 - 2; i <= (buffer.length - 1)/ 2 + 2; i++) {
        sum += sortedBuffer[i];  
      }
      return (short) Math.round(buffer.length / 5.0);
    }
    for (int i = buffer.length/2 - 2; i < buffer.length/2 + 2; i++) {
        sum += sortedBuffer[i];
    }
    return (short) Math.round(sum / 4.0);
    
  }
  
}