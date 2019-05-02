//--------------------------------------------------------------//
// Copyright (c) 2019, Joseph M. Shunia                         //
//                                                              //
// Distributed under the CC-BY 4.0 license (see: LICENSE.md).   //
// https://creativecommons.org/licenses/by/4.0/                 //
//--------------------------------------------------------------//
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;

namespace CompactBinaryDemo
{
    public static class Extensions
    {
        public static BitArray ToBitsCb(this BigInteger value) => CompactBinary.Encode(value);

        public static BitArray ToBits(this BigInteger value) => value.ToBits(true, true);

        public static BitArray ToBits(this BigInteger value, bool minimal, bool isUnsigned)
        {
            if (value == 0) return new BitArray(1, false);

            BitArray bits = value
                .ToByteArray(isUnsigned: isUnsigned && value > 0)
                .ToBitArray();

            if (minimal) bits = bits.TrimEnd(false);

            return bits;
        }

        public static byte[] ToBytes(this BitArray bits)
        {
            if (bits == null)
            {
                return null;
            }

            int bytesLength = bits.Length / 8;
            int remainder = bits.Length % 8;
            if (remainder > 0)
            {
                bytesLength++;
            }

            byte[] bytes = new byte[bytesLength];
            bits.CopyTo(bytes, 0);

            if (bytes.Length < 1)
            {
                bytes = new byte[] { 0x0 };
            }

            return bytes;
        }

        public static BitArray TrimEnd(this BitArray bits, bool value)
        {
            int length = bits.Length, takeCount = length;
            for (int i = length - 1; i >= 0; i--)
            {
                bool b = bits[i];
                if (b == value) takeCount--;
                else break;
            }
            if (takeCount < 1) return new BitArray(0);

            return bits.Slice(0, takeCount);
        }

        public static bool IsEqualTo(this BitArray bits, BitArray other)
        {
            if (bits.Length != other.Length) return false;
            for (int i = 0; i < bits.Length; i++)
                if (bits[i] != other[i]) return false;

            return true;
        }

        public static BigInteger ToIntegerCb(this BitArray bits) => CompactBinary.Decode(bits).ToInteger();

        public static BigInteger ToInteger(this BitArray bits) => bits.ToBytes().ToInteger();

        public static BigInteger ToInteger(this byte[] value) => value.ToInteger(true, false);

        public static BigInteger ToInteger(this byte[] value, bool isUnsigned, bool bigEndian)
        {
            if (value == null) return 0;

            BigInteger intValue = isUnsigned
                ? new BigInteger(value.Concat(new byte[] { 0x0 }).ToArray(), bigEndian)
                : new BigInteger(value, isBigEndian: bigEndian);

            return intValue;
        }

        public static int Hamming(this BitArray bits)
        {
            int h = 0;
            for (int i = 0; i < bits.Length; i++) if (bits[i]) h++;
            return h;
        }

        public static BitArray Flip(this BitArray bits, int position)
        {
            bits[position] = !bits[position];
            return bits;
        }

        public static BitArray Add(this BitArray bits, uint value)
        {
            for (uint i = 0; i < value; i++)
            {
                // Increment the binary value by 1.
                bits = bits.Increment();
            }

            return bits;
        }

        public static BitArray Subtract(this BitArray bits, uint value)
        {
            for (uint i = 0; i < value; i++)
            {
                // Decrement the binary value by 1.
                bits = bits.Decrement();
            }

            return bits;
        }

        public static BitArray Decrement(this BitArray bits)
        {
            var resultBits = (BitArray)bits.Clone();

            // Decrement the binary value by 1.
            bool carry = true;
            for (int i = 0; i < bits.Length; i++)
            {
                // Flip the current bit.
                resultBits[i] = !bits[i];

                if (bits[i])
                {
                    // The current bit is 1, so there is no carry and we can stop flipping bits.
                    carry = false;

                    // Add the remaining bits.
                    while (++i < bits.Length)
                        resultBits[i] = bits[i];

                    break;
                }
            }

            // Remove the carry bit.
            if (carry)
                resultBits = resultBits.LeftShift(1);

            return resultBits;
        }

        public static BitArray Increment(this BitArray bits)
        {
            var resultBits = (BitArray)bits.Clone();

            // Increment the binary value by 1.
            bool carry = true;
            for (int i = 0; i < bits.Length; i++)
            {
                // Flip the current bit.
                resultBits[i] = !bits[i];

                if (!bits[i])
                {
                    // The current bit is 0, so there is no carry and we can stop flipping bits.
                    carry = false;

                    // Add the remaining bits.
                    while (++i < bits.Length)
                        resultBits[i] = bits[i];

                    break;
                }
            }

            // Add the carry bit.
            if (carry)
                resultBits = resultBits.Append(true);

            return resultBits;
        }

        public static BitArray Concat(this BitArray a, BitArray b)
        {
            int length = a.Length + b.Length;
            var bits = new BitArray(length);
            for (int i = 0; i < a.Length; i++)
                bits[i] = a[i];

            int offset = a.Length;
            for (int i = 0; i < b.Length; i++)
                bits[i + offset] = b[i];

            return bits;
        }

        public static BitArray Append(this BitArray bits, bool bit)
        {
            var resultBits = bits.RightShift(1);
            if (bits.Length >= resultBits.Length)
                return resultBits;

            resultBits[bits.Length] = bit;

            return resultBits;
        }

        public static BitArray Reverse(this BitArray bits)
        {
            BitArray resultBits = new BitArray(bits.Length);
            for (int i = bits.Length - 1, j = 0; i >= 0; i--, j++)
            {
                resultBits[j] = bits[i];
            }

            return resultBits;
        }

        public static BitArray Slice(this BitArray bits, int start, int count)
        {
            if (start == 0 && count == bits.Length)
                return (BitArray)bits.Clone();

            int length = start + count;
            var resultBits = new BitArray(length);
            for (int i = start; i < count; i++)
            {
                resultBits[i] = bits[i];
            }

            return resultBits;
        }

        public static BitArray ToBitArray(this byte[] bytes) => new BitArray(bytes);

        public static bool[] ToBools(this BitArray bits)
        {
            bool[] bitArray = new bool[bits.Length];
            bits.CopyTo(bitArray, 0);

            return bitArray;
        }

        public static BitArray ToBitArray(this IEnumerable<bool> bits)
        {
            if (bits == null || !bits.Any()) return null;
            return new BitArray(bits.ToArray());
        }

        public static string ToStringBinary(this BitArray bits)
        {
            if (bits == null) return null;

            var sb = new StringBuilder();
            for (int i = 0; i < bits.Length; i++)
                sb.Append(bits[i] ? '1' : '0');

            return sb.ToString();
        }
    }
}