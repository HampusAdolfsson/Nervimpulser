package filter;

public class EmptyFilter extends Filter {

    public EmptyFilter(){
        super(1);
    }

    public @Override
    short getNext(short next) {
        return next;
    }
}

