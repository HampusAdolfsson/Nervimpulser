public class MeanFilter extends Filter {
  public static final int PREFERED_NUM_VALUES = 20;
  
  public MeanFilter(int values) {
    super(values);
  }
  
  @Override
  public short processValues(short[] data, int offset) {
    int sum = 0;
    for (int i = offset; i > offset - values; i--) {
      sum += data[i < 0 ? i + data.length : i];
    }
    return (short) Math.round((float) sum / values);
  }
}