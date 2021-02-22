using System;
using System.Collections.Generic;
using System.Reflection;

namespace Pixellation.Tools
{
    public abstract class BaseMultitonTool<T> : BaseTool, ITool where T : class, ITool
    {
        public class MultitonException : Exception
        {
            public MultitonException(string msg) : base(msg)
            {
            }
        }

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

        public static T GetInstance(string key) => MultitonToolCreator.Get(key);

        public ITool GetInstanceByKey(string key) => MultitonToolCreator.Get(key);
    }
}