class MeanFilter extends Filter {
  int sum = 0;
  int offset = 0;
  
  MeanFilter(int num_values) {
    super(num_values);
  }
  
  @Override
  short getNext(short next) {
    sum += next;
    sum -= buffer[offset];
    buffer[offset] = next;
    if (++offset == buffer.length) offset = 0;
    return (short) Math.round((float) sum / buffer.length);
  }
}