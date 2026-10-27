using System.IO;
using SixLabors.Fonts;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SixLabors.ImageSharp.Formats.Png;
using log4net;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.Formats.Jpeg;

namespace LZ.Common
{
    public class ImageSharp
    {
        public static async Task ImageResize(string file,int width,string toPath,int quality = 50)
        {
            var image =await Image.LoadAsync(file);
            var ow = image.Width*1.0M;
            var oh = image.Height * 1.0M;
            // 如何制定宽度大于原图， 使用原图宽度
            decimal scale = (ow <= width) ? 1 : (width / ow);
            // 新的宽度和高度
            int targetWidth = (int)(ow * scale);
            int targetHeight = (int)(oh * scale);
            // 对图像进行调整大小
            image.Mutate(x => x.Resize(
                new ResizeOptions
                {
                    Size = new Size(targetWidth, targetHeight),
                    Mode = ResizeMode.Max
                }
             ));
            // configure the encoder options 
            var encoder = new SixLabors.ImageSharp.Formats.Jpeg.JpegEncoder(){
                Quality = quality,
            };
            //var saveStream = new System.IO.MemoryStream();
            image.Save(toPath, encoder);
        }
        public static async Task<string> CombinText(string picture, string savePath, string txt)
        {
            if (string.IsNullOrEmpty(picture))
            {
                return "请传入模板大图片";
            }
            var f = Directory.GetCurrentDirectory();
            if (picture.ToLower().IndexOf(f.ToLower()) == -1)
            {
                picture = f + picture;
            }
            try
            {
                var imagesTemle =await Image.LoadAsync(picture);
                var install_Family = new FontCollection().Add(f + "/wwwroot/font/msyh.ttf");
                var font = new Font(install_Family, 28, FontStyle.Bold);  //字体
                var image = new Image<Rgba32>(imagesTemle.Width, imagesTemle.Height + 60, new Rgba32(255, 255, 255));
                image.Mutate(a =>
                {
                    a.DrawImage(imagesTemle, new Point(0, 0), 1);
                });
                var textOptions = new TextOptions(font)
                {
                    //TabWidth = 16,
                    //ApplyKerning = true,
                    VerticalAlignment = VerticalAlignment.Center,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    //WrapTextWidth = 430
                };

                var graphicsOptions = new GraphicsOptions()
                {
                    Antialias = true
                };

                var options = new DrawingOptions()
                {
                    GraphicsOptions = graphicsOptions,
                };
                var size = TextMeasurer.MeasureSize(txt, textOptions);
                image.Mutate(a =>
                {
                    PointF point = new PointF();
                    point.X = (imagesTemle.Width / 2) - (size.Width / 2);
                    point.Y = imagesTemle.Height + 10;
                    a.DrawText(options, txt, font, Color.Black, point);
                });
                if (!string.IsNullOrEmpty(savePath))
                {
                    var fsavePath = f + savePath + DateTime.Now.ToString("yyyyMMdd") + "/";
                    if (!Directory.Exists(fsavePath))
                    {
                        Directory.CreateDirectory(fsavePath);
                    }
                    savePath = fsavePath + "/" + Guid.NewGuid().ToString("n") + ".jpg";
                }
                else
                {
                    savePath = picture;
                }
                await image.SaveAsync(savePath);
                image.Dispose();
                imagesTemle.Dispose();
                return savePath;
            }
            catch (Exception ex)
            {
                StaticLog.Error("图片处理错误", ex);
                return "图片处理错误";
            }
        }

        /// <summary>
        /// 添加文字水印
        /// </summary>
        /// <param name="inputStream">原始图片流</param>
        /// <param name="outputStream">输出图片流</param>
        /// <param name="watermarkText">水印文字</param>
        /// <param name="fontPath">字体文件路径 (如 ./Fonts/SimHei.ttf)</param>
        /// <param name="fontSize">字体大小</param>
        /// <param name="colorHex">颜色十六进制字符串 (如 "#80FFFFFF" 表示半透明白色)</param>
        public static void AddTextWatermark(Stream inputStream, Stream outputStream, string watermarkText, string fontPath, float fontSize = 24, string colorHex = "#80FFFFFF")
        {
            // 1. 加载图片
            using var image = Image.Load(inputStream);

            // 2. 加载字体
            // 注意：Linux下必须指定具体字体文件路径，不能依赖系统字体名
            var fontCollection = new FontCollection();
            if (string.IsNullOrWhiteSpace(fontPath))
            {
                fontPath = Directory.GetCurrentDirectory() + "/wwwroot/Fonts/msyh.ttf";
            }
            var fontFamily = new FontCollection().Add(fontPath);
            var font = fontFamily.CreateFont(fontSize, FontStyle.BoldItalic);
            // 3. 解析颜色 (支持 Alpha 通道)
            var color = Color.ParseHex(colorHex);
            // 4. 配置文本选项
            var textOptions = new TextOptions(font)
            {
                HorizontalAlignment = HorizontalAlignment.Right, // 右对齐
                VerticalAlignment = VerticalAlignment.Bottom,    // 底部对齐
                Origin = new PointF(10, 10) // 距离右下角的边距 (Right=10px, Bottom=10px)
            };
            PointF pointF = new PointF(10, image.Height/2); // 右下角位置
            // 5. 绘制水印
            image.Mutate(x => x.DrawText(watermarkText, font,Brushes.Solid(color),pointF));
            // 6. 保存结果 (根据原图格式或指定格式保存)
            // 如果原图是 JPEG，保存为 JPEG 会丢失透明度，但文字已合成进去，所以没问题
            image.Save(outputStream, new JpegEncoder() { Quality = 90 });
        }
    }
}
