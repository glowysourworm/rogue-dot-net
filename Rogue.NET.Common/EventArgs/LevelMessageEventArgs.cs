namespace Rogue.NET.Common.EventArgs
{
    public class LevelMessageEventArgs : System.EventArgs
    {
        public string Message { get; set; }
        public LevelMessageEventArgs(string message)
        {
            this.Message = message;
        }
    }   
}
