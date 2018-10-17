using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using DummyImage.Models;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;

namespace DummyImage.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [Route("/{size}")]
        public IActionResult RenderImage([FromRoute]string size, [FromQuery]string text = "")
        {
            var sizeData = GetSize(size);
            var bytes = GenerateImage(sizeData.Width, sizeData.Height, text);
            return File(bytes, "image/png");
        }

        [Route("/{size}/{bgcolor:alpha::minlength(3):maxlength(6)}")]
        public IActionResult RenderImageWithBgColor([FromRoute]string size, [FromRoute]string bgcolor, [FromQuery]string text = "")
        {
            var sizeData = GetSize(size);
            var bytes = GenerateImage(sizeData.Width, sizeData.Height, bgcolor, text);
            return File(bytes, "image/png");
        }

        [Route("/{size}/{bgcolor:alpha::minlength(3):maxlength(6)}/{forecolor:alpha:minlength(3):maxlength(6)}")]
        public IActionResult RenderImageWithForeColorNoExtension([FromRoute]string size, [FromRoute]string bgcolor, [FromRoute]string fgcolor, [FromQuery]string text = "")
        {
            var sizeData = GetSize(size);
            var bytes = GenerateImage(sizeData.Width, sizeData.Height, bgcolor, fgcolor, text);
            return File(bytes, "image/png");
        }

        [Route("/{size}/{bgcolor:alpha::minlength(3):maxlength(6)}/{fgcolor:alpha:minlength(3):maxlength(6)}.{format:alpha:minlength(3):maxlength(4)}")]
        public IActionResult RenderImageWithForeColor([FromRoute]string size, [FromRoute]string bgcolor, [FromRoute]string fgcolor, [FromRoute]string format, [FromQuery]string text = "")
        {
            var sizeData = GetSize(size);
            var bytes = GenerateImage(sizeData.Width, sizeData.Height, bgcolor, fgcolor, format, text);
            return File(bytes, GetImageFormat(format));
        }

        private string GetImageFormat(string format)
        {
            if (format.Equals("png", StringComparison.OrdinalIgnoreCase))
            {
                return "image/png";
            }
            if (format.Equals("jpg", StringComparison.OrdinalIgnoreCase) | format.Equals("jpeg", StringComparison.OrdinalIgnoreCase))
            {
                return "image/jpeg";
            }

            if (format.Equals("gif", StringComparison.OrdinalIgnoreCase))
            {
                return "image/gif";
            }

            return "image/png";
        }

        private Size GetSize(string size)
        {
            var sizeData = size.Split(new[] { 'x', 'X' }, StringSplitOptions.RemoveEmptyEntries);
            if (sizeData.Length == 2)
            {
                int.TryParse(sizeData[0], out int width);
                int.TryParse(sizeData[1], out int height);
                return new Size(width, height);
            }

            if (sizeData.Length == 1)
            {
                int.TryParse(sizeData[0], out int side);
                return new Size(side, side);
            }

            return new Size(800, 600);
        }

        private byte[] GenerateImage(int width, int height,
            string bgColor = "black", string fgColor = "white",
            string format = "png", string text = "")
        {
            var inputText = text?.Length == 0 ? $"{width} X {height}" : text;
            using (var bitmap = new Bitmap(width, height))
            {
                using (var graphics = Graphics.FromImage(bitmap))
                {
                    graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.SingleBitPerPixelGridFit;
                    graphics.CompositingQuality = CompositingQuality.HighQuality;
                    graphics.SmoothingMode = SmoothingMode.HighQuality;

                    var selectedBgColor = Color.FromName(bgColor);
                    var selectedFgColor = Color.FromName(fgColor);
                    var brush = new SolidBrush(selectedFgColor);
                    var selectedFont = new Font("Segoe UI", 30, FontStyle.Bold, GraphicsUnit.Pixel);
                    var font = GetAdjustedFont(graphics, inputText, selectedFont, width, 100, 30, true);

                    var textSize = graphics.MeasureString(inputText, font);
                    var position = new Point((bitmap.Width / 2 - ((int)textSize.Width / 2)),
                        (bitmap.Height / 2 - ((int)textSize.Height / 2)));

                    graphics.FillRectangle(new SolidBrush(selectedBgColor), 0, 0, width, height);

                    graphics.DrawString(inputText, font, brush, position);

                    using (var memoryStream = new MemoryStream())
                    {
                        bitmap.Save(memoryStream, GetImageFormatFromString(format));
                        return memoryStream.ToArray();
                    }
                }
            }
        }

        private ImageFormat GetImageFormatFromString(string format)
        {
            if (format.Equals("png", StringComparison.OrdinalIgnoreCase))
            {
                return ImageFormat.Png;
            }
            if (format.Equals("jpg", StringComparison.OrdinalIgnoreCase) | format.Equals("jpeg", StringComparison.OrdinalIgnoreCase))
            {
                return ImageFormat.Jpeg;
            }

            if (format.Equals("gif", StringComparison.OrdinalIgnoreCase))
            {
                return ImageFormat.Gif;
            }

            return ImageFormat.Png;
        }

        //Source : https://msdn.microsoft.com/en-us/library/bb986765.aspx
        public Font GetAdjustedFont(Graphics graphicRef, string graphicString,
            Font originalFont, int containerWidth, int maxFontSize, int minFontSize, bool smallestOnFail)
        {
            var font = default(Font);
            for (int adjustedSize = maxFontSize; adjustedSize >= minFontSize; adjustedSize--)
            {
                font = new Font(originalFont.Name, adjustedSize, originalFont.Style);
                var adjustedSizeNew = graphicRef.MeasureString(graphicString, font);
                if (containerWidth > Convert.ToInt32(adjustedSizeNew.Width))
                {
                    return font;
                }
            }

            if (smallestOnFail)
            {
                return font;
            }
            else
            {
                return originalFont;
            }
        }
    }
}
