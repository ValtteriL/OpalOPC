namespace Model
{
    public class Error
    {
        public string? Message { get; set; }

        // parameterless constructor for XML serializer
        internal Error()
        { }

        public Error(string Message)
        {
            this.Message = Message;
        }
    }
}
