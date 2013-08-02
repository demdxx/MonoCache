/**
 * CacheBase.cs
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
using System.Text;
using System.Security.Cryptography;

namespace MonoCache
{
  public abstract class CacheBase : ICache
  {
    /**
     * Cache lifetime
     */
    public long LifeTime { get; set; }

    /**
     * Constructor
     * @param lifeTime of cache
     */
    public CacheBase (long lifeTime)
    {
      LifeTime = lifeTime;
    }

    /**
     * Set value to cache
     * @param key
     * @param value
     * @param rewrite
     * @param checkExpired Check the end of the lifetime of
     * @return success state
     */
    public abstract bool Set(string key, object value, bool rewrite = false, bool checkExpired = true);

    /**
     * If object cached
     * @param key
     * @param checkExpired Check the end of the lifetime of
     * @return success status
     */
    public abstract bool Check(string key, bool checkExpired = true);

    /**
     * Get object by key
     * @param key
     * @param checkExpired Check the end of the lifetime of
     * @return object
     */
    public abstract object Get(string key, bool checkExpired = true);

    /**
     * Remove value by key
     * @param key
     */
    public abstract void Remove(string key);

    /**
     * Remove value by id
     * @param id
     */
    protected abstract void RemoveFromId(string id);

    /**
     * Get object by string key
     * @param key
     * @return object
     */
    public object this [string key]
    {
      get { return Get (key); }
    }

    /**
     * List of expired keys
     * @return string []
     */
    protected abstract string [] ExpiredListOfKeys ();

    /**
     * Full list of keys
     * @return string []
     */
    protected abstract string [] ListOfKeys ();

    /**
     * Clear expired files
     */
    public virtual void ClearExpired()
    {
      string[] list = ExpiredListOfKeys ();
      if (null != list && list.Length > 0) {
        foreach (string id in list) {
          RemoveFromId (id);
        }
      }
    }

    /**
     * Clear cache
     */
    public virtual void Clear()
    {
      string[] list = ListOfKeys ();
      if (null != list && list.Length > 0) {
        foreach (string id in list) {
          RemoveFromId (id);
        }
      }
    }

    #region Helpers

    protected bool IsExpired(DateTime time, long lifeTime = 0)
    {
      if (lifeTime < 1) {
        lifeTime = LifeTime;
        if (lifeTime < 1) {
          return false;
        }
      }
      return lifeTime < (long)((DateTime.UtcNow - time).TotalMilliseconds * 1000);
    }

    protected string StringToMD5(string str)
    {
      byte[] input = ASCIIEncoding.ASCII.GetBytes (str);
      byte[] output = MD5.Create ().ComputeHash (input);
      StringBuilder sb = new StringBuilder();
      for(int i=0;i<output.Length;i++) {
        sb.Append(output[i].ToString("x2"));
      }
      return sb.ToString();
    }

    #endregion // Helpers
  }
}

