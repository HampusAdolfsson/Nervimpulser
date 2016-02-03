public class EmptyFilter extends Filter {
  
  public EmptyFilter(){
    super(1);  
  }
  
  @Override
  public short getNext(short next) {
    return next;
  }
}