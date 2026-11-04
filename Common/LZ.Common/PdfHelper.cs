using Microsoft.AspNetCore.Http;
using PDFtoImage;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.IO;

public static class PdfHelper
{
    /// <summary>
    /// PDF 页面尺寸信息
    /// </summary>
    public class PageSizeInfo
    {
        public int PageNumber { get; set; }

        /// <summary>页面宽度（Point）</summary>
        public float Width { get; set; }

        /// <summary>页面高度（Point）</summary>
        public float Height { get; set; }

        /// <summary>页面宽度（毫米）</summary>
        public double WidthMm => Width * 25.4 / 72;

        /// <summary>页面高度（毫米）</summary>
        public double HeightMm => Height * 25.4 / 72;

        /// <summary>渲染后的图片宽度（像素）</summary>
        public int ImageWidth { get; set; }

        /// <summary>渲染后的图片高度（像素）</summary>
        public int ImageHeight { get; set; }
    }

    /// <summary>
    /// 获取 PDF 每一页的尺寸
    /// </summary>
    public static List<PageSizeInfo> GetPageSizes(
        IFormFile file,
        int dpi = 150)
    {
        if (file == null || file.Length == 0)
            throw new ArgumentException("上传文件不能为空");

        if (dpi <= 0)
            throw new ArgumentOutOfRangeException(nameof(dpi));

        using var stream = file.OpenReadStream();

        var sizes = Conversion.GetPageSizes(stream);

        var result = new List<PageSizeInfo>();

        for (int i = 0; i < sizes.Count; i++)
        {
            var size = sizes[i];

            result.Add(new PageSizeInfo
            {
                PageNumber = i + 1,
                Width = size.Width,
                Height = size.Height,
                ImageWidth = (int)Math.Ceiling(size.Width * dpi / 72.0),
                ImageHeight = (int)Math.Ceiling(size.Height * dpi / 72.0)
            });
        }

        return result;
    }

    /// <summary>
    /// PDF 转图片
    /// </summary>
    public static List<byte[]> ConvertToImages(
        IFormFile file,
        int dpi = 150,
        string format = "png",
        int quality = 90)
    {
        if (file == null || file.Length == 0)
            throw new ArgumentException("上传文件不能为空");

        if (dpi <= 0)
            throw new ArgumentOutOfRangeException(nameof(dpi));

        format = format.ToLowerInvariant();

        if (format != "png" && format != "jpg")
            throw new ArgumentException("仅支持 PNG 和 JPG");

        if (quality < 1 || quality > 100)
            throw new ArgumentOutOfRangeException(nameof(quality));

        var result = new List<byte[]>();

        using var stream = file.OpenReadStream();

        var images = Conversion.ToImages(
            stream,
            options: new RenderOptions(Dpi: dpi));

        foreach (var bitmap in images)
        {
            using (bitmap)
            using (var memoryStream = new MemoryStream())
            {
                var imageFormat = format == "png"
                    ? SKEncodedImageFormat.Png
                    : SKEncodedImageFormat.Jpeg;

                bitmap.Encode(memoryStream, imageFormat, quality);

                result.Add(memoryStream.ToArray());
            }
        }

        return result;
    }
}