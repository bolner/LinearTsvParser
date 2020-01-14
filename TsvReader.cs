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
    /// Read TSV from an input stream
    /// </summary>
    public class TsvReader : IDisposable {
        /// <summary>
        /// The TSV lines are read from this stream
        /// </summary>
        private Stream inputStream;

        /// <summary>
        /// Helper object for reading from the stream
        /// </summary>
        private StreamReader streamReader;

        /// <summary>
        /// Contains the number of lines read
        /// </summary>
        private Int64 linesRead = 0;

        /// <summary>
        /// Helps decoding the special characters in the fields
        /// </summary>
        private StringBuilder fieldBuffer = new StringBuilder();

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="inputStream">The TSV file will be read from this stream</param>
        public TsvReader(Stream inputStream) {
            this.inputStream = inputStream;
            this.streamReader = new StreamReader(inputStream, Encoding.UTF8);
        }

        /// <summary>
        /// For the IDisposable interface.
        /// Gets rid of the 'StreamReader' it created.
        /// </summary>
        public void Dispose() {
            this.streamReader.Dispose();
            this.streamReader = null;
        }

        /// <summary>
        /// Returns the number of lines read
        /// </summary>
        public Int64 LinesRead {
            get {
                return LinesRead;
            }
        }

        /// <summary>
        /// Gives true if the input stream has ended. False otherwise.
        /// </summary>
        public bool EndOfStream {
            get {
                return this.streamReader.EndOfStream;
            }
        }

        /// <summary>
        /// Read the fields from the next line of the TSV file
        /// </summary>
        public List<string> ReadLine() {
            if (streamReader.EndOfStream) {
                return null;
            }

            linesRead++;
            string line = streamReader.ReadLine();

            return ParseLine(line);
        }

        /// <summary>
        /// Read the fields from the next line of the TSV file
        /// </summary>
        public async Task<List<string>> ReadLineAsync() {
            if (streamReader.EndOfStream) {
                return null;
            }

            linesRead++;
            string line = await streamReader.ReadLineAsync();

            return ParseLine(line);
        }

        /// <summary>
        /// Parses a single line of TSV data into a list of fields
        /// </summary>
        public List<string> ParseLine(string line) {
            bool esc = false;
            List<string> fields = new List<string>();
            fieldBuffer.Clear();

            foreach(char c in line) {
                if (esc) {
                    switch(c) {
                        case '\\': fieldBuffer.Append('\\'); break;
                        case 'n': fieldBuffer.Append('\n'); break;
                        case 'r': fieldBuffer.Append('\r'); break;
                        case 't': fieldBuffer.Append('\t'); break;
                        default:
                            fieldBuffer.Append('\\').Append(c);
                            break;
                    }

                    esc = false;
                }
                else if (c == '\\') {
                    esc = true;
                }
                else if (c == '\t') {
                    fields.Add(fieldBuffer.ToString());
                    fieldBuffer.Clear();
                }
                else {
                    fieldBuffer.Append(c);
                }
            }

            fields.Add(fieldBuffer.ToString());

            return fields;
        }
    }
}
