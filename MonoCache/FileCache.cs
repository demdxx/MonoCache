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
using System.IO;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;

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
    /**
     * Directory for store caches
     */
    public string BaseDir { get; private set; }

    /**
     * Constructor
     * @param baseDir
     * @param liveTime default 3 days
     */
    public FileCache (string baseDir, long lifeTime = 1000 * 60 * 60 * 24 * 3) : base (lifeTime)
    {
      if (null == baseDir || baseDir.Length<1) {
        throw new ArgumentNullException ("baseDir", "It is file cache root directory. Can`t be null!");
      }
      if (!Directory.Exists (baseDir)) {
        if (null == Directory.CreateDirectory (baseDir)) {
          throw new Exception ("Can`t create cache root directory.");
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
      if (checkExpired && !rewrite && File.Exists (filePath)) {
        if (!IsExpired (File.GetLastWriteTimeUtc (filePath))) {
          return false;
        }
      }
      using (FileStream fs = File.OpenWrite (filePath)) {
        BinaryFormatter bf = new BinaryFormatter ();
        bf.Serialize (fs, value);
      }
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
      string filePath = FilePath (key);
      if (!File.Exists (filePath)) {
        return false;
      }
      if (checkExpired && IsExpired (File.GetLastWriteTimeUtc (filePath))) {
        File.Delete (filePath);
        return false;
      }
      return true;
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
      if (!File.Exists (filePath)) {
        return null;
      }
      if (checkExpired && IsExpired (File.GetLastWriteTimeUtc (filePath))) {
        File.Delete (filePath);
        return null;
      }

      // Read file bynary
      using (FileStream fs = File.OpenRead (filePath)) {
        BinaryFormatter bf = new BinaryFormatter ();
        return bf.Deserialize (fs);
      }
    }

    /**
     * Remove value by key
     * @[aram key
     */
    public override void Remove(string key)
    {
      string filePath = FilePath (key);
      if (File.Exists (filePath)) {
        File.Delete (filePath);
      }
    }

    /**
     * Remove value by id
     * @param id
     */
    protected override void RemoveFromId(string id)
    {
      string filePath = FilePathByID (id);
      if (File.Exists (filePath)) {
        File.Delete (filePath);
      }
    }

    /**
     * List of expired keys
     * @return string []
     */
    protected override string [] ExpiredListOfKeys ()
    {
      string [] files = Directory.GetFiles (BaseDir, "*");
      List<string> result = new List<string> ();
      if (null != files) {
        foreach (string file in files) {
          if (IsExpired (File.GetLastWriteTimeUtc (file))) {
            result.Add (Path.GetFileNameWithoutExtension (file));
          }
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
      string[] files = Directory.GetFiles (BaseDir, "*");
      if (null != files) {
        for (int i = 0; i < files.Length; i++) {
          files [i] = Path.GetFileNameWithoutExtension (files [i]);
        }
      }
      return files;
    }

    #endregion // Implementation

    #region Helpers

    protected string FilePath (string key)
    {
      return Path.Combine (BaseDir, PrepareName (key));
    }

    protected string FilePathByID (string id)
    {
      string[] files = Directory.GetFiles (BaseDir, id+"*");
      if (null == files && files.Length > 0) {
        return Path.Combine (BaseDir, files [0]);
      }
      return null;
    }

    protected string PrepareName (string key)
    {
      string ext = Path.GetExtension (key);
      string name = StringToMD5 (key);
      return string.IsNullOrEmpty (ext) ? name : name + "." + ext;
    }

    #endregion
  }
}