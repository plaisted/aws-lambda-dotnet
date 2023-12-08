/*
 * Copyright 2019 Amazon.com, Inc. or its affiliates. All Rights Reserved.
 * 
 * Licensed under the Apache License, Version 2.0 (the "License").
 * You may not use this file except in compliance with the License.
 * A copy of the License is located at
 * 
 *  http://aws.amazon.com/apache2.0
 * 
 * or in the "license" file accompanying this file. This file is distributed
 * on an "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either
 * express or implied. See the License for the specific language governing
 * permissions and limitations under the License.
 */
using System;
using System.IO;
using System.Text.Json;
using static Amazon.Lambda.RuntimeSupport.InternalRuntimeApiClient;

namespace Amazon.Lambda.RuntimeSupport
{
    /// <summary>
    /// Class used to wrap an existing stream and add the Lambda streaming prelude json prefix to the stream.
    /// </summary>
    internal class StreamedResponseWrapper : Stream
    {
        public Stream InputStream { get; internal set; }

        private int writtenBytes = 0;
        private bool headerWritten = false;
        private byte[] header;

        public override bool CanRead => InputStream.CanRead;

        public override bool CanSeek => false;

        public override bool CanWrite => false;

        public override long Length => throw new NotSupportedException();

        public override long Position { get => InputStream.Position + writtenBytes; set => throw new NotSupportedException(); }

        public StreamedResponseWrapper(StreamedResponse response)
        {
#if NET6_0_OR_GREATER
            header = JsonSerializer.SerializeToUtf8Bytes(response, RuntimeApiSerializationContext.Default.StreamedResponse);
#else
            header = JsonSerializer.SerializeToUtf8Bytes(response);
#endif
            InputStream = response.Stream;
        }

        public new void Dispose()
        {
            InputStream?.Dispose();
            base.Dispose();
        }

        public override void Flush() => InputStream.Flush();

        public override int Read(byte[] buffer, int offset, int count)
        {
            if (headerWritten)
            {
                return InputStream.Read(buffer, offset, count);
            }

            var start = writtenBytes;
            // write json prelude if not already written
            if (writtenBytes < header.Length)
            {
                int bytesToWrite = Math.Min(header.Length - writtenBytes, count);
                Array.Copy(header, writtenBytes, buffer, offset, bytesToWrite);
                writtenBytes += bytesToWrite;
                offset += bytesToWrite;
                count -= bytesToWrite;
            }

            // write null bytes if not already written
            if (writtenBytes < header.Length + 8)
            {
                var bytesToWrite = Math.Min(header.Length + 8 - writtenBytes, count);
                for (var i = 0; i < bytesToWrite; i++)
                {
                    buffer[offset + i] = 0;
                }
                offset += bytesToWrite;
                count -= bytesToWrite;
                writtenBytes += bytesToWrite;
            }

            headerWritten = writtenBytes >= header.Length + 8;

            if (count > 0)
            {
                return writtenBytes - start + InputStream.Read(buffer, offset, count);
            }
            return writtenBytes - start;
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            throw new NotSupportedException();
        }

        public override void SetLength(long value)
        {
            throw new NotSupportedException();
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            throw new NotSupportedException();
        }
    }
}
