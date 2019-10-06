﻿using System;
using System.Collections;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace ChessPuzzle
{
    public class AssertException : Exception
    {
        private const string DEFAULT_MESSAGE = "Unexpected internal program state";

        public AssertException() : this(DEFAULT_MESSAGE)
        {
        }

        public AssertException(string message) : base(message)
        {
        }

        public AssertException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }

    class Throw
    {
        [DoesNotReturn]
        public static void ArgumentNullException(string argName) =>
            throw new ArgumentNullException(argName);

        [DoesNotReturn]
        public static void ArgumentOutOfRangeException(string argName) =>
            throw new ArgumentOutOfRangeException(argName);

        [DoesNotReturn]
        public static void AssertException(string msg) =>
            throw new AssertException(msg);
    }

    class Assert
    {
        [Conditional("DEBUG")]
        public static void That([DoesNotReturnIf(false)] bool condition)
        {
            if (!condition)
                Throw.AssertException($"Unexpected condition value: {condition}");
        }
    }

    struct ArgChecker<T>
    {
        public string ArgName { get; }
        public T ArgValue { get; }

        public ArgChecker(T value, string argName)
        {
            ArgName = argName;
            ArgValue = value;
        }

        private void EnsureIsNotNull([AllowNull]T value) =>
            EnsureCondition(value != null, Throw.ArgumentNullException);

        [Conditional("DEBUG")]
        public void IsNotNull() => EnsureIsNotNull(ArgValue);

        [Conditional("DEBUG")]
        public void IsNotNullOrEmpty()
        {
            IsNotNull();
            EnsureCondition(
                (ArgValue as IEnumerable)?.GetEnumerator().MoveNext() ?? false,
                Throw.ArgumentOutOfRangeException);
        }

        private void EnsureCondition([DoesNotReturnIf(false)] bool condition, Action<string> argThrower)
        {
            if (!condition)
                argThrower(ArgName);
        }
    }

    class Ensure
    {
        public static ArgChecker<T> Arg<T>(T value, string argName) => new ArgChecker<T>(value, argName);
    }
}
