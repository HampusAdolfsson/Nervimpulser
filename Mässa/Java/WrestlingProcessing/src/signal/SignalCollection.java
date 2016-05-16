package signal;

import WrProcessing.DataClient;
import filter.Filter;

public class SignalCollection {
    private Signal[] signals;
    public int offset;
    private int bufferSize;
    private int filterSize = Signal.DEFAULT_WINDOW_SIZE;
    private Filter.FilterType filterType = Filter.FilterType.Mean;

    public SignalCollection(int collectionSize, int bufferSize) {
        signals = new Signal[collectionSize];
        for (int i = 0; i < signals.length; i++) {
            signals[i] = new Signal(bufferSize);
        }
        this.bufferSize = bufferSize;
    }

    public short[] getValues(int index) {
        if (index >= signals.length) return null;
        return signals[index].getValues();
    }

    public void addValues(short... values) {
        if (values.length != signals.length) return;
        String toSend = "";
        for (int i = 0; i < values.length; i++) {
            signals[i].addValue(values[i], offset);
            toSend += values[i] + ".";
        }
        DataClient.send(toSend.substring(0, toSend.length()-1));
        if (++offset == bufferSize) offset = 0;
    }

    public void setSignalBufferSize(int bufferSize) {
        if (this.bufferSize == bufferSize) return;
        this.bufferSize = bufferSize;
        for (Signal signal : signals) {
            signal.reset(bufferSize);
        }
    }

    public void setFilterType(Filter.FilterType type) {
        filterType = type;
        try {
            for (Signal signal : signals) {
                signal.setFilter(type, filterSize);
            }
        } catch (ReflectiveOperationException e) { e.printStackTrace(); }
    }

    public void setFilterSize(int size) {
        filterSize = size;
        try {
            for (Signal signal : signals) {
                signal.setFilter(filterType, size);
            }
        } catch (ReflectiveOperationException e) { e.printStackTrace(); }
    }

    public void incrementVerticalOffset(int index, short amount) {
        if (index >= signals.length) return;
        signals[index].incrementVertOffs(amount);
    }

    public short getVerticalOffset(int index) {
        return signals[index].verticalOffset;
    }
}

