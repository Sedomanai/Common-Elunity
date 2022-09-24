using System;
using System.IO;
using System.Text;

namespace Elang
{
    /// <summary>
    /// <br>Abstract base Seralizer Stream. </br> 
    /// <br>Can serialize the following data types: int16, int32, int64, bool, float, string</br>
    /// </summary>
    public abstract class SerializeStream
    {
        public SerializeStream(byte[] data_) { data = data_; }

        public SerializeStream parse(out Int16 value) {
            value = unchecked((Int16)FromBytes(data, reader, 2));
            reader += 2;
            return this;
        }
        public SerializeStream parse(out Int32 value) {
            value = unchecked((Int32)FromBytes(data, reader, 4));
            reader += 4;
            return this;
        }
        public SerializeStream parse(out Int64 value) {
            value = unchecked((Int64)FromBytes(data, reader, 8));
            reader += 8;
            return this;
        }
        public SerializeStream parse(out string value) {
            Int32 strSize;
            parse(out strSize);
            value = Encoding.Default.GetString(data, reader, strSize);
            reader += strSize;
            return this;
        }
        public SerializeStream parse(out bool value) {
            value = (unchecked((char)FromBytes(data, reader, 1)) > 0) ? true : false;
            reader += 1;
            return this;
        }
        public SerializeStream parse(out float value) {
            value = BitConverter.ToSingle(data, reader);
            reader += 4;
            return this;
        }


        // This same method can be used by int16, int32 and int64.
        protected abstract long FromBytes(byte[] buffer, int startIndex, int len);

        int reader;
        byte[] data;
    }

    /// <summary>
    /// <br>Global metohods for creatina a stream from a given file path or bytes. </br> 
    /// </summary>
    public class StreamCreator
    {
        public static SerializeStream create(string filePath) {
            return create(File.ReadAllBytes(filePath));
        }
        public static SerializeStream create(byte[] bytes) {
            return BitConverter.IsLittleEndian ? new LittleEndianStream(bytes) : new BigEndianStream(bytes);
        }
    }

    /// <summary>
    /// <br> Stream for serializing in Big Endian. </br> 
    /// </summary>
    public class BigEndianStream : SerializeStream
    {
        public BigEndianStream(byte[] bytes) : base(bytes) { }

        protected override long FromBytes(byte[] buffer, int startIndex, int len) {
            long ret = 0;
            for (int i = 0; i < len; i++) {
                ret = unchecked((ret << 8) | buffer[startIndex + i]);
            }
            return ret;
        }
    }

    /// <summary>
    /// <br> Stream for serializing in Little Endian. </br> 
    /// </summary>
    public class LittleEndianStream : SerializeStream
    {
        public LittleEndianStream(byte[] bytes) : base(bytes) { }
        protected override long FromBytes(byte[] buffer, int startIndex, int len) {
            long ret = 0;
            for (int i = 0; i < len; i++) {
                ret = unchecked((ret << 8) | buffer[startIndex + len - 1 - i]);
            }
            return ret;
        }
    }
}