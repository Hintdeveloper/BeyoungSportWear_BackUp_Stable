using BusinessLogicLayer.Services.Interface;
using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZXing.Common;
using ZXing.CoreCompat.Rendering;
using ZXing;
using CloudinaryDotNet.Actions;
using CloudinaryDotNet;
using System.IO;

namespace BusinessLogicLayer.Services.Implements
{
    public class BarcodeGeneratorService : IBarcodeGeneratorService
    {
        private readonly Cloudinary _cloudinary;

        public BarcodeGeneratorService(Cloudinary cloudinary)
        {
            _cloudinary = cloudinary;
        }

        public async Task<string> GenerateBarcode(string KeyCode)
        {
            string barcodeText = KeyCode.ToString();

            var barcodeWriter = new BarcodeWriter<Bitmap>
            {
                Format = BarcodeFormat.CODE_128,
                Options = new EncodingOptions
                {
                    Width = 600,
                    Height = 300,
                    Margin = 10,
                    PureBarcode = true
                },
                Renderer = new BitmapRenderer()
            };

            using (Bitmap bitmap = barcodeWriter.Write(barcodeText))
            {
                bitmap.SetResolution(300, 300);

                using (var stream = new MemoryStream())
                {
                    ImageCodecInfo pngEncoder = GetEncoder(ImageFormat.Png);
                    EncoderParameters encoderParams = new EncoderParameters(1);
                    encoderParams.Param[0] = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, 100L);

                    bitmap.Save(stream, pngEncoder, encoderParams);

                    stream.Position = 0;

                    var uploadParams = new ImageUploadParams
                    {
                        File = new FileDescription("barcode.png", stream),
                        Folder = "BeyoungSportWear/ImageProduct/Options/Barcode"
                    };

                    var uploadResult = await _cloudinary.UploadAsync(uploadParams);

                    if (uploadResult.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        return uploadResult.SecureUrl.ToString();
                    }
                    else
                    {
                        throw new Exception("Failed to upload image to Cloudinary");
                    }
                }
            }
        }

        private ImageCodecInfo GetEncoder(ImageFormat format)
        {
            ImageCodecInfo[] codecs = ImageCodecInfo.GetImageDecoders();
            foreach (ImageCodecInfo codec in codecs)
            {
                if (codec.FormatID == format.Guid)
                {
                    return codec;
                }
            }
            return null;
        }

    }
}
