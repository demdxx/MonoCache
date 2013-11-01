/**
 * FileCache.cs
 * 
 * @project MonoCache
 * @author Dmitry Ponomarev <demdxx@gmail.com>
 * @license MIT Copyright (c) 2013 demdxx. All rights reserved.
 *
 *
 * Copyright (C) <2013> Dmitry Ponomarev <demdxx@gmail.com>
 *
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 *
 * The above copyright notice and this permission notice shall be included in
 * all copies or substantial portions of the Software.
 *
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
 * SOFTWARE.
 */
using System;
using System.Collections.Generic;

#if !IO_INTERFACE
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
#endif

namespace MonoCache
{
  /**
   * Stores data in the file system in a special directory.
   * Accessing data is on a key that can be of any unique
   * string that is converted into a hash key and possibly
   * trying to get the file extension from the passed name.
   * 
   * Example:
   * 
   * // Get file cache manager
   * var fc = new FileCache (cacheDir, 1000 * 60 * 60 * 24);
   * 
   * // Store data
   * fc.Add (url, data);
   * 
   * // ...
   * 
   * // Restore data
   * object data = fc.Get (url);
   * 
   * // ...
   * 
   * // Remove data from cache
   * fc.Remove (url);
   * 
   * // OR
   * 
   * fc.Clear ();
   * 
   * // OR
   * 
   * fc.ClearExpired ();
   */
  public sealed class FileCache : CacheBase
  {
#if IO_INTERFACE
    private IOInterface IO;
#endif

    /**
     * Directory for store caches
     */
    public string BaseDir { get; private set; }

    /**
     * Constructor
     * @param baseDir
     * @param liveTime default 3 days
     */
#if IO_INTERFACE
    public FileCache (string baseDir, IOInterface io, long lifeTime = 1000 * 60 * 60 * 24 * 3) : base (lifeTime)
#else // IO_INTERFACE
    public FileCache (string baseDir, long lifeTime = 1000 * 60 * 60 * 24 * 3) : base (lifeTime)
#endif // END IO_INTERFACE
    {

      if (null == baseDir || baseDir.Length<1) {
        throw new ArgumentNullException ("baseDir", "It is file cache root directory. Can`t be null!");
      }
#if IO_INTERFACE
      IO = io;
      if (null == IO)
      {
        throw new ArgumentNullException("io", "IO interface. Can`t be null!");
      }

      if (!IO.DirectoryExists(baseDir))
      {
        if (!IO.DirectoryCreate(baseDir))
        {
#else // IO_INTERFACE
      if (!Directory.Exists (baseDir))
      {
        if (null == Directory.CreateDirectory (baseDir))
        {
#endif // END IO_INTERFACE
          throw new Exception("Can`t create cache root directory.");
        }
      }
      BaseDir = baseDir;
    }

    #region Implementation

    /**
     * Set value to cache
     * @param key
     * @param value
     * @param rewrite
     * @param checkExpired Check the end of the lifetime of
     * @return success state
     */
    public override bool Set(string key, object value, bool rewrite = false, bool checkExpired = true)
    {
      string filePath = FilePath (key);
#if IO_INTERFACE
      if (checkExpired && !rewrite && IO.FileExists(filePath))
      {
        if (!IsExpired(IO.FileGetLastWriteTimeUtc(filePath)))
        {
          return false;
        }
      }
      IO.FileSerialize(filePath, value);
#else // IO_INTERFACE
      if (checkExpired && !rewrite && File.Exists (filePath)) {
        if (!IsExpired (File.GetLastWriteTimeUtc (filePath))) {
          return false;
        }
      }
      using (FileStream fs = File.OpenWrite (filePath)) {
        BinaryFormatter bf = new BinaryFormatter();
        bf.Serialize (fs, value);
      }
#endif // END IO_INTERFACE
      return true;
    }

    private bool CheckFile(string filePath, bool checkExpired = true)
    {
#if IO_INTERFACE
      if (!IO.FileExists(filePath))
      {
        return false;
      }
      if (checkExpired && IsExpired(IO.FileGetLastWriteTimeUtc(filePath)))
      {
        IO.FileDelete(filePath);
        return false;
      }
#else // IO_INTERFACE
      if (!File.Exists (filePath)) {
        return false;
      }
      if (checkExpired && IsExpired (File.GetLastWriteTimeUtc (filePath))) {
        File.Delete (filePath);
        return false;
      }
#endif // END IO_INTERFACE
      return true;
    }

    /**
     * If object cached
     * @param key
     * @param checkExpired Check the end of the lifetime of
     * @return success status
     */
    public override bool Check(string key, bool checkExpired = true)
    {
      return CheckFile(FilePath(key), checkExpired);
    }

    /**
     * Get object by key
     * @param key
     * @param checkExpired Check the end of the lifetime of
     * @return object
     */
    public override object Get(string key, bool checkExpired = true)
    {
      string filePath = FilePath (key);
      if (!CheckFile(filePath, checkExpired))
      {
        return null;
      }
      // Read file bynary
#if IO_INTERFACE
      return IO.FileDeserialize(filePath);
#else // IO_INTERFACE
      using (FileStream fs = File.OpenRead (filePath)) {
        BinaryFormatter bf = new BinaryFormatter ();
        return bf.Deserialize (fs);
      }
#endif // END IO_INTERFACE
    }

    /**
     * Get address by key
     * @param key
     * @param checkExpired Check the end of the lifetime of
     * @return object
     */
    public override string GetAddress(string key, bool checkExpired = true)
    {
      string filePath = FilePath (key);
      if (!CheckFile(filePath, checkExpired))
      {
        return null;
      }
      return filePath;
    }

    private void RemoveFile(string filePath)
    {
#if IO_INTERFACE
      if (IO.FileExists(filePath))
      {
        IO.FileDelete(filePath);
      }
#else // IO_INTERFACE
      if (File.Exists (filePath)) {
        File.Delete (filePath);
      }
#endif // END IO_INTERFACE
    }

    /**
     * Remove value by key
     * @[aram key
     */
    public override void Remove(string key)
    {
      RemoveFile(FilePath(key));
    }

    /**
     * Remove value by id
     * @param id
     */
    protected override void RemoveFromId(string id)
    {
      RemoveFile(FilePathByID(id));
    }

    /**
     * List of expired keys
     * @return string []
     */
    protected override string [] ExpiredListOfKeys ()
    {
#if IO_INTERFACE
      string[] files = IO.DirectoryGetFiles(BaseDir, "*");
#else // IO_INTERFACE
      string [] files = Directory.GetFiles (BaseDir, "*");
#endif // END IO_INTERFACE
      List<string> result = new List<string> ();
      if (null != files) {
        foreach (string file in files) {
#if IO_INTERFACE
          if (IsExpired(IO.FileGetLastWriteTimeUtc(file)))
          {
            result.Add(IO.PathGetFileNameWithoutExtension(file));
          }
#else // IO_INTERFACE
          if (IsExpired (File.GetLastWriteTimeUtc (file))) {
            result.Add (Path.GetFileNameWithoutExtension (file));
          }
#endif // END IO_INTERFACE
        }
      }
      return result.ToArray ();
    }

    /**
     * Full list of keys
     * @return string []
     */
    protected override string [] ListOfKeys ()
    {
#if IO_INTERFACE
      string[] files = IO.DirectoryGetFiles(BaseDir, "*");
#else // IO_INTERFACE
      string [] files = Directory.GetFiles (BaseDir, "*");
#endif // END IO_INTERFACE
      if (null != files) {
        for (int i = 0; i < files.Length; i++) {
#if IO_INTERFACE
          files[i] = IO.PathGetFileNameWithoutExtension(files[i]);
#else // IO_INTERFACE
          files [i] = Path.GetFileNameWithoutExtension (files [i]);
#endif // END IO_INTERFACE
        }
      }
      return files;
    }

    #endregion // Implementation

    #region Helpers

    private string FilePath (string key)
    {
#if IO_INTERFACE
      return IO.PathCombine(BaseDir, PrepareName(key));
#else // IO_INTERFACE
      return Path.Combine (BaseDir, PrepareName (key));
#endif // END IO_INTERFACE
    }

    private string FilePathByID (string id)
    {
#if IO_INTERFACE
      string[] files = IO.DirectoryGetFiles(BaseDir, id + "*");
      if (null == files && files.Length > 0)
      {
        return IO.PathCombine(BaseDir, files[0]);
      }
#else // IO_INTERFACE
      string[] files = Directory.GetFiles (BaseDir, id+"*");
      if (null == files && files.Length > 0) {
        return Path.Combine (BaseDir, files [0]);
      }
#endif // END IO_INTERFACE
      return null;
    }

    private string PrepareName (string key)
    {
#if IO_INTERFACE
      string ext = IO.PathGetExtension(key);
#else // IO_INTERFACE
      string ext = Path.GetExtension (key);
#endif // END IO_INTERFACE
      string name = StringToMD5 (key);
      return string.IsNullOrEmpty (ext)
        ? name : ('.' == ext[0] ? name + ext : name + "." + ext);
    }

    #endregion
  }
}