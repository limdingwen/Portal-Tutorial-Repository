using System;
using System.Collections.Generic;
using UnityEngine;

public class RenderTexturePool : MonoBehaviour
{
    public static RenderTexturePool Instance;
    
    public int maxSize = 100;

    private List<PoolItem> pool = new List<PoolItem>();

    private void Awake()
    {
        Instance = this;
    }

    // Gets a new temporary texture from the pool.
    public PoolItem GetTexture()
    {
        // Check all pool items. Are any one of them unused?
        // If so, take the first unused one we come across, mark it as used, and return it.
        
        foreach (var poolItem in pool)
        {
            if (!poolItem.Used)
            {
                poolItem.Used = true;
                return poolItem;
            }
        }
        
        // Are none of them unused? Time to expand!

        if (pool.Count >= maxSize)
        {
            Debug.LogError("Pool is full!");
            throw new OverflowException();
        }
        
        var newPoolItem = CreateTexture();
        pool.Add(newPoolItem);
        Debug.Log($"New RenderTexture created, pool is now {pool.Count} items big.");
        newPoolItem.Used = true;
        return newPoolItem;
    }

    // Releases the temporary texture back into the pool.
    public void ReleaseTexture(PoolItem item)
    {
        // When releasing a texture, simply mark it as unused.
        // No need to overwrite it or anything!
        
        item.Used = false;
    }

    // Releases all temporary textures back to the pool.
    public void ReleaseAllTextures()
    {
        foreach (var poolItem in pool)
        {
            ReleaseTexture(poolItem);
        }
    }
    
    // Actually create a new texture, taking up memory and all!
    private PoolItem CreateTexture()
    {
        // As before, create a new RenderTexture with the full screen width and height.
        // Use .Create() to create it on the GPU as well.
        
        var newTexture = new RenderTexture(Screen.width, Screen.height, 24, RenderTextureFormat.DefaultHDR);
        newTexture.Create();
        
        return new PoolItem
        {
            Texture = newTexture,
            Used = false
        };
    }

    // Actually destroy the texture. It'll be gone for good!
    private void DestroyTexture(PoolItem item)
    {
        // First, release on the GPU...
        
        item.Texture.Release();
        
        // Then Destroy() to remove it from Unity completely.
        
        Destroy(item.Texture);
    }

    private void OnDestroy()
    {
        // Do cleanup!

        foreach (var poolItem in pool)
        {
            DestroyTexture(poolItem);
        }
    }

    public class PoolItem
    {
        public RenderTexture Texture;
        public bool Used;
    }
}
