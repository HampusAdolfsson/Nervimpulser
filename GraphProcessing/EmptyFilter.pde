public class EmptyFilter extends Filter {
  
  public EmptyFilter(){
    super(1, new short[1], 0);  
  }
  
  @Override
  public short getNext(short next) {
    return next;
  }
}