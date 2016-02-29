// https://en.wikipedia.org/wiki/Moving_average#Weighted_moving_average
class WeightedMeanFilter extends Filter {
  int sum, numerator;
  int denominator;
  int offset;
  
  WeightedMeanFilter(int num_values) {
    super(num_values);
    denominator = num_values * (num_values - 1) / 2; //n + (n - 1) + ... + 2 + 1
  }
  
  short getNext(short next) {
    numerator += (next * buffer.length) - sum;
    
    sum += next - buffer[offset];
    buffer[offset] = next;
    if (++offset == buffer.length) offset = 0;
    
    return (short) Math.round((float)numerator / denominator);
  }
  
  
}