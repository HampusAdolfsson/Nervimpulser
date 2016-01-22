public class EmptyFilter extends Filter {
  public EmptyFilter() {
    super(1);  
  }
  
  @Override
  public short processValues(short[] data, int offset) {
    return data[offset];
  }
}