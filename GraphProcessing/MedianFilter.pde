public class MedianFilter extends Filter {
  public static final int PREFERED_NUM_VALUES = 20;
  
  public MedianFilter(int values) {
    super(values);  
  }
  
  @Override
  public short processValues(short[] data, int offset) {
    short[] buff = new short[values];
    for (int i = 0; i < values; i++) {
      buff[i] = data[offset - i < 0 ? values - (offset - i) : offset - i];
    }
    quicksort(buff, 0, buff.length - 1);
    if (buff.length % 2 == 1) return buff[(buff.length - 1)/2];
    return (short)((buff[buff.length / 2] + buff[buff.length/2-1]) / 2);
  }
}


public static void quicksort(short[] array, int min, int max) {
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
}