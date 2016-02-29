package filter;

public class ModeFilter extends Filter {
    short sortedBuffer[];
    int offset;

    public ModeFilter(int num_values) {
        super(num_values);
        sortedBuffer = new short[buffer.length];
    }

    public short getNext(short next) {
        if (buffer.length < 5) return next;
        // ta bort det \u00e4ldsta v\u00e4rdet, s\u00e4tt in next i den sorterade arrayen
        boolean move = false;
        if (next < buffer[offset]){
            int n = sortedBuffer.length - 1;
            while (n >= 0 && next < sortedBuffer[n]) {
                if (move) sortedBuffer[n+1] = sortedBuffer[n];
                else if (sortedBuffer[n] == buffer[offset]) move = true;
                n--;
            }
            sortedBuffer[n+1] = next;
        } else if (next > buffer[offset]){
            int n = 0;
            while (n < sortedBuffer.length && next > sortedBuffer[n]){
                if (move) sortedBuffer[n-1] = sortedBuffer[n];
                else if (sortedBuffer[n] == buffer[offset]) move = true;
                n++;
            }
            sortedBuffer[n - 1] = next;
        };
        buffer[offset] = next;
        if (++offset == sortedBuffer.length) offset = 0;

        // ta medelv\u00e4rdet av de mittersta sorterade v\u00e4rdena
        int sum = 0;
        if (buffer.length % 2 == 1) {
            for (int i = (buffer.length - 1)/ 2 - 2; i <= (buffer.length - 1)/ 2 + 2; i++) {
                sum += sortedBuffer[i];
            }
            return (short) Math.round(buffer.length / 5.0f);
        }
        for (int i = buffer.length/2 - 2; i < buffer.length/2 + 2; i++) {
            sum += sortedBuffer[i];
        }
        return (short) Math.round(sum / 4.0f);

    }

}

