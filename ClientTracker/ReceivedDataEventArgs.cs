using System;

namespace ClientTracker
{
    internal class ReceivedDataEventArgs: EventArgs
    {
        private Packet packet;

        public ReceivedDataEventArgs(Packet _packet)
        {
            this.packet = _packet;
        }

        public Packet Packet
        {
            get { return packet; }
        }
    }
}