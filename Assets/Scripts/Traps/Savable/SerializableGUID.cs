using System;
using System.Runtime.InteropServices;
using UnityEngine;
namespace TrapSystem{
    [Serializable]
    public struct SerializableGUID : IEquatable<SerializableGUID>{
        [SerializeField, HideInInspector] public uint Part1;
        [SerializeField, HideInInspector] public uint Part2;
        [SerializeField, HideInInspector] public uint Part3;
        [SerializeField, HideInInspector] public uint Part4;

        public readonly static SerializableGUID Empty = new(0, 0, 0, 0);

        public SerializableGUID(uint part1, uint part2, uint part3, uint part4){
            Part1 = part1;
            Part2 = part2;
            Part3 = part3;
            Part4 = part4;
        }

        public SerializableGUID(Guid guid){
            Span<byte> span = stackalloc byte[16];
            guid.TryWriteBytes(span);

            Part1 = MemoryMarshal.Cast<byte, uint>(span.Slice(0, 4))[0];
            Part2 = MemoryMarshal.Cast<byte, uint>(span.Slice(4, 4))[0];
            Part3 = MemoryMarshal.Cast<byte, uint>(span.Slice(8, 4))[0];
            Part4 = MemoryMarshal.Cast<byte, uint>(span.Slice(12, 4))[0];
        }

        public static SerializableGUID NewGuid() => new SerializableGUID(Guid.NewGuid());

        public static implicit operator SerializableGUID(Guid guid) => new SerializableGUID(guid);

        public override readonly string ToString() => $"{Part1:X8}-{Part2:X8}-{Part3:X8}-{Part4:X8}";
        public override readonly bool Equals(object obj)
        {
            return obj is SerializableGUID other && Equals(other);
        }

        public readonly bool Equals(SerializableGUID other)
        {
            return Part1 == other.Part1 && Part2 == other.Part2 && Part3 == other.Part3 && Part4 == other.Part4;
        }

        public override readonly int GetHashCode()
        {
            return HashCode.Combine(Part1, Part2, Part3, Part4);
        }

        public static bool operator ==(SerializableGUID left, SerializableGUID right) => left.Equals(right);
        public static bool operator !=(SerializableGUID left, SerializableGUID right) => !left.Equals(right); 
    }
}