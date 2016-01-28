public class MeanFilter extends Filter {
  int sum = 0;
  int offset = 0;
  
  public MeanFilter(int num_values, short[] startdata, int offs) {
    super(num_values, startdata, offs);
  }
  
  @Override
  public short getNext(short next) {
    sum += next;
    buffer[offset] = next;
    if (++offset == buffer.length) offset = 0;
    sum -= buffer[offset];
    return (short) Math.round((float) sum / num_values);
  }
}