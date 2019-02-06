
namespace PacketRipper.OpCodes
{
    public class NetworkOpCodes
    {
        // Net Op Codes in net order. Underneath, there are actually channels
        // 0-3, where 0x0900 is OP_Packet on channel 0, 0x0a00 is OP_Packet on
        // channel 1, 0x0b00 is OP_Packet on channel 2, etc. and the same
        // with OP_Oversized for 0x0d00-0x1000. Only channel 0 seems used.
        public const ushort OP_SessionRequest = 0x01;
        public const ushort OP_SessionResponse = 0x02;
        public const ushort OP_Combined = 0x03;
        public const ushort OP_SessionDisconnect = 0x05;
        public const ushort OP_KeepAlive = 0x06;
        public const ushort OP_SessionStatRequest = 0x07;
        public const ushort OP_SessionStatResponse = 0x08;
        public const ushort OP_Packet = 0x09;
        public const ushort OP_Oversized = 0x0d;
        public const ushort OP_AckFuture = 0x11;
        public const ushort OP_Ack = 0x15;
        public const ushort OP_AppCombined = 0x19;
        public const ushort OP_AckAfterDisconnect = 0x1d;
        public const ushort OP_Sixty = 0x60;
    }
}
