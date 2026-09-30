// Copyright (c) 2026 Gavin W. Landon (chizl.com)
// Licensed under the MIT License. See LICENSE file http://www.chizl.com/LICENSE.txt for full license information.
// SPDX-License-Identifier: MIT

namespace Chizl.ThreadSupport
{
    using System;
    using System.Threading;

    /// <summary>
    /// Thread Safe System.Boolean value..
    /// <para>Why ABool:</para>
    /// <list type="bullet">
    ///   <item><description>Correct on all .NET versions</description></item>
    ///   <item><description>Correct on all CPU architectures</description></item>
    ///   <item><description>Strong visibility guarantees</description></item>
    ///   <item><description>Thread-safe visibility</description></item>
    ///   <item><description>Atomic write</description></item>
    ///   <item><description>Minimal overhead</description></item>
    ///   <item><description>No #if</description></item>
    ///   <item><description>No 64-bit interlocked penalties</description></item>
    ///   <item><description>Matches how CancellationTokenSource, SpinWait, etc. work internally</description></item>
    ///   <item><description>Implemented as a reference type to preserve shared identity and avoid copy-related bugs</description></item>
    /// </list>
    /// <para><b>ABool has bool behavior:</b></para>
    /// <list type="bullet">
    ///   <item><description>if (x)</description></item>
    ///   <item><description>Assign true/false</description></item>
    ///   <item><description>No API changes</description></item>
    ///   <item><description>No locks</description></item>
    ///   <item><description>Cheap</description></item>
    /// </list>
    /// </summary>
    internal sealed class ABool : IEquatable<ABool>
    {
        private const int FALSE = 0;
        private const int TRUE = 1;

        // Value uses for True (1) or False (0).
        private int _boolValue;

        /// <summary>
        /// Thread Safe System.Boolean value..<br/>
        /// <code>
        /// static ABool _myVar = ABool.True;
        /// or
        /// static ABool _myVar = ABool.False;
        /// ...
        /// var prevValue = _myVar.SetVal(false);
        /// Console.WriteLine($"The old value was: {prevValue}.  The new value is: {_myVar}");
        /// </code>
        /// </summary>
        /// <param name="defaultValue">(Optional) Default: false</param>
        private ABool(bool defaultValue = false) => _boolValue = defaultValue ? TRUE : FALSE;

        #region Factory Properties
        /// <summary>
        /// Returns a new instances of ABool with a default value of false.
        /// </summary>
        public static ABool False => new ABool(false);
        /// <summary>
        /// Returns a new instances of ABool with a default value of true.
        /// </summary>
        public static ABool True => new ABool(true);
        /// <summary>
        /// Property to return current state of bool value.  Usage is in comparison to .Equals()
        /// <code>
        /// static ABool _myVar1 = ABool.True;
        /// static ABool _myVar2 = ABool.True;
        /// ...
        /// Console.WriteLine($"{_myVar1.Value == _myVar2.Value)}");
        ///     Return: True
        /// Console.WriteLine($"{_myVar1.Equals(_myVar2)}");
        ///     Return: True
        /// Console.WriteLine($"{_myVar1 == _myVar2}");
        ///     Return: True
        /// </code>
        /// </summary>
        public bool Value => Volatile.Read(ref _boolValue) == TRUE;
        #endregion

        #region Factory Methods
        /// <summary>
        /// Lock-free Toggle (atomic) using CAS loop, which flips the boolean value (true to false, or false to true).
        /// </summary>
        /// <returns>The previous value before flipping.</returns>
        public bool Toggle()
        {
            while (true)
            {
                int oldVal = Volatile.Read(ref _boolValue);
                int newVal = (oldVal == TRUE) ? FALSE : TRUE;

                // if _boolValue is still oldVal, swap it; otherwise retry
                int observed = Interlocked.CompareExchange(ref _boolValue, newVal, oldVal);
                if (observed == oldVal)
                    return oldVal == TRUE; // returns previous value
            }
        }
        /// <summary>
        /// Developer Preferences Method as alternative to SetVal(true).<br/>
        /// Sets ABool value to True even if it's already True, then returns the previous value.
        /// <code>
        /// var prevABool = _myVar.SetTrue();
        /// or
        /// if (!_myVar.SetTrue())
        ///     Console.WriteLine($"Value was false, but now set to {_myVar}.");
        /// </code>
        /// </summary>
        /// <returns>Previous Boolean value before SetTrue().</returns>
        public bool SetTrue() => SetVal(true);
        /// <summary>
        /// Developer Preferences Method as alternative to SetVal(false).<br/>
        /// Sets ABool value to False even if it's already False, then returns the previous value.
        /// <code>
        /// var prevABool = _myVar.SetFalse();
        /// or
        /// if (!_myVar.SetFalse())
        ///     Console.WriteLine($"Value was already set to false.  Current value is {_myVar}.");
        /// </code>
        /// </summary>
        /// <returns>Previous Boolean value before SetTrue.</returns>
        public bool SetFalse() => SetVal(false);
        /// <summary>
        /// Only sets the value to True if it is currently False.
        /// </summary>
        /// <returns>True if the value was changed from False to True. 
        /// False if it was already True.</returns>
        public bool TrySetTrue() => Interlocked.CompareExchange(ref _boolValue, TRUE, FALSE) == FALSE;
        /// <summary>
        /// Only sets the value to False if it is currently True.
        /// </summary>
        /// <returns>True if the value was changed from True to False. 
        /// False if it was already False.</returns>
        public bool TrySetFalse() => Interlocked.CompareExchange(ref _boolValue, FALSE, TRUE) == TRUE;
        /// <summary>
        /// Only sets the Value based on bool parameter if the current Value isn't already equal to it.
        /// </summary>
        /// <returns>True if the value was changed. False if it was already set.</returns>
        public bool TrySetValue(bool value)
        {
            (int newValue, int ifValue) = value ? (TRUE, FALSE) : (FALSE, TRUE);
            return Interlocked.CompareExchange(ref _boolValue, newValue, ifValue) == ifValue;
        }
        /// <summary>
        /// Can change the Boolean value.
        /// <code>
        /// var prevABool = _myVar.SetVal(true);
        /// or
        /// if (!_myVar.SetVal(true))
        ///     Console.WriteLine($"Value was false, but now set to {_myVar}.");
        /// else
        ///     Console.WriteLine($"Value was already set to true.  Current value is {_myVar}.");
        /// </code>
        /// </summary>
        /// <param name="value">New Boolean value.</param>
        /// <returns>Previous Boolean value before SetVal().</returns>
        public bool SetVal(bool value) => Interlocked.Exchange(ref _boolValue, value ? TRUE : FALSE) == TRUE;
        #endregion

        #region Equality and Conversion Operators
        /// <summary>
        /// Determines whether two ABool instances are equal.
        /// </summary>
        /// <param name="a">The first ABool to compare.</param>
        /// <param name="b">The second ABool to compare.</param>
        /// <returns>true if the specified ABool instances are equal; otherwise, false.</returns>
        public static bool operator ==(ABool a, ABool b) => ReferenceEquals(a, null) ? ReferenceEquals(b, null) : a.Equals(b);
        /// <summary>
        /// Determines whether two ABool instances are not equal.
        /// </summary>
        /// <param name="a">The first ABool to compare.</param>
        /// <param name="b">The second ABool to compare.</param>
        /// <returns>true if the specified ABool instances are not equal; otherwise, false.</returns>
        public static bool operator !=(ABool a, ABool b) => !(a == b);

        /// <summary>
        /// Converts an ABool instance to a Boolean value.
        /// </summary>
        /// <param name="obj">The ABool instance to convert.</param>
        public static implicit operator bool(ABool obj) => obj?.Value ?? false;
        /// <summary>
        /// Converts a Boolean value to an ABool instance.
        /// </summary>
        /// <param name="value">The Boolean value to convert.</param>
        public static implicit operator ABool(bool value) => new ABool(value);
        #endregion

        #region Reference Equality Operators
        /// <summary>
        /// Fallback equality for system-level calls
        /// Determines whether this instance and another specified Chizl.ThreadSupport.ABool object have the same value.
        /// </summary>
        /// <param name="obj">The ABool object to compare to this instance.</param>
        /// <returns>true if the value of the obj parameter is the same as the value of this instance; otherwise, false. If value is null, the method returns false.</returns>
        public override bool Equals(object obj) => obj is ABool other && Equals(other)
                                                || obj is bool other2 && Equals(other2);
        /// <summary>
        /// explicit + fast; same as bool hash behavior.<br/>
        /// NOTE: Do not mutate while used as a key in hash-based collections.
        /// </summary>
        /// <returns>Based on Value, True returns 1, False returns 0.</returns>
        public override int GetHashCode() => Value ? 1 : 0;
        /// <summary>
        /// To display in strings, this override will show up correctly.
        /// <code>
        /// static ABool _myVar = ABool.False;
        /// ...
        /// if (!_myVar)
        ///     Console.WriteLine($"_myVar = {_myVar}");
        ///     
        /// Returns: "_myVar = false"
        /// </code>
        /// </summary>
        /// <returns></returns>
        public override string ToString() => Value.ToString();
        #endregion

        #region IEquatable Implementation
        /// <summary>
        /// <see cref="IEquatable{T}"/>, High-performance, strongly-typed equality
        /// Determines whether this instance and another specified Chizl.ThreadSupport.ABool object have the same value.
        /// </summary>
        /// <param name="other">The ABool object to compare to this instance.</param>
        /// <returns>true if the value of the obj parameter is the same as the value of this instance; otherwise, false. If value is null, the method returns false.</returns>
        public bool Equals(ABool other)
        {
            if (ReferenceEquals(other, null)) return false;
            if (ReferenceEquals(this, other)) return true;

            return this.Equals(other.Value);
        }
        /// <summary>
        /// Indicates whether the current object is equal to the specified Boolean value.
        /// </summary>
        /// <param name="other">The Boolean value to compare with the current object.</param>
        /// <returns>true if the current object and the specified value are equal; otherwise, false.</returns>
        public bool Equals(bool other)
        {
            // read each side once
            bool a = this.Value;
            bool b = other;
            return a == b;
        }
        #endregion

    }
}