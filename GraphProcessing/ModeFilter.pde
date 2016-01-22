public class ModeFilter extends Filter {
  public static final int PREFERED_NUM_VALUES = 20;
  
  public ModeFilter(int values) {
    super(values);  
  }
  
  public short processValues(short[] data, int offset) {
    short[] buff = new short[values];
    for (int i = 0; i < values; i++) {
      buff[i] = data[offset - i < 0 ? values - (offset - i) : offset - i];  
    }
    quicksort(buff, 0, buff.length - 1);
    int sum = 0;
    if (buff.length % 2 == 1) {
      for (int i = (buff.length - 1)/ 2 - 1; i <= (buff.length - 1)/ 2 + 1; i++) {
        sum += buff[i];  
      }
      return (short) Math.round(sum / 3.0);
    }
    for (int i = buff.length/2 - 2; i < buff.length/2 + 2; i++) {
        sum += buff[i];
    }
    return (short) Math.round(sum / 4.0);
    
  }
  
}