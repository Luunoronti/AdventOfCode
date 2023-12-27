using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace AdventOfCodeVisualizerWPF
{


    public class AocVis
    {
        public static void Initialize()
        {
            // create a background thread that will 
            // listen for connections.
            // each connection will be accepted and processed on the thread
            // then, we will pass this to app

            // to do so, we need a dispatcher
            new Thread(ThreadFunc) { IsBackground = true }.Start();
        }


        private static void ThreadFunc()
        {
            var listener = new TcpListener(new IPEndPoint(IPAddress.Any, 58739));
            listener.Start();
            var clients = new List<ClientConn>();
            byte[] input = new byte[1024 * 1024 * 10];

            while (true)
            {
                Thread.Sleep(1);
                while (listener.Pending())
                {
                    var client = listener.AcceptTcpClient();
                    clients.Add(new ClientConn { tcp = client, stream = client.GetStream(), expected = 0, queue = new ByteQueue() });
                }

                for (var i = 0; i < clients.Count; i++)
                {
                    var client = clients[i];

                    var avail = client.tcp.Available;
                    if (avail > 0)
                    {
                        var read = client.stream.Read(input, 0, avail);
                        client.queue.Enqueue(input, 0, read);
                        if (client.expected == 0)
                            client.expected = BitConverter.ToInt32(input, 0);

                        while (client.expected != 0 && client.expected <= client.queue.Length)
                        {
                            var data = new byte[client.expected];
                            client.queue.Dequeue(data, 0, data.Length);
                            MainWindow.Instance.DispatchMessage(data);
                            if (client.queue.Length >= 4)
                            {
                                var q = client.queue;
                                var d = new byte[4] { q.PeekOne(0), q.PeekOne(1), q.PeekOne(2), q.PeekOne(3) };
                                client.expected = BitConverter.ToInt32(d, 0);
                            }
                            else
                            {
                                client.expected = 0;
                            }
                        }
                    }
                }
            }
        }

        class ClientConn
        {
            public TcpClient tcp;
            public NetworkStream stream;
            public int expected;
            public ByteQueue queue;
        }

    }


    /// <summary>
    /// Defines a class that represents a resizable circular byte queue.
    /// </summary>
    public sealed class ByteQueue
    {
        // Private fields
        private int fHead;
        private int fTail;
        private int fSize;
        private int fSizeUntilCut;
        private byte[] fInternalBuffer;

        /// <summary>
        /// Gets the length of the byte queue
        /// </summary>
        public int Length
        {
            get
            {
                return fSize;
            }
        }

        /// <summary>
        /// Constructs a new instance of a byte queue.
        /// </summary>
        public ByteQueue()
        {
            fInternalBuffer = new byte[2048];
        }

        /// <summary>
        /// Clears the byte queue
        /// </summary>
        internal void Clear()
        {
            fHead = 0;
            fTail = 0;
            fSize = 0;
            fSizeUntilCut = fInternalBuffer.Length;
        }

        /// <summary>
        /// Clears the byte queue
        /// </summary>
        internal void Clear(int size)
        {
            lock (this)
            {
                if (size > fSize)
                    size = fSize;

                if (size == 0)
                    return;

                fHead = (fHead + size) % fInternalBuffer.Length;
                fSize -= size;

                if (fSize == 0)
                {
                    fHead = 0;
                    fTail = 0;
                }

                fSizeUntilCut = fInternalBuffer.Length - fHead;
                return;
            }
        }

        /// <summary>
        /// Extends the capacity of the bytequeue
        /// </summary>
        private void SetCapacity(int capacity)
        {
            byte[] newBuffer = new byte[capacity];

            if (fSize > 0)
            {
                if (fHead < fTail)
                {
                    Buffer.BlockCopy(fInternalBuffer, fHead, newBuffer, 0, fSize);
                }
                else
                {
                    Buffer.BlockCopy(fInternalBuffer, fHead, newBuffer, 0, fInternalBuffer.Length - fHead);
                    Buffer.BlockCopy(fInternalBuffer, 0, newBuffer, fInternalBuffer.Length - fHead, fTail);
                }
            }

            fHead = 0;
            fTail = fSize;
            fInternalBuffer = newBuffer;
        }


        /// <summary>
        /// Enqueues a buffer to the queue and inserts it to a correct position
        /// </summary>
        /// <param name="buffer">Buffer to enqueue</param>
        /// <param name="offset">The zero-based byte offset in the buffer</param>
        /// <param name="size">The number of bytes to enqueue</param>
        internal void Enqueue(byte[] buffer, int offset, int size)
        {
            if (size == 0)
                return;

            lock (this)
            {
                if ((fSize + size) > fInternalBuffer.Length)
                    SetCapacity((fSize + size + 2047) & ~2047);

                if (fHead < fTail)
                {
                    int rightLength = (fInternalBuffer.Length - fTail);

                    if (rightLength >= size)
                    {
                        Buffer.BlockCopy(buffer, offset, fInternalBuffer, fTail, size);
                    }
                    else
                    {
                        Buffer.BlockCopy(buffer, offset, fInternalBuffer, fTail, rightLength);
                        Buffer.BlockCopy(buffer, offset + rightLength, fInternalBuffer, 0, size - rightLength);
                    }
                }
                else
                {
                    Buffer.BlockCopy(buffer, offset, fInternalBuffer, fTail, size);
                }

                fTail = (fTail + size) % fInternalBuffer.Length;
                fSize += size;
                fSizeUntilCut = fInternalBuffer.Length - fHead;
            }
        }

        /// <summary>
        /// Dequeues a buffer from the queue
        /// </summary>
        /// <param name="buffer">Buffer to enqueue</param>
        /// <param name="offset">The zero-based byte offset in the buffer</param>
        /// <param name="size">The number of bytes to dequeue</param>
        /// <returns>Number of bytes dequeued</returns>
        internal int Dequeue(byte[] buffer, int offset, int size)
        {
            lock (this)
            {
                if (size > fSize)
                    size = fSize;

                if (size == 0)
                    return 0;

                if (fHead < fTail)
                {
                    Buffer.BlockCopy(fInternalBuffer, fHead, buffer, offset, size);
                }
                else
                {
                    int rightLength = (fInternalBuffer.Length - fHead);

                    if (rightLength >= size)
                    {
                        Buffer.BlockCopy(fInternalBuffer, fHead, buffer, offset, size);
                    }
                    else
                    {
                        Buffer.BlockCopy(fInternalBuffer, fHead, buffer, offset, rightLength);
                        Buffer.BlockCopy(fInternalBuffer, 0, buffer, offset + rightLength, size - rightLength);
                    }
                }

                fHead = (fHead + size) % fInternalBuffer.Length;
                fSize -= size;

                if (fSize == 0)
                {
                    fHead = 0;
                    fTail = 0;
                }

                fSizeUntilCut = fInternalBuffer.Length - fHead;
                return size;
            }
        }


        /// <summary>
        /// Peeks a byte with a relative index to the fHead
        /// Note: should be used for special cases only, as it is rather slow
        /// </summary>
        /// <param name="index">A relative index</param>
        /// <returns>The byte peeked</returns>
        public byte PeekOne(int index)
        {
            return index >= fSizeUntilCut
                ? fInternalBuffer[index - fSizeUntilCut]
                : fInternalBuffer[fHead + index];
        }


    }
}