public abstract class Filter {
  int values;
  
  public Filter(int num_values) {
    values = num_values;
  }
  
  public abstract short processValues(short[] data, int offset);
}