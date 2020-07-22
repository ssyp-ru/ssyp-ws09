// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the README.md file in the project root for more information.

using System;
using System.Diagnostics.CodeAnalysis;

namespace TheGrapho.Parser.Utilities
{
    public readonly struct TextSpan : IEquatable<TextSpan>, IComparable<TextSpan>
    {
        public void Deconstruct(out int start, out int length)
        {
            start = Start;
            length = Length;
        }

        public TextSpan(int start, int length)
        {
            if (start < 0)
                throw new ArgumentOutOfRangeException(nameof(start));

            if (start + length < start)
                throw new ArgumentOutOfRangeException(nameof(length));

            Start = start;
            Length = length;
        }

        public int Start { get; }

        public int End => Start + Length;

        public int Length { get; }

        public bool IsEmpty => Length == 0;

        public bool Contains(int position) => unchecked((uint) (position - Start) < (uint) Length);

        public bool Contains(TextSpan span) => span.Start >= Start && span.End <= End;

        public bool OverlapsWith(TextSpan span)
        {
            var overlapStart = Math.Max(Start, span.Start);
            var overlapEnd = Math.Min(End, span.End);
            return overlapStart < overlapEnd;
        }

        [return: MaybeNull]
        public TextSpan? Overlap(TextSpan span)
        {
            var overlapStart = Math.Max(Start, span.Start);
            var overlapEnd = Math.Min(End, span.End);

            return overlapStart < overlapEnd
                ? FromBounds(overlapStart, overlapEnd)
                : (TextSpan?) null;
        }

        public bool IntersectsWith(TextSpan span) => span.Start <= End && span.End >= Start;

        public bool IntersectsWith(int position) => unchecked((uint) (position - Start) <= (uint) Length);

        [return: MaybeNull]
        public TextSpan? Intersection(TextSpan span)
        {
            var intersectStart = Math.Max(Start, span.Start);
            var intersectEnd = Math.Min(End, span.End);

            return intersectStart <= intersectEnd
                ? FromBounds(intersectStart, intersectEnd)
                : (TextSpan?) null;
        }

        public static TextSpan FromBounds(int start, int end)
        {
            if (start < 0)
                throw new ArgumentOutOfRangeException(nameof(start), "start must not be negative");

            if (end < start)
                throw new ArgumentOutOfRangeException(nameof(end), "end must not be less than start");

            return new TextSpan(start, end - start);
        }

        public static bool operator ==(TextSpan left, TextSpan right) => left.Equals(right);

        public static bool operator !=(TextSpan left, TextSpan right) => !left.Equals(right);

        public bool Equals(TextSpan other) => Start == other.Start && Length == other.Length;

        public override bool Equals([AllowNull] object? obj) => obj is TextSpan span && Equals(span);

        public override int GetHashCode() => HashCode.Combine(Start, Length);

        [return: NotNull]
        public override string ToString() => $"{nameof(Start)}: {Start}, {nameof(Length)}: {Length}";

        public int CompareTo(TextSpan other)
        {
            var (start, length) = other;
            var diff = Start - start;

            if (diff != 0)
                return diff;

            return Length - length;
        }
    }
}