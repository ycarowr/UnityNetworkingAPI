using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace UnityGameServer
{
    /// <summary>
    ///     Packets that store data
    /// </summary>
    public class Packet : IDisposable
    {
        List<byte> _data;
        PacketId _id;
        int _index;
        bool _isDisposed;
        byte[] _readableData;

        /// <summary>
        ///     Used by subclasses.
        /// </summary>
        protected Packet()
        {
        }

        /// <summary>
        ///     Creates a new packet with a given ID. Used for sending.
        /// </summary>
        public Packet(PacketId id)
        {
            _id = id;
            _data = new List<byte>(); // Initialize buffer
            _index = 0; // Set readPos to 0
        }

        /// <summary>
        ///     Creates a packet from raw data. Used when receiving bytes.
        /// </summary>
        public Packet(byte[] data)
        {
            _data = new List<byte>(); // Initialize buffer
            _index = 0; // Set readPos to 0

            SetBytes(data);
        }

        /// <summary>
        ///     Length of the packet's content.
        /// </summary>
        public int Length => _data.Count; // Return the length of buffer

        /// <summary>
        ///     Id of the packet.
        /// </summary>
        public PacketId Id => _id;

        /// <summary>
        ///     Length of the unread data in the packet.
        /// </summary>
        public int UnreadLength => Length - _index;

        /// <summary>
        ///     Disposes the package.
        /// </summary>
        public void Dispose()
        {
            DisposeInternal(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        ///     Internal dispose implementation.
        /// </summary>
        protected virtual void DisposeInternal(bool disposing)
        {
            if (_isDisposed)
                return;

            if (disposing)
            {
                _data = null;
                _readableData = null;
                _index = 0;
            }

            _isDisposed = true;
        }

        public override string ToString()
        {
            return $"Packet id {Id}\n" +
                   $"Length: {Length}\n" +
                   $"Unread length: {UnreadLength}\n" +
                   $"Count: {Length}\n";
        }

        public void Print()
        {
            Debug.Log(ToString());
        }

        public void SetId(PacketId packetId)
        {
            _id = packetId;
        }

        #region Functions

        /// <summary>
        ///     Sets the packet's content and prepares it to be read.
        /// </summary>
        public void SetBytes(byte[] data)
        {
            Write(data);
            _readableData = _data.ToArray();
        }

        /// <summary>
        ///     Inserts the length of the packet's content at the start of the buffer.
        /// </summary>
        public void WriteLength()
        {
            var bytes = BitConverter.GetBytes(_data.Count);
            _data.InsertRange(0, bytes);
        }

        /// <summary>
        ///     Inserts the given int at the start of the buffer.
        /// </summary>
        public void InsertInt(int value)
        {
            var bytes = BitConverter.GetBytes(value);
            _data.InsertRange(0, bytes);
        }

        /// <summary>
        ///     Converts the packet's content to array format.
        /// </summary>
        public byte[] ToArray()
        {
            _readableData = _data.ToArray();
            return _readableData;
        }

        /// <summary>
        ///     Resets the packet instance to allow it to be reused.
        /// </summary>
        public void Reset(bool shouldReset = true)
        {
            if (shouldReset)
            {
                _data.Clear(); // Clear buffer
                _readableData = null;
                _index = 0; // Reset readPos
            }
            else
            {
                _index -= 4; // "Unread" the last read int
            }
        }

        #endregion

        #region Write Data

        /// <summary>
        ///     Adds a byte to the packet.
        /// </summary>
        public void Write(byte value)
        {
            _data.Add(value);
        }

        /// <summary>
        ///     Adds an array of bytes to the packet.
        /// </summary>
        public void Write(byte[] value)
        {
            _data.AddRange(value);
        }

        /// <summary>
        ///     Adds a short to the packet.
        /// </summary>
        public void Write(short value)
        {
            _data.AddRange(BitConverter.GetBytes(value));
        }

        /// <summary>
        ///     Adds an int to the packet.
        /// </summary>
        public void Write(int value)
        {
            _data.AddRange(BitConverter.GetBytes(value));
        }

        /// <summary>
        ///     Adds a long to the packet.
        /// </summary>
        public void Write(long value)
        {
            _data.AddRange(BitConverter.GetBytes(value));
        }

        /// <summary>
        ///     Adds a float to the packet.
        /// </summary>
        public void Write(float value)
        {
            _data.AddRange(BitConverter.GetBytes(value));
        }

        /// <summary>
        ///     Adds a bool to the packet.
        /// </summary>
        public void Write(bool value)
        {
            _data.AddRange(BitConverter.GetBytes(value));
        }

        /// <summary>
        ///     Adds a string to the packet.
        /// </summary>
        public void Write(string value)
        {
            Write(value.Length); // Add the length of the string to the packet
            _data.AddRange(Encoding.ASCII.GetBytes(value)); // Add the string itself
        }

        /// <summary>
        ///     Adds a Vector3 to the packet.
        /// </summary>
        public void Write(Vector3 value)
        {
            Write(value.x);
            Write(value.y);
            Write(value.z);
        }

        /// <summary>
        ///     Adds a Quaternion to the packet.
        /// </summary>
        public void Write(Quaternion value)
        {
            Write(value.x);
            Write(value.y);
            Write(value.z);
            Write(value.w);
        }

        void WriteInternal(byte[] bytes)
        {
            _data.AddRange(bytes);
        }

        #endregion

        #region Read Data

        /// <summary>
        ///     Reads a byte from the packet.
        /// </summary>
        public byte ReadByte(bool moveReadPosition = true)
        {
            if (_data.Count > _index)
            {
                // If there are unread bytes
                var value = _readableData[_index]; // Get the byte at readPos' position
                if (moveReadPosition)
                    // If moveReadPosition is true
                    _index += 1; // Increase readPos by 1
                return value; // Return the byte
            }

            throw new Exception("Could not read value of type 'byte'!");
        }

        /// <summary>
        ///     Reads an array of bytes from the packet.
        /// </summary>
        public byte[] ReadBytes(int length, bool moveReadPosition = true)
        {
            if (_data.Count <= _index)
                throw new Exception("Could not read value of type 'byte[]'!");

            // If there are unread bytes
            var value =
                _data.GetRange(_index, length)
                    .ToArray(); // Get the bytes at readPos' position with a range of length
            if (moveReadPosition)
                // If moveReadPosition is true
                _index += length; // Increase readPos by length
            return value; // Return the bytes
        }

        /// <summary>
        ///     Reads a short from the packet.
        /// </summary>
        public short ReadShort(bool moveReadPosition = true)
        {
            if (_data.Count <= _index)
                throw new Exception("Could not read value of type 'short'!");

            // If there are unread bytes
            var value = BitConverter.ToInt16(_readableData, _index); // Convert the bytes to a short
            if (moveReadPosition)
                // If moveReadPosition is true and there are unread bytes
                _index += 2; // Increase readPos by 2
            return value; // Return the short
        }

        /// <summary>
        ///     Reads an int from the packet.
        /// </summary>
        public int ReadInt(bool moveReadPosition = true)
        {
            if (_data.Count <= _index)
                throw new Exception("Could not read value of type 'int'!");

            // If there are unread bytes
            var value = BitConverter.ToInt32(_readableData, _index); // Convert the bytes to an int
            if (moveReadPosition)
                // If moveReadPosition is true
                _index += 4; // Increase readPos by 4
            return value; // Return the int
        }

        /// <summary>
        ///     Reads a long from the packet.
        /// </summary>
        public long ReadLong(bool moveReadPosition = true)
        {
            if (_data.Count <= _index)
                throw new Exception("Could not read value of type 'long'!");

            // If there are unread bytes
            var value = BitConverter.ToInt64(_readableData, _index); // Convert the bytes to a long
            if (moveReadPosition)
                // If moveReadPosition is true
                _index += 8; // Increase readPos by 8
            return value; // Return the long
        }

        /// <summary>
        ///     Reads a float from the packet.
        /// </summary>
        public float ReadFloat(bool moveReadPosition = true)
        {
            if (_data.Count <= _index)
                throw new Exception("Could not read value of type 'float'!");

            // If there are unread bytes
            var value = BitConverter.ToSingle(_readableData, _index); // Convert the bytes to a float
            if (moveReadPosition)
                // If moveReadPosition is true
                _index += 4; // Increase readPos by 4
            return value; // Return the float
        }

        /// <summary>
        ///     Reads a bool from the packet.
        /// </summary>
        public bool ReadBool(bool moveReadPosition = true)
        {
            if (_data.Count <= _index)
                throw new Exception("Could not read value of type 'bool'!");

            // If there are unread bytes
            var value = BitConverter.ToBoolean(_readableData, _index); // Convert the bytes to a bool
            if (moveReadPosition)
                // If moveReadPosition is true
                _index += 1; // Increase readPos by 1
            return value; // Return the bool
        }

        /// <summary>
        ///     Reads a string from the packet.
        /// </summary>
        public string ReadString(bool moveReadPosition = true)
        {
            try
            {
                var length = ReadInt(); // Get the length of the string
                var value = Encoding.ASCII.GetString(_readableData, _index, length); // Convert the bytes to a string
                if (moveReadPosition && value.Length > 0)
                    // If moveReadPosition is true string is not empty
                    _index += length; // Increase readPos by the length of the string
                return value; // Return the string
            }
            catch
            {
                throw new Exception("Could not read value of type 'string'!");
            }
        }

        /// <summary>
        ///     Reads a Vector3 from the packet.
        /// </summary>
        public Vector3 ReadVector3(bool moveReadPosition = true)
        {
            return new Vector3(ReadFloat(moveReadPosition), ReadFloat(moveReadPosition), ReadFloat(moveReadPosition));
        }

        /// <summary>
        ///     Reads a Quaternion from the packet.
        /// </summary>
        public Quaternion ReadQuaternion(bool moveReadPosition = true)
        {
            return new Quaternion(ReadFloat(moveReadPosition),
                ReadFloat(moveReadPosition), ReadFloat(moveReadPosition), ReadFloat(moveReadPosition));
        }

        #endregion
        
        public void InsertId()
        {
            InsertInt((int)_id);
        }
    }
}