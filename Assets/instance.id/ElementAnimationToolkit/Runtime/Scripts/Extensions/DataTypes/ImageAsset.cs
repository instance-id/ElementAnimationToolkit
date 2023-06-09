using System.Collections.Generic;
using UnityEngine;

namespace instance.id.EATK
{
#if IS_DEV
    [CreateAssetMenu(fileName = "ImageAsset.asset", menuName = "instance.id/ProStream/Collections/ImageAsset", order = 0)]
#endif
    public class ImageAsset : AssetInstance<ImageAsset>
    {
        public Texture2D[] imageFrames;
        public readonly Dictionary<string, Texture2D> imageLookup = new Dictionary<string, Texture2D>();

        private void OnEnable()
        {
            imageLookup.Clear();
            for (int i = 0; i < imageFrames.Length; i++)
            {
                imageLookup.TryAdd(imageFrames[i].name, imageFrames[i]);
            }
        }
    }
}
