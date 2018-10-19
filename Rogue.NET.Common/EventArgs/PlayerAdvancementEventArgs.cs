namespace Rogue.NET.Common.EventArgs
{
    public class PlayerAdvancementEventArgs : System.EventArgs
    {
        public string Header { get; set; }
        public string[] MessageLines { get; set; }

        public PlayerAdvancementEventArgs(string header, string[] lines)
        {
            this.Header = header;
            this.MessageLines = lines;
        }
    }
}
