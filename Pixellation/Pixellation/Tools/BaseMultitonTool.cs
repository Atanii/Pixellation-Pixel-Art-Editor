using System;
using System.Collections.Generic;
using System.Reflection;

namespace Pixellation.Tools
{
    public abstract class BaseMultitonTool<T> : BaseTool where T : class
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
                    throw new MultitonException($"Classes inherited from {typeof(BaseMultitonTool<T>)} must have a private constructor.");
                }
            }

            private static readonly Dictionary<string, T> instances = new Dictionary<string, T>();

            internal static T Get(string id)
            {
                if (!instances.ContainsKey(id))
                {
                    instances.Add(id, CreateInstance());
                }
                return instances[id];
            }
        }

        protected BaseMultitonTool() : base()
        {
        }

        public static T GetInstance(string id) => MultitonToolCreator.Get(id);
    }
}