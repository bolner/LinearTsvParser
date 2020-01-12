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
using System.Collections.Generic;

namespace LinearTsvParser {
    /// <summary>
    /// Write TSV into an output stream
    /// </summary>
    public class TsvWriter : IDisposable {
        /// <summary>
        /// This tells whether the StreamWriter was passed to the class
        /// from the outside or it created it.
        /// </summary>
        private bool weOwnTheStreamWriter = false;

        /// <summary>
        /// Helper object for writing into the stream
        /// </summary>
        private StreamWriter streamWriter;

        /// <summary>
        /// Contains the number of lines written
        /// </summary>
        private Int64 linesWritten = 0;

        /// <summary>
        /// For building output lines
        /// </summary>
        private StringBuilder lineBuffer = new StringBuilder();

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="outputStream">The TSV file is written into this stream</param>
        public TsvWriter(Stream outputStream) {
            this.streamWriter = new StreamWriter(outputStream);
            this.weOwnTheStreamWriter = true;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="outputWriter">The TSV file is written into this stream</param>
        public TsvWriter(StreamWriter outputWriter) {
            this.streamWriter = outputWriter;
        }

        /// <summary>
        /// For the IDisposable interface.
        /// Gets rid of the 'StreamWriter' it created.
        /// </summary>
        public void Dispose() {
            if (this.weOwnTheStreamWriter) {
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
        /// The 'fields'
        /// </summary>
        /// <param name="fields">Can be any enumarable of strings, like an array or a list for example.</param>
        public void WriteLine(IEnumerable<string> fields) {
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
            this.streamWriter.Write(lineBuffer);
            linesWritten++;
        }

        /// <summary>
        /// Flushes the underlying StreamWriter object
        /// </summary>
        public void Flush() {
            this.streamWriter.Flush();
        }
    }
}
