public class MedianFilter extends Filter {
  short sortedBuffer[];
  int offset;
  
  public MedianFilter(int num_values, short[] startdata, int offs) {
    super(num_values, startdata, offs);  
    sortedBuffer = new short[num_values];
  }
  
  @Override
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
    
    if (num_values % 2 == 1) return buffer[(buffer.length - 1)/2];
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