// beräknar medelvärdet av ett antal mediantal
class ModeFilter extends Filter {
  short sortedBuffer[];
  int offset;
  
  ModeFilter(int num_values) {
    super(num_values);
    sortedBuffer = new short[buffer.length];
  }
  
  short getNext(short next) {
    if (buffer.length < 5) return next;
    // ta bort det äldsta värdet, sätt in next i den sorterade arrayen
    boolean move = false;
    if (next < buffer[offset]){
        int n = sortedBuffer.length - 1;
        while (n >= 0 && next < sortedBuffer[n]) {
            if (move) sortedBuffer[n+1] = sortedBuffer[n];
            else if (sortedBuffer[n] == buffer[offset]) move = true;
            n--;
        }
        sortedBuffer[n+1] = next;
    } else if (next > buffer[offset]){
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
    
    // ta medelvärdet av de mittersta sorterade värdena
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