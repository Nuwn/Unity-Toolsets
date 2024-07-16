using System;
using UnityEngine;

namespace LazySaveSystem
{
    public class Texture2DConverter : Converter<Texture2D>
    {
        public override object Serialize(Texture2D data)
        {
            return SerializeTexture(data);
        }

        public override Texture2D Deserialize(object data)
        {
            return data is TextureData textureData
                ? DeserializeTexture(textureData)
                : throw new ArgumentException("Invalid data type for deserialization.", nameof(data));
        }

        internal static TextureData SerializeTexture(Texture2D texture)
        {
            byte[] bytes = texture.EncodeToPNG();
            string base64String = Convert.ToBase64String(bytes);

            return new TextureData
            {
                Width = texture.width,
                Height = texture.height,
                Format = texture.format.ToString(),
                Base64Image = base64String
            };
        }

        internal static Texture2D DeserializeTexture(TextureData data)
        {
            byte[] bytes = Convert.FromBase64String(data.Base64Image);

            Texture2D texture = new Texture2D(data.Width, data.Height);
            texture.LoadImage(bytes); // Load texture from byte array
            texture.Apply();

            return texture;
        }

        [Serializable]
        internal class TextureData
        {
            public int Width;
            public int Height;
            public string Format;
            public string Base64Image;
        }
    }
}
