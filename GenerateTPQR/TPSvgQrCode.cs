using System.Drawing;
using QRCoder;
using static QRCoder.SvgQRCode;

internal sealed class TPSvgQrCode(QRCodeData data) : AbstractQRCode(data)
{
    public string GetGraphic(int pixelsPerModule, string darkColorHex, string icon)
    {
        int offset = 4; // No quiet zone
        int drawableModulesCount = QrCodeData.ModuleMatrix.Count - offset * 2;
        int size = pixelsPerModule * drawableModulesCount;

        int cornerSize = 7 * pixelsPerModule;
        int cornerClipSize = 5 * pixelsPerModule;
        int cornerInnerOffset = 2 * pixelsPerModule;
        int cornerInnerSize = 3 * pixelsPerModule;
        int endCornerOffset = size - cornerSize;
        int cornerRoundingOuter = (int)(pixelsPerModule * 1.5);
        int cornerRoundingInner = (int)(pixelsPerModule * 1.3);

        int iconOriginalSize = 640;
        int iconSize = (int)Math.Ceiling(drawableModulesCount / 5.0) * pixelsPerModule;
        int iconOffset = (size - iconSize) / 2;

        // Build SVG opening tag with size attributes
        var svgFile = new StringBuilder();

        svgFile.AppendLine(
            CultureInfo.InvariantCulture,
            $"""
            <svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 {size} {size}" shape-rendering="crispEdges" width="{size}" height="{size}" fill="{darkColorHex}">
            """);

        DrawCorner(0, 0);
        DrawCorner(endCornerOffset, 0);
        DrawCorner(0, endCornerOffset);
        DrawModulesDots();
        DrawIcon();

        svgFile.Append(@"</svg>");
        return svgFile.ToString();

        void DrawCorner(int x, int y)
        {
            svgFile.AppendLine(
                CultureInfo.InvariantCulture,
                //<path d="M{x},{y} h{cornerSize} v{cornerSize} h-{cornerSize} z M{x + pixelsPerModule},{y + pixelsPerModule} v{cornerClipSize} h{cornerClipSize} v-{cornerClipSize}z" />
                $"""
                <path d="M{x + cornerRoundingOuter},{y}
                    h{cornerSize - 2 * cornerRoundingOuter}
                    a{cornerRoundingOuter},{cornerRoundingOuter} 0 0 1 {cornerRoundingOuter} {cornerRoundingOuter}
                    v{cornerSize - 2 * cornerRoundingOuter}
                    a{cornerRoundingOuter},{cornerRoundingOuter} 0 0 1 -{cornerRoundingOuter} {cornerRoundingOuter}
                    h-{cornerSize - 2 * cornerRoundingOuter}
                    a{cornerRoundingOuter},{cornerRoundingOuter} 0 0 1 -{cornerRoundingOuter} -{cornerRoundingOuter}
                    v-{cornerSize - 2 * cornerRoundingOuter}
                    a{cornerRoundingOuter},{cornerRoundingOuter} 0 0 1 {cornerRoundingOuter} -{cornerRoundingOuter}
                    z
                    M{x + pixelsPerModule},{y + pixelsPerModule + cornerRoundingInner}
                    v{cornerClipSize - 2 * cornerRoundingInner}
                    a{cornerRoundingInner},{cornerRoundingInner} 0 0 0 {cornerRoundingInner} {cornerRoundingInner}
                    h{cornerClipSize - 2 * cornerRoundingInner}
                    a{cornerRoundingInner},{cornerRoundingInner} 0 0 0 {cornerRoundingInner} -{cornerRoundingInner}
                    v-{cornerClipSize - 2 * cornerRoundingInner}
                    a{cornerRoundingInner},{cornerRoundingInner} 0 0 0 -{cornerRoundingInner} -{cornerRoundingInner}
                    h-{cornerClipSize - 2 * cornerRoundingInner}
                    a{cornerRoundingInner},{cornerRoundingInner} 0 0 0 -{cornerRoundingInner} {cornerRoundingInner}
                    z" />
                <circle cx="{x + cornerInnerOffset + cornerInnerSize / 2}" cy="{y + cornerInnerOffset + cornerInnerSize / 2}" r="{cornerInnerSize / 2}" />
                """);
        }

        void DrawModulesDots()
        {
            for (int row = offset; row < QrCodeData.ModuleMatrix.Count - offset; row++)
            {
                for (int col = offset; col < QrCodeData.ModuleMatrix.Count - offset; col++)
                {
                    if (QrCodeData.ModuleMatrix[row][col] && !IsInCorner(row - offset, col - offset) && !IsInIconArea(row - offset, col - offset))
                    {
                        int x = (col - offset) * pixelsPerModule;
                        int y = (row - offset) * pixelsPerModule;
                        svgFile.AppendLine(
                            CultureInfo.InvariantCulture,
                            $"""
                            <circle cx="{x + pixelsPerModule / 2}" cy="{y + pixelsPerModule / 2}" r="{pixelsPerModule / 2}" />
                            """);
                    }
                }
            }
        }

        bool IsInCorner(int row, int col)
        {
            bool inTopLeft = row < 7 && col < 7;
            bool inTopRight = row < 7 && col >= drawableModulesCount - 7;
            bool inBottomLeft = row >= drawableModulesCount - 7 && col < 7;
            return inTopLeft || inTopRight || inBottomLeft;
        }

        bool IsInIconArea(int row, int col)
        {
            int iconModuleSize = iconSize / pixelsPerModule;
            int iconStart = (drawableModulesCount - iconModuleSize) / 2;
            int iconEnd = iconStart + iconModuleSize;
            return row >= iconStart && row < iconEnd && col >= iconStart && col < iconEnd;
        }

        void DrawIcon()
        {
            svgFile.AppendLine(
                CultureInfo.InvariantCulture,
                $"""
                <g transform="translate({iconOffset}, {iconOffset}) scale({(double)iconSize / iconOriginalSize})">
                {icon}
                </g>
                """);
        }
    }

}
