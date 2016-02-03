public class MedianFilter extends Filter {
  short sortedBuffer[];
  int offset;
  
  public MedianFilter(int num_values) {
    super(num_values);  
    sortedBuffer = new short[num_values];
  }
  
  @Override
  public short getNext(short next) {
    // ta bort det äldsta värdet, sätt in next i den sorterade arrayen
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
    
    if (buffer.length % 2 == 1) return buffer[(buffer.length - 1)/2];
    return (short)((buffer[buffer.length / 2] + buffer[buffer.length/2-1]) / 2);
  }
}

/*public static void quicksort(short[] array, int min, int max) {
  int i = min, j = max;
  short temp;
  int pivot = array[(min + max)/2];
  
  while(i <= max) {
    while (array[i] < pivot) {
      i++;  
    }
    while (array[j] > pivot) {
      j--;  
    }
    
    if (i <= j) {
      temp = array[j];
      array[j] = array[i];
      array[i] = temp;
      i++;
      j--;
    }
    
    if (min < j)
      quicksort(array, min, j);
    if (max > i)
      quicksort(array, i, max); 
  }
}*/