public abstract class Filter {
  short[] buffer;
  
  public Filter(int num_values) {
    buffer = new short[num_values];
  }
  
  // räkna ut nästa värde baserat på next och värdena i buffern
  abstract short getNext(short next);
}


public class EmptyFilter extends Filter {
  
  public EmptyFilter(){
    super(1);  
  }
  
  @Override
  short getNext(short next) {
    return next;
  }
}


public class LowPassFilter extends Filter {
  public LowPassFilter(int windowSize) {
    super(windowSize);  
  }
  short lastValue;
  
  short getNext(short value) {
    lastValue += (value - lastValue) / 2;
    return lastValue;
  }
}


public class MeanFilter extends Filter {
  int sum = 0;
  int _offs = 0;
  
  public MeanFilter(int num_values) {
    super(num_values);
  }
  
  @Override
  short getNext(short next) {
    sum += next;
    sum -= buffer[_offs];
    buffer[_offs] = next;
    if (++_offs == buffer.length) _offs = 0;
    return (short) Math.round((float) sum / buffer.length);
  }
}


public class MedianFilter extends Filter {
  short sortedBuffer[];
  int _offs;
  
  public MedianFilter(int num_values) {
    super(num_values);  
    sortedBuffer = new short[num_values];
  }
  
  @Override
  short getNext(short next) {
    // ta bort det äldsta värdet, sätt in next i den sorterade arrayen
    boolean move = false;
    if (next < buffer[_offs]) {
        int n = sortedBuffer.length - 1;
        while (n >= 0 && next < sortedBuffer[n]) {
            if (move) sortedBuffer[n+1] = sortedBuffer[n];
            else if (sortedBuffer[n] == buffer[_offs]) move = true;
            n--;
        }
        sortedBuffer[n+1] = next;
    } else if (next > buffer[_offs]) {
        int n = 0;
        while (n < sortedBuffer.length && next > sortedBuffer[n]) {
            if (move) sortedBuffer[n-1] = sortedBuffer[n];
            else if (sortedBuffer[n] == buffer[_offs]) move = true;
            n++;
        }
        sortedBuffer[n - 1] = next;
    }
    buffer[_offs] = next;
    if (++_offs == sortedBuffer.length) _offs = 0;
    if (buffer.length % 2 == 1) return buffer[(buffer.length - 1)/2];
    return (short)((buffer[buffer.length / 2] + buffer[buffer.length/2-1]) / 2);
  }
}


// beräknar medelvärdet av ett antal mediantal
public class MedianMeanFilter extends Filter {
  short sortedBuffer[];
  int _offs;
  
  public MedianMeanFilter(int num_values) {
    super(num_values);
    sortedBuffer = new short[buffer.length];
  }
  
  short getNext(short next) {
    if (buffer.length < 5) return next;
    // ta bort det äldsta värdet, sätt in next i den sorterade arrayen
    boolean move = false;
    if (next < buffer[_offs]){
        int n = sortedBuffer.length - 1;
        while (n >= 0 && next < sortedBuffer[n]) {
            if (move) sortedBuffer[n+1] = sortedBuffer[n];
            else if (sortedBuffer[n] == buffer[_offs]) move = true;
            n--;
        }
        sortedBuffer[n+1] = next;
    } else if (next > buffer[_offs]){
        int n = 0;
        while (n < sortedBuffer.length && next > sortedBuffer[n]){
            if (move) sortedBuffer[n-1] = sortedBuffer[n];
            else if (sortedBuffer[n] == buffer[_offs]) move = true;
            n++;
        }
        sortedBuffer[n - 1] = next;
    };
    buffer[_offs] = next;
    if (++_offs == sortedBuffer.length) _offs = 0;
    
    // ta medelvärdet av de mittersta sorterade värdena
    int sum = 0;
    if (buffer.length % 2 == 1) {
      for (int i = (buffer.length - 1)/ 2 - 2; i <= (buffer.length - 1)/ 2 + 2; i++) {
        sum += sortedBuffer[i];  
      }
      return (short) Math.round(buffer.length / 5.0);
    }
    for (int i = buffer.length/2 - 2; i < buffer.length/2 + 2; i++) {
        sum += sortedBuffer[i];
    }
    return (short) Math.round(sum / 4.0);
    
  }
  
}


// https://en.wikipedia.org/wiki/Moving_average#Weighted_moving_average
public class WeightedMeanFilter extends Filter {
  int sum, numerator;
  int denominator;
  int _offs;
  
  public WeightedMeanFilter(int num_values) {
    super(num_values);
    denominator = num_values * (num_values - 1) / 2; //n + (n - 1) + ... + 2 + 1
  }
  
  short getNext(short next) {
    numerator += (next * buffer.length) - sum;
    
    sum += next - buffer[_offs];
    buffer[_offs] = next;
    if (++_offs == buffer.length) _offs = 0;
    
    return (short) Math.round((float)numerator / denominator);
  }
}


public class RMSFilter extends Filter {
  public RMSFilter(int num_values) {
    super(width / PIXELS_PER_POINT);
  }
  
  int _offs;
  int sum;
  
  short getNext(short next) {
    sum += next;
    sum -= buffer[_offs];
    
    buffer[_offs] = next;
    if (++_offs == buffer.length) _offs = 0;
    
    return (short) Math.round(Math.sqrt(sum) / buffer.length);
  }  
}