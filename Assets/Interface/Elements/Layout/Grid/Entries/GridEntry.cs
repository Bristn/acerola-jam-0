namespace Interface.Elements
{
    public class GridEntry<TElement, TData> where TElement : IGridEntryElement<TData> where TData : GridEntryData
    {
        public TElement Element { get; protected set; }
        public TData Data { get; set; }

        public GridEntry(TElement element, TData data)
        {
            this.Element = element;
            this.Data = data;
        }
    }
}