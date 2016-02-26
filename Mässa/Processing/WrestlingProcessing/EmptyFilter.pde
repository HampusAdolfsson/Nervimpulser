class EmptyFilter extends Filter {
  
  EmptyFilter(){
    super(1);  
  }
  
  @Override
  short getNext(short next) {
    return next;
  }
}