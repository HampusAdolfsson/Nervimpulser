public class MeanFilter extends Filter {
  int sum = 0;
  int offset = 0;
  
  public MeanFilter(int num_values) {
    super(num_values);
  }
  
  @Override
  public short getNext(short next) {
    sum += next;
    buffer[offset] = next;
    if (++offset == buffer.length) offset = 0;
    sum -= buffer[offset];
    return (short) Math.round((float) sum / buffer.length);
  }
}