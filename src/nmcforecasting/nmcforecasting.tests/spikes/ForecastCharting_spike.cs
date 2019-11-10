using System.IO;
using System.Linq;
using SkiaSharp;
using Xunit;

namespace nmcforecasting.tests
{
    // Drawing a distribution chart using SkiaSharp
    public class ForecastCharting_spike
    {
        [Fact]
        public void experiment()
        {
            var fc = new Forecast(new[] {
                (1, 0, 0.1, 10.0),
                (200, 0, 0.5, 60.0),
                (1000, 0, 0.25, 90.0),
                (5000, 0, 0.1, 100.0)
            });

            
            const int BAR_WIDTH = 20;
            const int BAR_SPACE_WIDTH = 2;
            const int MARGIN_LEFT_RIGHT = 10;
            const int MARGIN_BOTTOM_TOP = 10;
            
            var width = 2 * MARGIN_LEFT_RIGHT + (fc.Entries.Length - 1) * (BAR_WIDTH + BAR_SPACE_WIDTH) + BAR_WIDTH;

            var height = 100;
            
            var img = new SKImageInfo(width, height);
            
            
            using (var surface = SKSurface.Create(img))
            {
                var canvas = surface.Canvas;
                canvas.Clear(SKColors.Beige);

                using (var brush = new SKPaint())
                {
                    brush.Color = SKColors.Blue;
                    brush.StrokeWidth = 1;
                    brush.Style = SKPaintStyle.Fill;

                    var x = MARGIN_LEFT_RIGHT;
                    var maxBarHeight = height - 2 * MARGIN_BOTTOM_TOP;
                    for (var i = 0; i < fc.Entries.Length; i++)
                    {
                        canvas.DrawRect(x, MARGIN_BOTTOM_TOP, BAR_WIDTH, (float)fc.Entries[i].Probability*maxBarHeight, brush);
                        x += BAR_WIDTH + BAR_SPACE_WIDTH;
                    }
                }
                
                
                using (SKImage image = surface.Snapshot())
                using (SKData data = image.Encode(SKEncodedImageFormat.Png, 100)) {
                    File.WriteAllBytes("test.png", data.ToArray());
                }
                    
            }
        }
    }
}