using System;
using System.Collections.Generic;
using System.Drawing;

namespace ChatClient
{
    public static class CalculatePositions
    {
        public static List<PointF> Calculate(int count, int radius, int startPositionX, int startPositionY)
        {

            if (count == 0)
                return new List<PointF>();
            float corner = 360 / count;
            var positionPlayer = new List<PointF>();
            for (int i = 1; i < count + 1; i++)
            {
                PointF point = new PointF()
                {
                    X = (float)(startPositionX + radius * Math.Cos(corner * Math.PI * i / 180)),
                    Y = (float)(startPositionY + radius * Math.Sin(corner * Math.PI * i / 180)),
                };

                positionPlayer.Add(point);
            }
            return positionPlayer;
        }
    }
}


