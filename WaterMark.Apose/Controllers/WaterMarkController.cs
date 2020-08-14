﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Linq;
using System.Threading.Tasks;
using Aspose.Words;
using Aspose.Words.Drawing;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using SkiaSharp;

namespace WaterMark.Apose.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WaterMarkController : ControllerBase
    {
        [HttpPost("watermark")]
        public IActionResult InsertWaterMark()
        {
            try
            {
                string sourceFileName = @"C:\Users\Mohd\Source\Repos\WaterMark.Apose\WaterMark.Apose\download\demo.docx";
                string destFileName = @"C:\Users\Mohd\Source\Repos\WaterMark.Apose\WaterMark.Apose\download\demo222.docx";
                string bookMarkImage = @"C:\Users\Mohd\Source\Repos\WaterMark.Apose\WaterMark.Apose\download\watmark.jpg";
                bool greyScale = false;
                double zoom = 100;
                double imageAngle = 0;



                var doc = new Document(sourceFileName);
                var watermark = new Shape(doc, ShapeType.Rectangle)
                {
                    Rotation = imageAngle,
                    RelativeHorizontalPosition = RelativeHorizontalPosition.Page,
                    RelativeVerticalPosition = RelativeVerticalPosition.Page,
                    WrapType = WrapType.None,
                    VerticalAlignment = VerticalAlignment.Center,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    ZOrder = -10000, // Appear behind other images
                    BehindText = true,
                    Name = "WaterMark"
                };

                //var imagefilename = Config.Configuration.WorkingDirectory + imageFolderName + "/" + imageFileName;
                //if (!System.IO.File.Exists(imagefilename))
                //    imagefilename = Config.Configuration.OutputDirectory + imageFolderName + "/" + imageFileName;

                var image = Image.FromFile(bookMarkImage);
                watermark.ImageData.SetImage(SKBitmap.Decode(bookMarkImage));
                // watermark.ImageData.SetImage(ResizeImage(image, zoom / 100));
                watermark.ImageData.GrayScale = greyScale;

                AddWatermark(doc, watermark);

                doc.Save(destFileName);
                return Ok(null);
            }
            catch (Exception ex)
            {
                return Ok(null);
            }
        }

        [HttpPost("remove")]
        public IActionResult RemoveWaterMark()
        {
            try
            {

                string sourceFileName = @"C:\Users\Mohd\Source\Repos\WaterMark.Apose\WaterMark.Apose\download\demo222.docx";

                string destFileName = @"C:\Users\Mohd\Source\Repos\WaterMark.Apose\WaterMark.Apose\download\demo_removed.docx";

                var doc = new Document(sourceFileName);
                foreach (HeaderFooter hf in doc.GetChildNodes(NodeType.HeaderFooter, true))
                    foreach (Shape shape in hf.GetChildNodes(NodeType.Shape, true))
                        // if (shape.Name.Contains("WaterMark") || shape.TextPath.Text.Contains("WaterMark")) // WORDSNET-15559
                        shape.Remove();



                doc.Save(destFileName);
                return Ok(null);
            }
            catch (Exception ex)
            {
                return Ok(null);
            }
        }
        private static void InsertWatermarkIntoHeader(Paragraph watermarkPara, Section sect, HeaderFooterType headerType)
        {
            var header = sect.HeadersFooters[headerType];
            if (header == null)
            {
                // There is no header of the specified type in the current section, create it.
                header = new HeaderFooter(sect.Document, headerType);
                sect.HeadersFooters.Add(header);
            }
            header.AppendChild(watermarkPara.Clone(true));
        }
        private static void AddWatermark(Document doc, Shape watermark)
        {
            var watermarkPara = new Paragraph(doc);
            watermarkPara.AppendChild(watermark);
            foreach (Section sect in doc.Sections)
            {
                // There could be up to three different headers in each section, since we want
                // The watermark to appear on all pages, insert into all headers.
                InsertWatermarkIntoHeader(watermarkPara, sect, HeaderFooterType.HeaderPrimary);
                InsertWatermarkIntoHeader(watermarkPara, sect, HeaderFooterType.HeaderFirst);
                InsertWatermarkIntoHeader(watermarkPara, sect, HeaderFooterType.HeaderEven);
            }
        }

        public static Bitmap ResizeImage(Image image, double zoom)
        {
            var width = (int)(image.Width * zoom);
            var height = (int)(image.Height * zoom);

            var destRect = new Rectangle(0, 0, width, height);
            var destImage = new Bitmap(width, height);

            // destImage.SetResolution(image.HorizontalResolution, image.VerticalResolution);

            using (var graphics = Graphics.FromImage(destImage))
            {
                graphics.CompositingMode = CompositingMode.SourceCopy;
                graphics.CompositingQuality = CompositingQuality.HighQuality;
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.SmoothingMode = SmoothingMode.HighQuality;
                graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

                using (var wrapMode = new ImageAttributes())
                {
                    wrapMode.SetWrapMode(WrapMode.TileFlipXY);
                    graphics.DrawImage(image, destRect, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, wrapMode);
                }
            }

            return destImage;

        }


    }
}
