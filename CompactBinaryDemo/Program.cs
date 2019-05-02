//--------------------------------------------------------------//
// Copyright (c) 2019, Joseph M. Shunia                         //
//                                                              //
// Distributed under the CC-BY 4.0 license (see: LICENSE.md).   //
// https://creativecommons.org/licenses/by/4.0/                 //
//--------------------------------------------------------------//
using System;
using System.Collections;
using System.Numerics;

namespace CompactBinaryDemo
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("{0,-16}{1,-16}{2,-16}{3,-16}", 
                "n", "binary", "compressed", "decompressed");

            for (BigInteger n = 0; n <= 128; n++)
            {
                BitArray bits = n.ToBits();
                BitArray compressedBits = CompactBinary.Encode(n);
                BigInteger decompressed = CompactBinary.Decode(compressedBits).ToInteger();
                Console.WriteLine("{0,-16}{1,-16}{2,-16}{3,-16}", 
                    n, bits.ToStringBinary(), compressedBits.ToStringBinary(), decompressed);
            }
        }
    }
}
