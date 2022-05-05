namespace SmileTV.Models
{
    [Serializable]
    public class ButtonUpdate
    {
        public string menuName { get; set; }
        public int pos { get; set; }
        public string newCaption { get; set; }
    }
}
