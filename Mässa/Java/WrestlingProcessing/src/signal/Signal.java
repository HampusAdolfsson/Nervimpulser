package signal;

import filter.*;
import filter.Filter.FilterType;

public class Signal {
    public static final int DEFAULT_WINDOW_SIZE = 10;

    private short values[];
    private Filter filter;
    private short verticalOffset;

    protected Signal(int bufferSize) {
        filter = new MeanFilter(DEFAULT_WINDOW_SIZE);
        values = new short[bufferSize];
    }

    protected void addValue(short value, int offset) {
        values[offset] = (short) (filter.getNext(value) - verticalOffset);
    }

    protected void reset() {
        reset(values.length);
    }

    protected void reset(int bufferSize) {
        values = new short[bufferSize];
    }

    protected void setFilter(FilterType filterType, int size) throws ReflectiveOperationException {
        switch (filterType) {
            case Mean:
                filter = new MeanFilter(size);
                break;
            case Mode:
                filter = new ModeFilter(size);
                break;
            case WeigthedMean:
                filter = new WeightedMeanFilter(size);
                break;
            case Empty:
                filter = new EmptyFilter();
                break;
            case Median:
                filter = new MedianFilter(size);
                break;
        }
    }

    protected void incrementVertOffs(short offset) {
        verticalOffset += offset;
    }

    public short[] getValues() {
        return values;
    }
}
