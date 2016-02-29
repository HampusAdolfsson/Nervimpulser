package filter;

public abstract class Filter {
    short[] buffer;

    Filter(int num_values) {
        buffer = new short[num_values];
    }

    // r\u00e4kna ut n\u00e4sta v\u00e4rde baserat p\u00e5 next och v\u00e4rdena i buffern
    public abstract short getNext(short next);
}