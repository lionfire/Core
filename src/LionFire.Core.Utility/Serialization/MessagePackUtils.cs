#if MSGPACK
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;


using SUtils.Serialization.MsgPack;



namespace LionFire.Serialization
{
    public enum LionPackEncodingTypes : byte
    {
        [EncodingLength(1)]
        Literal_ChildNull = 193,
    }

    public static class MessagePackUtils
    {
        //public static bool ConsumeNull(this Stream stream)
        //{
        //    const int nullSize = 1;
        //    int bytesRead = 0;

        //    byte[] b = new byte[nullSize];

        //    bytesRead = stream.Read(b, 0, nullSize);

        //    if (bytesRead == 0) return false;
        //    if (b[0] == (byte)EncodingTypes.Literal_Null)
        //    {
        //        return true;
        //    }

        //    stream.Seek(-nullSize, SeekOrigin.Current);
        //    return false;
        //}



        public static bool IsChildNull(this Stream stream)
        {
            return TryConsume(stream, (byte)LionPackEncodingTypes.Literal_ChildNull);
        }

        public static bool IsNull(this Stream stream)
        {
            return TryConsume(stream, (byte) EncodingTypes.Literal_Null);
            //const int nullSize = 1;
            //int bytesRead = 0;

            //byte[] b = new byte[nullSize];

            //bytesRead = stream.Read(b, 0, nullSize);

            //if (bytesRead == 0) return false;
            //if (b[0] == (byte)EncodingTypes.Literal_Null)
            //{
            //    return true;
            //}

            //stream.Seek(-nullSize, SeekOrigin.Current);
            //return false;

        }

        public static bool TryConsume(this Stream stream, byte expectedByte)
        {
            return TryConsume(stream, new byte[] { expectedByte });
        }

        public static bool TryConsume(this Stream stream, byte[] expectedBytes)
        {
            int bytesRead = 0;

            byte[] actualBytes = new byte[expectedBytes.Length];

            bytesRead = stream.Read(actualBytes, 0, expectedBytes.Length);

            if (bytesRead != expectedBytes.Length)
            {
                stream.Seek(-bytesRead, SeekOrigin.Current);
                return false;
            }

            for (int i = 0; i < expectedBytes.Length; i++)
            {
                if (expectedBytes[i] != actualBytes[i])
                {
                    stream.Seek(-bytesRead, SeekOrigin.Current);
                    return false;
                }
            }

            return true;
        }
    }
}
#endif