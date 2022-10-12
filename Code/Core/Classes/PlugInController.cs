using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace WireGuard.Core.Classes
{
    /// <summary>
    /// Class for the loading of plugins
    /// </summary>
    public class PlugInController<T> : IEnumerable<T>
    {
        #region Delegates & Events

        /// <summary>
        /// Delegate for a mehtod to handel when an object cant be created
        /// </summary>
        /// <param name="file">File in which the error occured</param>
        /// <param name="type">Type of object that failed to create</param>
        public delegate void UnableToCreateObjectEventHandler(string file, Type type);

        /// <summary>
        /// Event which gets called if an object is unable to be created
        /// </summary>
        public event UnableToCreateObjectEventHandler UnableToCreateObject;

        #endregion

        #region Variables

        /// <summary>
        /// Typte of object to be loaded by the manager
        /// </summary>
        Type plugInType;

        /// <summary>
        /// List of loaded Plugins
        /// </summary>
        List<T> lstLoaded;

        #endregion

        #region Methods

        /// <summary>
        /// Constructor
        /// </summary>
        public PlugInController()
        {
            plugInType = typeof(T);
            lstLoaded = new List<T>();
        }

        /// <summary>
        /// Loads plugins from the specified file
        /// </summary>
        /// <param name="file">File to load the plugins from</param>
        public void Load(string file = null, object[] args = null)
        {
            Assembly dll = null;

            if (file != null)
            {
                if (!System.IO.File.Exists(file))
                    throw new System.IO.FileNotFoundException(file);

                dll = Assembly.LoadFrom(file);
            }
            else
                dll = Assembly.GetCallingAssembly();

            // Get all types in the assembly which inherit from the specified type
            Type[] types = dll.GetTypes().Where(t => plugInType.IsAssignableFrom(t) && t.IsAbstract == false && t.IsClass).ToArray();

            // Loop throug the types and try to create an object from them
            foreach (Type t in types)
            {
                object instance = null;

                if (args == null)
                    instance = Activator.CreateInstance(t);
                else
                    instance = Activator.CreateInstance(t, args);

                if (instance != null)
                    lstLoaded.Add((T)instance);
                else
                    UnableToCreateObject?.Invoke(file, t);
            }
        }

#nullable enable

        /// <summary>
        /// Fins an object with the specfied conditions or returns null
        /// </summary>
        /// <param name="predicate">Predicate function to compare the objects</param>
        /// <returns>Returns <see cref="T"/> or NULL</returns>
        public T? Find(Predicate<T> predicate) => lstLoaded.Find(predicate);

#nullable disable

        #region Interface methods

        public IEnumerator<T> GetEnumerator() => lstLoaded.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        #endregion

        #endregion

        #region Propertys

        /// <summary>
        /// Gets the number of loaded plugins
        /// </summary>
        public int Count => lstLoaded.Count;

        /// <summary>
        /// Indexer for returing the object on the specifed position
        /// </summary>
        /// <param name="index">index to retrief</param>
        /// <returns><see cref="T"/></returns>
        public T this[int index] => lstLoaded[index];

        #endregion
    }
}
