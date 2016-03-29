using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XMLManipulation
{
    public static class GLTextureFormats
    {
        public const string GL_OES_compressed_ETC1_RGB8_texture = "GL_OES_compressed_ETC1_RGB8_texture";
        public const string GL_OES_compressed_paletted_texture = "GL_OES_compressed_paletted_texture";
        public const string GL_AMD_compressed_3DC_texture = "GL_AMD_compressed_3DC_texture";
        public const string GL_AMD_compressed_ATC_texture = "GL_AMD_compressed_ATC_texture";
        public const string GL_EXT_texture_compression_latc = "GL_EXT_texture_compression_latc";
        public const string GL_EXT_texture_compression_dxt1 = "GL_EXT_texture_compression_dxt1";
        public const string GL_EXT_texture_compression_s3tc = "GL_EXT_texture_compression_s3tc";
        public const string GL_IMG_texture_compression_pvrtc = "GL_IMG_texture_compression_pvrtc";

        public static readonly Dictionary<string, string> glTextureFormats = new Dictionary<string, string> ()
        {
            { "GL_OES_compressed_ETC1_RGB8_texture", GL_OES_compressed_ETC1_RGB8_texture },
            { "GL_OES_compressed_paletted_texture", GL_OES_compressed_paletted_texture },
            { "GL_AMD_compressed_3DC_texture", GL_AMD_compressed_3DC_texture },
            { "GL_AMD_compressed_ATC_texture", GL_AMD_compressed_ATC_texture },
            { "GL_EXT_texture_compression_latc", GL_EXT_texture_compression_latc },
            { "GL_EXT_texture_compression_dxt1", GL_EXT_texture_compression_dxt1 },
            { "GL_EXT_texture_compression_s3tc", GL_EXT_texture_compression_s3tc },
            { "GL_IMG_texture_compression_pvrtc", GL_IMG_texture_compression_pvrtc }
		};
    }
}
