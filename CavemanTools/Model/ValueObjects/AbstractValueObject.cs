using System;

namespace CavemanTools.Model.ValueObjects
{
    public abstract class AbstractValueObject<T>
    {
        protected T _value;

        /// <summary>
        /// 
        /// </summary>
        /// <exception cref="ArgumentException"></exception>
        /// <param name="value"></param>
        public AbstractValueObject(T value)
        {
            if (!Validate(value)) throw new ArgumentException();
            _value = value;
        }

        /// <summary>
        /// Is automatically invoked by the constructor
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        protected abstract bool Validate(T value);

        public T Value
        {
            get { return _value; }        
        }


        public override string ToString()
        {
            return "[{0}]{1}".ToFormat(GetType().Name,_value);
        }

        
        public override int GetHashCode()
        {
            return _value.GetHashCode();
        }
     }
}