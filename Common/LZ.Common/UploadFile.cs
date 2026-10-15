using System.Drawing;
using Microsoft.AspNetCore.Http;

namespace LZ.Common
{
    public class UploadFile
    {
        #region 生成缩略图
        /// <summary>
        /// 生成缩略图
        /// </summary>
        /// <param name="originalImagePath">原始图片路径</param>
        /// <param name="thumbnailPath">缩略图路径</param>
        /// <param name="width">生成的缩略图宽度</param>
        /// <param name="height">生成的缩略图高度</param>
        /// <param name="mode">模式：“H”表示以高度为基准缩放；“W”表示以宽度为基准缩放</param>
        public static bool MakeThumbnail(string originalImagePath, string thumbnailPath, int width, int height, string mode, bool path = false)
        {
            if (path)
            {
                var f = Directory.GetCurrentDirectory();
                originalImagePath = f + originalImagePath;
                thumbnailPath = f + thumbnailPath;
            }
            System.Drawing.Image originalImage = System.Drawing.Image.FromFile(originalImagePath);

            int towidth = width;
            int toheight = height;

            int x = 0;
            int y = 0;
            int ow = originalImage.Width;
            int oh = originalImage.Height;
            if (ow < towidth && oh < toheight)
            {
                return false;
            }
            switch (mode)
            {
                case "HW"://指定高宽缩放（可能变形）                
                    break;
                case "W"://指定宽，高按比例                    
                    toheight = originalImage.Height * width / originalImage.Width;
                    break;
                case "H"://指定高，宽按比例
                    towidth = originalImage.Width * height / originalImage.Height;
                    break;
                case "Cut"://指定高宽裁减（不变形）                
                    if ((double)originalImage.Width / (double)originalImage.Height > (double)towidth / (double)toheight)
                    {
                        oh = originalImage.Height;
                        ow = originalImage.Height * towidth / toheight;
                        y = 0;
                        x = (originalImage.Width - ow) / 2;
                    }
                    else
                    {
                        ow = originalImage.Width;
                        oh = originalImage.Width * height / towidth;
                        x = 0;
                        y = (originalImage.Height - oh) / 2;
                    }
                    break;
                default:
                    break;
            }

            //新建一个bmp图片
            System.Drawing.Image bitmap = new System.Drawing.Bitmap(towidth, toheight);

            //新建一个画板
            Graphics g = System.Drawing.Graphics.FromImage(bitmap);

            //设置高质量插值法
            g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.High;

            //设置高质量,低速度呈现平滑程度
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;

            //清空画布并以透明背景色填充
            g.Clear(Color.Transparent);

            //在指定位置并且按指定大小绘制原图片的指定部分
            g.DrawImage(originalImage, new Rectangle(0, 0, towidth, toheight),
                new Rectangle(x, y, ow, oh),
                GraphicsUnit.Pixel);

            try
            {
                //以jpg格式保存缩略图
                bitmap.Save(thumbnailPath, System.Drawing.Imaging.ImageFormat.Jpeg);
                return true;
            }
            catch (System.Exception e)
            {
                throw e;
            }
            finally
            {
                originalImage.Dispose();
                bitmap.Dispose();
                GC.Collect();
                g.Dispose();
            }
        }//按比例生成高质量缩略图
        #endregion

        #region 获取图片宽、高
        /// <summary>
        /// 获取图片宽度
        /// </summary>
        /// <param name="PicPath">图片路径</param>
        /// <returns></returns>
        public static int GetImageWidth(string PicPath)
        {
            System.Drawing.Image image = System.Drawing.Image.FromFile(Directory.GetCurrentDirectory() + "/" + PicPath);
            int result = image.Width;
            image.Dispose();
            return result;
        }
        /// <summary>
        /// 获取图片高度
        /// </summary>
        /// <param name="PicPath">图片路径</param>
        /// <returns></returns>
        public static int GetImageHeight(string PicPath)
        {
            System.Drawing.Image image = System.Drawing.Image.FromFile(Directory.GetCurrentDirectory() + "/" + PicPath);
            int result = image.Height;
            image.Dispose();
            return result;
        }
        #endregion

        #region 上传文件
        /// <summary>
        /// 
        /// </summary>
        /// <param name="file"></param>
        /// <param name="path"></param>
        /// <param name="type"></param>
        /// <param name="fname"></param>
        /// <returns></returns>
        public static async Task<string> UpFile(IFormFile file, string path, bool isCreateTime = true, string type = "|.txt|.doc|.docx|.xls|.xlsx|.ppt|.pptx|.zip|.rar|.jpg|.gif|.png|.bmp|.pdf|.swf|.flv|.7z|.wps|.et|.tiff|.gd|.mp4|.wmv|.dwg|.cdr|.key|", string fname = "")
        {
            if (isCreateTime)
            {
                path += DateTime.Now.ToString("yyyyMMdd") + "/";
            }
            string FilePath = System.IO.Directory.GetCurrentDirectory() + path;
            string fName = "", fileFolder;
            string fileName, fileExtension, NewfileName;
            fileName = Path.GetFileName(file.FileName);
            if (!string.IsNullOrEmpty(fname))
            {
                NewfileName = fname + "_" + Guid.NewGuid().ToString().Replace("-", "");
            }
            else
            {
                NewfileName = Guid.NewGuid().ToString().Replace("-", "");
            }
            if (fileName != "")
            {
                fileExtension = Path.GetExtension(fileName).ToLower();
                string FileContentType = file.ContentType;
                if (type.IndexOf("|" + fileExtension + "|") != -1)
                {
                    if (!Directory.Exists(FilePath))
                    {
                        Directory.CreateDirectory(FilePath);
                    }
                    fileFolder = FilePath + NewfileName + fileExtension; // 服务器端文件路径
                    if (!File.Exists(fileFolder))
                    {
                        try
                        {
                            using (var stream = new FileStream(fileFolder, FileMode.Create))
                            {
                                await file.CopyToAsync(stream);
                            }
                            fName = path + NewfileName + fileExtension;
                        }
                        catch (Exception ex)
                        {
                            return "error--提示：文件上传失败，失败原因：" + ex.Message;
                        }
                    }
                    else
                    {
                        return "error--提示：文件已经存在，请重命名后上传";
                    }
                }
                else
                {
                    return "error--上传文件类型不正确";
                }
            }
            return fName;
        }

        #endregion

        #region 上传图片
        /// <summary>
        /// 自动生成当日时间的文件夹，并上传到该文件夹中
        /// </summary>
        /// <param name="file"></param>
        /// <param name="fPath"></param>
        /// <param name="tPath"></param>
        /// <param name="picWidth"></param>
        /// <param name="picHeight"></param>
        /// <param name="fname"></param>
        /// <param name="tname"></param>
        /// <returns></returns>
        public static async Task<string> UpImage(IFormFile file, string fPath, string tPath = "", int picWidth = 0, int picHeight = 0, bool isCreateTime = true, string fname = "", string tname = "")
        {
            var f = System.IO.Directory.GetCurrentDirectory();
            if (string.IsNullOrEmpty(tPath)) tPath = fPath;
            if (isCreateTime)
            {
                fPath += DateTime.Now.ToString("yyyyMMdd") + "/";
                tPath += DateTime.Now.ToString("yyyyMMdd") + "/";
            }
            string FilePath = f + fPath;
            string Thumb = f + tPath;
            string fName = "", fileFolder, T_fileFolder;
            string fileName, fileExtension, NewfileName, T_NewfileName;
            fileName = Path.GetFileName(file.FileName);
            var gid = Guid.NewGuid().ToString("n");
            if (!string.IsNullOrEmpty(fname))
            {
                NewfileName = fname + "_" + gid;
            }
            else
            {
                NewfileName = gid;
            }
            if (fileName != "")
            {
                fileExtension = Path.GetExtension(fileName).ToLower();//上传的文件格式
                string FileContentType = file.ContentType;
                if (FileContentType == "image/bmp" || FileContentType == "image/gif" || FileContentType == "image/jpeg" || FileContentType == "image/png")
                {
                    if (!Directory.Exists(FilePath))
                    {
                        Directory.CreateDirectory(FilePath);
                    }
                    fileFolder = FilePath + NewfileName + fileExtension; // 服务器端文件路径
                    if (!String.IsNullOrEmpty(tname))
                    {
                        T_NewfileName = tname + "_" + gid;
                    }
                    else
                    {
                        T_NewfileName = Guid.NewGuid().ToString("n");
                    }
                    if (!String.IsNullOrEmpty(tPath)) // 服务器端缩略图路径
                    {
                        if (!Directory.Exists(Thumb))
                        {
                            Directory.CreateDirectory(Thumb);
                        }
                        T_fileFolder = Thumb + T_NewfileName + fileExtension;
                    }
                    else
                    {
                        T_fileFolder = FilePath + T_NewfileName + fileExtension;
                    }
                    if (!File.Exists(fileFolder))
                    {
                        try
                        {
                            using (var stream = new FileStream(fileFolder, FileMode.Create))
                            {
                                await file.CopyToAsync(stream);
                            }
                            fName = fPath + NewfileName + fileExtension;
                            MakeThumbnail(fileFolder, T_fileFolder, picWidth, picHeight, "H");    // 生成缩略图方法
                        }
                        catch (Exception ex)
                        {
                            return "error--提示：文件上传失败，失败原因：" + ex.Message;
                        }
                    }
                    else
                    {
                        return "error--提示：文件已经存在，请重命名后上传";
                    }
                }
                else
                {
                    return "error--上传文件类型不正确";
                }
            }
            return fName;
        }

        public static async Task<string> OnlyUpImage(IFormFile file, string fPath, string tPath = "", bool isCreateTime = true, string fname = "")
        {
            var f = System.IO.Directory.GetCurrentDirectory();
            if (string.IsNullOrEmpty(tPath)) tPath = fPath;
            if (isCreateTime)
            {
                fPath += DateTime.Now.ToString("yyyyMMdd") + "/";
                tPath += DateTime.Now.ToString("yyyyMMdd") + "/";
            }
            string FilePath = f + fPath;
            string Thumb = f + tPath;
            string fName = "", fileFolder;
            string fileName, fileExtension, NewfileName;
            fileName = Path.GetFileName(file.FileName);
            var gid = Guid.NewGuid().ToString("n");
            if (!string.IsNullOrEmpty(fname))
            {
                NewfileName = fname + "_" + gid;
            }
            else
            {
                NewfileName = gid;
            }
            if (fileName != "")
            {
                fileExtension = Path.GetExtension(fileName).ToLower();//上传的文件格式
                string FileContentType = file.ContentType;
                if (FileContentType == "image/bmp" || FileContentType == "image/gif" || FileContentType == "image/jpeg" || FileContentType == "image/png")
                {
                    if (!Directory.Exists(FilePath))
                    {
                        Directory.CreateDirectory(FilePath);
                    }
                    fileFolder = FilePath + NewfileName + fileExtension; // 服务器端文件路径
                    if (!File.Exists(fileFolder))
                    {
                        try
                        {
                            using (var stream = new FileStream(fileFolder, FileMode.Create))
                            {
                                await file.CopyToAsync(stream);
                            }
                            fName = fPath + NewfileName + fileExtension;
                        }
                        catch (Exception ex)
                        {
                            return "error--提示：文件上传失败，失败原因：" + ex.Message;
                        }
                    }
                    else
                    {
                        return "error--提示：文件已经存在，请重命名后上传";
                    }
                }
                else
                {
                    return "error--上传文件类型不正确";
                }
            }
            return fName;
        }
        #endregion
    }
}
