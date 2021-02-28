using System;
using System.Collections.Generic;
using System.Reflection;

namespace Pixellation.Tools
{
    /// <summary>
    /// Base multiton pattern class for all drawing tools used in Pixellation.
    /// </summary>
    /// <typeparam name="T">Type of inheriting class.</typeparam>
    public abstract class BaseMultitonTool<T> : BaseTool, ITool where T : class, ITool
    {
        public class MultitonException : Exception
        {
            public MultitonException(string msg) : base(msg)
            {
            }
        }

        /// <summary>
        /// Class for creating instance of the inheriting class. Inheriting class must have a private, parameterless constructor!
        /// </summary>
        private class MultitonToolCreator
        {
            static MultitonToolCreator()
            {
            }

            private static T CreateInstance()
            {
                ConstructorInfo constructorInfo = typeof(T).GetConstructor(BindingFlags.Instance |
                    BindingFlags.NonPublic, Type.DefaultBinder, Type.EmptyTypes, null);

                if (constructorInfo != null)
                {
                    return constructorInfo.Invoke(null) as T;
                }
                else
                {
                    throw new MultitonException($"Classes inherited from {typeof(BaseMultitonTool<T>)} must have a private constructor!");
                }
            }

            private static readonly Dictionary<string, T> instances = new Dictionary<string, T>();

            internal static T Get(string key)
            {
                if (key == string.Empty || key == null)
                {
                    throw new MultitonException($"Instance key cannot be empty string or null!");
                }

                var _key = key.ToLower();
                if (!instances.ContainsKey(_key))
                {
                    instances.Add(_key, CreateInstance());
                }

                return instances[_key];
            }
        }

        protected BaseMultitonTool() : base()
        {
        }

        /// <summary>
        /// Gets an instance of the inheriting class.
        /// </summary>
        /// <param name="key">Instancekey.</param>
        /// <returns>Created or already created instance.</returns>
        public static T GetInstance(string key) => MultitonToolCreator.Get(key);

        /// <summary>
        /// Gets an instance as <see cref="ITool"/>.
        /// </summary>
        /// <param name="key">Instancekey.</param>
        /// <returns>Created or already created instance.</returns>
        public ITool GetInstanceByKey(string key) => MultitonToolCreator.Get(key);
    }
}