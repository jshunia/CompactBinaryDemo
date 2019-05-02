//--------------------------------------------------------------//
// Copyright (c) 2019, Joseph M. Shunia                         //
//                                                              //
// Distributed under the CC-BY 4.0 license (see: LICENSE.md).   //
// https://creativecommons.org/licenses/by/4.0/                 //
//--------------------------------------------------------------//
using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;

namespace CompactBinaryDemo
{
    public static class CompactBinary
    {
        public static BitArray Encode(BigInteger value)
        {
            if (value < 0) throw new ArgumentOutOfRangeException(nameof(value));
            return Encode((value + 2).ToBits(), false);
        }

        public static BitArray Encode(BitArray bits, bool add = true)
        {
            if (add)
            {
                // Add 2 to the binary value.
                bits = bits.Add(2);
            }
            if (bits.Length == 0) return bits;

            // Initialize the list to hold the encoded bits.
            var encodedBitsList = new List<bool>(bits.Length);

            // Add the left-most bit.
            encodedBitsList.Add(bits[0]);

            // Invert all bits to the right of the left-most bit and truncate the right-most bit.
            for (int i = 1; i < bits.Length - 1; i++)
            {
                // Add the inverse of the current bit.
                encodedBitsList.Add(!bits[i]);
            }

            // Convert the encoded bits list to an array.
            BitArray encodedBits = new BitArray(encodedBitsList.ToArray());

            return encodedBits;
        }

        public static BitArray Decode(BitArray bits, bool subtract = true)
        {
            // Initialize the decoded bits list.
            var decodedBitsList = new List<bool>(bits.Length + 1);
            if (bits.Length == 0) return bits;

            // Add the left-most bit.
            decodedBitsList.Add(bits[0]);

            // Invert all bits to the right of the left-most bit and truncate the right-most bit.
            for (int i = 1; i < bits.Length; i++)
            {
                // Add the inverse of the current bit.
                decodedBitsList.Add(!bits[i]);
            }

            // Append a 1 bit as the right-most bit.
            decodedBitsList.Add(true);

            // Convert the decoded bits list to an array.
            BitArray decodedBits = new BitArray(decodedBitsList.ToArray());

            if (subtract)
            {
                // Subtract 2 from the decoded binary value.
                decodedBits = decodedBits.Subtract(2);
            }

            return decodedBits;
        }
    }
}