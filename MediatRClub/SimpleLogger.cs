using System.Collections.Generic;

namespace MediatRClub
{
    public class SimpleLogger
    {
        public List<string> Entries { get; } = new List<string>();

        public void Log(string message)
        {
            Entries.Add(message);
        }
    }
}
