/*
    Copyright 2020 Tamas Bolner
    
    Licensed under the Apache License, Version 2.0 (the "License");
    you may not use this file except in compliance with the License.
    You may obtain a copy of the License at
    
      http://www.apache.org/licenses/LICENSE-2.0
    
    Unless required by applicable law or agreed to in writing, software
    distributed under the License is distributed on an "AS IS" BASIS,
    WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
    See the License for the specific language governing permissions and
    limitations under the License.
*/
using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace LinearTsvParser {
    /// <summary>
    /// Write TSV into an output stream
    /// </summary>
    public class TsvWriter : IDisposable {
        /// <summary>
        /// Helper object for writing into the stream.
        /// Only needed when this class wasn't instantiated
        /// by passing a TextWriter to the constructor.
        /// </summary>
        private StreamWriter streamWriter = null;

        /// <summary>
        /// Generic interface for the underlying stream.
        /// Required for supporting different kinds of output streams.
        /// </summary>
        private TextWriter textWriter;

        /// <summary>
        /// For building output lines
        /// </summary>
        private StringBuilder lineBuffer = new StringBuilder();

        /// <summary>
        /// Contains the number of lines written
        /// </summary>
        private Int64 linesWritten = 0;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="outputStream">The TSV file is written into this stream</param>
        public TsvWriter(Stream outputStream) {
            this.streamWriter = new StreamWriter(outputStream);
            this.textWriter = (TextWriter)this.streamWriter;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="textWriter">The TSV file is written into this stream</param>
        public TsvWriter(TextWriter textWriter) {
            this.streamWriter = null;
            this.textWriter = textWriter;
        }

        /// <summary>
        /// For the IDisposable interface.
        /// Gets rid of the 'StreamWriter' it created.
        /// </summary>
        public void Dispose() {
            if (this.streamWriter != null) {
                this.streamWriter.Dispose();
                this.streamWriter = null;
            }
        }

        /// <summary>
        /// Returns the number of lines written
        /// </summary>
        public Int64 LinesWritten {
            get {
                return linesWritten;
            }
        }

        /// <summary>
        /// Output a line into the stream as linear TSV.
        /// </summary>
        /// <param name="fields">Can be any enumarable of strings, like an array or a list for example.</param>
        public void WriteLine(IEnumerable<string> fields) {
            BuildLine(fields);
            this.textWriter.Write(lineBuffer);
            linesWritten++;
        }

        /// <summary>
        /// Output a line into the stream as linear TSV.
        /// </summary>
        /// <param name="fields">Can be any enumarable of strings, like an array or a list for example.</param>
        public async Task WriteLineAsync(IEnumerable<string> fields) {
            BuildLine(fields);
            await this.textWriter.WriteAsync(lineBuffer);
            linesWritten++;
        }

        /// <summary>
        /// Builds a line of text in linear TSV format out of the
        /// input values.
        /// Uses the private 'lineBuffer' property.
        /// </summary>
        private void BuildLine(IEnumerable<string> fields) {
            lineBuffer.Clear();
            var enumerator = fields.GetEnumerator();
            bool notLast = enumerator.MoveNext();
            string field;

            while(notLast) {
                field = enumerator.Current;

                foreach(char c in field) {
                    switch (c) {
                        case '\\': lineBuffer.Append("\\\\"); break;
                        case '\r': lineBuffer.Append("\\r"); break;
                        case '\n': lineBuffer.Append("\\n"); break;
                        case '\t': lineBuffer.Append("\\t"); break;
                        default: lineBuffer.Append(c); break;
                    }
                }

                notLast = enumerator.MoveNext();
                if (notLast) {
                    lineBuffer.Append('\t');
                }
            }

            lineBuffer.Append('\n');
        }

        /// <summary>
        /// Flushes the underlying StreamWriter object
        /// </summary>
        public void Flush() {
            this.textWriter.Flush();
        }

        /// <summary>
        /// Flushes the underlying StreamWriter object
        /// </summary>
        public async Task FlushAsync() {
            await this.textWriter.FlushAsync();
        }
    }
}
