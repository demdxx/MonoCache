/**
 * IOHelper.cs
 * 
 * @author Dmitry Ponomarev <demdxx@gmail.com>
 * @license MIT Copyright (c) 2013 demdxx. All rights reserved.
 * @project MonoCache.Android
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
using System.IO.IsolatedStorage;
using System.Runtime.Serialization;

namespace MonoCache.WindowsPhone
{
#if IO_INTERFACE
  public class IOHelper : IOInterface
  {
    [DataContract]
    public class SerealizedItem
    {
      [DataMember]
      public object value;

      public SerealizedItem(object v)
      {
        value = v;
      }
    }

    private IsolatedStorageFile Storage
    {
      get
      {
        return IsolatedStorageFile.GetUserStoreForApplication();
      }
    }

    /* File path */
    public virtual string PathCombine(string path1, string path2)
    {
      return Path.Combine(path1, path2);
    }
    public virtual string PathCombineLocalApplicationData(string path)
    {
      return ""; // Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), path);
    }
    public virtual string PathGetExtension(string path)
    {
      return Path.GetExtension(path);
    }
    public virtual string PathGetFileNameWithoutExtension(string path)
    {
      return Path.GetFileNameWithoutExtension(path);
    }

    /* Directory */
    public virtual bool DirectoryExists(string path)
    {
      return Storage.DirectoryExists(path); // Directory.Exists(path);
    }
    public virtual bool DirectoryCreate(string path)
    {
      try
      {
        Storage.CreateDirectory(path);
      }
      catch (Exception e)
      {
        return false;
      }
      return true; // null != Directory.CreateDirectory(path);
    }
    public virtual string[] DirectoryGetFiles(string path, string pattern)
    {
      return Storage.GetFileNames(path+"/"+pattern); // Directory.GetFiles(path, pattern);
    }

    /* File */
    public virtual bool FileExists(string path)
    {
      return Storage.FileExists(path); // File.Exists(path);
    }
    public virtual void FileDelete(string path)
    {
      // File.Delete(path);
      Storage.DeleteFile(path);
    }
    public virtual DateTime FileGetLastWriteTimeUtc(string path)
    {
      return Storage.GetLastWriteTime(path).DateTime; // File.GetLastWriteTime(path);
    }
    public virtual bool FileSerialize(string path, object value)
    {
      IsolatedStorageFile storage = Storage;
      string pathDir = Path.GetDirectoryName(path);
      if (!storage.DirectoryExists(pathDir))
      {
        storage.CreateDirectory(pathDir);
      }
      using (FileStream fs = storage.OpenFile(path, FileMode.Create))
      {
        var dataContractSerializer = new DataContractSerializer(typeof(SerealizedItem));
        dataContractSerializer.WriteObject(fs, value);
      }
      return true;
    }
    public virtual object FileDeserialize(string path)
    {
      object result = null;
      IsolatedStorageFile storage = Storage;
      if (storage.FileExists(path))
      {
        using (FileStream fs = storage.OpenFile(path, FileMode.Open))
        {
          try
          {
            var dataContractSerializer = new DataContractSerializer(typeof(SerealizedItem));
            result = dataContractSerializer.ReadObject(fs);
          }
          catch (System.Runtime.Serialization.SerializationException e)
          {
            System.Diagnostics.Debug.WriteLine("Serialization exception: {0}", e);
          }
        }
      }
      return null != result ? ((SerealizedItem) result).value : null;
    }
  }
#endif // END IO_INTERFACE
}
