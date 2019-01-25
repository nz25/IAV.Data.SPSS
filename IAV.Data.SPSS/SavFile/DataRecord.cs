﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IAV.Data.SPSS.SavFile
{
    public class DataRecord
    {
        public SavFile File { get; set; }
        public byte[][] Values { get; set; } // decompressed values - 8 bytes per variable record
        public byte[] LeftOver { get; set; } // left over command bytes, that either were read from the stream, but belong to the next data record or were passed from the previous data record for processing

        public DataRecord(SavFile file)
        {
            this.File = file;
            this.Values = new byte[file.VariableRecords.Count][];
        }

        public void ReadFromStream (byte[] leftOver)
        {
            BinaryReader r = this.File.Reader;
            this.LeftOver = leftOver;

            if (this.File.FileHeaderRecord.Compressed == 1)
            {
                this.ReadFromCompressedFile();
            }
            else
            {
                this.ReadFromUncompressedFile();
            }
        }        

        private void ReadFromUncompressedFile()
        {
            BinaryReader r = this.File.Reader;
            // in uncompressed file all values are stored in 8 byte chunks
            int variableCount = this.File.VariableRecords.Count;
            for (int currentVariable = 0; currentVariable < variableCount; currentVariable++)
            {
                byte[] value = r.ReadBytes(8);
                this.Values[currentVariable] = value;
            }
        }

        private void ReadFromCompressedFile()
        {
            BinaryReader r = this.File.Reader;

            int currentVariable = 0;
            int variableCount = this.File.VariableRecords.Count;
            double bias = this.File.FileHeaderRecord.Bias;
            double systemMissingValue = this.File.MachineFloatingPointInfoRecord.SystemMissingValue;

            byte[] commandBytes; /* contain the sequence of 1-byte command codes. These codes have meanings as described below: 
             * 0 - Ignored
             * 1 through 251 - A number with value code - bias, where code is the value of the compression code and bias is the variable bias from the file header.
             * 252 - End of file
             * 253 - A numeric or string value that is not compressible. The value is stored in the 8 bytes following the current block of command bytes. 
             * 254 - An 8-byte string value that is all spaces.
             * 255 - The system-missing value.
             */

            while (currentVariable < variableCount)
            {
                // Checks if the previous data record had some leftover command bytes. If yes these have to be processed first.
                if (this.LeftOver.Length > 0)
                {
                    commandBytes = this.LeftOver;
                    this.LeftOver = new byte[0];
                }
                else // if not, gets the data from the stream
                {
                    commandBytes = r.ReadBytes(8);
                }

                for (int i = 0; i < commandBytes.Length; i++)
                {
                    switch ((CommandByte)commandBytes[i])
                    {
                        case CommandByte.Ignored:
                            break;
                        case CommandByte.EndOfFile:
                            break;
                        case CommandByte.NotCompressibleValue:
                            byte[] notCompressibleValue = r.ReadBytes(8);
                            this.Values[currentVariable] = notCompressibleValue;
                            currentVariable++;
                            break;
                        case CommandByte.AllSpacesString:
                            byte[] allSpacesBytes = Encoding.Default.GetBytes("        ");
                            this.Values[currentVariable] = allSpacesBytes;
                            currentVariable++;
                            break;
                        case CommandByte.SystemMissingValue:
                            byte[] systemMissingBytes = BitConverter.GetBytes(systemMissingValue);
                            this.Values[currentVariable] = systemMissingBytes;
                            currentVariable++;
                            break;
                        default: // 1 - 251
                            byte[] numericValueBytes = BitConverter.GetBytes((double)(commandBytes[i] - bias));
                            this.Values[currentVariable] = numericValueBytes;
                            currentVariable++;                            
                            break;
	                }

                    if (currentVariable == variableCount)
                    {
                        // after filling all variable values, saves the leftover command bytes, so the next data record can process them
                        this.LeftOver = new byte[7 - i];
                        Array.Copy(commandBytes, i + 1, this.LeftOver, 0, 7 - i);
                        break;
                    }
                }
            }
        }

        public void WriteToStream(byte[] leftOver)
        {
            BinaryWriter w = this.File.Writer;
            this.LeftOver = leftOver;

            if (this.File.FileHeaderRecord.Compressed == 1)
            {
                this.WriteToCompressedFile();
            }
            else
            {
                this.WriteToUncompressedFile();
            }
        }

        private void WriteToUncompressedFile()
        {
            BinaryWriter w = this.File.Writer;
            // in uncompressed file all values are stored in 8 byte chunks
            int variableCount = this.File.VariableRecords.Count;
            foreach (byte[] value in this.Values)
            {
                w.Write(value);
            }
        }

        private void WriteToCompressedFile()
        {
            throw new NotImplementedException();
        }
    }
}
