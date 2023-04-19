namespace Model
{
    public class Issue
    {
        public string Description { get; }

        public Issue(string description)
        {
            this.Description = description;
        }
    }
}