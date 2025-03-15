namespace Captcha.Core.Models;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;

public class CaptchaImage(CaptchaConfigurationData config)
{
    public Bitmap Create()
    {
        // Setup all the drawing stuff needed
        var bitmap = new Bitmap(config.Width, config.Height, PixelFormat.Format16bppRgb555);
        using var graphics = Graphics.FromImage(bitmap);
        graphics.SmoothingMode = SmoothingMode.AntiAlias;
        var rectangle = new Rectangle(0, 0, config.Width, config.Height);

        // Do the drawing
        using var font = GetFontThatFitsRectangle(rectangle, graphics);
        FillInTheBackground(rectangle, graphics);
        DrawWarpedText(font, rectangle, graphics);
        AddRandomNoise(rectangle, graphics);

        return bitmap;
    }

    private void AddRandomNoise(Rectangle rectangle, Graphics graphics)
    {
        using var hatchBrush = new HatchBrush(HatchStyle.LargeConfetti, config.SecondaryColor, config.PrimaryColor);
        var max = Math.Max(rectangle.Width, rectangle.Height);
        for (var i = 0; i < (int)(rectangle.Width * rectangle.Height / config.Frequency); i++)
        {
            var x = Random.Shared.Next(rectangle.Width);
            var y = Random.Shared.Next(rectangle.Height);
            var width = Random.Shared.Next(max / 50);
            var height = Random.Shared.Next(max / 50);
            graphics.FillEllipse(hatchBrush, x, y, width, height);
        }
    }

    private void DrawWarpedText(Font font, Rectangle rectangle, Graphics graphics)
    {
        // Set up the text to be in the middle
        var format = new StringFormat
        {
            Alignment = StringAlignment.Center,
            LineAlignment = StringAlignment.Center
        };

        // Create a path using the text and warp it randomly.
        using var path = new GraphicsPath();
        path.AddString(config.Text, font.FontFamily, (int)font.Style, font.Size, rectangle, format);
        var divisor = 4F;   // TODO: We could use this one day as a parameter = how much to warp the text by
        PointF[] points =
        [
            new(Random.Shared.Next(rectangle.Width) / divisor, Random.Shared.Next(rectangle.Height) / divisor),
            new(rectangle.Width - (Random.Shared.Next(rectangle.Width) / divisor), Random.Shared.Next(rectangle.Height) / divisor),
            new(Random.Shared.Next(rectangle.Width) / divisor, rectangle.Height - (Random.Shared.Next(rectangle.Height) / divisor)),
            new(rectangle.Width - (Random.Shared.Next(rectangle.Width) / divisor), rectangle.Height - (Random.Shared.Next(rectangle.Height) / divisor))
        ];
        using var matrix = new Matrix();
        matrix.Translate(0F, 0F);
        path.Warp(points, rectangle, matrix, WarpMode.Perspective, 0F);

        // Draw the text.
        using var hatchBrush = new HatchBrush(HatchStyle.LargeConfetti, config.SecondaryColor, config.PrimaryColor);
        graphics.FillPath(hatchBrush, path);
    }

    private Font GetFontThatFitsRectangle(Rectangle rectangle, Graphics graphics)
    {
        SizeF size;
        float fontSize = rectangle.Height;

        // Adjust the font size until the text fits within the image.
        do
        {
            fontSize--;
            var font = new Font(config.Font, fontSize, FontStyle.Bold);
            size = graphics.MeasureString(config.Text, font);
        } while (size.Width > rectangle.Width);

        return new Font(config.Font, fontSize, FontStyle.Bold);
    }

    private void FillInTheBackground(Rectangle rectangle, Graphics graphics)
    {
        using var hatchBrush = new HatchBrush(HatchStyle.SmallConfetti, config.SecondaryColor, config.TertiaryColor);
        graphics.FillRegion(hatchBrush, new Region(rectangle));
    }
}
