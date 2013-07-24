/**
 * Manager.cs
 * 
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

namespace MonoCache
{
  public class CacheManager
  {
    /**
     * Scope of caches
     */
    private Dictionary<string, ICache> _Caches = new Dictionary<string, ICache> ();

    /**
     * Link to global cache
     */
    protected ICache _GlobalCache = null;

    /**
     * Instnace of manager
     */
    public static CacheManager _Instance = null;

    /**
     * Instnace property of manager
     */
    public static CacheManager Instance {
      get {
        if (null != _Instance) {
          _Instance = new CacheManager ();
        }
        return _Instance;
      }
    }

    /**
     * Constructor
     */
    public CacheManager ()
    {
      // ...
    }

    /**
     * Init new cache store
     * @param key Simbolic name
     * @param sector Depending on the type of cache
     * @param lifeTime
     * @return success state
     */
    public bool InitCache (string key, string sector, long lifeTime)
    {
      if (_Caches.ContainsKey (key)) {
        return false;
      }
      _Caches [key] = new FileCache (sector, lifeTime);
      return true;
    }

    /**
     * Get cache interface if exists
     * @param key
     * @return Cache interface
     */
    public ICache CacheFor (string key)
    {
      ICache cache;
      if (!_Caches.TryGetValue (key, out cache)) {
        return null;
      }
      return cache;
    }

    /**
     * Operator [] for access to Cache interface by string key
     * @param key
     * @return Cache interface
     */
    public ICache this [string key]
    {
      get { return CacheFor (key); }
    }

    /**
     * Initialize global cache
     * @param basePath Global cache base path
     * @param lifeTime
     */
    public virtual void InitGlobalCache (string basePath = null, long lifeTime = 1000 * 60 * 60 * 24 * 3)
    {
      if (string.IsNullOrEmpty (basePath)) {
        basePath = Path.Combine (
          Environment.GetFolderPath (Environment.SpecialFolder.LocalApplicationData),
          "g.cache");
      }
      if (!InitCache ("global", basePath, lifeTime)) {
        throw new Exception ("Can`t init global cache!");
      }
      _GlobalCache = CacheFor ("global");
    }

    /**
     * Get global cache interface
     * @return ICache
     */
    public ICache GlobalCacheInterface
    {
      get {
        if (null != _GlobalCache) {
          return _GlobalCache;
        }
        InitGlobalCache ();
        return _GlobalCache;
      }
    }

    /**
     * Get global cache interface
     * @return ICache
     */
    public static ICache GlobalCache
    {
      get {
        return Instance.GlobalCacheInterface;
      }
    }
  }
}