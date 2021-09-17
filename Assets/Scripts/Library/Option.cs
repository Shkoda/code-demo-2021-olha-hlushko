using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Library
{

    public static class OptionExtension
    {
        public static T? ToNullable<T>(this Option<T> opt) where T : struct
        {
            return opt.Select(v => new T?(v)).ValueOr(null);
        }

        public static Option<string> AsNoneIfNullOrEmpty(this string s)
        {
            return string.IsNullOrEmpty(s) ? Option<string>.None : Option.Some(s);
        }
    }
    public class Option
    {

        public static Option<T> Some<T>(T? value) where T : struct
        {
            return value.HasValue ? new Option<T>(value.Value) : Option<T>.None;
        }

        public static Option<T> Some<T>(T value)
        {
            return Option<T>.NullChecker.Invoke(value) ? Option<T>.None : new Option<T>(value);
        }

        [Conditional("DEBUG")]
        private static void NullCheck<T>(T toCheck)
        {
            if (toCheck == null)
            {
                throw new ArgumentException("Option.Some shouldn't be null");
            }
        }
    }
    public readonly struct Option<T> : IEquatable<Option<T>>
    {
        public static readonly Func<T, bool> NullChecker = typeof(T).IsValueType ? new Func<T, bool>((_) => false) : new Func<T, bool>((v) => v == null);
        public static Option<T> None = default;
        private readonly bool _hasValue;
        private readonly T _value;

        public bool TryGetValue(out T value)
        {
            value = _hasValue ? _value : default;
            return _hasValue;
        }

        public T ValueOr(T defaultCase) => _hasValue ? _value : defaultCase;
        public Option<T> ValueOrTry(Option<T> defaultCase) => _hasValue ? this : defaultCase;
        public T ValueOrWith(Func<T> defaultCase) => _hasValue ? _value : defaultCase();
        public Option<T> ValueOrTryWith(Func<Option<T>> defaultCase) => _hasValue ? this : defaultCase();

        public bool IsSome => _hasValue;
        public bool IsNone => !IsSome;

        public Option(T value)
        {
            _value = value;
            _hasValue = true;
        }

        public Option<U> Cast<U>()
        {
            if (TryGetValue(out var v) && v is U u)
            {
                return Option.Some(u);
            }

            return Option<U>.None;
        }

        public Option<(T, U)> Zip<U>(Option<U> other)
        {
            if (TryGetValue(out var v) && other.TryGetValue(out var v2))
            {
                return Option.Some((v, v2));
            }

            return Option<(T, U)>.None;
        }

        /// <summary>
        /// "Filter" function in Option context.
        /// Returns Some if Option is Some and it's value satisfies predicate.
        /// </summary>
        /// <param name="predicate">Predicate value should satisfy</param>
        /// <returns>Option of T</returns>
        public Option<T> Where(Func<T, bool> predicate)
        {
            if (TryGetValue(out var v) && predicate(v))
            {
                return Option.Some(v);
            }

            return None;
        }

        /// <summary>
        /// "Map" function in Option context
        /// </summary>
        /// <typeparam name="U"></typeparam>
        /// <param name="map">Mapping from T to U</param>
        /// <returns>Option of U</returns>
        public Option<U> Select<U>(Func<T, U> map)
        {
            return TryGetValue(out var v) ? Option.Some(map(v)) : Option<U>.None;
        }

        /// <summary>
        /// "Bind" function in Option context
        /// </summary>
        /// <typeparam name="U"></typeparam>
        /// <param name="bind">Mapping from T to Option<U></param>
        /// <returns>Option of U</returns>
        public Option<U> SelectMany<U>(Func<T, Option<U>> bind)
        {
            return TryGetValue(out var v) ? bind(v) : Option<U>.None;
        }

        public void ForEach(Action<T> action)
        {
            if (TryGetValue(out var value))
            {
                action(value);
            }
        }

        public bool Contains(T value)
        {
            return _hasValue && EqualityComparer<T>.Default.Equals(_value, value);
        }

        public bool Exists(Func<T, bool> predicate)
        {
            return TryGetValue(out var v) && predicate(v);
        }

        #region Equality
        public bool Equals(Option<T> other)
        {
            return _hasValue == other._hasValue && EqualityComparer<T>.Default.Equals(_value, other._value);
        }

        public override bool Equals(object obj)
        {
            return obj is Option<T> other && Equals(other);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (_hasValue.GetHashCode() * 397) ^ EqualityComparer<T>.Default.GetHashCode(_value);
            }
        }

        public static bool operator ==(Option<T> left, Option<T> right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(Option<T> left, Option<T> right)
        {
            return !left.Equals(right);
        }

        #endregion

        public override string ToString()
        {
            return _hasValue ? $"Some: {_value}" : "None";
        }
    }
}